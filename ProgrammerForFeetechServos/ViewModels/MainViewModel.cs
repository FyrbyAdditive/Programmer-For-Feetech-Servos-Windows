// Copyright © 2025 Timothy Ellis, Fyrby Additive Manufacturing & Engineering

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ProgrammerForFeetechServos.Models;
using ProgrammerForFeetechServos.Services;
using ProgrammerForFeetechServos.Views;

namespace ProgrammerForFeetechServos.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ServoManager _servoManager;
    private ServoInfo? _selectedServo;

    public MainViewModel()
    {
        _servoManager = new ServoManager();

        ConnectCommand = new AsyncRelayCommand(
            async () => await _servoManager.ConnectAsync(),
            () => !string.IsNullOrEmpty(_servoManager.SelectedPort) && _servoManager.IsDisconnected
        );

        DisconnectCommand = new RelayCommand(
            () => _servoManager.Disconnect(),
            () => _servoManager.IsConnected
        );

        RefreshPortsCommand = new RelayCommand(
            () => _servoManager.RefreshPorts()
        );

        ScanServosCommand = new AsyncRelayCommand(
            async () => await _servoManager.ScanServosAsync(),
            () => _servoManager.IsConnected && !_servoManager.IsScanning
        );

        ChangeServoIdCommand = new RelayCommand(
            () => ShowChangeIdDialog(),
            () => SelectedServo != null
        );

        ShowHelpCommand = new RelayCommand(() =>
        {
            var helpWindow = new HelpWindow();
            helpWindow.Show();
        });

        ShowAboutCommand = new RelayCommand(() =>
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        });

        _servoManager.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ServoManager.IsConnected) ||
                e.PropertyName == nameof(ServoManager.IsDisconnected) ||
                e.PropertyName == nameof(ServoManager.IsScanning))
            {
                (ConnectCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                (DisconnectCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (ScanServosCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
            
            // Clear selection when disconnected
            if (e.PropertyName == nameof(ServoManager.IsDisconnected) && _servoManager.IsDisconnected)
            {
                SelectedServo = null;
            }
        };

        // Monitor servo collection changes to clear selection if servo list is cleared
        _servoManager.Servos.CollectionChanged += (s, e) =>
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                SelectedServo = null;
            }
            // Also clear selection if the selected servo was removed
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove && 
                     SelectedServo != null && 
                     e.OldItems != null && 
                     e.OldItems.Contains(SelectedServo))
            {
                SelectedServo = null;
            }
        };

        // Note: Auto-connect on startup disabled to allow user to select port first
        // Application.Current.Dispatcher.InvokeAsync(async () =>
        // {
        //     await Task.Delay(100); // Let UI initialize
        //     if (!string.IsNullOrEmpty(_servoManager.SelectedPort))
        //     {
        //         await _servoManager.ConnectAsync();
        //     }
        // });
    }

    public ServoManager ServoManager => _servoManager;

    public ObservableCollection<ServoInfo> Servos => _servoManager.Servos;

    public ServoInfo? SelectedServo
    {
        get => _selectedServo;
        set
        {
            if (SetProperty(ref _selectedServo, value))
            {
                (ChangeServoIdCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public ICommand ConnectCommand { get; }
    public ICommand DisconnectCommand { get; }
    public ICommand RefreshPortsCommand { get; }
    public ICommand ScanServosCommand { get; }
    public ICommand ChangeServoIdCommand { get; }
    public ICommand ShowHelpCommand { get; }
    public ICommand ShowAboutCommand { get; }

    private void ShowChangeIdDialog()
    {
        if (SelectedServo == null)
            return;

        var dialog = new ChangeIdDialog
        {
            Owner = Application.Current.MainWindow,
            DataContext = new ChangeIdViewModel(SelectedServo, _servoManager)
        };

        dialog.ShowDialog();

        // Clear selection after dialog closes
        SelectedServo = null;
    }
}
