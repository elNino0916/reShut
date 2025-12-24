using System;
using System.Management;
using System.Runtime.Versioning;

namespace reShut.Services;

/// <summary>
/// Provides methods to perform remote power operations on Windows computers in the network.
/// </summary>
[SupportedOSPlatform("windows")]
public static class RemoteService
{
    public static (bool Success, string? ErrorMessage) TriggerRemoteOperation(
        string host, 
        string? username, 
        string? password, 
        bool reboot)
    {
        try
        {
            ConnectionOptions options = new();
            if (!string.IsNullOrWhiteSpace(username))
            {
                options.Username = username;
                options.Password = password ?? string.Empty;
            }

            ManagementScope scope = new($"\\\\{host}\\root\\cimv2", options);
            scope.Connect();

            ObjectQuery query = new("SELECT * FROM Win32_OperatingSystem");
            using ManagementObjectSearcher searcher = new(scope, query);
            foreach (ManagementObject os in searcher.Get())
            {
                // 6 = Forced Reboot, 5 = Forced Shutdown
                os.InvokeMethod("Win32Shutdown", [reboot ? 6 : 5, 0]);
            }

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Remote operation failed: {ex.Message}");
        }
    }

    public static (bool Success, string? ErrorMessage) RemoteShutdown(string host, string? username = null, string? password = null)
    {
        return TriggerRemoteOperation(host, username, password, reboot: false);
    }

    public static (bool Success, string? ErrorMessage) RemoteReboot(string host, string? username = null, string? password = null)
    {
        return TriggerRemoteOperation(host, username, password, reboot: true);
    }
}
