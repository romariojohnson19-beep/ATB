# Test if WPF app is listening on port 8080

Write-Host "Testing connection to WPF Bridge Server..." -ForegroundColor Cyan

try {
    $response = Invoke-WebRequest -Uri "http://127.0.0.1:8080/heartbeat" -Method POST -Body '{"test":"true"}' -ContentType "application/json" -TimeoutSec 3 -ErrorAction Stop
    
    if ($response.StatusCode -eq 200) {
        Write-Host "SUCCESS! Server is running and responding!" -ForegroundColor Green
        Write-Host "Status: $($response.StatusCode)" -ForegroundColor Green
        Write-Host ""
        Write-Host "Server is ready. Now try attaching the EA to MT5 chart." -ForegroundColor Yellow
    }
}
catch {
    Write-Host "FAILED! Server is NOT responding!" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "SOLUTION:" -ForegroundColor Yellow
    Write-Host "1. Make sure WPF app is running" -ForegroundColor White
    Write-Host "2. Go to EA Control tab (5th tab)" -ForegroundColor White
    Write-Host "3. Click 'START SERVER' button" -ForegroundColor White
    Write-Host "4. Run this test again" -ForegroundColor White
}

Write-Host ""
Write-Host "Press any key to close..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
