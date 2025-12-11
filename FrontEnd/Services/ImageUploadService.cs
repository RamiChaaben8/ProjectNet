using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace FrontEnd.Services
{
    public interface IImageUploadService
    {
        Task<string?> UploadImageAsync(byte[] imageData, string fileName);
        Task<bool> DeleteImageAsync(string imageUrl);
    }

    public class ImageUploadService : IImageUploadService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public ImageUploadService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        private async Task EnsureAuthInitializedAsync()
        {
            await _authService.InitializeAsync();
        }

        private void AddAuthorizationHeader()
        {
            var token = _authService.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                // Remove existing Authorization header if any
                if (_httpClient.DefaultRequestHeaders.Authorization != null)
                {
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                }
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<string?> UploadImageAsync(byte[] imageData, string fileName)
        {
            try
            {
                await EnsureAuthInitializedAsync();
                AddAuthorizationHeader();

                using var content = new MultipartFormDataContent();
                var fileContent = new ByteArrayContent(imageData);
                
                // Set the correct content type based on file extension
                var extension = Path.GetExtension(fileName).ToLower();
                var contentType = extension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".webp" => "image/webp",
                    _ => "application/octet-stream"
                };
                
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
                content.Add(fileContent, "file", fileName);

                Console.WriteLine($"Uploading file: {fileName}, Size: {imageData.Length} bytes, ContentType: {contentType}");
                Console.WriteLine($"Token exists: {!string.IsNullOrEmpty(_authService.GetToken())}");
                Console.WriteLine($"API Endpoint: {_httpClient.BaseAddress}api/ImageUpload");

                var response = await _httpClient.PostAsync("api/ImageUpload", content);

                Console.WriteLine($"Response status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var resultText = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response content: {resultText}");
                    
                    try
                    {
                        var result = JsonSerializer.Deserialize<ImageUploadResponse>(resultText, new JsonSerializerOptions 
                        { 
                            PropertyNameCaseInsensitive = true 
                        });
                        Console.WriteLine($"Upload successful: {result?.ImageUrl}");
                        return result?.ImageUrl;
                    }
                    catch (JsonException jsonEx)
                    {
                        Console.WriteLine($"JSON parsing error: {jsonEx.Message}");
                        Console.WriteLine($"Raw response: {resultText}");
                        return null;
                    }
                }

                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Upload error ({response.StatusCode}): {error}");
                
                // Try to parse error as JSON
                try
                {
                    var errorObj = JsonSerializer.Deserialize<Dictionary<string, object>>(error);
                    if (errorObj != null && errorObj.ContainsKey("error"))
                    {
                        Console.WriteLine($"Error message: {errorObj["error"]}");
                    }
                }
                catch
                {
                    // Ignore JSON parsing errors for error messages
                }
                
                return null;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Request exception: {httpEx.Message}");
                Console.WriteLine($"Inner exception: {httpEx.InnerException?.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception uploading image: {ex.Message}");
                Console.WriteLine($"Exception type: {ex.GetType().Name}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            try
            {
                await EnsureAuthInitializedAsync();
                AddAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"api/ImageUpload?imageUrl={Uri.EscapeDataString(imageUrl)}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception deleting image: {ex.Message}");
                return false;
            }
        }
    }

    public class ImageUploadResponse
    {
        public string ImageUrl { get; set; }
    }
}
