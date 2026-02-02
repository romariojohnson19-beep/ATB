using System.IO;
using System.Text;
using Microsoft.Win32;
using MaterialDesignThemes.Wpf;
using AkhenTraderElite.Models;

namespace AkhenTraderElite.Services
{
    /// <summary>
    /// Service responsible for exporting generated code and strategy files
    /// </summary>
    public class FileExportService
    {
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;

        public FileExportService(ISnackbarMessageQueue snackbarMessageQueue)
        {
            _snackbarMessageQueue = snackbarMessageQueue;
        }

        /// <summary>
        /// Save MQL5 code to a .mq5 file
        /// </summary>
        /// <param name="code">The MQL5 source code to save</param>
        /// <param name="defaultFileName">Default filename for the save dialog</param>
        public async Task SaveMq5Async(string code, string defaultFileName = "MyStrategyEA.mq5")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    _snackbarMessageQueue.Enqueue("No code to save. Please generate a strategy first.");
                    return;
                }

                var saveFileDialog = new SaveFileDialog
                {
                    Title = "Save MQL5 Expert Advisor",
                    FileName = defaultFileName,
                    Filter = "MQL5 Files (*.mq5)|*.mq5|All Files (*.*)|*.*",
                    DefaultExt = ".mq5",
                    AddExtension = true
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    // Save file asynchronously
                    await File.WriteAllTextAsync(saveFileDialog.FileName, code, Encoding.UTF8);

                    _snackbarMessageQueue.Enqueue(
                        $"? Successfully saved: {Path.GetFileName(saveFileDialog.FileName)}",
                        "OPEN FOLDER",
                        () => OpenFileLocation(saveFileDialog.FileName));
                }
            }
            catch (Exception ex)
            {
                _snackbarMessageQueue.Enqueue($"? Error saving .mq5 file: {ex.Message}");
            }
        }

        /// <summary>
        /// Save strategy parameters to a .set file (MT5 parameter file format)
        /// </summary>
        /// <param name="strategy">The strategy configuration</param>
        /// <param name="preset">The prop firm preset</param>
        /// <param name="defaultFileName">Default filename for the save dialog</param>
        public async Task SaveSetFileAsync(Strategy strategy, PropFirmPreset preset, string defaultFileName = "MyStrategy.set")
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Title = "Save MT5 Parameter File",
                    FileName = defaultFileName,
                    Filter = "SET Files (*.set)|*.set|All Files (*.*)|*.*",
                    DefaultExt = ".set",
                    AddExtension = true
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    // Generate .set file content
                    var setContent = GenerateSetFileContent(strategy, preset);

                    // Save file asynchronously
                    await File.WriteAllTextAsync(saveFileDialog.FileName, setContent, Encoding.UTF8);

                    _snackbarMessageQueue.Enqueue(
                        $"? Successfully saved: {Path.GetFileName(saveFileDialog.FileName)}",
                        "OPEN FOLDER",
                        () => OpenFileLocation(saveFileDialog.FileName));
                }
            }
            catch (Exception ex)
            {
                _snackbarMessageQueue.Enqueue($"? Error saving .set file: {ex.Message}");
            }
        }

        /// <summary>
        /// Export complete project folder with .mq5 file, .set file, and readme
        /// </summary>
        /// <param name="code">The MQL5 source code</param>
        /// <param name="strategy">The strategy configuration</param>
        /// <param name="preset">The prop firm preset</param>
        public async Task ExportProjectFolderAsync(string code, Strategy strategy, PropFirmPreset preset)
        {
            try
            {
                // Create folder browser dialog
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Title = "Select Export Location",
                    FileName = "SelectFolder",
                    Filter = "Folder Selection|*.folder",
                    CheckFileExists = false
                };

                if (dialog.ShowDialog() == true)
                {
                    // Get directory path
                    var folderPath = Path.GetDirectoryName(dialog.FileName);
                    if (string.IsNullOrEmpty(folderPath))
                        return;

                    // Create project subfolder
                    var projectName = strategy.Name.Replace(" ", "_").Replace("/", "_").Replace("\\", "_");
                    var projectFolder = Path.Combine(folderPath, projectName);
                    Directory.CreateDirectory(projectFolder);

                    // Save .mq5 file
                    var mq5Path = Path.Combine(projectFolder, $"{projectName}.mq5");
                    await File.WriteAllTextAsync(mq5Path, code, Encoding.UTF8);

                    // Save .set file
                    var setPath = Path.Combine(projectFolder, $"{projectName}.set");
                    var setContent = GenerateSetFileContent(strategy, preset);
                    await File.WriteAllTextAsync(setPath, setContent, Encoding.UTF8);

                    // Create README
                    var readmePath = Path.Combine(projectFolder, "README.txt");
                    var readmeContent = GenerateReadmeContent(strategy, preset);
                    await File.WriteAllTextAsync(readmePath, readmeContent, Encoding.UTF8);

                    _snackbarMessageQueue.Enqueue(
                        $"? Project exported to: {projectName}",
                        "OPEN FOLDER",
                        () => OpenFolder(projectFolder));
                }
            }
            catch (Exception ex)
            {
                _snackbarMessageQueue.Enqueue($"? Error exporting project: {ex.Message}");
            }
        }

        #region Helper Methods

        /// <summary>
        /// Generate .set file content in MT5 format
        /// </summary>
        private static string GenerateSetFileContent(Strategy strategy, PropFirmPreset preset)
        {
            var sb = new StringBuilder();

            sb.AppendLine(";");
            sb.AppendLine($"; MT5 Parameter File for: {strategy.Name}");
            sb.AppendLine($"; Generated by Prop Strategy Builder");
            sb.AppendLine($"; Date: {DateTime.Now:yyyy.MM.dd HH:mm:ss}");
            sb.AppendLine(";");
            sb.AppendLine();

            // Risk Management Parameters
            sb.AppendLine(";--- Risk Management ---");
            sb.AppendLine($"RiskPercent={strategy.RiskSettings.RiskPercentPerTrade}");
            sb.AppendLine($"StopLossPips={strategy.RiskSettings.StopLossPips}");
            sb.AppendLine($"TakeProfitPips={strategy.RiskSettings.TakeProfitPips}");
            sb.AppendLine($"UseTrailingStop={strategy.RiskSettings.UseTrailingStop}");
            
            if (strategy.RiskSettings.UseTrailingStop)
            {
                sb.AppendLine($"TrailingStopPips={strategy.RiskSettings.TrailingStopPips}");
            }
            
            sb.AppendLine();

            // Prop Firm Settings
            sb.AppendLine(";--- Prop Firm Settings ---");
            sb.AppendLine($"MaxDailyDrawdown={preset.DailyDrawdownPercent}");
            sb.AppendLine($"MaxTotalDrawdown={preset.MaxDrawdownPercent}");
            sb.AppendLine($"MaxOpenTrades={preset.MaxOpenTrades}");
            sb.AppendLine($"MagicNumber={preset.MagicNumber}");
            sb.AppendLine();

            // Strategy Parameters (placeholder - will be enhanced when custom conditions are added)
            sb.AppendLine(";--- Strategy Parameters ---");
            sb.AppendLine("MA_Period=20");
            sb.AppendLine("RSI_Period=14");
            sb.AppendLine("RSI_Overbought=70.0");
            sb.AppendLine("RSI_Oversold=30.0");

            return sb.ToString();
        }

        /// <summary>
        /// Generate README content for the exported project
        /// </summary>
        private static string GenerateReadmeContent(Strategy strategy, PropFirmPreset preset)
        {
            var sb = new StringBuilder();

            sb.AppendLine("=================================================================");
            sb.AppendLine($"  {strategy.Name}");
            sb.AppendLine("  Generated by Prop Strategy Builder");
            sb.AppendLine($"  {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine("=================================================================");
            sb.AppendLine();

            sb.AppendLine("STRATEGY OVERVIEW");
            sb.AppendLine("-----------------");
            sb.AppendLine($"Name: {strategy.Name}");
            sb.AppendLine($"Description: {strategy.Description}");
            sb.AppendLine($"Target Prop Firm: {preset.FirmName}");
            sb.AppendLine();

            sb.AppendLine("RISK MANAGEMENT");
            sb.AppendLine("---------------");
            sb.AppendLine($"Risk per Trade: {strategy.RiskSettings.RiskPercentPerTrade}%");
            sb.AppendLine($"Stop Loss: {strategy.RiskSettings.StopLossPips} pips");
            sb.AppendLine($"Take Profit: {strategy.RiskSettings.TakeProfitPips} pips");
            sb.AppendLine($"Trailing Stop: {(strategy.RiskSettings.UseTrailingStop ? $"Enabled ({strategy.RiskSettings.TrailingStopPips} pips)" : "Disabled")}");
            sb.AppendLine();

            sb.AppendLine("PROP FIRM COMPLIANCE");
            sb.AppendLine("--------------------");
            sb.AppendLine($"Daily Drawdown Limit: {preset.DailyDrawdownPercent}%");
            sb.AppendLine($"Max Drawdown Limit: {preset.MaxDrawdownPercent}%");
            sb.AppendLine($"Max Open Trades: {preset.MaxOpenTrades}");
            sb.AppendLine($"Magic Number: {preset.MagicNumber}");
            sb.AppendLine($"Visible SL/TP: {(preset.EnforceVisibleSLTP ? "Enforced" : "Not Enforced")}");
            sb.AppendLine($"Drawdown Monitoring: {(preset.EnableDrawdownMonitoring ? "Enabled" : "Disabled")}");
            sb.AppendLine();

            sb.AppendLine("FILES INCLUDED");
            sb.AppendLine("--------------");
            sb.AppendLine($"1. {strategy.Name.Replace(" ", "_")}.mq5 - Expert Advisor source code");
            sb.AppendLine($"2. {strategy.Name.Replace(" ", "_")}.set - MT5 parameter file");
            sb.AppendLine("3. README.txt - This file");
            sb.AppendLine();

            sb.AppendLine("INSTALLATION INSTRUCTIONS");
            sb.AppendLine("-------------------------");
            sb.AppendLine("1. Open MT5 MetaEditor (press F4 in MetaTrader 5)");
            sb.AppendLine("2. File ? Open ? Select the .mq5 file");
            sb.AppendLine("3. Click 'Compile' button (F7) to compile the EA");
            sb.AppendLine("4. Close MetaEditor and return to MT5");
            sb.AppendLine("5. Open Navigator (Ctrl+N) ? Expert Advisors");
            sb.AppendLine("6. Drag the EA onto a chart");
            sb.AppendLine("7. Right-click EA on chart ? EA Properties ? Inputs tab");
            sb.AppendLine("8. Click 'Load' and select the .set file");
            sb.AppendLine("9. Review settings and click OK");
            sb.AppendLine();

            sb.AppendLine("IMPORTANT WARNINGS");
            sb.AppendLine("------------------");
            sb.AppendLine("?? Always test on a demo account first!");
            sb.AppendLine("?? Review all parameters before live trading");
            sb.AppendLine("?? Ensure your prop firm rules match the EA settings");
            sb.AppendLine("?? Monitor the EA closely, especially during news events");
            sb.AppendLine("?? This EA is generated code - verify logic before use");
            sb.AppendLine();

            sb.AppendLine("DISCLAIMER");
            sb.AppendLine("----------");
            sb.AppendLine("This Expert Advisor is provided as-is without warranty.");
            sb.AppendLine("Trading involves risk. Past performance does not guarantee future results.");
            sb.AppendLine("Use at your own risk. The author is not responsible for any losses.");
            sb.AppendLine();

            sb.AppendLine("=================================================================");

            return sb.ToString();
        }

        /// <summary>
        /// Open the folder containing the exported file
        /// </summary>
        private static void OpenFileLocation(string filePath)
        {
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                {
                    System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{filePath}\"");
                }
            }
            catch
            {
                // Silently fail if can't open folder
            }
        }

        /// <summary>
        /// Open a folder in Windows Explorer
        /// </summary>
        private static void OpenFolder(string folderPath)
        {
            try
            {
                if (Directory.Exists(folderPath))
                {
                    System.Diagnostics.Process.Start("explorer.exe", folderPath);
                }
            }
            catch
            {
                // Silently fail if can't open folder
            }
        }

        #endregion
    }
}
