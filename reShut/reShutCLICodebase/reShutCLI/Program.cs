using System;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
namespace reShutCLI;

/// <summary>
/// Main class for the reShut CLI application.
/// Handles application initialization, main loop, UI rendering, and input processing.
/// </summary>
[SupportedOSPlatform("windows")]
internal class Program
{

    /// <summary>
    /// The main entry point for the application.
    /// Initializes the application, displays the EULA if necessary, and enters the main program loop.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    public static void Main(string[] args)
    {
        Console.Clear();
        InitializeApp(args);

        // Load languages for string resources
        CultureInfo culture = new CultureInfo(Variables.lang);
        ResourceManager rm = new ResourceManager(Constants.ResourceAssemblyName, typeof(Program).Assembly);

        Console.Title = rm.GetString("ConsoleTitle", culture) + " " + Variables.fullversion;

        // Checks if EULA is accepted
        if (RegistryWorker.ReadFromRegistry(Constants.RegistryPathConfig, Constants.RegistryValueEulaAccepted) == Constants.EulaNotAcceptedValue)
        {
            // Prompt EULA
            if (!ShowEULA.Start())
            {
                Environment.Exit(Constants.ExitCodeSuccess);
            }
        }
        while (true)
        {
            Console.Title = rm.GetString("ConsoleTitle", culture) + " " + Variables.fullversion;
            // Prints ascii art
            PrintLogo();


            // Print the main menu
            string key = MainMenu();

            // Check the input
            CheckInput(key);
        }
    }

    /// <summary>
    /// Now redirects to UIDraw.cs
    /// </summary>
    /// <param name="messageText">The text of the message to display.</param>
    private static void DisplayBorderedMessage(string messageText)
    {
        UIDraw.DrawBoxedMessage(messageText);
    }

    /// <summary>
    /// Displays a confirmation prompt if skipConfirmation is not enabled in the registry.
    /// The prompt is a bordered message asking for user confirmation.
    /// </summary>
    private static void ConfirmationPrompt()
    {
        // Check registry setting to see if confirmation can be skipped.
        if (RegistryWorker.ReadFromRegistry(Constants.RegistryPathConfig, Constants.RegistryValueSkipConfirmation) != Constants.SkipConfirmationEnabledValue)
        {
            CultureInfo culture = new CultureInfo(Variables.lang);
            ResourceManager rm = new ResourceManager(Constants.ResourceAssemblyName, typeof(Program).Assembly);
            string confirmationText = rm.GetString("ConfirmationText", culture); // Get localized confirmation text.

            UIDraw.TextColor = Variables.SecondaryColor; // Set text color for the prompt.
            UIDraw.DrawCenteredLine("\n"); // Add a newline for spacing.
            DisplayBorderedMessage(confirmationText); // Display the actual bordered message.
        }
    }

    /// <summary>
    /// Displays the license information text.
    /// </summary>
    private static void License()
    {
        CultureInfo culture = new CultureInfo(Variables.lang);
        ResourceManager rm = new ResourceManager(Constants.ResourceAssemblyName, typeof(Program).Assembly);
        UIDraw.DrawLine(rm.GetString("LicenseText", culture)); // Get and print localized license text.
    }

