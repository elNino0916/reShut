using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using WinRT.Interop;
using reShut.Pages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace reShut
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private AppWindow? _appWindow;

        public MainWindow()
        {
            InitializeComponent();
            
            // Set window size and title bar
            SetupWindow();
        }

        private void SetupWindow()
        {
            // Get the AppWindow for customization
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            _appWindow = AppWindow.GetFromWindowId(windowId);

            if (_appWindow != null)
            {
                // Set window size
                _appWindow.Resize(new Windows.Graphics.SizeInt32(1100, 700));

                // Set up custom title bar
                if (AppWindowTitleBar.IsCustomizationSupported())
                {
                    var titleBar = _appWindow.TitleBar;
                    titleBar.ExtendsContentIntoTitleBar = true;
                    titleBar.ButtonBackgroundColor = Colors.Transparent;
                    titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                    
                    // Set the drag region
                    SetTitleBar(TitleBarGrid);
                }
            }
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // Navigate to home page by default
            MainNavView.SelectedItem = MainNavView.MenuItems[0];
            MainContentFrame.Navigate(typeof(HomePage));
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
}
