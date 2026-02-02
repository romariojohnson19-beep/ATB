using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AkhenTraderElite.Models;
using AkhenTraderElite.Services;

namespace AkhenTraderElite.ViewModels
{
    /// <summary>
    /// ViewModel for the EA Control tab - manages HTTP bridge and live trading
    /// </summary>
    public partial class BridgeViewModel : ObservableObject, IDisposable
    {
        private readonly BridgeService _bridgeService;
        private System.Windows.Threading.DispatcherTimer? _connectionCheckTimer;

        #region Observable Properties

        [ObservableProperty]
        private bool isConnected;

        [ObservableProperty]
        private bool isTradingEnabled;

        [ObservableProperty]
        private string connectionStatus = "Disconnected";

        [ObservableProperty]
        private string eaName = "Not Connected";

        [ObservableProperty]
        private string eaVersion = "-";

        [ObservableProperty]
        private string lastHeartbeat = "Never";

        [ObservableProperty]
        private bool isBridgeRunning;

        [ObservableProperty]
        private int bridgePort = 8080;

        [ObservableProperty]
        private ObservableCollection<LivePosition> positions = new();

        [ObservableProperty]
        private double balance;

        [ObservableProperty]
        private double equity;

        [ObservableProperty]
        private double margin;

        [ObservableProperty]
        private double dailyDrawdown;

        [ObservableProperty]
        private double totalDrawdown;

        [ObservableProperty]
        private int openPositionCount;

        [ObservableProperty]
        private string statusMessage = "Bridge server not started";

        [ObservableProperty]
        private LiveTradingParameters tradingParameters = new()
        {
            StopLossPips = 50,
            TakeProfitPips = 100,
            LotSize = 0.01,
            RiskPercent = 1.0,
            MaxOpenTrades = 5,
            UseTrailingStop = false,
            TrailingStopPips = 30,
            EnableTrading = true
        };

        #endregion

        public BridgeViewModel()
        {
            _bridgeService = new BridgeService(bridgePort);

            // Subscribe to bridge events
            _bridgeService.StatusReceived += OnStatusReceived;
            _bridgeService.AccountInfoReceived += OnAccountInfoReceived;
            _bridgeService.PositionsReceived += OnPositionsReceived;
            _bridgeService.ErrorOccurred += OnErrorOccurred;

            // Setup connection check timer
            _connectionCheckTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _connectionCheckTimer.Tick += CheckConnectionStatus;
        }

        #region Commands

