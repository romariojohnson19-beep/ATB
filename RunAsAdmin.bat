@echo off
echo =============================================
echo Running Akhen Trader Elite as Administrator
echo =============================================
echo.
echo This may fix the "listening but not connecting" issue.
echo Windows often requires admin rights for HTTP servers.
echo.
pause

cd /d "%~dp0"
PowerShell -Command "Start-Process '.\bin\Release\net8.0-windows\AKHENS TRADER.exe' -Verb RunAs"

echo.
echo App should start with admin rights now.
echo Try clicking START SERVER again!
echo.
pause
