using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using reShut.Helpers;
using reShut.Services;
using System;

namespace reShut.Pages;

public sealed partial class RemotePage : Page
{
    public RemotePage()
    {
        InitializeComponent();
    }

    private async void ExecuteButton_Click(object sender, RoutedEventArgs e)
    {
        string host = HostBox.Text?.Trim() ?? string.Empty;
        
        if (string.IsNullOrWhiteSpace(host))
        {
            ShowStatus("Please enter a hostname or IP address.", InfoBarSeverity.Warning);
            return;
        }

        string? username = string.IsNullOrWhiteSpace(UserBox.Text) ? null : UserBox.Text.Trim();
        string? password = string.IsNullOrWhiteSpace(PassBox.Password) ? null : PassBox.Password;

        var selectedRadio = ActionButtons.SelectedItem as RadioButton;
        bool isReboot = selectedRadio?.Tag?.ToString() == "restart";
        string action = isReboot ? "restart" : "shutdown";

        bool confirmed = await DialogHelper.ShowConfirmationAsync(
            XamlRoot,
            $"Confirm Remote {(isReboot ? "Restart" : "Shutdown")}",
            $"Are you sure you want to {action} the computer '{host}'?\n\nThis will affect the remote computer immediately.");

        if (!confirmed) return;

        ExecButton.IsEnabled = false;
        ShowStatus($"Connecting to {host}...", InfoBarSeverity.Informational);

        try
        {
            var result = isReboot 
                ? RemoteService.RemoteReboot(host, username, password)
                : RemoteService.RemoteShutdown(host, username, password);

            if (result.Success)
            {
                ShowStatus($"Successfully triggered {action} on {host}.", InfoBarSeverity.Success);
            }
            else
            {
                ShowStatus(result.ErrorMessage ?? "An unknown error occurred.", InfoBarSeverity.Error);
            }
        }
        finally
        {
            ExecButton.IsEnabled = true;
        }
    }

    private void ShowStatus(string message, InfoBarSeverity severity)
    {
        StatusBar.Message = message;
        StatusBar.Severity = severity;
        StatusBar.IsOpen = true;
    }
}