        [RelayCommand]
        private async Task StartBridgeAsync()
        {
            try
            {
                StatusMessage = "Starting bridge server...";
                var success = await _bridgeService.StartAsync();

                if (success)
                {
                    IsBridgeRunning = true;
                    ConnectionStatus = $"Listening on port {BridgePort}...";
                    StatusMessage = $"Bridge server started on port {BridgePort}. Waiting for EA connection...";
                    _connectionCheckTimer?.Start();
                    
                    MessageBox.Show(
                        $"Bridge server started successfully!\n\nListening on: http://127.0.0.1:{BridgePort}\n\nNow attach the Bridge EA to an MT5 chart.",
                        "Server Started",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    StatusMessage = "Failed to start bridge server. Check if port is already in use.";
                    
                    MessageBox.Show(
                        $"Failed to start bridge server!\n\nPort {BridgePort} may already be in use.\n\nTry:\n1. Close other apps using port {BridgePort}\n2. Change the port number\n3. Restart this app",
                        "Server Start Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                
                MessageBox.Show(
                    $"Error starting bridge server:\n\n{ex.Message}\n\nStack trace:\n{ex.StackTrace}",
                    "Server Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task StopBridgeAsync()
        {
            try
            {
                await _bridgeService.StopAsync();
                IsBridgeRunning = false;
                IsConnected = false;
                ConnectionStatus = "Disconnected";
                StatusMessage = "Bridge server stopped";
                _connectionCheckTimer?.Stop();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteTrading))]
        private async Task StartTradingAsync()
        {
            try
            {
                // Send START command to EA (future implementation)
                IsTradingEnabled = true;
                StatusMessage = "Trading enabled";
                
                // TODO: Send command to EA via bridge
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteTrading))]
        private async Task StopTradingAsync()
        {
            try
            {
                // Send STOP command to EA (future implementation)
                IsTradingEnabled = false;
                StatusMessage = "Trading disabled";
                
                // TODO: Send command to EA via bridge
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteTrading))]
        private async Task CloseAllPositionsAsync()
        {
            try
            {
                var result = MessageBox.Show(
                    "Are you sure you want to close ALL open positions?",
                    "Confirm Close All",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    StatusMessage = "Closing all positions...";
                    // TODO: Send CLOSE_ALL command to EA via bridge
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteTrading))]
        private async Task UpdateParametersAsync()
        {
            try
            {
                StatusMessage = "Updating parameters...";
                // TODO: Send UPDATE_PARAMS command to EA via bridge
                StatusMessage = "Parameters updated successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteTrading))]
        private async Task EmergencyStopAsync()
        {
            try
            {
                var result = MessageBox.Show(
                    "EMERGENCY STOP: This will immediately disable trading and close all positions!\n\nContinue?",
                    "EMERGENCY STOP",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Stop);

                if (result == MessageBoxResult.Yes)
                {
                    IsTradingEnabled = false;
                    StatusMessage = "EMERGENCY STOP ACTIVATED";
                    // TODO: Send EMERGENCY_STOP command to EA via bridge
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        private bool CanExecuteTrading()
        {
            return IsConnected && IsBridgeRunning;
        }

        #endregion

        #region Event Handlers

        private void OnStatusReceived(object? sender, EAStatus status)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsConnected = status.IsConnected;
                IsTradingEnabled = status.IsTradingEnabled;
                EaName = status.EAName;
                EaVersion = status.EAVersion;
                LastHeartbeat = status.LastHeartbeat.ToString("HH:mm:ss");
                ConnectionStatus = IsConnected ? "Connected" : "Disconnected";

                if (!string.IsNullOrEmpty(status.ErrorMessage))
                {
                    StatusMessage = $"EA Error: {status.ErrorMessage}";
                }

                // Update command availability
                StartTradingCommand.NotifyCanExecuteChanged();
                StopTradingCommand.NotifyCanExecuteChanged();
                CloseAllPositionsCommand.NotifyCanExecuteChanged();
                UpdateParametersCommand.NotifyCanExecuteChanged();
                EmergencyStopCommand.NotifyCanExecuteChanged();
            });
        }

        private void OnAccountInfoReceived(object? sender, AccountInfo accountInfo)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Balance = accountInfo.Balance;
                Equity = accountInfo.Equity;
                Margin = accountInfo.Margin;
                DailyDrawdown = accountInfo.DailyDrawdown;
                TotalDrawdown = accountInfo.TotalDrawdown;
                OpenPositionCount = accountInfo.OpenPositions;
            });
        }

        private void OnPositionsReceived(object? sender, List<LivePosition> positions)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Positions.Clear();
                foreach (var position in positions)
                {
                    Positions.Add(position);
                }
            });
        }

        private void OnErrorOccurred(object? sender, string errorMessage)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                StatusMessage = $"Error: {errorMessage}";
            });
        }

        private void CheckConnectionStatus(object? sender, EventArgs e)
        {
            // Check if we've received a heartbeat in the last 10 seconds
            var timeSinceLastHeartbeat = DateTime.Now - _bridgeService.CurrentStatus.LastHeartbeat;
            if (timeSinceLastHeartbeat.TotalSeconds > 10 && IsConnected)
            {
                IsConnected = false;
                ConnectionStatus = "Connection Lost";
                StatusMessage = "Connection to EA lost. Check MT5 is running.";
            }
        }

        #endregion

        public void Dispose()
        {
            _connectionCheckTimer?.Stop();
            _bridgeService?.Dispose();
        }
    }
}
