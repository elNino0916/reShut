using Microsoft.UI.Xaml;

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
    }
}
