using System.Collections.ObjectModel;
using AKHENS_TRADER.Models;

namespace AKHENS_TRADER.Services
{
    /// <summary>
    /// Service providing predefined trading strategies
    /// </summary>
    public class PreloadedStrategiesService
    {
        /// <summary>
        /// Get all preloaded strategies
        /// </summary>
        public List<PreloadedStrategyInfo> GetAllStrategies()
        {
            return
            [
                GetMACrossoverStrategy(),
                GetRSIOversoldStrategy(),
                GetBollingerBounceStrategy(),
                GetMACDSignalStrategy(),
                GetStochasticStrategy(),
                GetATRBreakoutStrategy(),
                GetCCIExtremeStrategy(),
                GetMultiConfirmationStrategy()
            ];
        }

        /// <summary>
        /// Get strategy by name
        /// </summary>
        public PreloadedStrategyInfo? GetStrategyByName(string name)
        {
            return GetAllStrategies().FirstOrDefault(s => s.Name == name);
        }

        #region Strategy Definitions

        /// <summary>
        /// 1. MA Crossover Strategy
        /// </summary>
        private static PreloadedStrategyInfo GetMACrossoverStrategy()
        {
            Strategy strategy = new()
            {
                Name = "MA Crossover Strategy",
                Description = "Classic moving average crossover: Fast EMA(9) crosses above/below Slow EMA(21)",
                IsPreloaded = true,
                RiskSettings = new()
                {
                    RiskPercentPerTrade = 1.0,
                    StopLossPips = 40,
                    TakeProfitPips = 80,
                    UseTrailingStop = false
                },
                EntryConditions = [new()
                {
                    Type = IndicatorType.EMA,
                    Period = 9,
                    Operator = ComparisonOperator.CrossAbove,
                    Level = 0, // Will compare with another MA
                    IsAnd = true
                }],
                ExitConditions = [new()
                {
                    Type = IndicatorType.EMA,
                    Period = 9,
                    Operator = ComparisonOperator.CrossBelow,
                    Level = 0,
                    IsAnd = true
                }]
            };

            return new()
            {
                Name = strategy.Name,
                Description = strategy.Description,
                Category = "Trend Following",
                Difficulty = "Beginner",
                Strategy = strategy
            };
        }

        /// <summary>
        /// 2. RSI Oversold/Overbought Strategy
        /// </summary>
        private static PreloadedStrategyInfo GetRSIOversoldStrategy()
        {
            var strategy = new Strategy
            {
                Name = "RSI Oversold Strategy",
                Description = "Buy when RSI drops below 30 (oversold), sell when RSI rises above 70 (overbought)",
                IsPreloaded = true,
                RiskSettings = new RiskManagement
                {
                    RiskPercentPerTrade = 1.0,
                    StopLossPips = 50,
                    TakeProfitPips = 100,
                    UseTrailingStop = false
                },
                EntryConditions = new ObservableCollection<IndicatorCondition>
                {
                    new IndicatorCondition
                    {
                        Type = IndicatorType.RSI,
                        Period = 14,
                        Operator = ComparisonOperator.LessThan,
                        Level = 30.0,
                        IsAnd = true
                    }
                },
                ExitConditions = new ObservableCollection<IndicatorCondition>
                {
                    new IndicatorCondition
                    {
                        Type = IndicatorType.RSI,
                        Period = 14,
                        Operator = ComparisonOperator.GreaterThan,
                        Level = 70.0,
                        IsAnd = true
                    }
                }
            };

            return new PreloadedStrategyInfo
            {
                Name = strategy.Name,
                Description = strategy.Description,
                Category = "Mean Reversion",
                Difficulty = "Beginner",
                Strategy = strategy
            };
        }

        /// <summary>
        /// 3. Bollinger Band Bounce Strategy
        /// </summary>
        private static PreloadedStrategyInfo GetBollingerBounceStrategy()
        {
            var strategy = new Strategy
            {
                Name = "Bollinger Band Bounce",
                Description = "Buy when price touches lower Bollinger Band, exit at middle band",
                IsPreloaded = true,
                RiskSettings = new RiskManagement
                {
                    RiskPercentPerTrade = 1.0,
                    StopLossPips = 35,
                    TakeProfitPips = 70,
                    UseTrailingStop = true,
                    TrailingStopPips = 25
                },
                EntryConditions = new ObservableCollection<IndicatorCondition>
                {
                    new IndicatorCondition
                    {
                        Type = IndicatorType.BollingerBands,
                        Period = 20,
                        Deviation = 2.0,
                        Operator = ComparisonOperator.CrossBelow,
                        Level = 0, // Lower band
                        IsAnd = true
                    }
                },
                ExitConditions = new ObservableCollection<IndicatorCondition>
                {
                    new IndicatorCondition
                    {
                        Type = IndicatorType.SMA,
                        Period = 20,
                        Operator = ComparisonOperator.CrossAbove,
                        Level = 0,
                        IsAnd = true
                    }
                }
            };

            return new PreloadedStrategyInfo
            {
                Name = strategy.Name,
                Description = strategy.Description,
                Category = "Mean Reversion",
                Difficulty = "Intermediate",
                Strategy = strategy
            };
        }

