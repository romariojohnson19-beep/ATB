using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using AKHENS_TRADER.Models;
using AKHENS_TRADER.Services;

namespace AKHENS_TRADER.ViewModels
{
    /// <summary>
    /// Main ViewModel for the Prop Strategy Builder application
    /// Manages the overall application state and coordinates between different views
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        #region Services

        private readonly CodeGenerationService _codeGenerationService;
        private readonly PropFirmService _propFirmService;
        private readonly FileExportService _fileExportService;
        private readonly PreloadedStrategiesService _preloadedStrategiesService;
        private readonly ConfigurationService _configurationService;
        private readonly DispatcherTimer _autoRegenerateTimer;

        #endregion

        #region Observable Properties

        /// <summary>

        /// Collection of preloaded strategies with full info
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<PreloadedStrategyInfo> preloadedStrategies;

        /// <summary>
        /// Collection of available prop firms
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<string> availablePropFirms;

        /// <summary>
        /// Currently selected prop firm name
        /// </summary>
        [ObservableProperty]
        private string selectedPropFirmName = "FTMO";

        /// <summary>
        /// Generated MQL5 code for preview
        /// </summary>
        [ObservableProperty]
        private string generatedCode = string.Empty;

        /// <summary>
        /// Status message displayed in the footer
        /// </summary>
        [ObservableProperty]
        private string statusMessage;

        /// <summary>
        /// Current strategy being configured
        /// </summary>
        [ObservableProperty]
        private Strategy currentStrategy;

        /// <summary>
        /// Current prop firm preset
        /// </summary>
        [ObservableProperty]
        private PropFirmPreset currentPropFirmPreset;

        /// <summary>
        /// Custom builder view model
        /// </summary>
        [ObservableProperty]
        private CustomBuilderViewModel customBuilderViewModel;

        /// <summary>
        /// Indicates if code is currently being generated
        /// </summary>
        [ObservableProperty]
        private bool isGenerating;

        /// <summary>
        /// Snackbar message queue for notifications
        /// </summary>
        public ISnackbarMessageQueue SnackbarMessageQueue { get; }

        #endregion

        #region Constructor



        public MainViewModel()
        {
            // Initialize Snackbar message queue
            SnackbarMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

            // Initialize services
            _codeGenerationService = new CodeGenerationService();
            _propFirmService = new PropFirmService();
            _fileExportService = new FileExportService(SnackbarMessageQueue);
            _preloadedStrategiesService = new PreloadedStrategiesService();
            _configurationService = new ConfigurationService(SnackbarMessageQueue);

            // Initialize auto-regenerate timer (800ms debounce)
            _autoRegenerateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(800)

            };
            _autoRegenerateTimer.Tick += (s, e) =>
            {
                _autoRegenerateTimer.Stop();
                GenerateStrategy();
            };


            // Initialize preloaded strategies
            PreloadedStrategies = new ObservableCollection<PreloadedStrategyInfo>(
                _preloadedStrategiesService.GetAllStrategies());

            // Initialize available prop firms
            AvailablePropFirms = new ObservableCollection<string>(PropFirmService.GetAvailableFirms());

            // Initialize default strategy
            CurrentStrategy = new Strategy
            {
                Name = "My Custom Strategy",
                Description = "Custom trading strategy",
                RiskSettings = new RiskManagement
                {
                    RiskPercentPerTrade = 1.0,
                    StopLossPips = 50,
                    TakeProfitPips = 100,
                    UseTrailingStop = false,
                    TrailingStopPips = 30
                },
                EntryConditions = new ObservableCollection<IndicatorCondition>(),
                ExitConditions = new ObservableCollection<IndicatorCondition>()
            };

            // Initialize custom builder view model
            CustomBuilderViewModel = new CustomBuilderViewModel(CurrentStrategy);

            // Subscribe to property changes for auto-regeneration
            CurrentStrategy.RiskSettings.PropertyChanged += (s, e) => TriggerAutoRegenerate();
            CurrentStrategy.PropertyChanged += (s, e) => TriggerAutoRegenerate();
            CustomBuilderViewModel.EntryConditions.CollectionChanged += (s, e) => TriggerAutoRegenerate();
            CustomBuilderViewModel.ExitConditions.CollectionChanged += (s, e) => TriggerAutoRegenerate();

            // Initialize default prop firm preset (FTMO)
            CurrentPropFirmPreset = _propFirmService.GetPreset("FTMO");
            SelectedPropFirmName = "FTMO";

            // Generate initial code
            GenerateStrategy();

