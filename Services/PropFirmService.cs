using AkhenTraderElite.Models;

namespace AkhenTraderElite.Services
{
    /// <summary>
    /// Service responsible for managing prop firm presets and configurations
    /// </summary>
    public class PropFirmService
    {
        /// <summary>
        /// Get a predefined prop firm preset by name
        /// </summary>
        /// <param name="firmName">Name of the prop firm</param>
        /// <returns>PropFirmPreset with the firm's default settings</returns>
        public PropFirmPreset GetPreset(string firmName)
        {
            return firmName switch
            {
                "FTMO" => new PropFirmPreset
                {
                    FirmName = "FTMO",
                    DailyDrawdownPercent = 5.0,
                    MaxDrawdownPercent = 10.0,
                    MaxOpenTrades = 3,
                    MagicNumber = 123456,
                    EnforceVisibleSLTP = true,
                    EnableDrawdownMonitoring = true,
                    UseNewsFilter = false
                },

                "FundedNext" => new PropFirmPreset
                {
                    FirmName = "FundedNext",
                    DailyDrawdownPercent = 5.0,
                    MaxDrawdownPercent = 10.0,
                    MaxOpenTrades = 5,
                    MagicNumber = 234567,
                    EnforceVisibleSLTP = true,
                    EnableDrawdownMonitoring = true,
                    UseNewsFilter = false
                },

                "The5%ers" => new PropFirmPreset
                {
                    FirmName = "The5%ers",
                    DailyDrawdownPercent = 4.0,
                    MaxDrawdownPercent = 8.0,
                    MaxOpenTrades = 4,
                    MagicNumber = 345678,
                    EnforceVisibleSLTP = true,
                    EnableDrawdownMonitoring = true,
                    UseNewsFilter = false
                },

                "DNA Funded" => new PropFirmPreset
                {
                    FirmName = "DNA Funded",
                    DailyDrawdownPercent = 5.0,
                    MaxDrawdownPercent = 10.0,
                    MaxOpenTrades = 3,
                    MagicNumber = 456789,
                    EnforceVisibleSLTP = true,
                    EnableDrawdownMonitoring = true,
                    UseNewsFilter = false
                },

                "MyForexFunds" => new PropFirmPreset
                {
                    FirmName = "MyForexFunds",
                    DailyDrawdownPercent = 5.0,
                    MaxDrawdownPercent = 12.0,
                    MaxOpenTrades = 5,
                    MagicNumber = 567890,
                    EnforceVisibleSLTP = true,
                    EnableDrawdownMonitoring = true,
                    UseNewsFilter = false
                },

                "Custom" => new PropFirmPreset
                {
                    FirmName = "Custom",
                    DailyDrawdownPercent = 5.0,
                    MaxDrawdownPercent = 10.0,
                    MaxOpenTrades = 3,
                    MagicNumber = 123456,
                    EnforceVisibleSLTP = true,
                    EnableDrawdownMonitoring = true,
                    UseNewsFilter = false
                },

                _ => GetDefaultPreset()
            };
        }

        /// <summary>
        /// Get a list of all available prop firm names
        /// </summary>
        /// <returns>List of prop firm names</returns>
        public static List<string> GetAvailableFirms()
        {
            return
            [
                "FTMO",
                "FundedNext",
                "The5%ers",
                "DNA Funded",
                "MyForexFunds",
                "Custom"
            ];
        }

        /// <summary>
        /// Get detailed information about a specific prop firm
        /// </summary>
        /// <param name="firmName">Name of the prop firm</param>
        /// <returns>Formatted string with firm details</returns>
        public static string GetFirmDetails(string firmName)
        {
            return firmName switch
            {
                "FTMO" => "FTMO: One of the most popular prop firms. Strict rules: 5% daily DD, 10% max DD. Requires visible SL/TP.",
                "FundedNext" => "FundedNext: Flexible prop firm. 5% daily DD, 10% max DD. Allows up to 5 concurrent trades.",
                "The5%ers" => "The5%ers: Aggressive scaling plan. Tighter limits: 4% daily DD, 8% max DD.",
                "DNA Funded" => "DNA Funded: Standard prop firm. 5% daily DD, 10% max DD. Focus on consistency.",
                "MyForexFunds" => "MyForexFunds: Generous limits. 5% daily DD, 12% max DD. Allows more trading freedom.",
                "Custom" => "Custom: Define your own rules. Adjust all parameters to match your prop firm's requirements.",
                _ => "Unknown firm. Please select a valid prop firm or use Custom settings."
            };
        }