        /// <summary>
        /// 4. MACD Signal Strategy
        /// </summary>
        private static PreloadedStrategyInfo GetMACDSignalStrategy()
        {
            var strategy = new Strategy
            {
                Name = "MACD Signal Strategy",
                Description = "Enter when MACD crosses above signal line with bullish EMA trend",
                IsPreloaded = true,
                RiskSettings = new RiskManagement
                {
                    RiskPercentPerTrade = 1.0,
                    StopLossPips = 45,
                    TakeProfitPips = 90,
                    UseTrailingStop = false
                },
                EntryConditions = new ObservableCollection<IndicatorCondition>
                {
                    new IndicatorCondition
                    {
                        Type = IndicatorType.MACD,
                        FastEMA = 12,
                        SlowEMA = 26,
                        SignalSMA = 9,
                        Operator = ComparisonOperator.CrossAbove,
                        Level = 0,
                        IsAnd = true
                    },
                    new IndicatorCondition
                    {
                        Type = IndicatorType.EMA,
                        Period = 50,
                        Operator = ComparisonOperator.GreaterThan,
                        Level = 0,
                        IsAnd = true
                    }
                },
                ExitConditions = new ObservableCollection<IndicatorCondition>
                {
                    new IndicatorCondition
                    {
                        Type = IndicatorType.MACD,
                        FastEMA = 12,
                        SlowEMA = 26,
                        SignalSMA = 9,
                        Operator = ComparisonOperator.CrossBelow,
                        Level = 0,
                        IsAnd = true
                    }
                }
            };

            return new PreloadedStrategyInfo
            {
                Name = strategy.Name,
                Description = strategy.Description,
                Category = "Trend Following",
                Difficulty = "Intermediate",
                Strategy = strategy
            };
        }

        /// <summary>
        /// 5. Stochastic Overbought/Oversold Strategy
        /// </summary>
        private static PreloadedStrategyInfo GetStochasticStrategy()
        {
            var strategy = new Strategy
            {
                Name = "Stochastic Extreme Strategy",
                Description = "Trade reversals when Stochastic enters oversold (<20) or overbought (>80) zones",
                IsPreloaded = true,
                RiskSettings = new RiskManagement
                {
                    RiskPercentPerTrade = 1.0,
                    StopLossPips = 40,
                    TakeProfitPips = 80,
                    UseTrailingStop = false
                },
                EntryConditions = new ObservableCollection<IndicatorCondition>
                {
                    new IndicatorCondition
                    {
                        Type = IndicatorType.Stochastic,
                        KPeriod = 5,
                        DPeriod = 3,
                        Slowing = 3,
                        Operator = ComparisonOperator.LessThan,
                        Level = 20.0,
                        IsAnd = true
                    }
                },
                ExitConditions = new ObservableCollection<IndicatorCondition>
                {
                    new IndicatorCondition
                    {
                        Type = IndicatorType.Stochastic,
                        KPeriod = 5,
                        DPeriod = 3,
                        Slowing = 3,
                        Operator = ComparisonOperator.GreaterThan,
                        Level = 80.0,
                        IsAnd = true
                    }
                }
            };

            return new PreloadedStrategyInfo
            {
                Name = strategy.Name,
                Description = strategy.Description,
                Category = "Mean Reversion",
                Difficulty = "Intermediate",
                Strategy = strategy
            };
        }

        /// <summary>
        /// 6. ATR Volatility Breakout Strategy
        /// </summary>
        private static PreloadedStrategyInfo GetATRBreakoutStrategy()
        {
            var strategy = new Strategy
            {
                Name = "ATR Volatility Breakout",
                Description = "Enter when ATR increases significantly, indicating high volatility breakout",
                IsPreloaded = true,
                RiskSettings = new RiskManagement
                {
                    RiskPercentPerTrade = 1.5,
                    StopLossPips = 60,
                    TakeProfitPips = 120,
                    UseTrailingStop = true,
                    TrailingStopPips = 40
                },
                EntryConditions = new ObservableCollection<IndicatorCondition>
                {
                    new IndicatorCondition
                    {
                        Type = IndicatorType.ATR,
                        Period = 14,
                        Operator = ComparisonOperator.GreaterThan,
                        Level = 0.0015, // Threshold depends on symbol
                        IsAnd = true
                    },
                    new IndicatorCondition
                    {
                        Type = IndicatorType.EMA,
                        Period = 20,
                        Operator = ComparisonOperator.GreaterThan,
                        Level = 0,
                        IsAnd = true
                    }
                },
                ExitConditions = new ObservableCollection<IndicatorCondition>
                {
                    new IndicatorCondition
                    {
                        Type = IndicatorType.ATR,
                        Period = 14,
                        Operator = ComparisonOperator.LessThan,
                        Level = 0.0008,
                        IsAnd = true
                    }
                }
            };

            return new PreloadedStrategyInfo
            {
                Name = strategy.Name,
                Description = strategy.Description,
                Category = "Breakout",
                Difficulty = "Advanced",
                Strategy = strategy
            };
        }

