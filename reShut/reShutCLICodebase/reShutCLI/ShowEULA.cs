using System;
using System.Runtime.Versioning;
using System.Threading;

namespace reShutCLI
{
    internal class ShowEULA
    {

        public static bool Start()
        {
        Retry:
            bool EULAAccepted = false;
            UIDraw.TextColor = ConsoleColor.Gray;
            UIDraw.DrawLine("\nEnd User License Agreement (EULA)");
            UIDraw.DrawLine("=================================");
            UIDraw.DrawLine("");
            UIDraw.DrawLine("Please read the following terms and conditions carefully before using this software.");
            UIDraw.DrawLine("");
            UIDraw.DrawLine("1. Updates & Web requests");
            UIDraw.DrawLine("   ---------");
            UIDraw.DrawLine("   reShut CLI checks for updates on GitHub, which can be disabled in the settings.");
            UIDraw.DrawLine("   Updates are provided via GitHub to ensure you have the latest features and security patches.");
            UIDraw.DrawLine("   Also, reShut CLI uses a API to receive latest theme data from elNino0916s servers.");
            UIDraw.DrawLine("");
            UIDraw.DrawLine("2. Disclaimer of Liability");
            UIDraw.DrawLine("   -------------------------");
            UIDraw.DrawLine("   Even though reShutCLI releases are tested, bugs can still occur.");
            UIDraw.DrawLine("   The authors are not responsible for any data loss or damages that may occur while using this software.");
            UIDraw.DrawLine("");
            UIDraw.DrawLine("3. Acceptance");
            UIDraw.DrawLine("   ------------");
            UIDraw.DrawLine("   By accepting this agreement, you agree to the terms and conditions stated above.");
            UIDraw.TextColor = ConsoleColor.Red;
            UIDraw.DrawBoxedMessage("Press (1) to accept these terms or (2) to decline.");
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            if (keyInfo.Key == ConsoleKey.D1 || keyInfo.Key == ConsoleKey.NumPad1)
            {
                EULAAccepted = true;
            }
            else if (keyInfo.Key == ConsoleKey.D2 || keyInfo.Key == ConsoleKey.NumPad2)
            {
                EULAAccepted = false;
            }
            else
            {
                goto Retry;
            }
            if (EULAAccepted)
            {
                Console.Clear();
                RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "EULAAccepted", "STRING", "1");
                return true;
            }
            else
            {
                UIDraw.TextColor = ConsoleColor.Red;
                Console.Clear();
                UIDraw.DrawBoxedMessage("Sorry, you cannot use reShut CLI without accepting the EULA");
                RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "EULAAccepted", "STRING", "0");
                Thread.Sleep(2000);
                Environment.Exit(0);
                return false;
            }
        }
    }
}
