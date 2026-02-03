# Quick diagnostic - check if port 8080 is actually listening

Write-Host "=== PORT 8080 DIAGNOSTIC ===" -ForegroundColor Cyan
Write-Host ""

# Check if anything is listening on port 8080
$listener = Get-NetTCPConnection -LocalPort 8080 -State Listen -ErrorAction SilentlyContinue

if ($listener) {
    Write-Host "? SOMETHING IS LISTENING on port 8080!" -ForegroundColor Green
    Write-Host "Process ID: $($listener.OwningProcess)" -ForegroundColor White
    
    # Find the process
    $process = Get-Process -Id $listener.OwningProcess -ErrorAction SilentlyContinue
    if ($process) {
        Write-Host "Process Name: $($process.ProcessName)" -ForegroundColor White
        Write-Host "Process Path: $($process.Path)" -ForegroundColor White
    }
    
    Write-Host ""
    Write-Host "Now testing if it responds to HTTP requests..." -ForegroundColor Cyan
    
    # Try to connect
    try {
        $response = Invoke-WebRequest -Uri "http://127.0.0.1:8080/heartbeat" -Method POST -Body '{"test":"true"}' -ContentType "application/json" -TimeoutSec 3 -ErrorAction Stop
        Write-Host "? SERVER RESPONDS! Status: $($response.StatusCode)" -ForegroundColor Green
        Write-Host "Response: $($response.Content)" -ForegroundColor Gray
    }
    catch {
        Write-Host "? SERVER DOES NOT RESPOND!" -ForegroundColor Red
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "DIAGNOSIS: Port is listening but HTTP isn't working" -ForegroundColor Magenta
        Write-Host "LIKELY CAUSE: Need to run app as Administrator" -ForegroundColor Yellow
    }
}
else {
    Write-Host "? NOTHING IS LISTENING on port 8080!" -ForegroundColor Red
    Write-Host ""
    Write-Host "DIAGNOSIS: Server never started" -ForegroundColor Magenta
    Write-Host "SOLUTION:" -ForegroundColor Yellow
    Write-Host "1. Make sure you clicked 'START SERVER' in the app" -ForegroundColor White
    Write-Host "2. Check if you saw a success popup" -ForegroundColor White
    Write-Host "3. Try running the app as Administrator" -ForegroundColor White
}

Write-Host ""
Write-Host "=== END DIAGNOSTIC ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press any key to close..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
