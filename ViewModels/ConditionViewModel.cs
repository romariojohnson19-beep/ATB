using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AKHENS_TRADER.Models;

namespace AKHENS_TRADER.ViewModels
{
    /// <summary>
    /// ViewModel wrapper for IndicatorCondition to enable MVVM binding
    /// </summary>
    public partial class ConditionViewModel : ObservableObject
    {
        private readonly IndicatorCondition _condition;

        /// <summary>
        /// Event raised when condition should be removed
        /// </summary>
        public event Action<ConditionViewModel>? RemoveRequested;

        #region Observable Properties

        [ObservableProperty]
        private IndicatorType type;

        [ObservableProperty]
        private int period;

        [ObservableProperty]
        private double level;

        [ObservableProperty]
        private ComparisonOperator @operator;

        [ObservableProperty]
        private bool isAnd;

        [ObservableProperty]
        private int slowEMA;

        [ObservableProperty]
        private int fastEMA;

        [ObservableProperty]
        private int signalSMA;

        [ObservableProperty]
        private double deviation;

        [ObservableProperty]
        private int kPeriod;

        [ObservableProperty]
        private int dPeriod;

        [ObservableProperty]
        private int slowing;

        [ObservableProperty]
        private string timeframe;

        /// <summary>
        /// Available timeframes for MT5
        /// </summary>
        public List<string> AvailableTimeframes { get; } = new List<string>
        {
            "PERIOD_CURRENT",
            "PERIOD_M1",
            "PERIOD_M5",
            "PERIOD_M15",
            "PERIOD_M30",
            "PERIOD_H1",
            "PERIOD_H4",
            "PERIOD_D1",
            "PERIOD_W1",
            "PERIOD_MN1"
        };

        #endregion

        #region Constructor

        public ConditionViewModel(IndicatorCondition condition)
        {
            _condition = condition;

            // Initialize from model
            Type = condition.Type;
            Period = condition.Period;
            Level = condition.Level;
            Operator = condition.Operator;
            IsAnd = condition.IsAnd;
            Timeframe = condition.Timeframe;
            SlowEMA = condition.SlowEMA;
            FastEMA = condition.FastEMA;
            SignalSMA = condition.SignalSMA;
            Deviation = condition.Deviation;
            KPeriod = condition.KPeriod;
            DPeriod = condition.DPeriod;
            Slowing = condition.Slowing;

            // Subscribe to property changes to update the model
            PropertyChanged += (s, e) => UpdateModel();

        }

        #endregion

        #region Methods

        /// <summary>
        /// Update the underlying model with current values
        /// </summary>
        private void UpdateModel()
        {
            _condition.Type = Type;
            _condition.Period = Period;
            _condition.Level = Level;
            _condition.Operator = Operator;
            _condition.IsAnd = IsAnd;
            _condition.Timeframe = Timeframe;
            _condition.SlowEMA = SlowEMA;
            _condition.FastEMA = FastEMA;
            _condition.SignalSMA = SignalSMA;
            _condition.Deviation = Deviation;
            _condition.KPeriod = KPeriod;
            _condition.DPeriod = DPeriod;
            _condition.Slowing = Slowing;
        }


        /// <summary>
        /// Get the underlying indicator condition model
        /// </summary>
        public IndicatorCondition GetCondition() => _condition;

        /// <summary>
        /// Get display name for the condition
        /// </summary>
        public string DisplayName => $"{Type} {GetOperatorSymbol()} {Level:F2}";

        /// <summary>
        /// Get operator symbol for display
        /// </summary>
        private string GetOperatorSymbol()
        {
            return Operator switch
            {
                ComparisonOperator.GreaterThan => ">",
                ComparisonOperator.LessThan => "<",
                ComparisonOperator.CrossAbove => "crosses above",
                ComparisonOperator.CrossBelow => "crosses below",
                ComparisonOperator.Equal => "==",
                ComparisonOperator.NotEqual => "!=",
                _ => "?"
            };
        }

        /// <summary>
        /// Get visibility for indicator-specific parameters
        /// </summary>
        public bool ShowMACDParameters => Type == IndicatorType.MACD;
        public bool ShowBollingerParameters => Type == IndicatorType.BollingerBands;
        public bool ShowStochasticParameters => Type == IndicatorType.Stochastic;
        public bool ShowLevelParameter => Type switch
        {
            IndicatorType.RSI => true,
            IndicatorType.CCI => true,
            IndicatorType.Stochastic => true,
            _ => false
        };

        #endregion

        #region Commands

        /// <summary>
        /// Remove this condition
        /// </summary>
        [RelayCommand]
        private void Remove()
        {
            RemoveRequested?.Invoke(this);
        }

        #endregion

        #region Property Change Handlers

        partial void OnTypeChanged(IndicatorType value)
        {
            // Update visibility flags
            OnPropertyChanged(nameof(ShowMACDParameters));
            OnPropertyChanged(nameof(ShowBollingerParameters));
            OnPropertyChanged(nameof(ShowStochasticParameters));
            OnPropertyChanged(nameof(ShowLevelParameter));

            // Set default values based on indicator type
            switch (value)
            {
                case IndicatorType.RSI:
                    Period = 14;
                    Level = 30.0;
                    Operator = ComparisonOperator.LessThan;
                    break;
                case IndicatorType.MACD:
                    FastEMA = 12;
                    SlowEMA = 26;
                    SignalSMA = 9;
                    Operator = ComparisonOperator.CrossAbove;
                    break;
                case IndicatorType.BollingerBands:
                    Period = 20;
                    Deviation = 2.0;
                    Operator = ComparisonOperator.CrossBelow;
                    break;
                case IndicatorType.Stochastic:
                    KPeriod = 5;
                    DPeriod = 3;
                    Slowing = 3;
                    Level = 20.0;
                    Operator = ComparisonOperator.LessThan;
                    break;
                case IndicatorType.MA:
                case IndicatorType.EMA:
                case IndicatorType.SMA:
                    Period = 20;
                    Operator = ComparisonOperator.CrossAbove;
                    break;
            }
        }

        #endregion
    }
}