            StatusMessage = "Ready - Configure your strategy and generate MQL5 code";
        }

        #endregion

        #region Methods

        /// <summary>
        /// Trigger auto-regenerate with debounce
        /// </summary>
        private void TriggerAutoRegenerate()
        {
            // Restart timer for debounce
            _autoRegenerateTimer.Stop();
            _autoRegenerateTimer.Start();
        }

        #endregion

        #region Commands

        /// <summary>
        /// Copy generated code to clipboard
        /// </summary>
        [RelayCommand]
        private void CopyCode()
        {
            if (!string.IsNullOrEmpty(GeneratedCode))
            {
                Clipboard.SetText(GeneratedCode);
                StatusMessage = "Code copied to clipboard successfully!";
            }
            else
            {
                StatusMessage = "No code to copy. Please generate a strategy first.";
            }
        }

        /// <summary>
        /// Save generated code as .mq5 file
        /// </summary>
        [RelayCommand]
        private async Task SaveMq5Async()
        {
            try
            {
                StatusMessage = "Opening save dialog...";
                var fileName = $"{CurrentStrategy.Name.Replace(" ", "_")}.mq5";
                await _fileExportService.SaveMq5Async(GeneratedCode, fileName);
                StatusMessage = "Ready";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving .mq5 file: {ex.Message}";
                SnackbarMessageQueue.Enqueue($"? Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Save strategy parameters as .set file
        /// </summary>
        [RelayCommand]
        private async Task SaveSetAsync()
        {
            try
            {
                StatusMessage = "Opening save dialog...";
                var fileName = $"{CurrentStrategy.Name.Replace(" ", "_")}.set";
                await _fileExportService.SaveSetFileAsync(CurrentStrategy, CurrentPropFirmPreset, fileName);
                StatusMessage = "Ready";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving .set file: {ex.Message}";
                SnackbarMessageQueue.Enqueue($"? Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Export complete project folder
        /// </summary>
        [RelayCommand]
        private async Task ExportProjectAsync()
        {
            try
            {
                StatusMessage = "Exporting project...";
                await _fileExportService.ExportProjectFolderAsync(GeneratedCode, CurrentStrategy, CurrentPropFirmPreset);
                StatusMessage = "Ready";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error exporting project: {ex.Message}";
                SnackbarMessageQueue.Enqueue($"? Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Apply selected prop firm preset
        /// </summary>
        [RelayCommand]
        private void ApplyPropFirmPreset()
        {
            try
            {
                if (!string.IsNullOrEmpty(SelectedPropFirmName))
                {
                    CurrentPropFirmPreset = _propFirmService.GetPreset(SelectedPropFirmName);
                    StatusMessage = $"Applied {SelectedPropFirmName} preset";
                    SnackbarMessageQueue.Enqueue($"? {SelectedPropFirmName} preset applied");
                    
                    // Regenerate code with new preset
                    GenerateStrategy();
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error applying preset: {ex.Message}";
                SnackbarMessageQueue.Enqueue($"? Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Validate current strategy against prop firm rules
        /// </summary>
        [RelayCommand]
        private void ValidateStrategy()
        {
            try
            {
                var (isValid, errorMessage) = PropFirmService.ValidateStrategy(CurrentStrategy, CurrentPropFirmPreset);
                
                if (isValid)
                {
                    SnackbarMessageQueue.Enqueue("? Strategy is prop-compliant!");
                    StatusMessage = "Strategy validation passed";
                }
                else
                {
                    SnackbarMessageQueue.Enqueue($"?? Validation issues:\n{errorMessage}");
                    StatusMessage = "Strategy has validation warnings";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error validating strategy: {ex.Message}";
                SnackbarMessageQueue.Enqueue($"? Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Generate strategy code
        /// </summary>
        [RelayCommand]
        private void GenerateStrategy()
        {
            try
            {
                IsGenerating = true;
                StatusMessage = "Generating strategy code...";

                // Sync conditions from CustomBuilderViewModel to Strategy
                CustomBuilderViewModel.SyncToStrategy();


                // Generate MQL5 code using the service
                GeneratedCode = _codeGenerationService.GenerateMQL5Code(CurrentStrategy, CurrentPropFirmPreset);

                StatusMessage = $"Strategy '{CurrentStrategy.Name}' generated successfully for {CurrentPropFirmPreset.FirmName}!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error generating strategy: {ex.Message}";
                GeneratedCode = $"// Error generating code:\n// {ex.Message}\n\n{GeneratedCode}";
            }
            finally
            {
                IsGenerating = false;
            }
        }

        /// <summary>
        /// Load a preloaded strategy
        /// </summary>
        [RelayCommand]
        private void LoadPreloadedStrategy(PreloadedStrategyInfo? strategyInfo)
        {
            try
            {
                if (strategyInfo == null) return;

                // Deep copy the strategy to avoid modifying the original
                var loadedStrategy = strategyInfo.Strategy;
                
                // Update current strategy
                CurrentStrategy.Name = loadedStrategy.Name;
                CurrentStrategy.Description = loadedStrategy.Description;
                CurrentStrategy.RiskSettings.RiskPercentPerTrade = loadedStrategy.RiskSettings.RiskPercentPerTrade;
                CurrentStrategy.RiskSettings.StopLossPips = loadedStrategy.RiskSettings.StopLossPips;
                CurrentStrategy.RiskSettings.TakeProfitPips = loadedStrategy.RiskSettings.TakeProfitPips;
                CurrentStrategy.RiskSettings.UseTrailingStop = loadedStrategy.RiskSettings.UseTrailingStop;
                CurrentStrategy.RiskSettings.TrailingStopPips = loadedStrategy.RiskSettings.TrailingStopPips;

                // Clear existing conditions
                CurrentStrategy.EntryConditions.Clear();
                CurrentStrategy.ExitConditions.Clear();

                // Copy conditions
                foreach (var condition in loadedStrategy.EntryConditions)
                {
                    CurrentStrategy.EntryConditions.Add(condition);
                }
                foreach (var condition in loadedStrategy.ExitConditions)
                {
                    CurrentStrategy.ExitConditions.Add(condition);
                }

                // Reload CustomBuilderViewModel
                CustomBuilderViewModel = new CustomBuilderViewModel(CurrentStrategy);
                OnPropertyChanged(nameof(CustomBuilderViewModel));

                // Re-subscribe to events
                CustomBuilderViewModel.EntryConditions.CollectionChanged += (s, e) => TriggerAutoRegenerate();
                CustomBuilderViewModel.ExitConditions.CollectionChanged += (s, e) => TriggerAutoRegenerate();

                // Notify success
                SnackbarMessageQueue.Enqueue($"? Loaded: {strategyInfo.Name}");
                StatusMessage = $"Loaded preloaded strategy: {strategyInfo.Name}";

                // Trigger regeneration
                GenerateStrategy();
            }
            catch (Exception ex)
            {
                SnackbarMessageQueue.Enqueue($"? Error loading strategy: {ex.Message}");
                StatusMessage = $"Error loading strategy: {ex.Message}";
            }
        }

        /// <summary>
        /// Save strategy configuration to JSON file
        /// </summary>
        [RelayCommand]
        private async Task SaveConfigurationAsync()
        {
            try
            {
                StatusMessage = "Saving configuration...";
                
                // Sync conditions before saving
                CustomBuilderViewModel.SyncToStrategy();

                var config = new StrategyConfiguration
                {
                    Strategy = CurrentStrategy,
                    PropFirmPreset = CurrentPropFirmPreset,
                    SelectedPropFirmName = SelectedPropFirmName,
                    SavedAt = DateTime.Now,
                    Version = "1.0.0"
                };

                await _configurationService.SaveConfigurationAsync(config);
                StatusMessage = "Ready";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving configuration: {ex.Message}";
                SnackbarMessageQueue.Enqueue($"? Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Load strategy configuration from JSON file
        /// </summary>
        [RelayCommand]
        private async Task LoadConfigurationAsync()
        {
            try
            {
                StatusMessage = "Loading configuration...";

                var config = await _configurationService.LoadConfigurationAsync();
                if (config != null)
                {
                    // Update current strategy
                    CurrentStrategy.Name = config.Strategy.Name;
                    CurrentStrategy.Description = config.Strategy.Description;
                    CurrentStrategy.RiskSettings.RiskPercentPerTrade = config.Strategy.RiskSettings.RiskPercentPerTrade;
                    CurrentStrategy.RiskSettings.StopLossPips = config.Strategy.RiskSettings.StopLossPips;
                    CurrentStrategy.RiskSettings.TakeProfitPips = config.Strategy.RiskSettings.TakeProfitPips;
                    CurrentStrategy.RiskSettings.UseTrailingStop = config.Strategy.RiskSettings.UseTrailingStop;
                    CurrentStrategy.RiskSettings.TrailingStopPips = config.Strategy.RiskSettings.TrailingStopPips;

                    // Update prop firm settings
                    CurrentPropFirmPreset = config.PropFirmPreset;
                    SelectedPropFirmName = config.SelectedPropFirmName;

                    // Clear and reload conditions
                    CurrentStrategy.EntryConditions.Clear();
                    CurrentStrategy.ExitConditions.Clear();
                    foreach (var condition in config.Strategy.EntryConditions)
                    {
                        CurrentStrategy.EntryConditions.Add(condition);
                    }
                    foreach (var condition in config.Strategy.ExitConditions)
                    {
                        CurrentStrategy.ExitConditions.Add(condition);
                    }

                    // Reload CustomBuilderViewModel
                    CustomBuilderViewModel = new CustomBuilderViewModel(CurrentStrategy);
                    OnPropertyChanged(nameof(CustomBuilderViewModel));

                    // Re-subscribe to events
                    CustomBuilderViewModel.EntryConditions.CollectionChanged += (s, e) => TriggerAutoRegenerate();
                    CustomBuilderViewModel.ExitConditions.CollectionChanged += (s, e) => TriggerAutoRegenerate();

                    StatusMessage = $"Configuration loaded: {config.Strategy.Name}";
                    GenerateStrategy();
                }
                else
                {
                    StatusMessage = "Configuration load cancelled";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading configuration: {ex.Message}";
                SnackbarMessageQueue.Enqueue($"? Error: {ex.Message}");
            }
        }

        #endregion
    }
}


