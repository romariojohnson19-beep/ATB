//+------------------------------------------------------------------+
//|                                            AkhenTraderBridge.mq5 |
//|                    Akhen Trader Elite - HTTP Bridge EA for v2.1 |
//|                       Connects MT5 to Akhen Trader Elite WPF App |
//+------------------------------------------------------------------+
#property copyright "FuglyMan TokenArena 2026"
#property link      "https://github.com/romariojohnson19-beep/ATB"
#property version   "2.10"
#property description "Bridge EA for real-time communication with Akhen Trader Elite"
#property strict

//--- Input parameters
input string    BridgeServerURL = "http://127.0.0.1:8080";  // Bridge Server URL (use IP instead of localhost)
input int       HeartbeatInterval = 5;                       // Heartbeat interval (seconds)
input int       UpdateInterval = 2;                          // Status update interval (seconds)
input bool      EnableWebRequests = true;                    // Enable WebRequests
input int       MagicNumber = 777777;                        // Magic number for this EA

//--- Global variables
datetime g_lastHeartbeat = 0;
datetime g_lastUpdate = 0;
bool g_isConnected = false;
bool g_tradingEnabled = true;
string g_lastError = "";

//+------------------------------------------------------------------+
//| Expert initialization function                                     |
//+------------------------------------------------------------------+
int OnInit()
{
    Print("=================================================");
    Print("Akhen Trader Elite Bridge EA v2.1");
    Print("=================================================");
    Print("Bridge Server: ", BridgeServerURL);
    Print("Heartbeat Interval: ", HeartbeatInterval, " seconds");
    Print("Magic Number: ", MagicNumber);
    Print("=================================================");
    
    // Note: Make sure WebRequests are enabled in MT5!
    // Go to: Tools -> Options -> Expert Advisors -> Allow WebRequests
    // Add URL: ", BridgeServerURL
    
    // Send initial connection message
    if(SendHeartbeat())
    {
        Print("SUCCESS: Connected to Akhen Trader Elite!");
        g_isConnected = true;
    }
    else
    {
        Print("WARNING: Failed to connect to Akhen Trader Elite.");
        Print("Make sure:");
        Print("1. The WPF app is running");
        Print("2. Bridge server is started (click START SERVER in EA Control tab)");
        Print("3. WebRequests are enabled in MT5 Options");
        Print("4. URL is whitelisted: ", BridgeServerURL);
        // Don't fail initialization - just try again on next tick
    }
    
    return INIT_SUCCEEDED;
}

//+------------------------------------------------------------------+
//| Expert deinitialization function                                   |
//+------------------------------------------------------------------+
void OnDeinit(const int reason)
{
    Print("Bridge EA stopped. Reason: ", reason);
}

//+------------------------------------------------------------------+
//| Expert tick function                                               |
//+------------------------------------------------------------------+
void OnTick()
{
    // Send heartbeat periodically
    if(TimeCurrent() - g_lastHeartbeat >= HeartbeatInterval)
    {
        SendHeartbeat();
        g_lastHeartbeat = TimeCurrent();
    }
    
    // Send status update periodically
    if(TimeCurrent() - g_lastUpdate >= UpdateInterval)
    {
        SendStatusUpdate();
        SendAccountInfo();
        SendPositionsUpdate();
        g_lastUpdate = TimeCurrent();
    }
}

//+------------------------------------------------------------------+
//| Send heartbeat to keep connection alive                           |
//+------------------------------------------------------------------+
bool SendHeartbeat()
{
    string url = BridgeServerURL + "/heartbeat";
    string jsonData = "{\"timestamp\":\"" + TimeToString(TimeCurrent(), TIME_DATE|TIME_SECONDS) + "\"}";
    
    char data[];
    char result[];
    string headers;
    StringToCharArray(jsonData, data, 0, StringLen(jsonData));
    
    int timeout = 3000; // 3 seconds
    int res = WebRequest("POST", url, "Content-Type: application/json\r\n", timeout, data, result, headers);
    
    if(res == 200)
    {
        g_isConnected = true;
        return true;
    }
    else if(res == -1)
    {
        // WebRequest error - likely not whitelisted or disabled
        int error = GetLastError();
        g_lastError = "WebRequest error: " + IntegerToString(error);
        if(error == 4060) // ERR_FUNCTION_NOT_ALLOWED
        {
            Print("ERROR: WebRequest not allowed! Add URL to whitelist: ", BridgeServerURL);
            Print("Go to: Tools -> Options -> Expert Advisors -> Allow WebRequest for listed URL");
        }
        g_isConnected = false;
        return false;
    }
    else
    {
        g_isConnected = false;
        g_lastError = "HTTP error: " + IntegerToString(res);
        return false;
    }
}

//+------------------------------------------------------------------+
//| Send EA status to WPF app                                         |
//+------------------------------------------------------------------+
bool SendStatusUpdate()
{
    string url = BridgeServerURL + "/status";
    
    // Build JSON status
    string json = "{";
    json += "\"IsConnected\":" + (g_isConnected ? "true" : "false") + ",";
    json += "\"IsTradingEnabled\":" + (g_tradingEnabled ? "true" : "false") + ",";
    json += "\"EAName\":\"Akhen Trader Bridge\",";
    json += "\"EAVersion\":\"2.10\",";
    json += "\"LastHeartbeat\":\"" + TimeToString(TimeCurrent(), TIME_DATE|TIME_SECONDS) + "\",";
    json += "\"ErrorMessage\":\"" + g_lastError + "\"";
    json += "}";
    
    char data[];
    char result[];
    string headers;
    StringToCharArray(json, data, 0, StringLen(json));
    
    int timeout = 3000;
    int res = WebRequest("POST", url, "Content-Type: application/json\r\n", timeout, data, result, headers);
    
    return (res == 200);
}

