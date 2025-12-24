using System;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace reShutCLI
{
    internal class ThemeSettings
    {
        public static void OpenSettings()
        {
        step6:
            Console.Clear();
            CultureInfo culture = new CultureInfo(Variables.lang);
            ResourceManager rm = new ResourceManager("reShutCLI.Resources.Strings", typeof(Program).Assembly);
            UIDraw.DrawBoxedMessage(rm.GetString("SelectTheme", culture));
            UIDraw.DrawCenteredLine("");
            UIDraw.TextColor = Variables.SecondaryColor;
            UIDraw.DrawBoxedMessage("Current default theme name: " + Variables.UpdatedDefaultThemeName);
            UIDraw.TextColor = Variables.MenuColor;
            UIDraw.DrawCenteredLine("╭──────────────────────────╮");
            UIDraw.DrawCenteredLine("│ 1) Default               │");
            UIDraw.DrawCenteredLine("├──────────────────────────┤");
            UIDraw.DrawCenteredLine("│ 2) Red                   │");
            UIDraw.DrawCenteredLine("│ 3) Blue                  │");
            UIDraw.DrawCenteredLine("│ 4) Green                 │");
            UIDraw.DrawCenteredLine("│ 5) Nord                  │");
            UIDraw.DrawCenteredLine("├──────────────────────────┤");
            UIDraw.DrawCenteredLine("│ 9) Back                  │");
            UIDraw.DrawCenteredLine("╰──────────────────────────╯");
            UIDraw.DrawCenteredLine(""); // Added for spacing before input
            ConsoleKeyInfo keyInfo4 = Console.ReadKey();
            if (keyInfo4.Key == ConsoleKey.D1 || keyInfo4.Key == ConsoleKey.NumPad1)
            {
                RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "SelectedTheme", "STRING", "default");
            }
            else if (keyInfo4.Key == ConsoleKey.D2 || keyInfo4.Key == ConsoleKey.NumPad2)
            {
                RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "SelectedTheme", "STRING", "red");
            }
            else if (keyInfo4.Key == ConsoleKey.D3 || keyInfo4.Key == ConsoleKey.NumPad3)
            {
                RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "SelectedTheme", "STRING", "blue");
            }
            else if (keyInfo4.Key == ConsoleKey.D4 || keyInfo4.Key == ConsoleKey.NumPad4)
            {
                RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "SelectedTheme", "STRING", "green");
            }
            else if (keyInfo4.Key == ConsoleKey.D5 || keyInfo4.Key == ConsoleKey.NumPad5)
            {
                RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "SelectedTheme", "STRING", "nord");
            }
            else if (keyInfo4.Key == ConsoleKey.D9 || keyInfo4.Key == ConsoleKey.NumPad9)
            {
                return;
            }
            else
            {
                goto step6;
            }
            ThemeLoader.loadTheme();
            Console.Clear();
            AutoRestart.Init();
            return;
        }
    }
}
