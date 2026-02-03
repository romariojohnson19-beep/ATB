# Fix HttpListener URL reservation issue
# Run this as Administrator if you get "Access Denied" errors

Write-Host "=== HTTP.SYS URL RESERVATION FIX ===" -ForegroundColor Cyan
Write-Host ""

# Check if running as admin
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsIdentityBuiltInRole] "Administrator")

if (-not $isAdmin) {
    Write-Host "? NOT RUNNING AS ADMINISTRATOR!" -ForegroundColor Red
    Write-Host ""
    Write-Host "This script must run as Administrator." -ForegroundColor Yellow
    Write-Host "Right-click and select 'Run as Administrator'" -ForegroundColor Yellow
    Write-Host ""
    pause
    exit
}

Write-Host "? Running as Administrator" -ForegroundColor Green
Write-Host ""

# Add URL reservation for current user
$username = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name
Write-Host "Adding HTTP.SYS reservation for port 8080..." -ForegroundColor Cyan
Write-Host "User: $username" -ForegroundColor White

try {
    # Remove existing reservation if any
    netsh http delete urlacl url=http://+:8080/ 2>$null
    
    # Add new reservation
    $result = netsh http add urlacl url=http://+:8080/ user=$username
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "? URL reservation added successfully!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Now try starting the server in the app (no admin needed)." -ForegroundColor Yellow
    }
    else {
        Write-Host "? Failed to add URL reservation" -ForegroundColor Red
        Write-Host "Error: $result" -ForegroundColor Yellow
    }
}
catch {
    Write-Host "? Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== VERIFICATION ===" -ForegroundColor Cyan
Write-Host "Checking if reservation was added..." -ForegroundColor White
netsh http show urlacl url=http://+:8080/

Write-Host ""
Write-Host "Press any key to close..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
