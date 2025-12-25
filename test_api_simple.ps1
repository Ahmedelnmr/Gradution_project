# Simple API Testing Script
$baseUrl = "http://localhost:5235/api"
$results = @()

Write-Host "Starting API Tests..." -ForegroundColor Cyan

# Test 1: Get Properties
Write-Host "`nTest 1: GET /api/properties"
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/properties?pageNumber=1&pageSize=5" -Method GET
    Write-Host "SUCCESS - Found $($response.totalCount) properties" -ForegroundColor Green
    $results += "✅ GET /api/properties"
} catch {
    Write-Host "FAILED - $($_.Exception.Message)" -ForegroundColor Red
    $results += "❌ GET /api/properties"
}

# Test 2: Get Packages
Write-Host "`nTest 2: GET /api/subscriptions/packages"
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/subscriptions/packages" -Method GET
    Write-Host "SUCCESS - Found $($response.Count) packages" -ForegroundColor Green
    $results += "✅ GET /api/subscriptions/packages"
} catch {
    Write-Host "FAILED - $($_.Exception.Message)" -ForegroundColor Red
    $results += "❌ GET /api/subscriptions/packages"
}

# Test 3: Register
Write-Host "`nTest 3: POST /api/account/register"
try {
    $body = @{
        fullName = "Test User"
        phoneNumber = "01888888888"
        whatsAppNumber = "01888888888"
        email = "test888@test.com"
        password = "Test@123"
        confirmPassword = "Test@123"
        role = 2
    } | ConvertTo-Json
    
    $response = Invoke-RestMethod -Uri "$baseUrl/account/register" -Method POST -Body $body -ContentType "application/json"
    Write-Host "SUCCESS - User registered" -ForegroundColor Green
    $results += "✅ POST /api/account/register"
} catch {
    $msg = $_.Exception.Message
    if ($msg -like "*already exists*" -or $msg -like "*409*") {
        Write-Host "INFO - User already exists (expected)" -ForegroundColor Yellow
        $results += "✅ POST /api/account/register (user exists)"
    } else {
        Write-Host "FAILED - $msg" -ForegroundColor Red
        $results += "❌ POST /api/account/register"
    }
}

# Test 4: Login
Write-Host "`nTest 4: POST /api/account/login"
$token = $null
try {
    $body = @{
        email = "test888@test.com"
        password = "Test@123"
    } | ConvertTo-Json
    
    $response = Invoke-RestMethod -Uri "$baseUrl/account/login" -Method POST -Body $body -ContentType "application/json"
    $token = $response.token
    Write-Host "SUCCESS - Token obtained" -ForegroundColor Green
    $results += "✅ POST /api/account/login"
} catch {
    Write-Host "FAILED - $($_.Exception.Message)" -ForegroundColor Red
    $results += "❌ POST /api/account/login"
}

# Test 5: Get My Profile (if token obtained)
if ($token) {
    Write-Host "`nTest 5: GET /api/agents/profile (Authenticated)"
    try {
        $headers = @{
            "Authorization" = "Bearer $token"
        }
        $response = Invoke-RestMethod -Uri "$baseUrl/agents/profile" -Method GET -Headers $headers
        Write-Host "SUCCESS - Profile: $($response.fullName)" -ForegroundColor Green
        $results += "✅ GET /api/agents/profile"
    } catch {
        Write-Host "FAILED - $($_.Exception.Message)" -ForegroundColor Red
        $results += "❌ GET /api/agents/profile"
    }
    
    # Test 6: Get My Properties
    Write-Host "`nTest 6: GET /api/properties/my-properties (Authenticated)"
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/properties/my-properties" -Method GET -Headers $headers
        Write-Host "SUCCESS - Found $($response.totalCount) properties" -ForegroundColor Green
        $results += "✅ GET /api/properties/my-properties"
    } catch {
        Write-Host "FAILED - $($_.Exception.Message)" -ForegroundColor Red
        $results += "❌ GET /api/properties/my-properties"
    }
    
    # Test 7: Get My Subscription
    Write-Host "`nTest 7: GET /api/subscriptions/my-subscription (Authenticated)"
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/subscriptions/my-subscription" -Method GET -Headers $headers
        Write-Host "SUCCESS - Subscription active: $($response.isActive)" -ForegroundColor Green
        $results += "✅ GET /api/subscriptions/my-subscription"
    } catch {
        $msg = $_.Exception.Message
        if ($msg -like "*404*" -or $msg -like "*not found*") {
            Write-Host "INFO - No subscription (expected for new user)" -ForegroundColor Yellow
            $results += "✅ GET /api/subscriptions/my-subscription (no subscription)"
        } else {
            Write-Host "FAILED - $msg" -ForegroundColor Red
            $results += "❌ GET /api/subscriptions/my-subscription"
        }
    }
}

# Test 8: Get Agents List
Write-Host "`nTest 8: GET /api/agents"
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/agents?pageNumber=1&pageSize=5" -Method GET
    Write-Host "SUCCESS - Found $($response.totalCount) agents" -ForegroundColor Green
    $results += "✅ GET /api/agents"
} catch {
    Write-Host "FAILED - $($_.Exception.Message)" -ForegroundColor Red
    $results += "❌ GET /api/agents"
}

# Summary
Write-Host "`n" + ("=" * 60)
Write-Host "TEST SUMMARY" -ForegroundColor Cyan
Write-Host ("=" * 60)
foreach ($r in $results) {
    Write-Host $r
}

$passed = ($results | Where-Object { $_ -like "*✅*" }).Count
$total = $results.Count
Write-Host "`nPassed: $passed / $total" -ForegroundColor $(if($passed -eq $total) { "Green" } else { "Yellow" })
Write-Host ("=" * 60)

