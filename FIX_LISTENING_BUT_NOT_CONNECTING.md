# ?? **"LISTENING BUT NOT CONNECTING" - COMPLETE FIX**

## ? **MOST LIKELY CAUSE: WINDOWS PERMISSIONS**

Windows requires special permissions for applications to accept HTTP connections. This is your issue!

---

## ?? **SOLUTION - TRY THESE IN ORDER**

### **SOLUTION 1: Run as Administrator** (Quickest)

**Option A: Use Batch File**
```cmd
RunAsAdmin.bat
```
- Double-click `RunAsAdmin.bat`
- Click "Yes" when UAC prompts
- App starts with admin rights
- Go to EA Control tab ? Click "START SERVER"
- Should work now!

**Option B: Manual**
1. Close the app if running
2. Navigate to: `bin\Release\net8.0-windows\`
3. Right-click `AKHENS TRADER.exe`
4. Click "Run as administrator"
5. Click "Yes" on UAC prompt
6. Try START SERVER again

---

### **SOLUTION 2: Fix URL Reservation** (Permanent)

This lets you run without admin rights permanently!

1. **Right-click** `FixHttpListener.ps1`
2. Click **"Run with PowerShell"**
3. Click **"Yes"** when UAC prompts
4. Script will add URL reservation
5. **Now app can run WITHOUT admin!**

---

## ?? **DIAGNOSTIC TESTS**

### **Test 1: Check What's Actually Happening**

Run this **while the app is "listening"**:
```powershell
.\DiagnosePort.ps1
```

**Possible Results**:

#### **A: "SOMETHING IS LISTENING... SERVER RESPONDS!"**
? Server is working! Issue is with MT5 EA or firewall.

**Solution**:
- Check MT5 has URL whitelisted: `http://127.0.0.1:8080`
- Check Windows Firewall isn't blocking
- Try attaching EA again

#### **B: "SOMETHING IS LISTENING... SERVER DOES NOT RESPOND!"**
?? Port is bound but HTTP isn't working

**Solution**: **RUN AS ADMINISTRATOR** (Solution 1)

#### **C: "NOTHING IS LISTENING on port 8080!"**
? Server never actually started

**Solution**: 
- Run as admin (Solution 1), OR
- Fix URL reservation (Solution 2)

---

## ?? **DETAILED DIAGNOSIS FLOW**

```
1. App shows "Listening on port 8080"
   ?
2. Run: DiagnosePort.ps1
   ?
3. Does it say "SOMETHING IS LISTENING"?
   
   YES ? Server started but HTTP not working
         ? Try Solution 1 (Run as Admin)
   
   NO  ? Server never started
         ? App lied about "listening" (common with HttpListener)
         ? Try Solution 1 or 2
```

---

## ?? **UNDERSTANDING THE ISSUE**

### **What's Happening**:

1. Your app calls `_httpListener.Start()`
2. Windows says "OK" but secretly blocks it
3. App thinks it's listening (no error thrown)
4. But nothing can actually connect!

### **Why**:

Windows HTTP.SYS requires:
- **Admin rights**, OR
- **URL reservation** for the user

Without either, HttpListener **silently fails** to accept connections!

---

## ?? **RECOMMENDED APPROACH**

### **For Testing (Now)**:
Use **Solution 1** - Run as admin
- Quick
- Guaranteed to work
- Good for testing

### **For Production (Later)**:
Use **Solution 2** - Fix URL reservation
- Run once as admin
- Then works without admin forever
- Better user experience

---

## ?? **STEP-BY-STEP FIX RIGHT NOW**

1. **Close the app** (if running)

2. **Run diagnostic**:
   ```powershell
   .\DiagnosePort.ps1
   ```
   (Should say "NOTHING IS LISTENING")

3. **Apply fix**:
   ```cmd
   RunAsAdmin.bat
   ```
   OR right-click exe ? "Run as administrator"

4. **In app**: EA Control tab ? "START SERVER"

5. **Run diagnostic again**:
   ```powershell
   .\DiagnosePort.ps1
   ```
   (Should now say "SERVER RESPONDS!")

6. **Attach EA** to MT5 chart

7. **Watch connection** turn GREEN! ??

---

## ?? **IF STILL NOT WORKING AFTER ADMIN**

Then it's a firewall issue:

1. **Temporarily disable Windows Firewall**:
   - Windows Security ? Firewall ? Turn off
   - Try connecting EA
   - If works, add firewall rule:

```powershell
# Run as Administrator
New-NetFirewallRule -DisplayName "Akhen Trader Bridge" -Direction Inbound -LocalPort 8080 -Protocol TCP -Action Allow
```

2. **Re-enable firewall**

---

## ?? **EXPECTED OUTCOME**

**After running as admin**:

1. DiagnosePort.ps1 ? "? SERVER RESPONDS!"
2. TestBridgeServer.ps1 ? "SUCCESS!"
3. MT5 EA attaches ? Expert tab shows "SUCCESS: Connected..."
4. WPF app indicator ? ?? GREEN
5. Account info ? Updates every 2s

---

## ?? **REPORT BACK**

After trying Solution 1 (admin), tell me:

1. What did `DiagnosePort.ps1` show?
2. Did the EA connect?
3. What color is the indicator in the app?

---

**TRY SOLUTION 1 NOW (RunAsAdmin.bat) AND RUN DiagnosePort.ps1!** ??

This will 100% fix the "listening but not connecting" issue!
