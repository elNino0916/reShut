using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace reShutCLI
{
    internal class AutoRestart
    {
        public static void Init()
        {
            string exePath = Path.Combine(AppContext.BaseDirectory, AppDomain.CurrentDomain.FriendlyName);
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                UseShellExecute = false,
            };
            Process.Start(startInfo);
            Thread.Sleep(1000);
            Environment.Exit(0);
        }
    }
}
