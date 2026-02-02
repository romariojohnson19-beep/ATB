using System.Collections.ObjectModel;

namespace AkhenTraderElite.Models
{
    /// <summary>
    /// Represents a live trading position from MT5
    /// </summary>
    public class LivePosition
    {
        public long Ticket { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public string Type { get; set; } = "BUY"; // BUY or SELL
        public double Lots { get; set; }
        public double OpenPrice { get; set; }
        public double CurrentPrice { get; set; }
        public double StopLoss { get; set; }
        public double TakeProfit { get; set; }
        public double Profit { get; set; }
        public DateTime OpenTime { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    /// <summary>
    /// Account information from MT5
    /// </summary>
    public class AccountInfo
    {
        public double Balance { get; set; }
        public double Equity { get; set; }
        public double Margin { get; set; }
        public double FreeMargin { get; set; }
        public double DailyDrawdown { get; set; }
        public double TotalDrawdown { get; set; }
        public int OpenPositions { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    /// <summary>
    /// EA status information
    /// </summary>
    public class EAStatus
    {
        public bool IsConnected { get; set; }
        public bool IsTradingEnabled { get; set; }
        public string EAName { get; set; } = "Bridge EA";
        public string EAVersion { get; set; } = "1.0";
        public DateTime LastHeartbeat { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// Command to send to MT5 EA
    /// </summary>
    public class EACommand
    {
        public string Action { get; set; } = string.Empty; // START, STOP, UPDATE_PARAMS, CLOSE_ALL, etc.
        public Dictionary<string, object> Parameters { get; set; } = new();
    }

    /// <summary>
    /// Response from MT5 EA
    /// </summary>
    public class EAResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, object> Data { get; set; } = new();
    }

    /// <summary>
    /// Trading parameters that can be updated live
    /// </summary>
    public class LiveTradingParameters
    {
        public double StopLossPips { get; set; }
        public double TakeProfitPips { get; set; }
        public double LotSize { get; set; }
        public double RiskPercent { get; set; }
        public int MaxOpenTrades { get; set; }
        public bool UseTrailingStop { get; set; }
        public double TrailingStopPips { get; set; }
        public bool EnableTrading { get; set; }
    }
}
