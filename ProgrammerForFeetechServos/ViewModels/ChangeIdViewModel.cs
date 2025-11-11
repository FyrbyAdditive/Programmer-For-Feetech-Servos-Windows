// Copyright © 2025 Timothy Ellis, Fyrby Additive Manufacturing & Engineering

using System.Windows;
using System.Windows.Input;
using ProgrammerForFeetechServos.Models;
using ProgrammerForFeetechServos.Services;

namespace ProgrammerForFeetechServos.ViewModels;

public class ChangeIdViewModel : ViewModelBase
{
    private readonly ServoInfo _servo;
    private readonly ServoManager _servoManager;
    private string _newIdInput;
    private bool _isChanging;
    private string _progressMessage = "";

    public ChangeIdViewModel(ServoInfo servo, ServoManager servoManager)
    {
        _servo = servo;
        _servoManager = servoManager;
        _newIdInput = servo.Id.ToString();

        ConfirmCommand = new AsyncRelayCommand(
            async () => await ChangeIdAsync(),
            () => IsValid && !IsChanging
        );

        CancelCommand = new RelayCommand(
            () => CloseRequested?.Invoke(),
            () => !IsChanging
        );
    }

    public byte CurrentId => _servo.Id;

    public string NewIdInput
    {
        get => _newIdInput;
        set
        {
            if (SetProperty(ref _newIdInput, value))
            {
                OnPropertyChanged(nameof(IsValid));
                (ConfirmCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public bool IsChanging
    {
        get => _isChanging;
        set
        {
            if (SetProperty(ref _isChanging, value))
            {
                (ConfirmCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                (CancelCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public string ProgressMessage
    {
        get => _progressMessage;
        set => SetProperty(ref _progressMessage, value);
    }

    public bool IsValid
    {
        get
        {
            if (!byte.TryParse(NewIdInput, out byte newId))
                return false;

            return newId >= 1 && newId <= 252 && newId != CurrentId;
        }
    }

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }

    public event Action? CloseRequested;
    public event Action? SuccessfulChange;

    private async Task ChangeIdAsync()
    {
        if (!byte.TryParse(NewIdInput, out byte newId))
            return;

        IsChanging = true;
        ProgressMessage = "Starting...";

        var success = await _servoManager.ChangeServoIdAsync(
            CurrentId,
            newId,
            message =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ProgressMessage = message;
                });
            }
        );

        if (success)
        {
            // Small delay to show final message
            await Task.Delay(500);
            SuccessfulChange?.Invoke();
            CloseRequested?.Invoke();
        }

        IsChanging = false;
    }
}
