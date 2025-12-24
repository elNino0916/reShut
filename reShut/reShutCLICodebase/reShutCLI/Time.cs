using System;
using System.Linq;
using System.Management;

namespace reShutCLI
{
    internal class Time
    {
        public static string GetTime(bool use24HoursFormat)
        {
            return DateTime.Now.ToString(!use24HoursFormat ? "hh:mm:ss tt" : "HH:mm:ss");
        }
    }
}
