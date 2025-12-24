using System;
using System.Text;
using System.Threading;

namespace reShutCLI
{
    internal class Setup
    {
        // Updated for 2.0.0.0
        public static void FirstStartup()
        {
            // Check if setup is complete from registry
            if (RegistryWorker.ReadFromRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "SetupComplete") != "1")
            {
                Console.OutputEncoding = Encoding.UTF8;
                Console.CursorVisible = false;
                ThemeLoader.setDefaultTheme();
                UIDraw.TextColor = Variables.LogoColor;
                Console.Title = "Welcome to reShut CLI!";
                Program.CenterText();
                UIDraw.TextColor = ConsoleColor.Gray;
                Thread.Sleep(1500);
                Console.Clear();
                UIDraw.TextColor = Variables.MenuColor;
                Console.Title = "reShut CLI Setup";
                UIDraw.DrawCenteredLine("╭────────────────────────╮");
                UIDraw.DrawCenteredLine("│ Welcome to reShut CLI! │");
                UIDraw.DrawCenteredLine("╰────────────────────────╯");
                RegInit.Populate(false);
                Thread.Sleep(2000);
                Console.Clear();
                step2:
                Console.Clear();
                UIDraw.DrawCenteredLine("╭────────────────╮");
                UIDraw.DrawCenteredLine("│ Language (2/5) │");
                UIDraw.DrawCenteredLine("╰────────────────╯");
                UIDraw.DrawCenteredLine("");
                UIDraw.DrawCenteredLine("╭──────────────────────────────────────────────────────────────────────────────────────╮");
                UIDraw.DrawCenteredLine("│ Help translate reShut CLI to your language! https://github.com/elnino0916/reshut-cli │");
                UIDraw.DrawCenteredLine("╰──────────────────────────────────────────────────────────────────────────────────────╯");
                UIDraw.DrawCenteredLine("");
                UIDraw.DrawCenteredLine("╭────────────────────────────────────────────╮");
                UIDraw.DrawCenteredLine("│ Select the language you would like to use. │");
                UIDraw.DrawCenteredLine("╰────────────────────────────────────────────╯");
                UIDraw.DrawCenteredLine("");
                UIDraw.DrawCenteredLine("╭──────────────────────────────────╮");
                UIDraw.DrawCenteredLine("│ 1) English (US) [100%]           │");
                UIDraw.DrawCenteredLine("├──────────────────────────────────┤");
                UIDraw.DrawCenteredLine("│ 2) German (Deutsch) [100%]       │");
                UIDraw.DrawCenteredLine("╰──────────────────────────────────╯");
                var keyInfoL = Console.ReadKey();
                var key = keyInfoL.KeyChar.ToString();
                switch (key)
                {
                    case "1":
                        RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "Language", "STRING", "en-US");
                        break;

                    case "2":
                        RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "Language", "STRING", "de-DE");
                        break;

                    // Example to add more languages
                    /* case "3":
                         RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "Language", "STRING", "fr-FR"); // French for example
                         break;
                    */
                    default:
                        goto step2;
                }

                Console.Clear();
                UIDraw.DrawCenteredLine("╭────────────╮");
                UIDraw.DrawCenteredLine("│ EULA (2/5) │");
                UIDraw.DrawCenteredLine("╰────────────╯");
                UIDraw.DrawCenteredLine("");
                ShowEULA.Start();
                UIDraw.TextColor = Variables.MenuColor;

