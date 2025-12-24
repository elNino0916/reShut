using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Principal;

namespace reShut.Services;

/// <summary>
/// Provides methods to perform system-level power operations such as shutdown, reboot, and logoff.
/// </summary>
[SupportedOSPlatform("windows")]
public static class PowerService
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool OpenProcessToken(IntPtr processHandle, uint desiredAccess, out IntPtr tokenHandle);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool LookupPrivilegeValue(string? lpSystemName, string lpName, out LUID luid);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool AdjustTokenPrivileges(IntPtr tokenHandle, bool disableAllPrivileges,
        ref TOKEN_PRIVILEGES newState, uint bufferLength, IntPtr previousState, IntPtr returnLength);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    private const uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
    private const uint TOKEN_QUERY = 0x0008;
    private const uint SE_PRIVILEGE_ENABLED = 0x00000002;
    private const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";

    private const uint EWX_LOGOFF = 0x00000000;
    private const uint EWX_SHUTDOWN = 0x00000001;
    private const uint EWX_REBOOT = 0x00000002;
    private const uint EWX_FORCE = 0x00000004;

    [StructLayout(LayoutKind.Sequential)]
    private struct LUID
    {
        public uint LowPart;
        public int HighPart;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct TOKEN_PRIVILEGES
    {
        public uint PrivilegeCount;
        public LUID Luid;
        public uint Attributes;
    }

    public static bool IsAdmin()
    {
        return new WindowsPrincipal(WindowsIdentity.GetCurrent())
            .IsInRole(WindowsBuiltInRole.Administrator);
    }

    private static bool EnableShutdownPrivilege()
    {
        if (!OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out IntPtr tokenHandle))
            return false;

        if (!LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, out LUID luid))
            return false;

        TOKEN_PRIVILEGES tp = new()
        {
            PrivilegeCount = 1,
            Luid = luid,
            Attributes = SE_PRIVILEGE_ENABLED
        };

        bool result = AdjustTokenPrivileges(tokenHandle, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
        CloseHandle(tokenHandle);
        return result;
    }

    private static (bool Success, string? ErrorMessage) TryExitWindows(uint flags)
    {
        if (!IsAdmin())
        {
            return (false, "Administrator privileges required. Please run as administrator.");
        }

        if (!EnableShutdownPrivilege())
        {
            return (false, "Failed to enable shutdown privilege. Please run as administrator.");
        }

        if (!ExitWindowsEx(flags, 0))
        {
            int err = Marshal.GetLastWin32Error();
            return (false, $"Operation failed with error {err}: {new Win32Exception(err).Message}");
        }

        return (true, null);
    }

    public static (bool Success, string? ErrorMessage) Shutdown(bool force = true)
    {
        uint flags = EWX_SHUTDOWN;
        if (force) flags |= EWX_FORCE;
        return TryExitWindows(flags);
    }

    public static (bool Success, string? ErrorMessage) Reboot(bool force = true)
    {
        uint flags = EWX_REBOOT;
        if (force) flags |= EWX_FORCE;
        return TryExitWindows(flags);
    }

    public static (bool Success, string? ErrorMessage) Logoff()
    {
        if (!ExitWindowsEx(EWX_LOGOFF, 0))
        {
            int err = Marshal.GetLastWin32Error();
            return (false, $"Logoff failed with error {err}: {new Win32Exception(err).Message}");
        }
        return (true, null);
    }

    public static (bool Success, string? ErrorMessage) ScheduleShutdown(int delayInSeconds, bool reboot = false)
    {
        try
        {
            string action = reboot ? "/r" : "/s";
            var psi = new ProcessStartInfo
            {
                FileName = "shutdown.exe",
                Arguments = $"{action} /t {delayInSeconds}",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process.Start(psi);
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public static (bool Success, string? ErrorMessage) CancelScheduledShutdown()
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "shutdown.exe",
                Arguments = "/a",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process.Start(psi);
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}
