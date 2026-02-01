using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Win32;
using MaterialDesignThemes.Wpf;
using AKHENS_TRADER.Models;

namespace AKHENS_TRADER.Services
{
    /// <summary>
    /// Service for saving and loading strategy configurations as JSON
    /// </summary>
    public class ConfigurationService
    {
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        private readonly JsonSerializerOptions _jsonOptions;

        public ConfigurationService(ISnackbarMessageQueue snackbarMessageQueue)
        {
            _snackbarMessageQueue = snackbarMessageQueue;
            
            // Configure JSON serialization options
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter() }
            };
        }

        /// <summary>
        /// Save complete strategy configuration to JSON file
        /// </summary>
        public async Task SaveConfigurationAsync(StrategyConfiguration config, string? defaultFileName = null)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Title = "Save Strategy Configuration",
                    FileName = defaultFileName ?? $"{config.Strategy.Name.Replace(" ", "_")}_Config.json",
                    Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                    DefaultExt = ".json",
                    AddExtension = true
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var json = JsonSerializer.Serialize(config, _jsonOptions);
                    await File.WriteAllTextAsync(saveFileDialog.FileName, json);

                    _snackbarMessageQueue.Enqueue(
                        $"? Configuration saved: {Path.GetFileName(saveFileDialog.FileName)}",
                        "OPEN FOLDER",
                        () => OpenFileLocation(saveFileDialog.FileName));
                }
            }
            catch (Exception ex)
            {
                _snackbarMessageQueue.Enqueue($"? Error saving configuration: {ex.Message}");
            }
        }

        /// <summary>
        /// Load strategy configuration from JSON file
        /// </summary>
        public async Task<StrategyConfiguration?> LoadConfigurationAsync()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Title = "Load Strategy Configuration",
                    Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                    DefaultExt = ".json"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    var json = await File.ReadAllTextAsync(openFileDialog.FileName);
                    var config = JsonSerializer.Deserialize<StrategyConfiguration>(json, _jsonOptions);

                    if (config != null)
                    {
                        _snackbarMessageQueue.Enqueue($"? Configuration loaded: {config.Strategy.Name}");
                        return config;
                    }
                    else
                    {
                        _snackbarMessageQueue.Enqueue("? Invalid configuration file");
                    }
                }
            }
            catch (Exception ex)
            {
                _snackbarMessageQueue.Enqueue($"? Error loading configuration: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Export strategy configuration as JSON string
        /// </summary>
        public string ExportToJson(StrategyConfiguration config)
        {
            return JsonSerializer.Serialize(config, _jsonOptions);
        }

        /// <summary>
        /// Import strategy configuration from JSON string
        /// </summary>
        public StrategyConfiguration? ImportFromJson(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<StrategyConfiguration>(json, _jsonOptions);
            }
            catch
            {
                return null;
            }
        }

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
                // Silently fail
            }
        }
    }

    /// <summary>
    /// Complete strategy configuration for serialization
    /// </summary>
    public class StrategyConfiguration
    {
        public Strategy Strategy { get; set; } = new();
        public PropFirmPreset PropFirmPreset { get; set; } = new();
        public string SelectedPropFirmName { get; set; } = "FTMO";
        public DateTime SavedAt { get; set; } = DateTime.Now;
        public string Version { get; set; } = "1.0.0";
    }
}
