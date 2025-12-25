using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using reShut.Services;
using System;
using Windows.System;

namespace reShut.Pages;

public sealed partial class SettingsPage : Page
{
    private bool _isInitializing = true;

    public SettingsPage()
    {
        InitializeComponent();
        LoadSettings();
        
        // Subscribe to theme changes for logo switching
        ActualThemeChanged += SettingsPage_ActualThemeChanged;
        UpdateLogoForTheme();
        
        _isInitializing = false;
    }

    private void SettingsPage_ActualThemeChanged(FrameworkElement sender, object args)
    {
        UpdateLogoForTheme();
    }

    private void UpdateLogoForTheme()
    {
        // ActualTheme gives us the real current theme (Light or Dark), 
        // even when RequestedTheme is Default
        bool isDark = ActualTheme == ElementTheme.Dark;

        // Dark theme = show white logo, Light theme = show black logo
        LogoDark.Visibility = isDark ? Visibility.Collapsed : Visibility.Visible;
        LogoLight.Visibility = isDark ? Visibility.Visible : Visibility.Collapsed;
    }

    private void LoadSettings()
    {
        // Load theme from saved settings
        string savedTheme = SettingsService.Theme;
        ThemeSelector.SelectedIndex = savedTheme switch
        {
            "Light" => 1,
            "Dark" => 2,
            _ => 0 // Default
        };

        // Load other settings
        ConfirmToggle.IsOn = SettingsService.ConfirmationDialogsEnabled;
        ForceToggle.IsOn = SettingsService.ForceCloseApps;

        // Set version text from global AppInfo
        VersionText.Text = $"Version {AppInfo.Version}";
    }

    private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isInitializing) return;

        if (ThemeSelector.SelectedItem is ComboBoxItem selectedItem)
        {
            string? themeTag = selectedItem.Tag?.ToString() ?? "Default";

            // Save the setting
            SettingsService.Theme = themeTag;

            // Apply the theme
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

    private void ConfirmToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (_isInitializing) return;
        SettingsService.ConfirmationDialogsEnabled = ConfirmToggle.IsOn;
    }

    private void ForceToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (_isInitializing) return;
        SettingsService.ForceCloseApps = ForceToggle.IsOn;
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