//+------------------------------------------------------------------+
//| Send account information to WPF app                               |
//+------------------------------------------------------------------+
bool SendAccountInfo()
{
    string url = BridgeServerURL + "/account";
    
    double balance = AccountInfoDouble(ACCOUNT_BALANCE);
    double equity = AccountInfoDouble(ACCOUNT_EQUITY);
    double margin = AccountInfoDouble(ACCOUNT_MARGIN);
    double freeMargin = AccountInfoDouble(ACCOUNT_MARGIN_FREE);
    
    // Calculate drawdown (simplified)
    double dailyDD = 0;
    double totalDD = 0;
    if(balance > 0)
    {
        dailyDD = ((balance - equity) / balance) * 100.0;
        totalDD = dailyDD; // For now, same as daily
    }
    
    int openPos = PositionsTotal();
    
    // Build JSON
    string json = "{";
    json += "\"Balance\":" + DoubleToString(balance, 2) + ",";
    json += "\"Equity\":" + DoubleToString(equity, 2) + ",";
    json += "\"Margin\":" + DoubleToString(margin, 2) + ",";
    json += "\"FreeMargin\":" + DoubleToString(freeMargin, 2) + ",";
    json += "\"DailyDrawdown\":" + DoubleToString(dailyDD, 2) + ",";
    json += "\"TotalDrawdown\":" + DoubleToString(totalDD, 2) + ",";
    json += "\"OpenPositions\":" + IntegerToString(openPos) + ",";
    json += "\"LastUpdate\":\"" + TimeToString(TimeCurrent(), TIME_DATE|TIME_SECONDS) + "\"";
    json += "}";
    
    char data[];
    char result[];
    string headers;
    StringToCharArray(json, data, 0, StringLen(json));
    
    int timeout = 3000;
    int res = WebRequest("POST", url, "Content-Type: application/json\r\n", timeout, data, result, headers);
    
    return (res == 200);
}

//+------------------------------------------------------------------+
//| Send open positions to WPF app                                    |
//+------------------------------------------------------------------+
bool SendPositionsUpdate()
{
    string url = BridgeServerURL + "/positions";
    
    // Build JSON array of positions
    string json = "[";
    
    int total = PositionsTotal();
    for(int i = 0; i < total; i++)
    {
        ulong ticket = PositionGetTicket(i);
        if(ticket > 0)
        {
            string symbol = PositionGetString(POSITION_SYMBOL);
            ENUM_POSITION_TYPE type = (ENUM_POSITION_TYPE)PositionGetInteger(POSITION_TYPE);
            double lots = PositionGetDouble(POSITION_VOLUME);
            double openPrice = PositionGetDouble(POSITION_PRICE_OPEN);
            double currentPrice = PositionGetDouble(POSITION_PRICE_CURRENT);
            double sl = PositionGetDouble(POSITION_SL);
            double tp = PositionGetDouble(POSITION_TP);
            double profit = PositionGetDouble(POSITION_PROFIT);
            datetime openTime = (datetime)PositionGetInteger(POSITION_TIME);
            string comment = PositionGetString(POSITION_COMMENT);
            
            if(i > 0) json += ",";
            
            json += "{";
            json += "\"Ticket\":" + IntegerToString((long)ticket) + ",";
            json += "\"Symbol\":\"" + symbol + "\",";
            json += "\"Type\":\"" + (type == POSITION_TYPE_BUY ? "BUY" : "SELL") + "\",";
            json += "\"Lots\":" + DoubleToString(lots, 2) + ",";
            json += "\"OpenPrice\":" + DoubleToString(openPrice, 5) + ",";
            json += "\"CurrentPrice\":" + DoubleToString(currentPrice, 5) + ",";
            json += "\"StopLoss\":" + DoubleToString(sl, 5) + ",";
            json += "\"TakeProfit\":" + DoubleToString(tp, 5) + ",";
            json += "\"Profit\":" + DoubleToString(profit, 2) + ",";
            json += "\"OpenTime\":\"" + TimeToString(openTime, TIME_DATE|TIME_SECONDS) + "\",";
            json += "\"Comment\":\"" + comment + "\"";
            json += "}";
        }
    }
    
    json += "]";
    
    char data[];
    char result[];
    string headers;
    StringToCharArray(json, data, 0, StringLen(json));
    
    int timeout = 3000;
    int res = WebRequest("POST", url, "Content-Type: application/json\r\n", timeout, data, result, headers);
    
    return (res == 200);
}

//+------------------------------------------------------------------+
//| Close all positions (for emergency stop)                          |
//+------------------------------------------------------------------+
void CloseAllPositions()
{
    Print("? CLOSING ALL POSITIONS...");
    
    int total = PositionsTotal();
    for(int i = total - 1; i >= 0; i--)
    {
        ulong ticket = PositionGetTicket(i);
        if(ticket > 0)
        {
            MqlTradeRequest request;
            MqlTradeResult result;
            
            ZeroMemory(request);
            ZeroMemory(result);
            
            request.action = TRADE_ACTION_CLOSE_BY;
            request.position = ticket;
            request.magic = MagicNumber;
            
            if(!OrderSend(request, result))
            {
                Print("Failed to close position ", ticket, ": ", result.retcode);
            }
        }
    }
    
    Print("? All positions closed");
}

//+------------------------------------------------------------------+
