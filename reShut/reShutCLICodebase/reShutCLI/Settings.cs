using System;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text.RegularExpressions;

namespace reShutCLI
{
    internal class Settings
    {
        public static void UpdateSettings()
        {
        UpdateSettingsInit:
            Console.Clear();
            CultureInfo culture = new CultureInfo(Variables.lang);
            ResourceManager rm = new ResourceManager("reShutCLI.Resources.Strings", typeof(Program).Assembly);

            string updateSearchValue = RegistryWorker.ReadFromRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "EnableUpdateSearch");
            string autoUpdateValue = RegistryWorker.ReadFromRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "AutoUpdateOnStart");

            // Display status messages using UIDraw.DrawBoxedMessage
            UIDraw.DrawBoxedMessage(updateSearchValue == "1" ? rm.GetString("UpdateSettings_UpdateSearchEnabled", culture) : rm.GetString("UpdateSettings_UpdateSearchDisabled", culture));
            UIDraw.DrawLine(""); // Add a blank line for spacing
            UIDraw.DrawBoxedMessage(autoUpdateValue == "yes" ? rm.GetString("UpdateSettings_AutoUpdateEnabled", culture) : rm.GetString("UpdateSettings_AutoUpdateDisabled", culture));
            UIDraw.DrawLine(""); // Add a blank line for spacing

            // Display current settings using dynamic box drawing similar to other menus
            UIDraw.TextColor = Variables.MenuColor;
            string[] menuItems = [
                rm.GetString("UpdateSettings_ToggleUpdateSearch", culture),
                rm.GetString("UpdateSettings_ToggleAutoUpdate", culture),
                rm.GetString("Back", culture) // Already exists
            ];
            string settingsHeaderText = rm.GetString("UpdateSettings_Title", culture);

            int m1Length = ("1) " + menuItems[0]).Length;
            int m2Length = ("2) " + menuItems[1]).Length;
            int m9Length = ("9) " + menuItems[2]).Length;

            int maxFormattedItemLength = new[] { m1Length, m2Length, m9Length }.Max();
            int dynamicInnerWidth = Math.Max(settingsHeaderText.Length, maxFormattedItemLength) + 2;
            if (dynamicInnerWidth % 2 != 0) dynamicInnerWidth++;

            string topBorder = "╭" + new string('─', dynamicInnerWidth) + "╮";
            string bottomBorder = "╰" + new string('─', dynamicInnerWidth) + "╯";
            string separator = "├" + new string('─', dynamicInnerWidth) + "┤";

            int headerPaddingTotal = dynamicInnerWidth - settingsHeaderText.Length;
            int headerPaddingLeft = headerPaddingTotal / 2;
            string titleLine = "│" + new string(' ', headerPaddingLeft) + settingsHeaderText + new string(' ', dynamicInnerWidth - settingsHeaderText.Length - headerPaddingLeft) + "│";

            UIDraw.DrawCenteredLine(topBorder);
            UIDraw.DrawCenteredLine(titleLine);
            UIDraw.DrawCenteredLine(separator);
            UIDraw.DrawCenteredLine("│" + ("1) " + menuItems[0]).PadRight(dynamicInnerWidth) + "│");
            UIDraw.DrawCenteredLine("│" + ("2) " + menuItems[1]).PadRight(dynamicInnerWidth) + "│");
            UIDraw.DrawCenteredLine(separator);
            UIDraw.DrawCenteredLine("│" + ("9) " + menuItems[2]).PadRight(dynamicInnerWidth) + "│");
            UIDraw.DrawCenteredLine(bottomBorder);

            var keyInfo = Console.ReadKey();
            string key = keyInfo.KeyChar.ToString();
            switch (key)
            {
                case "1":
                    // Toggle Update Search
                    string newUpdateValue = (updateSearchValue == "1") ? "0" : "1";
                    RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "EnableUpdateSearch", "STRING", newUpdateValue);
                    goto UpdateSettingsInit;

