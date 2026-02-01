using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AKHENS_TRADER.Models
{
    /// <summary>
    /// Indicator type enumeration
    /// </summary>
    public enum IndicatorType
    {
        None,
        MA,
        EMA,
        SMA,
        RSI,
        MACD,
        BollingerBands,
        Stochastic,
        ATR,
        CCI
    }

    /// <summary>
    /// Comparison operator for conditions
    /// </summary>
    public enum ComparisonOperator
    {
        GreaterThan,
        LessThan,
        CrossAbove,
        CrossBelow,
        Equal,
        NotEqual
    }

    /// <summary>
    /// Represents a trading strategy configuration
    /// </summary>
    public class Strategy : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private string _description = string.Empty;

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<IndicatorCondition> EntryConditions { get; set; } = new();
        public ObservableCollection<IndicatorCondition> ExitConditions { get; set; } = new();
        public RiskManagement RiskSettings { get; set; } = new();
        public PropFirmPreset? PropFirmSettings { get; set; }
        public bool IsPreloaded { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Represents an indicator-based condition for entry or exit
    /// </summary>
    public class IndicatorCondition
    {
        public IndicatorType Type { get; set; } = IndicatorType.RSI;
        public int Period { get; set; } = 14;
        public int AppliedPrice { get; set; } = 0; // PRICE_CLOSE
        public double Level { get; set; } = 50.0;
        public ComparisonOperator Operator { get; set; } = ComparisonOperator.LessThan;
        public bool IsAnd { get; set; } = true; // AND vs OR for chaining
        public string Timeframe { get; set; } = "PERIOD_CURRENT"; // MT5 timeframe
        
        // Additional parameters for specific indicators
        public int SlowEMA { get; set; } = 26;  // For MACD
        public int FastEMA { get; set; } = 12;  // For MACD
        public int SignalSMA { get; set; } = 9; // For MACD
        public double Deviation { get; set; } = 2.0; // For Bollinger Bands
        public int KPeriod { get; set; } = 5;   // For Stochastic
        public int DPeriod { get; set; } = 3;   // For Stochastic
        public int Slowing { get; set; } = 3;   // For Stochastic
    }

    /// <summary>
    /// Risk management settings
    /// </summary>
    public class RiskManagement : INotifyPropertyChanged
    {
        private double _riskPercentPerTrade = 1.0;
        private int _stopLossPips = 50;
        private int _takeProfitPips = 100;
        private bool _useTrailingStop;
        private int _trailingStopPips = 30;

        public double RiskPercentPerTrade
        {
            get => _riskPercentPerTrade;
            set
            {
                if (_riskPercentPerTrade != value)
                {
                    _riskPercentPerTrade = value;
                    OnPropertyChanged();
                }
            }
        }

        public int StopLossPips
        {
            get => _stopLossPips;
            set
            {
                if (_stopLossPips != value)
                {
                    _stopLossPips = value;
                    OnPropertyChanged();
                }
            }
        }

        public int TakeProfitPips
        {
            get => _takeProfitPips;
            set
            {
                if (_takeProfitPips != value)
                {
                    _takeProfitPips = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool UseTrailingStop
        {
            get => _useTrailingStop;
            set
            {
                if (_useTrailingStop != value)
                {
                    _useTrailingStop = value;
                    OnPropertyChanged();
                }
            }
        }

        public int TrailingStopPips
        {
            get => _trailingStopPips;
            set
            {
                if (_trailingStopPips != value)
                {
                    _trailingStopPips = value;
                    OnPropertyChanged();
                }
            }
        }

        public string StopLossType { get; set; } = "Fixed";
        public string TakeProfitType { get; set; } = "Fixed";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Prop firm preset configuration
    /// </summary>
    public class PropFirmPreset
    {
        public string FirmName { get; set; } = string.Empty;
        public double DailyDrawdownPercent { get; set; } = 5.0;
        public double MaxDrawdownPercent { get; set; } = 10.0;
        public int MaxOpenTrades { get; set; } = 3;
        public int MagicNumber { get; set; } = 123456;
        public bool EnforceVisibleSLTP { get; set; } = true;
        public bool EnableDrawdownMonitoring { get; set; } = true;
        public bool UseNewsFilter { get; set; }
    }
}

