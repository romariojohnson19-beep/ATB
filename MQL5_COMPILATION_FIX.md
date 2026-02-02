# ? **MQL5 COMPILATION ERRORS FIXED!**

## ?? **WHAT WAS WRONG**

**Error**: Line 39 - `TERMINAL_WEBSOCKETS_ENABLED` undeclared identifier

**Cause**: `TERMINAL_WEBSOCKETS_ENABLED` doesn't exist in MQL5. This was an incorrect constant that caused compilation to fail.

---

## ? **WHAT WAS FIXED**

### **1. Removed Invalid Check**
- Removed the non-existent `TERMINAL_WEBSOCKETS_ENABLED` constant
- EA now tries to connect directly and handles errors gracefully

### **2. Better Error Handling**
- Added detection for error 4060 (ERR_FUNCTION_NOT_ALLOWED)
- Prints helpful message if WebRequest isn't whitelisted
- Shows exact steps to fix the issue

### **3. Improved Logging**
- Clearer startup messages
- Better connection failure messages
- Helpful troubleshooting hints in Expert tab

---

## ?? **COMPILE NOW!**

### **In MT5 MetaEditor**:

1. **Open the file**:
   - Press F4 (opens MetaEditor)
   - File ? Open
   - Browse to: `Templates\AkhenTraderBridge.mq5`

2. **Compile**:
   - Press F7
   - Should see: **"0 error(s), 0 warning(s)"** ?

3. **Close MetaEditor**

---

## ?? **COMPLETE TEST PROCEDURE**

### **Step 1: Setup MT5 WebRequest Whitelist**

1. In MT5: **Tools ? Options ? Expert Advisors**
2. Enable: **"Allow WebRequest for listed URL:"**
3. Add: **`http://127.0.0.1:8080`**
4. Click **OK**

### **Step 2: Start WPF App**

1. Run: `bin\Release\net8.0-windows\AKHENS TRADER.exe`
2. Go to **EA Control** tab (5th tab)
3. Click **"START SERVER"**
4. Status should show: **"Listening on port 8080..."**
5. Indicator turns **Orange** (server running, waiting for connection)

### **Step 3: Attach EA to MT5 Chart**

1. Open any chart (e.g., EURUSD M5)
2. Navigator ? Expert Advisors
3. Find **"AkhenTraderBridge"**
4. Drag onto chart
5. Settings dialog appears - keep defaults, click **OK**

### **Step 4: Verify Connection**

**In MT5 Expert Tab**, you should see:
```
=================================================
Akhen Trader Elite Bridge EA v2.1
=================================================
Bridge Server: http://127.0.0.1:8080
Heartbeat Interval: 5 seconds
Magic Number: 777777
=================================================
SUCCESS: Connected to Akhen Trader Elite!
```

**In WPF App (EA Control Tab)**, within 5 seconds:
- ? Indicator turns **GREEN**
- ? Connection Status: "Connected"
- ? EA Name: "Akhen Trader Bridge"
- ? EA Version: "2.10"
- ? Last Heartbeat: Updates every 5s
- ? Account info populates (Balance, Equity, etc.)

---

## ?? **TROUBLESHOOTING**

### **If EA shows "WebRequest error: 4060"**

**This means**: WebRequest is not whitelisted

**Solution**:
1. Go to MT5: Tools ? Options ? Expert Advisors
2. Make sure "Allow WebRequest for listed URL:" is **checked**
3. Add: `http://127.0.0.1:8080`
4. Click OK
5. Remove and re-attach EA to chart

### **If EA shows "HTTP error: -1" or "Connection refused"**

**This means**: Server not running

**Solution**:
1. Check WPF app is running
2. Go to EA Control tab
3. Click "START SERVER"
4. Make sure it says "Listening on port 8080"
5. Try attaching EA again

### **If indicator stays Orange (not Green)**

**This means**: Server running but EA not connecting

**Solution**:
1. Check Expert tab in MT5 for errors
2. Verify URL is correct: `http://127.0.0.1:8080`
3. Check Windows Firewall isn't blocking port 8080
4. Try restarting both app and EA

---

## ? **SUCCESS INDICATORS**

| What to Check | Expected Result | Status |
|---------------|-----------------|--------|
| MT5 Compilation | 0 errors, 0 warnings | ? |
| EA Attaches | No errors in Expert tab | ? |
| Connection Message | "SUCCESS: Connected..." | ? |
| App Indicator | GREEN circle | ? |
| Last Heartbeat | Updates every 5s | ? |
| Account Balance | Shows real balance | ? |

---

## ?? **TEST POSITION TRACKING**

Once connected:

1. **Open manual trade in MT5**:
   - Right-click chart ? Trading ? New Order
   - Buy 0.01 lot ? Click "Buy by Market"

2. **Check WPF app** (EA Control tab):
   - Within 2 seconds, trade appears in DataGrid
   - Shows: Ticket, Symbol, Type, Lots, Prices, Profit
   - Profit updates in real-time

3. **Close trade in MT5**:
   - Right-click position ? Close
   - Within 2 seconds, disappears from WPF app

? **If all works**: Bridge is fully functional!

---

## ?? **ERROR CODES REFERENCE**

| Error Code | Meaning | Solution |
|------------|---------|----------|
| 4060 | WebRequest not allowed | Whitelist URL in MT5 Options |
| -1 | Connection failed | Check server is running |
| 200 | Success | Everything working! |
| Other | HTTP error | Check firewall/antivirus |

---

## ?? **NEXT STEPS AFTER SUCCESS**

Once bridge works:

1. ? **Test with real strategies**
   - Generate EA from Custom Builder
   - Attach to chart
   - Monitor in real-time

2. ? **Explore features**
   - Live account monitoring
   - Position tracking
   - Drawdown monitoring

3. ? **Future enhancements (v2.2)**
   - Start/Stop trading from app
   - Close positions remotely
   - Update parameters live

---

## ?? **FILES UPDATED**

- ? `Templates/AkhenTraderBridge.mq5` - Fixed compilation errors
- ? Better error messages
- ? Clearer logging
- ? Helpful troubleshooting hints

---

**?? Ready to test! Compile the EA now and follow the test procedure!** ??

**Any issues? Check the troubleshooting section above!**
