# API Comprehensive Testing Script
# Tests all endpoints and generates report

$baseUrl = "http://localhost:5235/api"
$testResults = @()

# Helper function to test endpoint
function Test-Endpoint {
    param(
        [string]$Name,
        [string]$Method,
        [string]$Url,
        [hashtable]$Headers = @{},
        [object]$Body = $null,
        [string]$ContentType = "application/json"
    )
    
    try {
        $params = @{
            Uri = $Url
            Method = $Method
            Headers = $Headers
            ErrorAction = "Stop"
        }
        
        if ($Body) {
            if ($ContentType -eq "application/json") {
                $params.Body = ($Body | ConvertTo-Json -Depth 10)
                $params.ContentType = "application/json; charset=utf-8"
            }
        }
        
        $response = Invoke-RestMethod @params
        
        return @{
            Name = $Name
            Method = $Method
            Url = $Url
            Status = "✅ SUCCESS"
            StatusCode = 200
            Response = $response
        }
    }
    catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        return @{
            Name = $Name
            Method = $Method
            Url = $Url
            Status = "❌ FAILED"
            StatusCode = $statusCode
            Error = $_.Exception.Message
        }
    }
}

Write-Host "🚀 Starting API Testing..." -ForegroundColor Cyan
Write-Host "=" * 80

# ===== 1. AUTHENTICATION TESTS =====
Write-Host "`n📝 Testing Authentication Endpoints..." -ForegroundColor Yellow

# Test 1: Register (should work or fail if email exists)
$registerBody = @{
    fullName = "API Test User"
    phoneNumber = "01999999999"
    whatsAppNumber = "01999999999"
    email = "apitest@test.com"
    password = "Test@123456"
    confirmPassword = "Test@123456"
    role = 2
}

$result = Test-Endpoint `
    -Name "Register New User" `
    -Method "POST" `
    -Url "$baseUrl/account/register" `
    -Body $registerBody

$testResults += $result
Write-Host "  $($result.Status) - $($result.Name)" -ForegroundColor $(if($result.Status -like "*SUCCESS*"){"Green"}else{"Red"})

# Test 2: Login
$loginBody = @{
    email = "apitest@test.com"
    password = "Test@123456"
}

$loginResult = Test-Endpoint `
    -Name "Login User" `
    -Method "POST" `
    -Url "$baseUrl/account/login" `
    -Body $loginBody

$testResults += $loginResult
Write-Host "  $($loginResult.Status) - $($loginResult.Name)" -ForegroundColor $(if($loginResult.Status -like "*SUCCESS*"){"Green"}else{"Red"})

# Extract token for authenticated requests
$token = $null
if ($loginResult.Response.token) {
    $token = $loginResult.Response.token
    Write-Host "  🔑 Token obtained: $($token.Substring(0, 20))..." -ForegroundColor Green
}

$authHeaders = @{}
if ($token) {
    $authHeaders = @{
        "Authorization" = "Bearer $token"
    }
}

# ===== 2. PROPERTIES TESTS =====
Write-Host "`n🏠 Testing Properties Endpoints..." -ForegroundColor Yellow

# Test 3: Get Properties (Public)
$result = Test-Endpoint `
    -Name "Get Properties List (Public)" `
    -Method "GET" `
    -Url "$baseUrl/properties?pageNumber=1&pageSize=10"

$testResults += $result
Write-Host "  $($result.Status) - $($result.Name)" -ForegroundColor $(if($result.Status -like "*SUCCESS*"){"Green"}else{"Red"})
if ($result.Status -like "*SUCCESS*") {
    Write-Host "    Found: $($result.Response.totalCount) properties" -ForegroundColor Gray
}

# Test 4: Get Property by ID (if properties exist)
$propertyId = $null
if ($result.Response.items -and $result.Response.items.Count -gt 0) {
    $propertyId = $result.Response.items[0].id
    
    $result = Test-Endpoint `
        -Name "Get Property Details" `
        -Method "GET" `
        -Url "$baseUrl/properties/$propertyId"
    
    $testResults += $result
    Write-Host "  $($result.Status) - $($result.Name) (ID: $propertyId)" -ForegroundColor $(if($result.Status -like "*SUCCESS*"){"Green"}else{"Red"})
}

