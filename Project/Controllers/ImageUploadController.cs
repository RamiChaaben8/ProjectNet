using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,admin")]
    public class ImageUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ImageUploadController> _logger;

        public ImageUploadController(
            IWebHostEnvironment environment, 
            IConfiguration configuration,
            ILogger<ImageUploadController> logger)
        {
            _environment = environment;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            try
            {
                var webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
                var uploadsFolder = Path.Combine(webRootPath, "uploads", "articles");
                var exists = Directory.Exists(uploadsFolder);
                
                return Ok(new 
                { 
                    status = "healthy",
                    webRootPath = webRootPath,
                    uploadsFolder = uploadsFolder,
                    uploadsFolderExists = exists,
                    authenticated = User.Identity?.IsAuthenticated ?? false,
                    role = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                _logger.LogInformation($"Upload request received. File: {file?.FileName}, Size: {file?.Length}");
                _logger.LogInformation($"User authenticated: {User.Identity?.IsAuthenticated}");
                _logger.LogInformation($"User role: {User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value}");

                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("No file uploaded or file is empty");
                    return BadRequest(new { error = "No file uploaded" });
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    _logger.LogWarning($"Invalid file type: {fileExtension}");
                    return BadRequest(new { error = "Invalid file type. Only images are allowed (jpg, jpeg, png, gif, webp)" });
                }

                // Validate file size (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    _logger.LogWarning($"File too large: {file.Length} bytes");
                    return BadRequest(new { error = "File size exceeds 5MB limit" });
                }

                // Get wwwroot path or use ContentRootPath as fallback
                var webRootPath = _environment.WebRootPath;
                if (string.IsNullOrEmpty(webRootPath))
                {
                    webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
                    _logger.LogInformation($"WebRootPath is null, using ContentRootPath: {webRootPath}");
                }

                _logger.LogInformation($"Using root path: {webRootPath}");

                // Create unique filename
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                
                // Create uploads directory if it doesn't exist
                var uploadsFolder = Path.Combine(webRootPath, "uploads", "articles");
                _logger.LogInformation($"Target folder: {uploadsFolder}");

                if (!Directory.Exists(uploadsFolder))
                {
                    _logger.LogInformation($"Creating directory: {uploadsFolder}");
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);
                _logger.LogInformation($"Saving file to: {filePath}");

                // Save file with proper disposal and error handling
                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 
                        bufferSize: 4096, useAsync: true))
                    {
                        await file.CopyToAsync(stream);
                        await stream.FlushAsync();
                    }

                    // Verify file was created and has content
                    if (!System.IO.File.Exists(filePath))
                    {
                        _logger.LogError($"File was not created: {filePath}");
                        return StatusCode(500, new { error = "Failed to save file to disk" });
                    }

                    var fileInfo = new FileInfo(filePath);
                    if (fileInfo.Length == 0)
                    {
                        _logger.LogError($"File was created but is empty: {filePath}");
                        System.IO.File.Delete(filePath);
                        return StatusCode(500, new { error = "Saved file is empty" });
                    }

                    _logger.LogInformation($"File saved successfully: {fileName} ({fileInfo.Length} bytes)");
                }
                catch (IOException ioEx)
                {
                    _logger.LogError(ioEx, $"IO error saving file: {ioEx.Message}");
                    
                    // Clean up partial file if it exists
                    if (System.IO.File.Exists(filePath))
                    {
                        try { System.IO.File.Delete(filePath); } catch { }
                    }
                    
                    return StatusCode(500, new { error = $"Error saving file: {ioEx.Message}" });
                }

                // Return the URL
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var imageUrl = $"{baseUrl}/uploads/articles/{fileName}";

                _logger.LogInformation($"Returning URL: {imageUrl}");

                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading file: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { error = $"Error uploading file: {ex.Message}", details = ex.InnerException?.Message ?? "" });
            }
        }

        [HttpDelete]
        public IActionResult DeleteImage([FromQuery] string imageUrl)
        {
            try
            {
                _logger.LogInformation($"Delete request for: {imageUrl}");

                if (string.IsNullOrEmpty(imageUrl))
                {
                    return BadRequest(new { error = "Image URL is required" });
                }

                // Extract filename from URL
                var uri = new Uri(imageUrl);
                var fileName = Path.GetFileName(uri.LocalPath);
                
                var webRootPath = _environment.WebRootPath;
                if (string.IsNullOrEmpty(webRootPath))
                {
                    webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
                }

                var filePath = Path.Combine(webRootPath, "uploads", "articles", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    // Use retry logic for file deletion in case of transient issues
                    int maxRetries = 3;
                    for (int i = 0; i < maxRetries; i++)
                    {
                        try
                        {
                            System.IO.File.Delete(filePath);
                            _logger.LogInformation($"File deleted: {fileName}");
                            return Ok(new { message = "Image deleted successfully" });
                        }
                        catch (IOException) when (i < maxRetries - 1)
                        {
                            Thread.Sleep(100); // Wait a bit before retry
                        }
                    }
                    
                    // If we get here, all retries failed
                    return StatusCode(500, new { error = "Failed to delete file after multiple attempts" });
                }

                _logger.LogWarning($"File not found: {filePath}");
                return NotFound(new { error = "Image not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting file: {ex.Message}");
                return StatusCode(500, new { error = $"Error deleting file: {ex.Message}" });
            }
        }
    }
}
