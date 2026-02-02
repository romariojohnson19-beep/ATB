# ?? **BRIDGE SERVER NOT CONNECTING - COMPLETE DIAGNOSIS GUIDE**

## ? **QUICK FIX - TRY THIS FIRST**

### **Step 1: Start Fresh**
1. **Close everything**:
   - Close the WPF app if running
   - Remove EA from MT5 chart
   
2. **Start the app again**:
   ```powershell
   .\bin\Release\net8.0-windows\AKHENS TRADER.exe
   ```

3. **Go to EA Control tab** (5th tab - robot icon ??)

4. **Click "START SERVER"**
   - You should see a **popup message** saying "Bridge server started successfully!"
   - If you see an **error popup**, read it carefully and follow the instructions

---

## ?? **COMMON ISSUES & SOLUTIONS**

### **Issue 1: Port Already in Use**

**Error popup says**: "Port 8080 may already be in use"

**Solution**:
```powershell
# Find what's using port 8080
Get-NetTCPConnection -LocalPort 8080 -ErrorAction SilentlyContinue | Select-Object OwningProcess, State
```

**Fix**:
- Close the app using port 8080, OR
- Change the port (see "Change Port" section below)

---

### **Issue 2: HttpListener Permission Denied**

**Error popup says**: "Access Denied" or "HttpListener" error

**Solution**: Run app as **Administrator**
1. Close the app
2. Right-click `AKHENS TRADER.exe`
3. "Run as administrator"
4. Try starting server again

---

### **Issue 3: Windows Firewall Blocking**

**Error**: Connection refused from MT5

**Solution**:
1. **Temporarily disable** Windows Firewall
2. Try connecting EA
3. If it works, add firewall rule:

```powershell
# Add firewall rule (run as Administrator)
New-NetFirewallRule -DisplayName "Akhen Trader Bridge" -Direction Inbound -LocalPort 8080 -Protocol TCP -Action Allow
```

---

### **Issue 4: Button Does Nothing**

**Problem**: Clicking "START SERVER" does nothing

**Check**:
1. Is the button **enabled**? (not grayed out)
2. Try clicking "STOP SERVER" then "START SERVER"
3. Restart the app

---

## ?? **HOW TO CHANGE PORT**

If port 8080 is occupied:

### **In WPF App**:
Edit `ViewModels/BridgeViewModel.cs` line 45:
```csharp
private int bridgePort = 8081;  // Change from 8080 to 8081
```

### **In MT5 EA**:
When attaching EA, change:
- `BridgeServerURL` = `http://127.0.0.1:8081`

### **In MT5 Whitelist**:
- Add: `http://127.0.0.1:8081`

---

## ?? **DIAGNOSTIC TESTS**

### **Test 1: Check if App is Running**
```powershell
Get-Process | Where-Object {$_.ProcessName -like "*AKHENS*"}
```
? Should show process

### **Test 2: Check Port Status**
```powershell
Get-NetTCPConnection -LocalPort 8080 -ErrorAction SilentlyContinue
```
- **Nothing found** = Port is free ?
- **Shows connection** = Port occupied ?

### **Test 3: Manual Server Test**
```powershell
.\TestBridgeServer.ps1
```
- **SUCCESS** = Server running ?
- **FAILED** = Server not started ?

---

## ?? **VISUAL GUIDE**

### **What You Should See:**

#### **BEFORE Starting Server**:
```
Status: Disconnected
Indicator: ?? RED
Message: "Bridge server not started"
Buttons: [START SERVER] (enabled)  [STOP SERVER] (disabled)
```

#### **AFTER Clicking "START SERVER"**:
```
? POPUP: "Bridge server started successfully!"
Status: Listening on port 8080...
Indicator: ?? ORANGE (waiting for EA)
Message: "Bridge server started on port 8080. Waiting for EA connection..."
Buttons: [START SERVER] (disabled)  [STOP SERVER] (enabled)
```

