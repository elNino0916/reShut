using System;
using System.Globalization;
using System.Resources;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using System.Threading;

namespace reShutCLI
{
    internal class RegInit
    {
        public static void Populate(bool skipCreation)
        {

            if (!skipCreation)
            {
                if (RegistryWorker.ReadFromRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI", "RegistryPopulated") != "1")
                {
                    InitializeRegistry();
                    return;
                }
            }
            string currentRegistryVersion = RegistryWorker.ReadFromRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI", "RegistryVersion");

            // Check if the registry version has changed
            if (currentRegistryVersion != Variables.registryVersion)
            {
                CultureInfo culture = new CultureInfo(Variables.lang);
                ResourceManager rm = new ResourceManager("reShutCLI.Resources.Strings", typeof(Program).Assembly);
                // Get the translated string
                string invalidText = rm.GetString("RegUpdate", culture);

                // Calculate the maximum length (either the message or the box)
                int boxWidth = Math.Max(invalidText.Length + 2, 44); // Ensure minimum width of 44
                string topBorder = "╭" + new string('─', boxWidth) + "╮";
                string bottomBorder = "╰" + new string('─', boxWidth) + "╯";

                // Center the message within the box
                int paddingLeft = (boxWidth - invalidText.Length) / 2;
                string paddedMessage = "│" + new string(' ', paddingLeft) + invalidText + new string(' ', boxWidth - invalidText.Length - paddingLeft) + "│";

                // Center the entire box within the console window
                int windowWidth = Console.WindowWidth;

                // Print the confirmation message centered on the console
                UIDraw.TextColor = Variables.MenuColor;
                UIDraw.DrawCenteredLine(topBorder);
                UIDraw.DrawCenteredLine(paddedMessage);
                UIDraw.DrawCenteredLine(bottomBorder);
                Thread.Sleep(4000);
                Console.Clear();

                // Reset the registry if the app is downgraded
                if (string.Compare(currentRegistryVersion, Variables.registryVersion) > 0)
                {
                    RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI", "RegistryPopulated", "STRING", "0");
                    InitializeRegistry();
                    return;
                }

                // Delete no longer required keys.
                RegistryWorker.DeleteFromRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "EnableFastStartup"); // Removed in 2.0.0.0
                RegistryWorker.DeleteFromRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "EnableSounds"); // Removed in 1.0.4.0
                          
                string language = RegistryWorker.ReadFromRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "Language"); // Removed in a 2.0 pre-release
                if (language == "fr-FR" || language == "pt-PT" || language == "es-ES")
                {
                    RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "Language", "STRING", "en-US");
                }

                // Check and add new registry entries
                // AddOrUpdateRegistryEntry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "NewSetting1", "STRING", "DefaultValue1");

                // Update the registry version
                RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI", "RegistryVersion", "STRING", Variables.registryVersion);
            }
        }

        private static void InitializeRegistry()
        {
            RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI", "RegistryPopulated", "STRING", "1");
            RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI", "FirstStartupTime", "STRING", DateTime.Now.ToString("HH:mm:ss (dd.MM.yyyy)"));

            // Settings
            RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "EnableUpdateSearch", "STRING", "1");
            RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "EULAAccepted", "STRING", "0");
            RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "SetupComplete", "STRING", "0");
            RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "SelectedTheme", "STRING", "default");
            RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "SkipConfirmation", "STRING", "0");
            RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "Language", "STRING", "en-US");
            RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI", "RegistryVersion", "STRING", Variables.registryVersion);
            RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI", "reShutVersion", "STRING", Variables.version);
        }

        private static void AddOrUpdateRegistryEntry(string key, string valueName, string valueType, string defaultValue)
        {
            if (RegistryWorker.ReadFromRegistry(key, valueName) == null)
            {
                RegistryWorker.WriteToRegistry(key, valueName, valueType, defaultValue);
            }
        }
    }

}