        /// <summary>
        /// 7. CCI Extreme Strategy
        /// </summary>
        private static PreloadedStrategyInfo GetCCIExtremeStrategy()
        {
            var strategy = new Strategy
            {
                Name = "CCI Extreme Strategy",
                Description = "Trade reversals when CCI reaches extreme levels (+200/-200)",
                IsPreloaded = true,
                RiskSettings = new RiskManagement
                {
                    RiskPercentPerTrade = 1.0,
                    StopLossPips = 50,
                    TakeProfitPips = 100,
                    UseTrailingStop = false
                },
                EntryConditions = new ObservableCollection<IndicatorCondition>
                {
                    new IndicatorCondition
                    {
                        Type = IndicatorType.CCI,
                        Period = 14,
                        Operator = ComparisonOperator.LessThan,
                        Level = -200.0,
                        IsAnd = true
                    }
                },
                ExitConditions = new ObservableCollection<IndicatorCondition>
                {
                    new IndicatorCondition
                    {
                        Type = IndicatorType.CCI,
                        Period = 14,
                        Operator = ComparisonOperator.GreaterThan,
                        Level = 200.0,
                        IsAnd = true
                    }
                }
            };

            return new PreloadedStrategyInfo
            {
                Name = strategy.Name,
                Description = strategy.Description,
                Category = "Mean Reversion",
                Difficulty = "Intermediate",
                Strategy = strategy
            };
        }

        /// <summary>
        /// 8. Multi-Confirmation Strategy (RSI + MACD + EMA)
        /// </summary>
        private static PreloadedStrategyInfo GetMultiConfirmationStrategy()
        {
            var strategy = new Strategy
            {
                Name = "Multi-Confirmation Strategy",
                Description = "Triple confirmation: RSI oversold, MACD bullish crossover, and price above EMA(50)",
                IsPreloaded = true,
                RiskSettings = new RiskManagement
                {
                    RiskPercentPerTrade = 1.0,
                    StopLossPips = 55,
                    TakeProfitPips = 110,
                    UseTrailingStop = true,
                    TrailingStopPips = 35
                },
                EntryConditions = new ObservableCollection<IndicatorCondition>
                {
                    new IndicatorCondition
                    {
                        Type = IndicatorType.RSI,
                        Period = 14,
                        Operator = ComparisonOperator.LessThan,
                        Level = 35.0,
                        IsAnd = true
                    },
                    new IndicatorCondition
                    {
                        Type = IndicatorType.MACD,
                        FastEMA = 12,
                        SlowEMA = 26,
                        SignalSMA = 9,
                        Operator = ComparisonOperator.GreaterThan,
                        Level = 0,
                        IsAnd = true
                    },
                    new IndicatorCondition
                    {
                        Type = IndicatorType.EMA,
                        Period = 50,
                        Operator = ComparisonOperator.GreaterThan,
                        Level = 0,
                        IsAnd = true
                    }
                },
                ExitConditions = new ObservableCollection<IndicatorCondition>
                {
                    new IndicatorCondition
                    {
                        Type = IndicatorType.RSI,
                        Period = 14,
                        Operator = ComparisonOperator.GreaterThan,
                        Level = 65.0,
                        IsAnd = false // OR
                    },
                    new IndicatorCondition
                    {
                        Type = IndicatorType.MACD,
                        FastEMA = 12,
                        SlowEMA = 26,
                        SignalSMA = 9,
                        Operator = ComparisonOperator.CrossBelow,
                        Level = 0,
                        IsAnd = true
                    }
                }
            };

            return new PreloadedStrategyInfo
            {
                Name = strategy.Name,
                Description = strategy.Description,
                Category = "Confirmation",
                Difficulty = "Advanced",
                Strategy = strategy
            };
        }

        #endregion
    }

    /// <summary>
    /// Preloaded strategy information with metadata
    /// </summary>
    public class PreloadedStrategyInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public Strategy Strategy { get; set; } = new();
    }
}
