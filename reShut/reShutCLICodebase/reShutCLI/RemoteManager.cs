using System;
using System.Management;
using System.Runtime.Versioning;

namespace reShutCLI;

[SupportedOSPlatform("windows")]
internal static class RemoteManager
{
    public static bool Trigger(string host, string? username, string? password, bool reboot)
    {
            try
            {
                ConnectionOptions options = new ConnectionOptions();
                if (!string.IsNullOrWhiteSpace(username))
                {
                    options.Username = username;
                    options.Password = password ?? string.Empty;
                }

                ManagementScope scope = new ManagementScope($"\\\\{host}\\root\\cimv2", options);
                scope.Connect();

                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
                using ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                foreach (ManagementObject os in searcher.Get())
                {
                    os.InvokeMethod("Win32Shutdown", new object[] { reboot ? 6 : 5, 0 });
                }

                UIDraw.TextColor = ConsoleColor.Green;
                UIDraw.DrawLine($"Remote {(reboot ? "reboot" : "shutdown")} triggered on {host}.");
                return true;
            }
            catch (Exception ex)
            {
                UIDraw.TextColor = ConsoleColor.Red;
                UIDraw.DrawLine($"Remote operation failed: {ex.Message}");
                return false;
            }
            finally
            {
                UIDraw.TextColor = ConsoleColor.White;
            }
    }
}
