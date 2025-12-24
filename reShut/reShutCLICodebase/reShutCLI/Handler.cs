using reShutCLI;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Runtime.Versioning;
/// <summary>
/// Provides methods to perform system-level operations such as rebooting, shutting down, or logging off the current
/// user.
/// </summary>
/// <remarks>This class includes functionality to invoke Windows API calls for system operations.  The <see
/// cref="Reboot"/> and <see cref="Shutdown"/> methods require administrative privileges  and attempt to enable the
/// necessary shutdown privilege before executing the operation. The <see cref="Logoff"/> method does not require
/// administrative privileges.</remarks>
[SupportedOSPlatform("windows")]
internal class Handler
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool OpenProcessToken(IntPtr processHandle, uint desiredAccess, out IntPtr tokenHandle);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out LUID luid);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool AdjustTokenPrivileges(IntPtr tokenHandle, bool disableAllPrivileges,
        ref TOKEN_PRIVILEGES newState, uint bufferLength, IntPtr previousState, IntPtr returnLength);

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

    private static bool IsAdmin()
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

        TOKEN_PRIVILEGES tp = new TOKEN_PRIVILEGES
        {
            PrivilegeCount = 1,
            Luid = luid,
            Attributes = SE_PRIVILEGE_ENABLED
        };

        bool result = AdjustTokenPrivileges(tokenHandle, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
        CloseHandle(tokenHandle);
        return result;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    private static bool TryExitWindows(uint flags)
    {
        if (!IsAdmin())
        {
            ErrorHandler.ShowError("Error: You must run this application as an administrator.", true);
            return false;
        }

        if (!EnableShutdownPrivilege())
        {
            ErrorHandler.ShowError("Failed to enable shutdown privilege. Please run as administrator.", true);
            return false;
        }

        if (!ExitWindowsEx(flags, 0))
        {
            int err = Marshal.GetLastWin32Error();
            ErrorHandler.ShowError($"ExitWindowsEx failed with error {err}: {new System.ComponentModel.Win32Exception(err).Message}", true);
            return false;
        }

        return true;
    }

    public static void Reboot()
    {
        TryExitWindows(EWX_REBOOT | EWX_FORCE);
    }

    public static void Shutdown()
    {
        TryExitWindows(EWX_SHUTDOWN | EWX_FORCE);
    }

    public static void Logoff()
    {
        // Logoff does NOT require admin
        if (!ExitWindowsEx(EWX_LOGOFF, 0))
        {
            int err = Marshal.GetLastWin32Error();
            UIDraw.DrawLine($"Logoff failed with error {err}: {new System.ComponentModel.Win32Exception(err).Message}");
        }
    }
}