                case "2":
                    // Toggle Auto Update on Startup (Note: Original code had "Toggle Update Search" comment here, corrected)
                    string newAutoUpdateValue = (autoUpdateValue == "yes") ? "no" : "yes";
                    RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "AutoUpdateOnStart", "STRING", newAutoUpdateValue);
                    goto UpdateSettingsInit;

                case "9":
                    Console.Clear();
                    return;

                default:
                    goto UpdateSettingsInit;
            }
        }

        public static void Show()
        {
            Console.Clear();
        settings:
            // Settings
            CultureInfo culture = new CultureInfo(Variables.lang);
            ResourceManager rm = new ResourceManager("reShutCLI.Resources.Strings", typeof(Program).Assembly);
            // Prints the settings menu
            UIDraw.TextColor = Variables.MenuColor;
            // New UI
            Console.Title = rm.GetString("ConsoleTitle", culture) + " " + Variables.fullversion;
            string[] menuItems = [rm.GetString("General", culture), rm.GetString("MenuAndText", culture), rm.GetString("About", culture), rm.GetString("Back", culture)];
            string settingsHeaderText = rm.GetString("Settings", culture);

            // Calculate lengths of fully formatted items like "1) General"
            int m1Length = ("1) " + menuItems[0]).Length;
            int m2Length = ("2) " + menuItems[1]).Length;
            int m3Length = ("3) " + menuItems[2]).Length; // About
            int m9Length = ("9) " + menuItems[3]).Length; // Back

            int maxFormattedItemLength = new[] { m1Length, m2Length, m3Length, m9Length }.Max();

            int dynamicInnerWidth = Math.Max(settingsHeaderText.Length, maxFormattedItemLength);
            dynamicInnerWidth += 2; // Add 2 for single space padding on left and right

            if (dynamicInnerWidth % 2 != 0) dynamicInnerWidth++; // Ensure even width for better centering

            string topBorder = "╭" + new string('─', dynamicInnerWidth) + "╮";
            string bottomBorder = "╰" + new string('─', dynamicInnerWidth) + "╯";
            string separator = "├" + new string('─', dynamicInnerWidth) + "┤";

            // Centering the header text
            int headerPaddingTotal = dynamicInnerWidth - settingsHeaderText.Length;
            int headerPaddingLeft = headerPaddingTotal / 2;
            string titleLine = "│" + new string(' ', headerPaddingLeft) + settingsHeaderText + new string(' ', dynamicInnerWidth - settingsHeaderText.Length - headerPaddingLeft) + "│";

            UIDraw.DrawCenteredLine(topBorder);
            UIDraw.DrawCenteredLine(titleLine);
            UIDraw.DrawCenteredLine(separator);

            // Items
            string item1Text = "1) " + menuItems[0];
            UIDraw.DrawCenteredLine("│" + item1Text.PadRight(dynamicInnerWidth) + "│");
            string item2Text = "2) " + menuItems[1];
            UIDraw.DrawCenteredLine("│" + item2Text.PadRight(dynamicInnerWidth) + "│");
            string item3Text = "3) " + menuItems[2]; // About
            UIDraw.DrawCenteredLine("│" + item3Text.PadRight(dynamicInnerWidth) + "│");
            UIDraw.DrawCenteredLine(separator);
            string item9Text = "9) " + menuItems[3]; // Back
            UIDraw.DrawCenteredLine("│" + item9Text.PadRight(dynamicInnerWidth) + "│");
            UIDraw.DrawCenteredLine(bottomBorder);
            UIDraw.TextColor = ConsoleColor.White;
            // Key detect
            var setInfo = Console.ReadKey();
            var set = setInfo.KeyChar.ToString();

            switch (set)
            {
                case "1":
                // General
                GeneralInit:
                    Console.Clear();
                    UIDraw.TextColor = Variables.MenuColor;
                    string[] menuItemsGen = [rm.GetString("Update", culture), rm.GetString("Theme", culture), rm.GetString("ResetAllSettings", culture), rm.GetString("Language", culture), rm.GetString("Back", culture)];
                    string generalHeaderText = rm.GetString("General", culture);

                    int gen_m1Length = ("1) " + menuItemsGen[0]).Length;
                    int gen_m2Length = ("2) " + menuItemsGen[1]).Length;
                    int gen_m3Length = ("3) " + menuItemsGen[2]).Length;
                    int gen_m4Length = ("4) " + menuItemsGen[3]).Length; // Language
                    int gen_m9Length = ("9) " + menuItemsGen[4]).Length; // Back

                    int gen_maxFormattedItemLength = new[] { gen_m1Length, gen_m2Length, gen_m3Length, gen_m4Length, gen_m9Length }.Max();

                    int gen_dynamicInnerWidth = Math.Max(generalHeaderText.Length, gen_maxFormattedItemLength);
                    gen_dynamicInnerWidth += 2; // Add 2 for single space padding

                    if (gen_dynamicInnerWidth % 2 != 0) gen_dynamicInnerWidth++; // Ensure even width

                    string gen_topBorder = "╭" + new string('─', gen_dynamicInnerWidth) + "╮";
                    string gen_bottomBorder = "╰" + new string('─', gen_dynamicInnerWidth) + "╯";
                    string gen_separator = "├" + new string('─', gen_dynamicInnerWidth) + "┤";

                    int gen_headerPaddingTotal = gen_dynamicInnerWidth - generalHeaderText.Length;
                    int gen_headerPaddingLeft = gen_headerPaddingTotal / 2;
                    string gen_titleLine = "│" + new string(' ', gen_headerPaddingLeft) + generalHeaderText + new string(' ', gen_dynamicInnerWidth - generalHeaderText.Length - gen_headerPaddingLeft) + "│";

                    UIDraw.DrawCenteredLine(gen_topBorder);
                    UIDraw.DrawCenteredLine(gen_titleLine);
                    UIDraw.DrawCenteredLine(gen_separator);

                    UIDraw.DrawCenteredLine("│" + ("1) " + menuItemsGen[0]).PadRight(gen_dynamicInnerWidth) + "│");
                    UIDraw.DrawCenteredLine("│" + ("2) " + menuItemsGen[1]).PadRight(gen_dynamicInnerWidth) + "│");
                    UIDraw.DrawCenteredLine("│" + ("3) " + menuItemsGen[2]).PadRight(gen_dynamicInnerWidth) + "│");
                    UIDraw.DrawCenteredLine("│" + ("4) " + menuItemsGen[3]).PadRight(gen_dynamicInnerWidth) + "│"); // Language
                    UIDraw.DrawCenteredLine(gen_separator);
                    UIDraw.DrawCenteredLine("│" + ("9) " + menuItemsGen[4]).PadRight(gen_dynamicInnerWidth) + "│"); // Back

                    UIDraw.DrawCenteredLine(gen_bottomBorder);

                    /*
                    UIDraw.DrawCenteredLine("╭───────────────────────────────╮");
                    UIDraw.DrawCenteredLine("│            General            │");
                    UIDraw.DrawCenteredLine("├───────────────────────────────┤");
                    UIDraw.DrawCenteredLine("│ 1) Update                     │");
                    UIDraw.DrawCenteredLine("│ 2) Startup                    │");
                    UIDraw.DrawCenteredLine("│ 3) Theme                      │");
                    UIDraw.DrawCenteredLine("│ 4) Reset all settings         │");
                    UIDraw.DrawCenteredLine("│ 5) Language                   │");
                    UIDraw.DrawCenteredLine("├───────────────────────────────┤");
                    UIDraw.DrawCenteredLine("│ 9) Back                       │");
                    UIDraw.DrawCenteredLine("╰───────────────────────────────╯");
                    */

                    var setInfoGen = Console.ReadKey();
                    var setGen = setInfoGen.KeyChar.ToString();
                    switch (setGen)
                    {
                        case "1":
                            UpdateSettings(); // Changed from StartupSettings.UpdateSettings()
                            goto GeneralInit;
                        case "2":
                            Console.Clear();
                            UIDraw.TextColor = Variables.MenuColor;
                            ThemeSettings.OpenSettings();
                            goto GeneralInit;

                        case "3":
                            ConfigManager.Reset();
                            goto GeneralInit;

                        case "4":
                            Console.Clear();
                            UIDraw.TextColor = Variables.MenuColor;
                            UIDraw.DrawBoxedMessage(rm.GetString("Language", culture));
                            UIDraw.DrawCenteredLine("");
                            UIDraw.DrawBoxedMessage(rm.GetString("HelpTranslate", culture) + " https://github.com/elnino0916/reshut-cli");
                            UIDraw.DrawCenteredLine("");
                            UIDraw.DrawBoxedMessage(rm.GetString("SelectLang", culture));
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

                                /*
                            case "3":
                                RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "Language", "STRING", "fr-FR"); // French for example
                                break;
                                */
                                default:
                                    break;
                            }
                            AutoRestart.Init();
                            goto GeneralInit;

                        case "9":
                            Console.Clear();
                            goto settings;
                        default:
                            Console.Clear();
                            string invalidText1 = rm.GetString("InvalidInput", culture);
                            int boxWidth1 = Math.Max(invalidText1.Length + 2, 44); 
                            string topBorder1 = "╭" + new string('─', boxWidth1) + "╮";
                            string bottomBorder1 = "╰" + new string('─', boxWidth1) + "╯";
                            int paddingLeft1 = (boxWidth1 - invalidText1.Length) / 2;
                            string paddedMessage1 = "│" + new string(' ', paddingLeft1) + invalidText1 + new string(' ', boxWidth1 - invalidText1.Length - paddingLeft1) + "│";
                            int windowWidth1 = Console.WindowWidth;
                            UIDraw.TextColor = Variables.SecondaryColor;
                            UIDraw.DrawCenteredLine(topBorder1);
                            UIDraw.DrawCenteredLine(paddedMessage1);
                            UIDraw.DrawCenteredLine(bottomBorder1);
                            UIDraw.TextColor = ConsoleColor.White;
                            goto GeneralInit;
                    }
                case "9":
                    Console.Clear();
                    return;
                case "2":
                MenuAndTextInit:
                    Console.Clear();
                    string skipValue = RegistryWorker.ReadFromRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "SkipConfirmation");
                    UIDraw.TextColor = Variables.LogoColor;
                    string doubleText = "DoubleConfirmoff";
                    if (skipValue == "0")
                    {
                        doubleText = rm.GetString("DoubleConfirmOn", culture);
                    }
                    else 
                    {
                        doubleText = rm.GetString("DoubleConfirmOff", culture); 
                    }

                    int boxWidthStateDouble = Math.Max(doubleText.Length + 2, 44);
                    string topBorderD = "╭" + new string('─', boxWidthStateDouble) + "╮";
                    string bottomBorderD = "╰" + new string('─', boxWidthStateDouble) + "╯";
                    int paddingLeftD = (boxWidthStateDouble - doubleText.Length) / 2;
                    string paddedMessageD = "│" + new string(' ', paddingLeftD) + doubleText + new string(' ', boxWidthStateDouble - doubleText.Length - paddingLeftD) + "│";
                    UIDraw.TextColor = Variables.SecondaryColor;
                    UIDraw.DrawCenteredLine(topBorderD);
                    UIDraw.DrawCenteredLine(paddedMessageD);
                    UIDraw.DrawCenteredLine(bottomBorderD);
                    UIDraw.TextColor = Variables.MenuColor;

                    string[] menuItemsMsg = { rm.GetString("DoubleConfiguration", culture), rm.GetString("Back", culture) };
                    string messagesHeaderText = rm.GetString("MenuAndText", culture);

                    int msg_m1Length = ("1) " + menuItemsMsg[0]).Length;
                    int msg_m9Length = ("9) " + menuItemsMsg[1]).Length;

                    int msg_maxFormattedItemLength = Math.Max(msg_m1Length, msg_m9Length);

                    int msg_dynamicInnerWidth = Math.Max(messagesHeaderText.Length, msg_maxFormattedItemLength);
                    msg_dynamicInnerWidth += 2; // Add 2 for single space padding

                    if (msg_dynamicInnerWidth % 2 != 0) msg_dynamicInnerWidth++; // Ensure even width

                    string msg_topBorder = "╭" + new string('─', msg_dynamicInnerWidth) + "╮";
                    string msg_bottomBorder = "╰" + new string('─', msg_dynamicInnerWidth) + "╯";
                    string msg_separator = "├" + new string('─', msg_dynamicInnerWidth) + "┤";

                    int msg_headerPaddingTotal = msg_dynamicInnerWidth - messagesHeaderText.Length;
                    int msg_headerPaddingLeft = msg_headerPaddingTotal / 2;
                    string msg_titleLine = "│" + new string(' ', msg_headerPaddingLeft) + messagesHeaderText + new string(' ', msg_dynamicInnerWidth - messagesHeaderText.Length - msg_headerPaddingLeft) + "│";

                    UIDraw.DrawCenteredLine(msg_topBorder);
                    UIDraw.DrawCenteredLine(msg_titleLine);
                    UIDraw.DrawCenteredLine(msg_separator);
                    UIDraw.DrawCenteredLine("│" + ("1) " + menuItemsMsg[0]).PadRight(msg_dynamicInnerWidth) + "│");
                    UIDraw.DrawCenteredLine(msg_separator);
                    UIDraw.DrawCenteredLine("│" + ("9) " + menuItemsMsg[1]).PadRight(msg_dynamicInnerWidth) + "│");
                    UIDraw.DrawCenteredLine(msg_bottomBorder);

                    /*
                    UIDraw.DrawCenteredLine("╭───────────────────────────────╮");
                    UIDraw.DrawCenteredLine("│       Menus and Messages      │");
                    UIDraw.DrawCenteredLine("├───────────────────────────────┤");
                    UIDraw.DrawCenteredLine("│ 1) Toggle Double-Confirmation │");
                    UIDraw.DrawCenteredLine("├───────────────────────────────┤");
                    UIDraw.DrawCenteredLine("│ 9) Back                       │");
                    UIDraw.DrawCenteredLine("╰───────────────────────────────╯");
                    */
                    var setInfoMenu = Console.ReadKey();
                    var setGenMenu = setInfoMenu.KeyChar.ToString();
                    switch (setGenMenu)
                    {
                        case "1":
                            if (skipValue == "1")
                            {
                                RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "SkipConfirmation", "STRING", "0");
                            }
                            else
                            {
                                RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "SkipConfirmation", "STRING", "1");
                            }
                            goto MenuAndTextInit;

                        case "9":
                            Console.Clear();
                            goto settings;

                        default:
                            goto MenuAndTextInit;
                    }
                case "3":

                    AboutPage.Show();
                    goto settings;

                // Go back when invalid key has been pressed
                default:
                    Console.Clear();
                    UIDraw.TextColor = Variables.SecondaryColor;
                    UIDraw.DrawBoxedMessage(rm.GetString("InvalidInput", culture));
                    UIDraw.TextColor = ConsoleColor.White;
                    goto settings;

            }
        }
    }
}