#### **AFTER EA Connects**:
```
Status: Connected
Indicator: ?? GREEN
EA Name: "Akhen Trader Bridge"
Last Heartbeat: Updates every 5s
Buttons: [START SERVER] (disabled)  [STOP SERVER] (enabled)
```

---

## ?? **STEP-BY-STEP SUCCESS PATH**

### **Do This EXACTLY:**

1. **Close everything**
   - Kill app: `Stop-Process -Name "AKHENS TRADER" -Force`
   - Remove EA from MT5

2. **Start fresh**
   ```powershell
   cd "C:\Users\ASZIAM\Desktop\AKHENS TRADER Prop\AKHENS TRADER"
   .\bin\Release\net8.0-windows\AKHENS TRADER.exe
   ```

3. **Wait for app to fully load** (3-5 seconds)

4. **Click EA Control tab** (5th tab)

5. **Click "START SERVER"**
   - ? **Popup says success**: Continue to step 6
   - ? **Popup shows error**: Read error, follow fix above, try again

6. **Run test script**:
   ```powershell
   .\TestBridgeServer.ps1
   ```
   - Should say: "SUCCESS! Server is running and responding!"

7. **In MT5**:
   - Tools ? Options ? Expert Advisors
   - Whitelist: `http://127.0.0.1:8080`
   - Attach `AkhenTraderBridge.mq5` to chart

8. **Watch connection**:
   - App indicator turns GREEN within 5 seconds
   - MT5 Expert tab shows: "SUCCESS: Connected..."

---

## ?? **STILL NOT WORKING?**

### **Collect Debug Info:**

Run this diagnostic:
```powershell
Write-Host "=== AKHEN TRADER BRIDGE DIAGNOSTICS ===" -ForegroundColor Cyan

# 1. Check if app running
$proc = Get-Process | Where-Object {$_.ProcessName -like "*AKHENS*"}
if ($proc) {
    Write-Host "? App is running (PID: $($proc.Id))" -ForegroundColor Green
} else {
    Write-Host "? App is NOT running!" -ForegroundColor Red
}

# 2. Check port 8080
$port = Get-NetTCPConnection -LocalPort 8080 -ErrorAction SilentlyContinue
if ($port) {
    Write-Host "? Port 8080 is OCCUPIED by PID: $($port.OwningProcess)" -ForegroundColor Red
    $portProc = Get-Process -Id $port.OwningProcess -ErrorAction SilentlyContinue
    if ($portProc) {
        Write-Host "   Process name: $($portProc.ProcessName)" -ForegroundColor Yellow
    }
} else {
    Write-Host "? Port 8080 is FREE" -ForegroundColor Green
}

# 3. Test server
Write-Host "`nTesting server..." -ForegroundColor Cyan
try {
    $response = Invoke-WebRequest -Uri "http://127.0.0.1:8080/heartbeat" -Method POST -Body '{"test":"true"}' -ContentType "application/json" -TimeoutSec 3 -ErrorAction Stop
    Write-Host "? Server is RESPONDING! (Status: $($response.StatusCode))" -ForegroundColor Green
}
catch {
    Write-Host "? Server NOT responding: $($_.Exception.Message)" -ForegroundColor Red
}

# 4. Check firewall
$rule = Get-NetFirewallRule -DisplayName "Akhen Trader Bridge" -ErrorAction SilentlyContinue
if ($rule) {
    Write-Host "? Firewall rule exists" -ForegroundColor Green
} else {
    Write-Host "? No firewall rule (may need to allow)" -ForegroundColor Yellow
}

Write-Host "`n=== END DIAGNOSTICS ===" -ForegroundColor Cyan
```

**Send me the output!**

---

## ?? **EXPECTED BEHAVIOR**

**When working correctly**:

1. Click "START SERVER" ? **Popup** appears immediately
2. Test script ? **SUCCESS**
3. Attach EA ? **GREEN** indicator within 5 seconds
4. Account info ? **Updates** every 2 seconds

**If ANY step fails**, find that section above and follow the fix!

---

**Try the "QUICK FIX" section first, then report what popup message you see!** ??
