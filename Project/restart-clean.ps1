# Clean Restart Script for Development Server
# Use this script if you encounter any issues after the fix

Write-Host "=== Cleaning Project ===" -ForegroundColor Cyan

# Navigate to Project directory
Set-Location -Path "C:\Users\ramic\OneDrive\Documents\XuanZhi9\projectCs2-main\Project"

Write-Host "Stopping any running processes..." -ForegroundColor Yellow
Get-Process -Name "Project" -ErrorAction SilentlyContinue | Stop-Process -Force

Write-Host "Cleaning build artifacts..." -ForegroundColor Yellow
dotnet clean

Write-Host "Removing bin and obj directories..." -ForegroundColor Yellow
if (Test-Path "bin") { Remove-Item -Path "bin" -Recurse -Force }
if (Test-Path "obj") { Remove-Item -Path "obj" -Recurse -Force }

Write-Host "Ensuring uploads directory exists..." -ForegroundColor Yellow
$uploadsPath = "wwwroot\uploads\articles"
if (-not (Test-Path $uploadsPath)) {
    New-Item -Path $uploadsPath -ItemType Directory -Force | Out-Null
    Write-Host "Created uploads directory" -ForegroundColor Green
}

Write-Host "Building project..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n=== Build Successful ===" -ForegroundColor Green
    Write-Host "`nYou can now run the project with:" -ForegroundColor Cyan
    Write-Host "  dotnet run" -ForegroundColor White
    Write-Host "`nOr press F5 in Visual Studio to start debugging" -ForegroundColor White
    Write-Host "`nImage uploads should now work without crashing the server!" -ForegroundColor Green
} else {
    Write-Host "`n=== Build Failed ===" -ForegroundColor Red
    Write-Host "Please check the error messages above" -ForegroundColor Red
}
