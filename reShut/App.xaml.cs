using Microsoft.UI.Xaml;
using reShut.Services;

namespace reShut;

public partial class App : Application
{
    private static Window? _window;

    public static Window? MainWindow => _window;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();

        // Apply saved theme on startup
        ApplySavedTheme();
    }

    private void ApplySavedTheme()
    {
        if (_window?.Content is FrameworkElement rootElement)
        {
            string savedTheme = SettingsService.Theme;
            rootElement.RequestedTheme = savedTheme switch
            {
                "Light" => ElementTheme.Light,
                "Dark" => ElementTheme.Dark,
                _ => ElementTheme.Default
            };
        }
    }
}
