// Copyright © 2025 Timothy Ellis, Fyrby Additive Manufacturing & Engineering

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Feetech.ServoSDK;
using ProgrammerForFeetechServos.Models;

namespace ProgrammerForFeetechServos.Services;

public class ServoManager : INotifyPropertyChanged
{
    private const uint BaudRate = 1000000;
    private const int ProtocolEnd = 0; // STS/SMS series
    private const byte ServoIdAddress = 5; // Standard Feetech servo ID address

    private PortHandler? _portHandler;
    private IPacketHandler? _packetHandler;
    private CancellationTokenSource? _connectionCheckCts;
    private CancellationTokenSource? _scanCts;
    private CancellationTokenSource? _idChangeCts;

    private ConnectionStateInfo _connectionState = ConnectionStateInfo.Disconnected();
    private bool _isScanning;
    private double _scanProgress;
    private string _statusMessage = "";
    private string _selectedPort = "";
    private readonly ObservableCollection<string> _availablePorts = new();
    private readonly ObservableCollection<ServoInfo> _servos = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public ConnectionStateInfo ConnectionState
    {
        get => _connectionState;
        set
        {
            if (_connectionState != value)
            {
                _connectionState = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsConnected));
                OnPropertyChanged(nameof(IsDisconnected));
                OnPropertyChanged(nameof(StatusText));
            }
        }
    }

    public bool IsConnected => ConnectionState.State == Models.ConnectionState.Connected;
    public bool IsDisconnected => ConnectionState.State == Models.ConnectionState.Disconnected;

    public bool IsScanning
    {
        get => _isScanning;
        set
        {
            if (_isScanning != value)
            {
                _isScanning = value;
                OnPropertyChanged();
            }
        }
    }

    public double ScanProgress
    {
        get => _scanProgress;
        set
        {
            if (Math.Abs(_scanProgress - value) > 0.001)
            {
                _scanProgress = value;
                OnPropertyChanged();
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (_statusMessage != value)
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }
    }

    public string StatusText
    {
        get
        {
            return ConnectionState.State switch
            {
                Models.ConnectionState.Connected => "Connected",
                Models.ConnectionState.Connecting => "Connecting...",
                Models.ConnectionState.Disconnected => "Disconnected",
                Models.ConnectionState.Error => $"Error: {ConnectionState.ErrorMessage}",
                _ => "Unknown"
            };
        }
    }

    public string SelectedPort
    {
        get => _selectedPort;
        set
        {
            if (_selectedPort != value)
            {
                _selectedPort = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<string> AvailablePorts => _availablePorts;
    public ObservableCollection<ServoInfo> Servos => _servos;

    public ServoManager()
    {
        RefreshPorts();
    }

    public void RefreshPorts()
    {
        var ports = PortHandler.GetAvailablePorts();
        
        Application.Current.Dispatcher.Invoke(() =>
        {
            _availablePorts.Clear();
            foreach (var port in ports)
            {
                _availablePorts.Add(port);
            }

            // Auto-select first port if none selected
            if (string.IsNullOrEmpty(SelectedPort) && _availablePorts.Count > 0)
            {
                SelectedPort = _availablePorts[0];
            }
        });
    }

    public async Task ConnectAsync()
    {
        if (string.IsNullOrEmpty(SelectedPort))
        {
            ConnectionState = ConnectionStateInfo.Error("No serial port selected");
            return;
        }

        ConnectionState = ConnectionStateInfo.Connecting();
        StatusMessage = $"Connecting to {SelectedPort}...";

        await Task.Run(async () =>
        {
            try
            {
                // Create handlers
                _portHandler = new PortHandler(SelectedPort);
                _packetHandler = PacketHandlerFactory.CreatePacketHandler(ProtocolEnd);

                // Open port and set baud rate
                _portHandler.OpenPort();
                await UpdateStatusAsync("Port opened successfully");

                _portHandler.SetBaudRate(BaudRate);
                await UpdateStatusAsync($"Baud rate set to {BaudRate}");

                // Clear port buffer
                _portHandler.ClearPort();

                // Small delay to stabilize
                await Task.Delay(50);

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ConnectionState = ConnectionStateInfo.Connected();
                    StatusMessage = $"Connected at {BaudRate} baud";
                });

                // Start connection monitoring
                StartConnectionMonitoring();

                // Auto-scan on connect
                await ScanServosAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ConnectionState = ConnectionStateInfo.Error(ex.Message);
                    StatusMessage = $"Error: {ex.Message}";
                    IsScanning = false;
                    ScanProgress = 0.0;
                    Servos.Clear();
                });

                _portHandler?.ClosePort();
                _portHandler = null;
                _packetHandler = null;
                
                // Cancel any ongoing operations
                _scanCts?.Cancel();
                _scanCts = null;
                _idChangeCts?.Cancel();
                _idChangeCts = null;
            }
        });
    }

    public void Disconnect()
    {
        _connectionCheckCts?.Cancel();
        _connectionCheckCts = null;
        
        _scanCts?.Cancel();
        _scanCts = null;
        
        _idChangeCts?.Cancel();
        _idChangeCts = null;

        _portHandler?.ClosePort();
        _portHandler = null;
        _packetHandler = null;

        Application.Current.Dispatcher.Invoke(() =>
        {
            ConnectionState = ConnectionStateInfo.Disconnected();
            StatusMessage = "Disconnected";
            Servos.Clear();
            IsScanning = false;
            ScanProgress = 0.0;
        });
    }

    private void StartConnectionMonitoring()
    {
        _connectionCheckCts?.Cancel();
        _connectionCheckCts = new CancellationTokenSource();
        var token = _connectionCheckCts.Token;

        Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(1000, token);

                if (ConnectionState.State == Models.ConnectionState.Connected)
                {
                    if (!CheckConnection())
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }, token);
    }

    private bool CheckConnection()
    {
        if (_portHandler == null)
            return false;

        // Check if port is still open
        if (!_portHandler.IsOpen)
        {
            HandleDisconnection();
            return false;
        }

        // Check if port still exists in system
        var currentPorts = PortHandler.GetAvailablePorts();
        if (!currentPorts.Contains(SelectedPort))
        {
            HandleDisconnection();
            return false;
        }

        return true;
    }

    private void HandleDisconnection()
    {
        // Cancel all ongoing operations
        _connectionCheckCts?.Cancel();
        _connectionCheckCts = null;
        
        _scanCts?.Cancel();
        _scanCts = null;
        
        _idChangeCts?.Cancel();
        _idChangeCts = null;

        Application.Current.Dispatcher.Invoke(() =>
        {
            ConnectionState = ConnectionStateInfo.Disconnected();
            StatusMessage = "Connection lost - device disconnected";
            Servos.Clear();
            IsScanning = false;
            ScanProgress = 0.0;
        });

        _portHandler?.ClosePort();
        _portHandler = null;
        _packetHandler = null;
        
        // Refresh available ports after disconnection
        RefreshPorts();
    }

    public async Task ScanServosAsync()
    {
        if (_portHandler == null || _packetHandler == null || ConnectionState.State != Models.ConnectionState.Connected)
            return;

        if (!CheckConnection())
            return;

        // Cancel any existing scan
        _scanCts?.Cancel();
        _scanCts = new CancellationTokenSource();
        var token = _scanCts.Token;

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            IsScanning = true;
            ScanProgress = 0.0;
            StatusMessage = "Scanning for servos (IDs 1-252)...";
            Servos.Clear();
        });

        await Task.Run(async () =>
        {
            const double totalIds = 252.0;
            int consecutiveErrors = 0;
            const int maxConsecutiveErrors = 20;

            try
            {
                for (byte servoId = 1; servoId <= 252; servoId++)
                {
                    // Check for cancellation
                    if (token.IsCancellationRequested)
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            IsScanning = false;
                            StatusMessage = "Scan cancelled";
                        });
                        return;
                    }

                    // Periodic connection check
                    if (servoId % 50 == 0)
                    {
                        if (!CheckConnection())
                        {
                            await Application.Current.Dispatcher.InvokeAsync(() => IsScanning = false);
                            return;
                        }
                    }

                    var (modelNumber, result, error) = _packetHandler!.Ping(_portHandler!, servoId);

                    if (result == CommResult.Success && error.IsEmpty())
                    {
                        var newServo = new ServoInfo
                        {
                            Id = servoId,
                            ModelNumber = modelNumber,
                            IsConnected = true
                        };

                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            Servos.Add(newServo);
                            StatusMessage = $"Found servo ID {servoId} (Model: {modelNumber})";
                        });

                        consecutiveErrors = 0;
                    }
                    else if (result != CommResult.Success)
                    {
                        consecutiveErrors++;
                        if (consecutiveErrors >= maxConsecutiveErrors)
                        {
                            if (!CheckConnection())
                            {
                                await Application.Current.Dispatcher.InvokeAsync(() => IsScanning = false);
                                return;
                            }
                            consecutiveErrors = 0;
                        }
                    }

                    // Update progress
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        ScanProgress = servoId / totalIds;
                    });

                    // Small delay every 10 IDs to prevent overwhelming the bus
                    if (servoId % 10 == 0)
                    {
                        await Task.Delay(10, token);
                    }
                }

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    IsScanning = false;
                    ScanProgress = 1.0;

                    if (Servos.Count == 0)
                    {
                        StatusMessage = "No servos found. Check connections, power, and servo IDs.";
                    }
                    else
                    {
                        StatusMessage = $"Found {Servos.Count} servo(s)";
                    }
                });
            }
            catch (OperationCanceledException)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    IsScanning = false;
                    StatusMessage = "Scan cancelled";
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    IsScanning = false;
                    StatusMessage = $"Scan error: {ex.Message}";
                });
                
                // Port may have been disconnected, check connection
                CheckConnection();
            }
        }, token);
    }

    public async Task<bool> ChangeServoIdAsync(byte oldId, byte newId, Action<string> progressCallback)
    {
        if (_portHandler == null || _packetHandler == null || ConnectionState.State != Models.ConnectionState.Connected)
        {
            progressCallback("Not connected");
            return false;
        }

        if (!CheckConnection())
        {
            progressCallback("Connection lost");
            return false;
        }

        // Validate new ID
        if (newId < 1 || newId > 252)
        {
            progressCallback("Invalid ID. Must be between 1 and 252.");
            return false;
        }

        // Check if new ID already exists
        if (Servos.Any(s => s.Id == newId))
        {
            progressCallback($"ID {newId} already in use!");
            return false;
        }

        // Cancel any existing ID change operation
        _idChangeCts?.Cancel();
        _idChangeCts = new CancellationTokenSource();
        var token = _idChangeCts.Token;

        return await Task.Run(async () =>
        {
            try
            {
                if (token.IsCancellationRequested)
                {
                    progressCallback("Operation cancelled");
                    return false;
                }

                progressCallback("Writing new ID to servo...");

                var (result, error) = _packetHandler!.Write1ByteTxRx(_portHandler!, oldId, ServoIdAddress, newId);

                if (token.IsCancellationRequested)
                {
                    progressCallback("Operation cancelled");
                    return false;
                }

                if (result != CommResult.Success)
                {
                    progressCallback($"Failed to change ID: {result.GetDescription()}");
                    CheckConnection();
                    return false;
                }

                if (!error.IsEmpty())
                {
                    progressCallback($"Protocol error: {error.GetDescription()}");
                    return false;
                }

                progressCallback("Verifying new ID...");

                // Wait for servo to update
                await Task.Delay(500, token);

                if (token.IsCancellationRequested)
                {
                    progressCallback("Operation cancelled");
                    return false;
                }

                // Verify the change
                var (_, verifyResult, verifyError) = _packetHandler!.Ping(_portHandler!, newId);

                if (verifyResult == CommResult.Success && verifyError.IsEmpty())
                {
                    progressCallback("✓ ID changed successfully!");
                    progressCallback("Re-scanning for servos...");

                    // Refresh servo list
                    await ScanServosAsync();

                    progressCallback("✓ Scan complete!");
                    return true;
                }
                else
                {
                    progressCallback("ID changed but verification failed. Try refreshing.");
                    return false;
                }
            }
            catch (OperationCanceledException)
            {
                progressCallback("Operation cancelled - device disconnected");
                return false;
            }
        }, token);
    }

    private async Task UpdateStatusAsync(string message)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            StatusMessage = message;
        });
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
