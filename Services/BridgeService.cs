using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using AkhenTraderElite.Models;

namespace AkhenTraderElite.Services
{
    /// <summary>
    /// HTTP Bridge Server for communication with MT5 Bridge EA
    /// Listens for incoming connections from MT5 and sends commands
    /// </summary>
    public class BridgeService : IDisposable
    {
        private HttpListener? _httpListener;
        private bool _isRunning;
        private readonly int _port;
        private Task? _listenerTask;
        private CancellationTokenSource? _cancellationTokenSource;

        // Events for UI updates
        public event EventHandler<EAStatus>? StatusReceived;
        public event EventHandler<AccountInfo>? AccountInfoReceived;
        public event EventHandler<List<LivePosition>>? PositionsReceived;
        public event EventHandler<string>? ErrorOccurred;

        // Current state
        public EAStatus CurrentStatus { get; private set; } = new();
        public AccountInfo CurrentAccountInfo { get; private set; } = new();
        public List<LivePosition> CurrentPositions { get; private set; } = new();

        public bool IsRunning => _isRunning;

        public BridgeService(int port = 8080)
        {
            _port = port;
        }

        /// <summary>
        /// Start the HTTP bridge server
        /// </summary>
        public async Task<bool> StartAsync()
        {
            try
            {
                if (_isRunning) return true;

                _httpListener = new HttpListener();
                _httpListener.Prefixes.Add($"http://localhost:{_port}/");
                _httpListener.Start();

                _cancellationTokenSource = new CancellationTokenSource();
                _listenerTask = Task.Run(() => ListenForRequestsAsync(_cancellationTokenSource.Token));

                _isRunning = true;
                return true;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Failed to start bridge server: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Stop the HTTP bridge server
        /// </summary>
        public async Task StopAsync()
        {
            try
            {
                _isRunning = false;
                _cancellationTokenSource?.Cancel();
                _httpListener?.Stop();
                _httpListener?.Close();

                if (_listenerTask != null)
                {
                    await _listenerTask;
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Error stopping bridge server: {ex.Message}");
            }
        }

        /// <summary>
        /// Listen for incoming HTTP requests from MT5
        /// </summary>
        private async Task ListenForRequestsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && _httpListener != null)
            {
                try
                {
                    var context = await _httpListener.GetContextAsync();
                    _ = Task.Run(() => ProcessRequestAsync(context), cancellationToken);
                }
                catch (HttpListenerException)
                {
                    // Listener was stopped
                    break;
                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke(this, $"Error in listener loop: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Process incoming HTTP request from MT5
        /// </summary>
        private async Task ProcessRequestAsync(HttpListenerContext context)
        {
            try
            {
                var request = context.Request;
                var response = context.Response;

                // Read request body
                string requestBody;
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    requestBody = await reader.ReadToEndAsync();
                }

                // Parse request path
                var path = request.Url?.AbsolutePath ?? "/";

                string responseString;

                switch (path)
                {
                    case "/heartbeat":
                        responseString = await HandleHeartbeatAsync(requestBody);
                        break;

                    case "/status":
                        responseString = await HandleStatusUpdateAsync(requestBody);
                        break;

                    case "/positions":
                        responseString = await HandlePositionsUpdateAsync(requestBody);
                        break;

                    case "/account":
                        responseString = await HandleAccountInfoAsync(requestBody);
                        break;

                    default:
                        responseString = JsonSerializer.Serialize(new EAResponse
                        {
                            Success = false,
                            Message = "Unknown endpoint"
                        });
                        break;
                }

                // Send response
                var buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                response.ContentType = "application/json";
                await response.OutputStream.WriteAsync(buffer);
                response.OutputStream.Close();
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Error processing request: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle heartbeat from MT5 (keeps connection alive)
        /// </summary>
        private async Task<string> HandleHeartbeatAsync(string requestBody)
        {
            try
            {
                CurrentStatus.LastHeartbeat = DateTime.Now;
                CurrentStatus.IsConnected = true;

                return JsonSerializer.Serialize(new EAResponse
                {
                    Success = true,
                    Message = "Heartbeat acknowledged"
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new EAResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Handle EA status update from MT5
        /// </summary>
        private async Task<string> HandleStatusUpdateAsync(string requestBody)
        {
            try
            {
                var status = JsonSerializer.Deserialize<EAStatus>(requestBody);
                if (status != null)
                {
                    CurrentStatus = status;
                    CurrentStatus.LastHeartbeat = DateTime.Now;
                    StatusReceived?.Invoke(this, status);
                }

                return JsonSerializer.Serialize(new EAResponse
                {
                    Success = true,
                    Message = "Status updated"
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new EAResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Handle positions update from MT5
        /// </summary>
        private async Task<string> HandlePositionsUpdateAsync(string requestBody)
        {
            try
            {
                var positions = JsonSerializer.Deserialize<List<LivePosition>>(requestBody);
                if (positions != null)
                {
                    CurrentPositions = positions;
                    PositionsReceived?.Invoke(this, positions);
                }

                return JsonSerializer.Serialize(new EAResponse
                {
                    Success = true,
                    Message = "Positions updated"
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new EAResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Handle account info update from MT5
        /// </summary>
        private async Task<string> HandleAccountInfoAsync(string requestBody)
        {
            try
            {
                var accountInfo = JsonSerializer.Deserialize<AccountInfo>(requestBody);
                if (accountInfo != null)
                {
                    CurrentAccountInfo = accountInfo;
                    accountInfo.LastUpdate = DateTime.Now;
                    AccountInfoReceived?.Invoke(this, accountInfo);
                }

                return JsonSerializer.Serialize(new EAResponse
                {
                    Success = true,
                    Message = "Account info updated"
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new EAResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Send command to MT5 EA (for future use - requires EA polling endpoint)
        /// </summary>
        public async Task<EAResponse> SendCommandAsync(EACommand command)
        {
            // This will be implemented when we add a command queue system
            // For now, return a placeholder
            return await Task.FromResult(new EAResponse
            {
                Success = false,
                Message = "Command sending not yet implemented - requires EA polling"
            });
        }

        public void Dispose()
        {
            StopAsync().Wait();
            _httpListener?.Close();
            _cancellationTokenSource?.Dispose();
        }
    }
}