            step3:
                Console.Clear();
                UIDraw.DrawCenteredLine("╭───────────────╮");
                UIDraw.DrawCenteredLine("│ Updates (3/5) │");
                UIDraw.DrawCenteredLine("╰───────────────╯");
                UIDraw.DrawCenteredLine("");
                UIDraw.DrawCenteredLine("╭───────────────────────────────────────────────────────────────────────────────╮");
                UIDraw.DrawCenteredLine("│ Do you want to enable update checking? This uses GitHub to check for updates. │");
                UIDraw.DrawCenteredLine("╰───────────────────────────────────────────────────────────────────────────────╯");
                UIDraw.DrawCenteredLine("");
                UIDraw.DrawCenteredLine("╭───────────────────────╮");
                UIDraw.DrawCenteredLine("│ 1) Yes, enable        │");
                UIDraw.DrawCenteredLine("├───────────────────────┤");
                UIDraw.DrawCenteredLine("│ 2) No, disable        │");
                UIDraw.DrawCenteredLine("╰───────────────────────╯");
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.D1 || keyInfo.Key == ConsoleKey.NumPad1)
                {
                    RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "EnableUpdateSearch", "STRING", "1");
                }
                else if (keyInfo.Key == ConsoleKey.D2 || keyInfo.Key == ConsoleKey.NumPad2)
                {
                    RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "EnableUpdateSearch", "STRING", "0");
                }
                else
                {
                    goto step3;
                }
            step4:
                Console.Clear();
                UIDraw.DrawCenteredLine("╭───────────────╮");
                UIDraw.DrawCenteredLine("│ Updates (3/5) │");
                UIDraw.DrawCenteredLine("╰───────────────╯");
                UIDraw.DrawCenteredLine("");
                UIDraw.DrawCenteredLine("╭──────────────────────────────────────────────────────────────╮");
                UIDraw.DrawCenteredLine("│ Do you want to enable Auto Updates on startup? (recommended) │");
                UIDraw.DrawCenteredLine("╰──────────────────────────────────────────────────────────────╯");
                UIDraw.DrawCenteredLine("");
                UIDraw.DrawCenteredLine("╭───────────────────────╮");
                UIDraw.DrawCenteredLine("│ 1) Yes, enable        │");
                UIDraw.DrawCenteredLine("├───────────────────────┤");
                UIDraw.DrawCenteredLine("│ 2) No, disable        │");
                UIDraw.DrawCenteredLine("╰───────────────────────╯");
                ConsoleKeyInfo keyInfo3 = Console.ReadKey();
                if (keyInfo3.Key == ConsoleKey.D1 || keyInfo3.Key == ConsoleKey.NumPad1)
                {
                    RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "AutoUpdateOnStart", "STRING", "yes");
                }
                else if (keyInfo3.Key == ConsoleKey.D2 || keyInfo3.Key == ConsoleKey.NumPad2)
                {
                    RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "AutoUpdateOnStart", "STRING", "no");
                }
                else
                {
                    goto step4;
                }
            step5:
                Console.Clear();
                UIDraw.DrawCenteredLine("╭─────────────╮");
                UIDraw.DrawCenteredLine("│ Theme (4/5) │");
                UIDraw.DrawCenteredLine("╰─────────────╯");
                UIDraw.DrawCenteredLine("");
                UIDraw.DrawCenteredLine("╭────────────────────────────────────────────────╮");
                UIDraw.DrawCenteredLine("│ Select a theme to personalize your experience. │");
                UIDraw.DrawCenteredLine("╰────────────────────────────────────────────────╯");
                UIDraw.DrawCenteredLine("");
                UIDraw.TextColor = Variables.SecondaryColor;
                UIDraw.DrawBoxedMessage("Current default theme name: " + Variables.UpdatedDefaultThemeName);
                UIDraw.TextColor = Variables.MenuColor;
                UIDraw.DrawCenteredLine("");
                UIDraw.DrawCenteredLine("╭───────────────────────────────╮");
                UIDraw.DrawCenteredLine("│ 1) Default (Network required) │");
                UIDraw.DrawCenteredLine("├───────────────────────────────┤");
                UIDraw.DrawCenteredLine("│ 2) Red                        │");
                UIDraw.DrawCenteredLine("│ 3) Blue                       │");
                UIDraw.DrawCenteredLine("│ 4) Green                      │");
                UIDraw.DrawCenteredLine("│ 5) Nord                       │");
                UIDraw.DrawCenteredLine("╰───────────────────────────────╯");
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
                else
                {
                    goto step5;
                }
                Console.Clear();
                UIDraw.DrawCenteredLine("╭─────────────────────╮");
                UIDraw.DrawCenteredLine("│ Applying changes... │");
                UIDraw.DrawCenteredLine("╰─────────────────────╯");
                ThemeLoader.loadTheme();
                UIDraw.TextColor = Variables.MenuColor;

            step6:
                Console.Clear();
                UIDraw.DrawCenteredLine("╭────────────────────╮");
                UIDraw.DrawCenteredLine("│ Confirmation (5/5) │");
                UIDraw.DrawCenteredLine("╰────────────────────╯");
                UIDraw.DrawCenteredLine("");
                UIDraw.DrawCenteredLine("╭──────────────────────────────────────────────────────────────────────────────╮");
                UIDraw.DrawCenteredLine("│ Would you like to be asked twice before shutting down / rebooting your PC?   │");
                UIDraw.DrawCenteredLine("╰──────────────────────────────────────────────────────────────────────────────╯");
                UIDraw.DrawCenteredLine("");
                UIDraw.DrawCenteredLine("╭───────────────────────╮");
                UIDraw.DrawCenteredLine("│ 1) Yes, ask twice     │");
                UIDraw.DrawCenteredLine("├───────────────────────┤");
                UIDraw.DrawCenteredLine("│ 2) No, ask once       │");
                UIDraw.DrawCenteredLine("╰───────────────────────╯");
                ConsoleKeyInfo keyInfo5 = Console.ReadKey();
                if (keyInfo5.Key == ConsoleKey.D1 || keyInfo5.Key == ConsoleKey.NumPad1)
                {
                    RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "SkipConfirmation", "STRING", "0");
                }
                else if (keyInfo5.Key == ConsoleKey.D2 || keyInfo5.Key == ConsoleKey.NumPad2)
                {
                    RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "SkipConfirmation", "STRING", "1");
                }
                else
                {
                    goto step6;
                }

                Console.Clear();
                UIDraw.DrawCenteredLine("╭─────────────────╮");
                UIDraw.DrawCenteredLine("│ Setup complete! │");
                UIDraw.DrawCenteredLine("╰─────────────────╯");
                RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "SetupComplete", "STRING", "1");
                Thread.Sleep(3000);
                AutoRestart.Init();
                return;
            }

        }
    }
}
