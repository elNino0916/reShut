using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using reShut.Pages;
using System;
using System.Runtime.InteropServices;
using WinRT.Interop;

namespace reShut;

public sealed partial class MainWindow : Window
{
    private AppWindow? _appWindow;
    private IntPtr _hWnd;

    #region Native Methods

    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, uint attr, ref int attrValue, int attrSize);

    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [StructLayout(LayoutKind.Sequential)]
    private struct MARGINS
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
    }

    // DWM Window Attributes
    private const uint DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    private const uint DWMWA_WINDOW_CORNER_PREFERENCE = 33;
    private const uint DWMWA_SYSTEMBACKDROP_TYPE = 38;
    
    // Corner preferences
    private const int DWMWCP_ROUND = 2;
    
    // Backdrop types
    private const int DWMSBT_MAINWINDOW = 2; // Mica
    private const int DWMSBT_TABBEDWINDOW = 4; // Mica Alt

    // Window styles
    private const int GWL_STYLE = -16;
    private const int WS_CAPTION = 0x00C00000;
    private const int WS_THICKFRAME = 0x00040000;
    private const int WS_MINIMIZEBOX = 0x00020000;
    private const int WS_MAXIMIZEBOX = 0x00010000;
    private const int WS_SYSMENU = 0x00080000;

    // Minimum window size
    private const int MIN_WIDTH = 800;
    private const int MIN_HEIGHT = 600;

    #endregion

    public MainWindow()
    {
        InitializeComponent();

        // Set window title from AppInfo
        Title = AppInfo.AppName;
        TitleText.Text = AppInfo.AppName;

        SetupWindow();
        
        // Subscribe to size changed to enforce minimum size
        this.SizeChanged += MainWindow_SizeChanged;
    }

    private void MainWindow_SizeChanged(object sender, WindowSizeChangedEventArgs args)
    {
        // Enforce minimum window size
        if (_appWindow != null)
        {
            var size = _appWindow.Size;
            bool needsResize = false;
            int newWidth = size.Width;
            int newHeight = size.Height;

            if (size.Width < MIN_WIDTH)
            {
                newWidth = MIN_WIDTH;
                needsResize = true;
            }
            if (size.Height < MIN_HEIGHT)
            {
                newHeight = MIN_HEIGHT;
                needsResize = true;
            }

            if (needsResize)
            {
                _appWindow.Resize(new Windows.Graphics.SizeInt32(newWidth, newHeight));
            }
        }
    }

    private void SetupWindow()
    {
        _hWnd = WindowNative.GetWindowHandle(this);
        WindowId windowId = Win32Interop.GetWindowIdFromWindow(_hWnd);
        _appWindow = AppWindow.GetFromWindowId(windowId);

        // Ensure window has proper styles for animations
        EnsureWindowStyles();

        // Configure DWM for proper rendering and animations
        ConfigureDwmAttributes();

        if (_appWindow != null)
        {
            // Set initial size
            _appWindow.Resize(new Windows.Graphics.SizeInt32(1100, 700));

            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                var titleBar = _appWindow.TitleBar;
                titleBar.ExtendsContentIntoTitleBar = true;

                // Set title bar height to Tall (48px) so caption buttons fill the full height
                titleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

                SetTitleBar(TitleBarGrid);
            }
        }

        // Listen for theme changes
        if (Content is FrameworkElement rootElement)
        {
            rootElement.ActualThemeChanged += RootElement_ActualThemeChanged;
            UpdateTitleBarTheme(rootElement.ActualTheme);
        }
    }

    private void EnsureWindowStyles()
    {
        // Ensure the window has all the standard styles needed for proper animations
        int style = GetWindowLong(_hWnd, GWL_STYLE);
        style |= WS_CAPTION | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX | WS_SYSMENU;
        SetWindowLong(_hWnd, GWL_STYLE, style);
    }

    private void ConfigureDwmAttributes()
    {
        // Enable rounded corners (Windows 11)
        int cornerPreference = DWMWCP_ROUND;
        DwmSetWindowAttribute(_hWnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref cornerPreference, sizeof(int));

        // Set the system backdrop type to Mica Alt (matches our MicaBackdrop Kind="BaseAlt")
        int backdropType = DWMSBT_TABBEDWINDOW;
        DwmSetWindowAttribute(_hWnd, DWMWA_SYSTEMBACKDROP_TYPE, ref backdropType, sizeof(int));

        // Extend frame into client area - use 1 pixel top margin for animation support
        MARGINS margins = new() { Left = 0, Right = 0, Top = 1, Bottom = 0 };
        DwmExtendFrameIntoClientArea(_hWnd, ref margins);
    }

    private void RootElement_ActualThemeChanged(FrameworkElement sender, object args)
    {
        UpdateTitleBarTheme(sender.ActualTheme);
    }

    private void UpdateTitleBarTheme(ElementTheme theme)
    {
        if (_appWindow?.TitleBar == null || !AppWindowTitleBar.IsCustomizationSupported())
            return;

        var titleBar = _appWindow.TitleBar;
        bool isDark = theme == ElementTheme.Dark;

        // Set DWM attribute for immersive dark mode - this fixes the tooltip theming
        int useDarkMode = isDark ? 1 : 0;
        DwmSetWindowAttribute(_hWnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, sizeof(int));

        if (isDark)
        {
            // Dark theme colors
            titleBar.ButtonForegroundColor = Windows.UI.Color.FromArgb(255, 255, 255, 255);
            titleBar.ButtonHoverForegroundColor = Windows.UI.Color.FromArgb(255, 255, 255, 255);
            titleBar.ButtonPressedForegroundColor = Windows.UI.Color.FromArgb(200, 255, 255, 255);
            titleBar.ButtonInactiveForegroundColor = Windows.UI.Color.FromArgb(100, 255, 255, 255);
            titleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(25, 255, 255, 255);
            titleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(40, 255, 255, 255);
        }
        else
        {
            // Light theme colors
            titleBar.ButtonForegroundColor = Windows.UI.Color.FromArgb(255, 0, 0, 0);
            titleBar.ButtonHoverForegroundColor = Windows.UI.Color.FromArgb(255, 0, 0, 0);
            titleBar.ButtonPressedForegroundColor = Windows.UI.Color.FromArgb(200, 0, 0, 0);
            titleBar.ButtonInactiveForegroundColor = Windows.UI.Color.FromArgb(100, 0, 0, 0);
            titleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(20, 0, 0, 0);
            titleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(30, 0, 0, 0);
        }
    }

    private void NavView_Loaded(object sender, RoutedEventArgs e)
    {
        MainNavView.SelectedItem = MainNavView.MenuItems[0];
        MainContentFrame.Navigate(typeof(HomePage));

        if (Content is FrameworkElement rootElement)
        {
            UpdateTitleBarTheme(rootElement.ActualTheme);
        }
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {
            MainContentFrame.Navigate(typeof(SettingsPage));
            return;
        }

        if (args.SelectedItemContainer is NavigationViewItem selectedItem)
        {
            string? tag = selectedItem.Tag?.ToString();
            Type? pageType = tag switch
            {
                "home" => typeof(HomePage),
                "schedule" => typeof(SchedulePage),
                "remote" => typeof(RemotePage),
                _ => null
            };

            if (pageType != null)
            {
                MainContentFrame.Navigate(pageType);
            }
        }
    }
}