    /// <summary>
    /// Centers and displays the ASCII art logo, application version, and copyright information.
    /// This method is typically called when printing the main logo or header.
    /// </summary>
    public static void CenterText()
    {
        if (Debugger())
        {
            UIDraw.TextColor = Variables.SecondaryColor;
            if (Variables.DevelopmentBuild)UIDraw.DrawCenteredLine("Debugger attached!");
            if (!Variables.DevelopmentBuild) UIDraw.DrawCenteredLine("A debugger has been attached to a non-dev build of reShutCLI.");
        }
        UIDraw.TextColor = Variables.LogoColor;
        // ASCII art logo definition.
        string[] lines =
        [
            @"          __ _           _        ___   __   _____ ",
            @" _ __ ___/ _\ |__  _   _| |_     / __\ / /   \_   \",
            @"| '__/ _ \ \| '_ \| | | | __|   / /   / /     / /\/",
            @"| | |  __/\ \ | | | |_| | |_   / /___/ /___/\/ /_  ",
            @"|_|  \___\__/_| |_|\__,_|\__|  \____/\____/\____/  ",
            @"                                                   "
        ];

        var maxLength = lines.Max(line => line.Length); // Find the widest line in the ASCII art.
        // Calculate padding to center the ASCII art based on console width.
        var padding = (Console.WindowWidth - maxLength) / 2;

        // Print each line of the ASCII art centered.
        foreach (var line in lines)
        {
            var centeredLine = new string(' ', padding) + line;
            UIDraw.DrawLine(centeredLine);
        }

        string centeredText; // This will hold the version string.

        // Load resources for localized strings.
        CultureInfo culture = new CultureInfo(Variables.lang);
        ResourceManager rm = new ResourceManager(Constants.ResourceAssemblyName, typeof(Program).Assembly);

        // Construct the version string, adding "Pre-Release" if applicable.
        if (Variables.prerelease)
            centeredText =
                new string(' ', (Console.WindowWidth - rm.GetString("PreRelease", culture).Length - Variables.fullversion.Length) / 2) + // Calculate padding for pre-release version
                rm.GetString("PreRelease", culture) + Variables.fullversion;
        else
            centeredText = new string(' ', (Console.WindowWidth - Variables.fullversion.Length) / 2) + // Calculate padding for standard version
                           Variables.fullversion;

        // Construct the copyright string.
        var copyright = new string(' ', (Console.WindowWidth - rm.GetString("CopyrightText", culture).Length) / 2) + // Calculate padding for copyright
                        rm.GetString("CopyrightText", culture);

        // Print the centered version and copyright information.
        UIDraw.DrawLine(centeredText);
        UIDraw.DrawLine(copyright);
    }

    /// <summary>
    /// Displays the main menu of the application and waits for user input.
    /// </summary>
    /// <returns>A string representing the key pressed by the user.</returns>
    private static string MainMenu()
    {
        CultureInfo culture = new CultureInfo(Variables.lang);
        ResourceManager rm = new ResourceManager(Constants.ResourceAssemblyName, typeof(Program).Assembly);
        UIDraw.TextColor = Variables.MenuColor; // Set color for the menu text.

        // Get localized menu item strings.
        string[] menuItems = [rm.GetString("Shutdown", culture), rm.GetString("Reboot", culture), rm.GetString("Logoff", culture), rm.GetString("Schedule", culture), rm.GetString("Settings", culture), rm.GetString("Quit", culture)];

        // Display the menu structure with borders and items.
        // The main menu title is fetched from resources, allowing for localization.
        UIDraw.DrawCenteredLine("╭────────────────────────╮");
        UIDraw.DrawCenteredLine($"│       {rm.GetString("MainMenu", culture)}        │");
        UIDraw.DrawCenteredLine("├────────────────────────┤");
        for (var i = Constants.MainMenuStartIndex; i < menuItems.Length - 1; i++) // Loop through standard menu items (1-3).
            UIDraw.DrawCenteredLine("│ " + i + ") " + menuItems[i - 1].PadRight(Constants.MenuItemPaddingWidth) + "│");
        UIDraw.DrawCenteredLine("├────────────────────────┤");
        UIDraw.DrawCenteredLine("│ 9) " + menuItems[4].PadRight(Constants.MenuItemPaddingWidth) + "│"); // Settings item (9).
        UIDraw.DrawCenteredLine("│ 0) " + menuItems[5].PadRight(Constants.MenuItemPaddingWidth) + "│"); // Quit item (0).

        UIDraw.DrawCenteredLine("╰────────────────────────╯");

        // Read the user's key press.
        var keyInfo = Console.ReadKey();
        var key = keyInfo.KeyChar.ToString();
        return key;
    }

