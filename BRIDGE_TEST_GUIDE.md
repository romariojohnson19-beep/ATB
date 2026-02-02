# ?? **AKHEN TRADER ELITE v2.1 - BRIDGE TEST GUIDE**

## ? **BUILD SUCCESSFUL - READY TO TEST!**

**Status**: UI integrated, Bridge backend complete, Ready for testing!

---

## ?? **TESTING PROCEDURE**

### **Phase 1: Test WPF App Bridge Server** ?

1. **Launch the App**
   - Press `F5` in Visual Studio OR
   - Run: `bin\Release\net8.0-windows\AKHENS TRADER.exe`

2. **Navigate to EA Control Tab**
   - Click the 5th tab "EA Control"
   - You should see:
     - Bridge Server Control card
     - Account Information card (all zeros)
     - Live Positions DataGrid (empty)
     - Setup Instructions card

3. **Start the Bridge Server**
   - Click "START SERVER" button
   - Status should change from "Disconnected" to "Listening on port 8080..."
   - Button should disable after clicking

4. **Verify Server is Running**
   - Status indicator (circle) should turn **Orange** (server running, not connected)
   - Status Message: "Bridge server started on port 8080. Waiting for EA connection..."

---

### **Phase 2: Connect MT5 Bridge EA** ?

#### **A. Enable WebRequests in MT5**

1. Open MetaTrader 5
2. Go to: **Tools ? Options ? Expert Advisors**
3. Enable: **"Allow WebRequest for listed URL:"**
4. Add URL: `http://localhost:8080`
5. Click **OK**

#### **B. Compile Bridge EA**

1. In MT5, press `F4` (opens MetaEditor)
2. File ? Open ? Browse to:  
   `C:\Users\ASZIAM\Desktop\AKHENS TRADER Prop\AKHENS TRADER\Templates\AkhenTraderBridge.mq5`
3. Press `F7` to compile
4. Should see: **"0 error(s), 0 warning(s)"**
5. Close MetaEditor

#### **C. Attach EA to Chart**

1. In MT5, open any chart (e.g., EURUSD M5)
2. Navigator ? Expert Advisors ? Find "AkhenTraderBridge"
3. Drag it onto the chart
4. In settings dialog:
   - BridgeServerURL: `http://localhost:8080` (default)
   - HeartbeatInterval: `5` seconds (default)
   - UpdateInterval: `2` seconds (default)
   - Click **OK**

5. **Check Expert Tab**
   - Open Terminal (Ctrl+T)
   - Go to "Expert" tab
   - Should see:
     ```
     =================================================
     Akhen Trader Elite Bridge EA v2.1
     =================================================
     Bridge Server: http://localhost:8080
     ? Successfully connected to Akhen Trader Elite!
     ```

---

### **Phase 3: Verify Connection** ?

#### **In WPF App (EA Control Tab)**:

**Within 5 seconds, you should see**:

1. **Status Indicator** turns **GREEN** ?
2. **Connection Status**: "Connected"
3. **EA Name**: "Akhen Trader Bridge"
4. **EA Version**: "2.10"
5. **Last Heartbeat**: Updates every 5 seconds (shows time)

**Within 2 seconds, Account Info updates**:

6. **Balance**: Shows your MT5 account balance
7. **Equity**: Shows current equity
8. **Margin**: Shows margin used
9. **Daily/Total Drawdown**: Calculated percentages
10. **Open Positions**: Count of open positions

---

### **Phase 4: Test Real-Time Position Tracking** ??

1. **In MT5**: Open a **manual trade**
   - Right-click chart ? Trading ? New Order
   - Buy 0.01 lot
   - Click "Buy by Market"

2. **In WPF App**: Check "Live Positions" DataGrid
   - **Within 2 seconds**, the trade should appear!
   - Columns show: Ticket, Symbol, Type, Lots, Prices, Profit
   - Profit column should be **Green** if positive

3. **Test Updates**:
   - Watch the position profit update every 2 seconds as price moves
   - Current Price column should update live

4. **Close Trade in MT5**:
   - Right-click position ? Close
   - **Within 2 seconds**, position disappears from WPF app

---

## ? **SUCCESS CRITERIA**

| Test | Expected Result | Status |
|------|-----------------|--------|
| App launches | EA Control tab visible | ? |
| Start Server | Status: "Listening on port 8080" | ? |
| Attach EA | Expert tab shows connection success | ? |
| Connection | App shows "Connected" + Green indicator | ? |
| Heartbeat | Last Heartbeat updates every 5s | ? |
| Account Info | Balance/Equity display correctly | ? |
| Open Trade | Position appears in DataGrid within 2s | ? |
| Live Updates | Profit/Price update every 2s | ? |
| Close Trade | Position disappears within 2s | ? |

---

## ?? **TROUBLESHOOTING**

### **Problem**: Server won't start
- **Solution**: Port 8080 may be in use. Change port in BridgeViewModel.cs (line 45) and EA (line 11)

### **Problem**: EA can't connect
- **Check**:
  1. WebRequests enabled in MT5?
  2. URL added: `http://localhost:8080`?
  3. Windows Firewall blocking?
  4. Bridge server running in app?

### **Problem**: Connection shows "Connected" then "Connection Lost"
- **Check Expert tab** in MT5 for errors
- **Restart both** app and EA

### **Problem**: Positions don't appear
- **Check** if EA is attached to correct chart
- **Check Expert tab** for "Position update failed" messages
- **Restart EA** (remove and re-attach)

---

## ?? **WHAT TO OBSERVE**

### **In WPF App**:
- ? Green connection indicator
- ? Live balance/equity updates
- ? Positions appear/disappear automatically
- ? Profit updates in real-time
- ? Last heartbeat timestamp changes

### **In MT5 Expert Tab**:
- ? Connection success message
- ? No errors
- ? Periodic "Sending status..." messages (if verbose logging added)

---

## ?? **NEXT STEPS AFTER SUCCESSFUL TEST**

Once all tests pass:

1. ? **Commit to Git**
   ```
   git add -A
   git commit -m "v2.1 UI Integration Complete - Bridge Fully Functional"
   git push origin main
   ```

2. ? **Add Command System** (v2.2)
   - Add `/commands` endpoint for WPF ? MT5 commands
   - Implement START/STOP trading
   - Implement CLOSE_ALL positions
   - Add UPDATE_PARAMS

3. ? **Performance Dashboard** (v2.3)
   - Equity curve chart
   - Win rate statistics
   - Drawdown graph

---

## ?? **START TESTING NOW!**

**Run**: Press `F5` or execute:
```
bin\Release\net8.0-windows\AKHENS TRADER.exe
```

**Then follow Phase 1 ? Phase 2 ? Phase 3 ? Phase 4!**

---

**Good luck! The bridge is ready to rock!** ??
