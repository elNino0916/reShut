using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

namespace reShutCLI
{
    [SupportedOSPlatform("windows")]
    internal class CmdLine
    {
        public CmdLine(IEnumerable<string> args)
        {
            List<string> validPrefixes = ["-", "/"];
            Dictionary<string, string?> parsed = new(StringComparer.OrdinalIgnoreCase);

            foreach (var arg in args)
            {
                var prefix = validPrefixes.FirstOrDefault(p => arg.StartsWith(p));
                if (prefix != null)
                {
                    var option = arg[prefix.Length..];
                    var parts = option.Split([':', '='], 2);
                    var key = parts[0].ToLower();
                    var value = parts.Length > 1 ? parts[1] : null;
                    parsed[key] = value;
                }
                else
                {
                    UIDraw.TextColor = Variables.SecondaryColor;
                    UIDraw.DrawLine($"Missing Prefix in argument: {arg}");
                    UIDraw.TextColor = ConsoleColor.Gray;
                    Environment.Exit(0);
                }
            }

            if (parsed.TryGetValue("remote", out string? host))
            {
                if (host is null)
                {
                    UIDraw.TextColor = Variables.SecondaryColor;
                    UIDraw.DrawLine("Remote host specified but no host value provided.");
                    UIDraw.TextColor = ConsoleColor.Gray;
                    Environment.Exit(1);
                }
                parsed.TryGetValue("user", out string? user);
                parsed.TryGetValue("pass", out string? pass);
                bool reboot = parsed.ContainsKey("r") || parsed.ContainsKey("reboot");
                bool shutdown = parsed.ContainsKey("s") || parsed.ContainsKey("shutdown") || parsed.ContainsKey("poweroff");
                bool result;

                if (reboot)
                    result = RemoteManager.Trigger(host, user, pass, true);
                else if (shutdown)
                    result = RemoteManager.Trigger(host, user, pass, false);
                else
                {
                    UIDraw.TextColor = Variables.SecondaryColor;
                    UIDraw.DrawLine("Remote host specified but no action (-r/-s) provided.");
                    UIDraw.TextColor = ConsoleColor.Gray;
                    result = false;
                }

                if (result)
                    Environment.Exit(0);
                else
                    Environment.Exit(1);
            }

            foreach (var key in parsed.Keys)
            {
                switch (key)
                {
                    case "r":
                    case "reboot":
                        Handler.Reboot();
                        Environment.Exit(1);
                        break;
                    case "l":
                    case "logoff":
                        Handler.Logoff();
                        Environment.Exit(1);
                        break;
                    case "s":
                    case "shutdown":
                    case "poweroff":
                        Handler.Shutdown();
                        Environment.Exit(1);
                        break;
                    case "help":
                    case "?":
                    case "h":
                        UIDraw.TextColor = Variables.LogoColor;
                        UIDraw.DrawLine($"reShut CLI ({Variables.fullversion})");
                        UIDraw.TextColor = Variables.MenuColor;
                        UIDraw.DrawLine("╭────────────────────────────────────────╮");
                        UIDraw.DrawLine("│           Available Arguments:         │");
                        UIDraw.DrawLine("├────────────────────────────────────────┤");
                        UIDraw.DrawLine("│ [-s] [-shutdown] Shutdown this PC.     │");
                        UIDraw.DrawLine("│ [-r] [-reboot] Reboot this PC.         │");
                        UIDraw.DrawLine("│ [-l] [-logoff] Logout.                 │");
                        UIDraw.DrawLine("│ [-h] [-help] Prints this information.  │");
                        UIDraw.DrawLine("│ [-reset] Resets reShut CLI.            │");
                        UIDraw.DrawLine("│ [-remote:HOST] [-user:USER] [-pass:PW] │");
                        UIDraw.DrawLine("│    with -s or -r to manage remotely.   │");
                        UIDraw.DrawLine("╰────────────────────────────────────────╯");
                        UIDraw.TextColor = Variables.SecondaryColor;
                        UIDraw.TextColor = ConsoleColor.Gray;
                        Environment.Exit(0);
                        break;
                    case "reset":
                        RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI", "RegistryPopulated", "STRING", "0");
                        RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config", "SetupComplete", "STRING", "0");
                        UIDraw.TextColor = Variables.SecondaryColor;
                        UIDraw.DrawLine("reShut CLI has been reset.");
                        UIDraw.TextColor = ConsoleColor.Gray;
                        Environment.Exit(0);
                        break;
                    default:
                        UIDraw.TextColor = Variables.SecondaryColor;
                        UIDraw.DrawLine($"Unknown argument: {key}");
                        UIDraw.TextColor = ConsoleColor.Gray;
                        Environment.Exit(0);
                        break;
                }
            }
        }
    }
}