    /// <summary>
    /// Performs initial setup for the application.
    /// This includes setting console encoding, populating registry defaults,
    /// running first-time setup (like EULA), updating version info in registry,
    /// parsing command line arguments, and loading themes.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    private static void InitializeApp(string[] args)
    {
        // Set console output to UTF8 to support various characters.
        Console.OutputEncoding = Encoding.UTF8;

        const int STD_OUTPUT_HANDLE = -11;
        const int ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        [DllImport("kernel32.dll")] static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll")] static extern bool GetConsoleMode(IntPtr hConsoleHandle, out int mode);
        [DllImport("kernel32.dll")] static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
        var h = GetStdHandle(STD_OUTPUT_HANDLE);
        if (GetConsoleMode(h, out var mode))
            SetConsoleMode(h, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);

    // Initialize or update registry entries, false means not forced.
    RegInit.Populate(false);

        // Perform first startup routines (e.g., EULA display).
        Setup.FirstStartup();

        // Update the stored application version in the registry.
        RegistryWorker.WriteToRegistry(Constants.RegistryPathBase, Constants.RegistryValueReShutVersion, Constants.RegistryValueTypeString, Variables.version);

        // Process any command-line arguments.
        var cmdLine = new CmdLine(args);

        // Configure console appearance.
        Console.CursorVisible = false; // Hide cursor.
        UIDraw.BackgroundColor = ConsoleColor.Black; // Set background color.
        Console.Clear(); // Clear console.

        // Load user-selected theme.
        ThemeLoader.loadTheme();
        if (RegistryWorker.ReadFromRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI", "DefaultThemeRecentName") == "")
        { // If the registry is empty, write new default theme name.
            RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI", "DefaultThemeRecentName", "STRING", Variables.UpdatedDefaultThemeName);
        }

    }

    /// <summary>
    /// Prints the application logo (ASCII art and version info) and checks for updates if enabled.
    /// </summary>
    private static void PrintLogo()
    {
        if (RegistryWorker.ReadFromRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI", "DefaultThemeRecentName") != Variables.UpdatedDefaultThemeName) 
        {
            UIDraw.TextColor = Variables.SecondaryColor;
            UIDraw.DrawCenteredLine("New default theme installed: " + Variables.UpdatedDefaultThemeName);
            RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI", "DefaultThemeRecentName", "STRING", Variables.UpdatedDefaultThemeName);
        }
        UIDraw.TextColor = Variables.LogoColor; // Set color for the logo.
        CenterText(); // Display the centered ASCII art, version, and copyright.

        // Initialize resources for this method's scope
        CultureInfo culture = new CultureInfo(Variables.lang);
        ResourceManager rm = new ResourceManager(Constants.ResourceAssemblyName, typeof(Program).Assembly);

        // Check for updates if the feature is enabled in the registry.
        if (RegistryWorker.ReadFromRegistry(Constants.RegistryPathConfig, Constants.RegistryValueEnableUpdateSearch) == Constants.UpdateSearchEnabledValue)
        {
            try
            {
                UpdateChecker.MainCheck().Wait(); // Perform the update check.
            }
            catch // Handle potential errors during update check (e.g., network issues).
            {
                UIDraw.TextColor = Variables.SecondaryColor;
                UIDraw.DrawCenteredLine(rm.GetString("UpdateCheckFailed", culture)); // Display localized error message.
                UIDraw.TextColor = ConsoleColor.Gray; // Reset color.
            }
        }
        else
        {
            // If update search is disabled, display a message indicating so.
            UIDraw.TextColor = Variables.SecondaryColor;
            UIDraw.DrawCenteredLine(rm.GetString("UpdateSearchDisabled", culture)); // Display localized disabled message.
            UIDraw.TextColor = ConsoleColor.Gray; // Reset color.
        }
    }

    /// <summary>
    /// Processes the user's input key from the main menu and performs the corresponding action.
    /// </summary>
    /// <param name="key">The key pressed by the user.</param>
    private static void CheckInput(string key)
    {
        CultureInfo culture = new CultureInfo(Variables.lang);
        ResourceManager rm = new ResourceManager(Constants.ResourceAssemblyName, typeof(Program).Assembly);

        switch (key.ToLower()) // Convert key to lowercase to handle case-insensitivity.
        {
            case "l": HandleLicense(culture, rm); break;
            case "1": HandleShutdown(); break;
            case "2": HandleReboot(); break;
            case "3": HandleLogoff(); break;
            case "9": HandleSettings(); break;
            case "0": HandleQuit(); break;
            case "4": HandleSchedule(); break;
            case "u": HandleUpdate(culture, rm); break;
            default: HandleInvalidInput(culture, rm); break;
        }
    }

    /// <summary>
    /// Handles the display of the license information.
    /// </summary>
    /// <param name="culture">The culture information for localization.</param>
    /// <param name="rm">The resource manager for retrieving localized strings.</param>
    private static void HandleLicense(CultureInfo culture, ResourceManager rm)
    {
        Console.Clear();
        License(); // License method itself handles its own rm and culture
        string welcomeMessage = rm.GetString("PressAnyKeyToGoBack", culture);
        UIDraw.DrawLine(Environment.NewLine + welcomeMessage);
        Console.ReadKey();
        Console.Clear();
    }

    /// <summary>
    /// Handles the shutdown action, including confirmation.
    /// </summary>
    private static void HandleShutdown()
    {
        ConfirmationPrompt();
        Console.ReadKey();
        Handler.Shutdown();
    }

    /// <summary>
    /// Handles the reboot action, including confirmation.
    /// </summary>
    private static void HandleReboot()
    {
        ConfirmationPrompt();
        Console.ReadKey();
        Handler.Reboot();
    }

    /// <summary>
    /// Handles the logoff action, including confirmation.
    /// </summary>
    private static void HandleLogoff()
    {
        ConfirmationPrompt();
        Console.ReadKey();
        Handler.Logoff();
    }

    /// <summary>
    /// Handles displaying the settings menu.
    /// </summary>
    private static void HandleSettings()
    {
        Settings.Show();
    }

    /// <summary>
    /// Handles quitting the application.
    /// </summary>
    private static void HandleQuit()
    {
        Environment.Exit(Constants.ExitCodeSuccess);
    }

    /// <summary>
    /// Handles displaying the schedule planner.
    /// </summary>
    private static void HandleSchedule()
    {
        Schedule.Plan();
    }

    /// <summary>
    /// Handles the application update process.
    /// </summary>
    /// <param name="culture">The culture information for localization.</param>
    /// <param name="rm">The resource manager for retrieving localized strings.</param>
    private static void HandleUpdate(CultureInfo culture, ResourceManager rm)
    {
        if (Variables.isUpToDate)
        {
            Console.Clear();
            UIDraw.TextColor = Variables.SecondaryColor;
            UIDraw.DrawBoxedMessage(rm.GetString("UpToDate", culture));
            return;
        }
        Console.Clear();
        Console.Title = rm.GetString("UpdaterTitle", culture);
        UIDraw.TextColor = Variables.MenuColor;
        UIDraw.DrawCenteredLine(rm.GetString("UpdateDLStarted", culture));
        Thread.Sleep(Constants.UpdateDownloadMessageDelayMs);
#pragma warning disable CS4014 // Call is not awaited
        AutoUpdater.PerformUpdate();
#pragma warning restore CS4014
        Console.ReadLine();
        Environment.Exit(Constants.ExitCodeSuccess);
    }

    /// <summary>
    /// Handles invalid user input by displaying an error message.
    /// </summary>
    /// <param name="culture">The culture information for localization.</param>
    /// <param name="rm">The resource manager for retrieving localized strings.</param>
    private static void HandleInvalidInput(CultureInfo culture, ResourceManager rm)
    {
        Console.Clear();
        string invalidText = rm.GetString("InvalidInput", culture);
        UIDraw.TextColor = Variables.SecondaryColor;
        DisplayBorderedMessage(invalidText);
        UIDraw.TextColor = ConsoleColor.White;
    }
    public static bool Debugger()
    {
        if (System.Diagnostics.Debugger.IsAttached)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}