        /// <summary>
        /// Validate if a strategy complies with a prop firm's rules
        /// </summary>
        /// <param name="strategy">The trading strategy</param>
        /// <param name="preset">The prop firm preset</param>
        /// <returns>Tuple of (isValid, errorMessage)</returns>
        public static (bool IsValid, string ErrorMessage) ValidateStrategy(Strategy strategy, PropFirmPreset preset)
        {
            List<string> errors = [];

            // Check if SL and TP are set when required
            if (preset.EnforceVisibleSLTP)
            {
                if (strategy.RiskSettings.StopLossPips <= 0)
                {
                    errors.Add("Stop Loss must be greater than 0 (required by prop firm)");
                }

                if (strategy.RiskSettings.TakeProfitPips <= 0)
                {
                    errors.Add("Take Profit must be greater than 0 (required by prop firm)");
                }
            }

            // Check if risk % is reasonable
            if (strategy.RiskSettings.RiskPercentPerTrade > 5.0)
            {
                errors.Add("Risk per trade > 5% is very aggressive for prop trading");
            }

            if (strategy.RiskSettings.RiskPercentPerTrade <= 0)
            {
                errors.Add("Risk per trade must be greater than 0");
            }

            // Check if TP is larger than SL (risk-reward ratio)
            if (strategy.RiskSettings.TakeProfitPips <= strategy.RiskSettings.StopLossPips)
            {
                errors.Add("Warning: Take Profit should typically be larger than Stop Loss");
            }

            // Check drawdown limits
            if (preset.DailyDrawdownPercent <= 0 || preset.DailyDrawdownPercent > 20)
            {
                errors.Add("Daily drawdown % should be between 0 and 20");
            }

            if (preset.MaxDrawdownPercent <= 0 || preset.MaxDrawdownPercent > 30)
            {
                errors.Add("Max drawdown % should be between 0 and 30");
            }

            if (preset.MaxDrawdownPercent <= preset.DailyDrawdownPercent)
            {
                errors.Add("Max drawdown should be greater than daily drawdown");
            }

            // Check max trades
            if (preset.MaxOpenTrades <= 0 || preset.MaxOpenTrades > 20)
            {
                errors.Add("Max open trades should be between 1 and 20");
            }

            if (errors.Count > 0)
            {
                return (false, string.Join("\n", errors));
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Get the default prop firm preset (FTMO)
        /// </summary>
        /// <returns>Default PropFirmPreset</returns>
        private PropFirmPreset GetDefaultPreset()
        {
            return GetPreset("FTMO");
        }

        /// <summary>
        /// Get recommended settings for a specific account size
        /// </summary>
        /// <param name="accountSize">Account size in currency</param>
        /// <param name="firmName">Name of the prop firm</param>
        /// <returns>Recommended risk settings</returns>
        public RiskManagement GetRecommendedRiskSettings(double accountSize, string firmName)
        {
            var preset = GetPreset(firmName);

            // Conservative settings based on account size and firm rules
            return new RiskManagement
            {
                RiskPercentPerTrade = accountSize switch
                {
                    <= 10000 => 1.0,    // Small accounts: 1%
                    <= 50000 => 0.75,   // Medium accounts: 0.75%
                    <= 100000 => 0.5,   // Large accounts: 0.5%
                    _ => 0.25           // Very large accounts: 0.25%
                },
                StopLossPips = 50,
                TakeProfitPips = 100,
                UseTrailingStop = false,
                TrailingStopPips = 30,
                StopLossType = "Fixed",
                TakeProfitType = "Fixed"
            };
        }
    }
}