# Test 5: Track View Count
if ($propertyId) {
    $result = Test-Endpoint `
        -Name "Track Property View" `
        -Method "POST" `
        -Url "$baseUrl/properties/$propertyId/view"
    
    $testResults += $result
    Write-Host "  $($result.Status) - $($result.Name)" -ForegroundColor $(if($result.Status -like "*SUCCESS*"){"Green"}else{"Red"})
}

# Test 6: Get My Properties (Authenticated)
if ($token) {
    $result = Test-Endpoint `
        -Name "Get My Properties" `
        -Method "GET" `
        -Url "$baseUrl/properties/my-properties" `
        -Headers $authHeaders
    
    $testResults += $result
    Write-Host "  $($result.Status) - $($result.Name)" -ForegroundColor $(if($result.Status -like "*SUCCESS*"){"Green"}else{"Red"})
}

# ===== 3. AGENTS TESTS =====
Write-Host "`n👤 Testing Agents Endpoints..." -ForegroundColor Yellow

# Test 7: Get Agents List
$result = Test-Endpoint `
    -Name "Get Agents List" `
    -Method "GET" `
    -Url "$baseUrl/agents?pageNumber=1&pageSize=10"

$testResults += $result
Write-Host "  $($result.Status) - $($result.Name)" -ForegroundColor $(if($result.Status -like "*SUCCESS*"){"Green"}else{"Red"})

# Test 8: Get My Profile (Authenticated)
if ($token) {
    $result = Test-Endpoint `
        -Name "Get My Profile" `
        -Method "GET" `
        -Url "$baseUrl/agents/profile" `
        -Headers $authHeaders
    
    $testResults += $result
    Write-Host "  $($result.Status) - $($result.Name)" -ForegroundColor $(if($result.Status -like "*SUCCESS*"){"Green"}else{"Red"})
    
    if ($result.Status -like "*SUCCESS*") {
        Write-Host "    Profile: $($result.Response.fullName) - Verified: $($result.Response.isVerified)" -ForegroundColor Gray
    }
}

# ===== 4. SUBSCRIPTIONS TESTS =====
Write-Host "`n💳 Testing Subscriptions Endpoints..." -ForegroundColor Yellow

# Test 9: Get Packages
$result = Test-Endpoint `
    -Name "Get Subscription Packages" `
    -Method "GET" `
    -Url "$baseUrl/subscriptions/packages"

$testResults += $result
Write-Host "  $($result.Status) - $($result.Name)" -ForegroundColor $(if($result.Status -like "*SUCCESS*"){"Green"}else{"Red"})

if ($result.Status -like "*SUCCESS*") {
    Write-Host "    Found: $($result.Response.Count) packages" -ForegroundColor Gray
}

# Test 10: Get My Subscription (Authenticated)
if ($token) {
    $result = Test-Endpoint `
        -Name "Get My Subscription" `
        -Method "GET" `
        -Url "$baseUrl/subscriptions/my-subscription" `
        -Headers $authHeaders
    
    $testResults += $result
    Write-Host "  $($result.Status) - $($result.Name)" -ForegroundColor $(if($result.Status -like "*SUCCESS*"){"Green"}else{"Red"})
}

# ===== 5. NOTIFICATIONS TESTS =====
Write-Host "`n🔔 Testing Notifications Endpoints..." -ForegroundColor Yellow

# Test 11: Get My Notifications (Authenticated)
if ($token) {
    $result = Test-Endpoint `
        -Name "Get My Notifications" `
        -Method "GET" `
        -Url "$baseUrl/notifications?pageNumber=1&pageSize=10" `
        -Headers $authHeaders
    
    $testResults += $result
    Write-Host "  $($result.Status) - $($result.Name)" -ForegroundColor $(if($result.Status -like "*SUCCESS*"){"Green"}else{"Red"})
}

# ===== 6. SAVED PROPERTIES TESTS =====
Write-Host "`n⭐ Testing Saved Properties Endpoints..." -ForegroundColor Yellow

# Test 12: Get Saved Properties (Authenticated)
if ($token) {
    $result = Test-Endpoint `
        -Name "Get Saved Properties" `
        -Method "GET" `
        -Url "$baseUrl/savedproperties" `
        -Headers $authHeaders
    
    $testResults += $result
    Write-Host "  $($result.Status) - $($result.Name)" -ForegroundColor $(if($result.Status -like "*SUCCESS*"){"Green"}else{"Red"})
}

# ===== SUMMARY =====
Write-Host "`n" + ("=" * 80)
Write-Host "📊 TEST SUMMARY" -ForegroundColor Cyan
Write-Host ("=" * 80)

$successCount = ($testResults | Where-Object { $_.Status -like "*SUCCESS*" }).Count
$failedCount = ($testResults | Where-Object { $_.Status -like "*FAILED*" }).Count
$totalCount = $testResults.Count

Write-Host "`nTotal Tests: $totalCount" -ForegroundColor White
Write-Host "✅ Passed: $successCount" -ForegroundColor Green
Write-Host "❌ Failed: $failedCount" -ForegroundColor Red
Write-Host "Success Rate: $([math]::Round(($successCount / $totalCount) * 100, 2))%" -ForegroundColor $(if($successCount -eq $totalCount){"Green"}else{"Yellow"})

Write-Host "`n📋 Detailed Results:" -ForegroundColor Cyan
foreach ($result in $testResults) {
    $color = if ($result.Status -like "*SUCCESS*") { "Green" } else { "Red" }
    Write-Host "  $($result.Status) [$($result.Method)] $($result.Name)" -ForegroundColor $color
    if ($result.Error) {
        Write-Host "    Error: $($result.Error)" -ForegroundColor Red
    }
}

# Save results to file
$resultsJson = $testResults | ConvertTo-Json -Depth 10
$resultsJson | Out-File "test_results.json" -Encoding UTF8

Write-Host "`n💾 Results saved to: test_results.json" -ForegroundColor Green
Write-Host "=" * 80
