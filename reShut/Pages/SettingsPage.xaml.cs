using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using reShut.Services;
using System;
using Windows.System;

namespace reShut.Pages;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()
    {
        // Default to system theme
        ThemeSelector.SelectedIndex = 0;
        
        // Set version text from global AppInfo
        VersionText.Text = $"Version {AppInfo.Version}";
    }

    private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ThemeSelector.SelectedItem is ComboBoxItem selectedItem)
        {
            string? themeTag = selectedItem.Tag?.ToString();
            
            if (App.MainWindow?.Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = themeTag switch
                {
                    "Light" => ElementTheme.Light,
                    "Dark" => ElementTheme.Dark,
                    _ => ElementTheme.Default
                };
            }
        }
    }

    private async void GitHubButton_Click(object sender, RoutedEventArgs e)
    {
        await Launcher.LaunchUriAsync(new Uri(AppInfo.GitHubUrl));
    }

    private async void UpdateButton_Click(object sender, RoutedEventArgs e)
    {
        // Disable button and show checking state
        UpdateButton.IsEnabled = false;
        UpdateButtonText.Text = "Checking...";
        UpdateIcon.Glyph = "\uE895"; // Sync icon

        try
        {
            var result = await UpdateService.CheckForUpdatesAsync();

            if (result.Success)
            {
                if (result.UpdateAvailable)
                {
                    UpdateInfoBar.Title = "Update Available";
                    UpdateInfoBar.Message = $"Version {result.LatestVersion} is available. You are currently on version {AppInfo.Version}.";
                    UpdateInfoBar.Severity = InfoBarSeverity.Success;
                    UpdateInfoBar.IsOpen = true;
                    
                    // Add action button to open release page
                    UpdateInfoBar.ActionButton = new HyperlinkButton
                    {
                        Content = "Download",
                        NavigateUri = new Uri(result.ReleaseUrl ?? AppInfo.GitHubUrl)
                    };
                }
                else
                {
                    UpdateInfoBar.Title = "You're up to date";
                    UpdateInfoBar.Message = $"{AppInfo.AppName} {AppInfo.Version} is the latest version.";
                    UpdateInfoBar.Severity = InfoBarSeverity.Informational;
                    UpdateInfoBar.ActionButton = null;
                    UpdateInfoBar.IsOpen = true;
                }
            }
            else
            {
                UpdateInfoBar.Title = "Update Check Failed";
                UpdateInfoBar.Message = result.ErrorMessage ?? "An unknown error occurred.";
                UpdateInfoBar.Severity = InfoBarSeverity.Error;
                UpdateInfoBar.ActionButton = null;
                UpdateInfoBar.IsOpen = true;
            }
        }
        finally
        {
            // Reset button state
            UpdateButton.IsEnabled = true;
            UpdateButtonText.Text = "Check for Updates";
            UpdateIcon.Glyph = "\uE946"; // Refresh icon
        }
    }
}
