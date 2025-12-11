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
    $token = $loginResponse.token
    Write-Host "Login successful."
} catch {
    Write-Host "Login failed: $_"
    exit 1
}

$headers = @{
    Authorization = "Bearer $token"
}

# 2. Create Order
$createOrderBody = @{
    TotalPrice = 200.0
    OrderDate = (Get-Date).ToString("o")
    Status = "Pending"
    Receipt = @()
} | ConvertTo-Json

Write-Host "Creating order..."
try {
    $createdOrder = Invoke-RestMethod -Uri "$baseUrl/api/Order" -Method Post -Headers $headers -Body $createOrderBody -ContentType "application/json" -ErrorAction Stop
    $orderId = $createdOrder.orderID
    Write-Host "Order created. ID: $orderId"
} catch {
    Write-Host "Order creation failed: $_"
    exit 1
}

# 3. Update Status
$newStatus = "Shipped"
$updateStatusBody = @{
    OrderID = $orderId
    Status = $newStatus
} | ConvertTo-Json

Write-Host "Updating status to '$newStatus'..."
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/Order/status" -Method Put -Headers $headers -Body $updateStatusBody -ContentType "application/json" -ErrorAction Stop
    if ($response -eq $true) {
        Write-Host "Update status API returned true."
    }
} catch {
    $ex = $_.Exception
    if ($ex.Response) {
        $statusCode = $ex.Response.StatusCode
        $stream = $ex.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        $errorBody = $reader.ReadToEnd()
        Write-Host "Update status FAILED. Status: $statusCode. Response: $errorBody"
    } else {
        Write-Host "Update status FAILED. Exception: $ex"
    }
    exit 1
}

# 4. Verify Status
Write-Host "Verifying status..."
try {
    $fetchedOrder = Invoke-RestMethod -Uri "$baseUrl/api/Order/$orderId" -Method Get -Headers $headers -ErrorAction Stop
    if ($fetchedOrder.status -eq $newStatus) {
        Write-Host "SUCCESS: Order status is '$newStatus'."
    } else {
        Write-Host "FAILURE: Order status is '$fetchedOrder.status', expected '$newStatus'."
        exit 1
    }
} catch {
    Write-Host "Fetching order failed: $_"
    exit 1
}
