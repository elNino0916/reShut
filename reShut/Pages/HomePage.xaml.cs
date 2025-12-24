using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using reShut.Helpers;
using reShut.Services;
using System;

namespace reShut.Pages;

public sealed partial class HomePage : Page
{
    public HomePage()
    {
        InitializeComponent();
        UpdateAdminStatus();
    }

    private void UpdateAdminStatus()
    {
        bool isAdmin = PowerService.IsAdmin();
        if (isAdmin)
        {
            AdminText.Text = "Running as Administrator";
            AdminDesc.Text = "All power operations are available.";
            AdminIcon.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Green);
        }
        else
        {
            AdminText.Text = "Not running as Administrator";
            AdminDesc.Text = "Some operations may require elevated privileges.";
            AdminIcon.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Orange);
        }
    }

    private async void ShutdownButton_Click(object sender, RoutedEventArgs e)
    {
        bool confirmed = await DialogHelper.ShowConfirmationAsync(
            XamlRoot,
            "Confirm Shutdown",
            "Are you sure you want to shut down your computer? All unsaved work will be lost.");

        if (confirmed)
        {
            var result = PowerService.Shutdown();
            if (!result.Success)
            {
                await DialogHelper.ShowErrorAsync(XamlRoot, "Shutdown Failed", result.ErrorMessage ?? "An unknown error occurred.");
            }
        }
    }

    private async void RestartButton_Click(object sender, RoutedEventArgs e)
    {
        bool confirmed = await DialogHelper.ShowConfirmationAsync(
            XamlRoot,
            "Confirm Restart",
            "Are you sure you want to restart your computer? All unsaved work will be lost.");

        if (confirmed)
        {
            var result = PowerService.Reboot();
            if (!result.Success)
            {
                await DialogHelper.ShowErrorAsync(XamlRoot, "Restart Failed", result.ErrorMessage ?? "An unknown error occurred.");
            }
        }
    }

    private async void LogoffButton_Click(object sender, RoutedEventArgs e)
    {
        bool confirmed = await DialogHelper.ShowConfirmationAsync(
            XamlRoot,
            "Confirm Sign Out",
            "Are you sure you want to sign out? All unsaved work will be lost.");

        if (confirmed)
        {
            var result = PowerService.Logoff();
            if (!result.Success)
            {
                await DialogHelper.ShowErrorAsync(XamlRoot, "Sign Out Failed", result.ErrorMessage ?? "An unknown error occurred.");
            }
        }
    }
}
