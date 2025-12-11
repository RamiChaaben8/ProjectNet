$baseUrl = "http://localhost:5053"
$email = "admin_verify_v3@test.com"
$password = "Admin123!"

# 1. Login
$loginBody = @{
    Email = $email
    Password = $password
} | ConvertTo-Json

Write-Host "Logging in..."
try {
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/Account/login" -Method Post -Body $loginBody -ContentType "application/json" -ErrorAction Stop
    $token = $loginResponse.token
    Write-Host "Login successful."
} catch {
    Write-Host "Login failed: $_"
    exit 1
}

# 2. Try to Create Order
$headers = @{
    Authorization = "Bearer $token"
}

$orderBody = @{
    TotalPrice = 100.0
    OrderDate = (Get-Date).ToString("o")
    Status = "Pending"
    Receipt = @()
    Client = $null
    ClientID = 0
} | ConvertTo-Json

Write-Host "Creating order..."
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/Order" -Method Post -Headers $headers -Body $orderBody -ContentType "application/json" -ErrorAction Stop
    Write-Host "Order created successfully!"
} catch {
    $stream = $_.Exception.Response.GetResponseStream()
    $reader = New-Object System.IO.StreamReader($stream)
    $errorBody = $reader.ReadToEnd()
    Write-Host "Order creation FAILED. Response: $errorBody"
    exit 1
}
