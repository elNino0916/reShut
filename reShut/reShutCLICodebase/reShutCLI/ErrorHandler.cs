using System;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Runtime.Versioning;

namespace reShutCLI
{
    internal class ErrorHandler
    {
        [SupportedOSPlatform("windows")]
        public static void SetConsoleSize(int width, int height)
        {
            if (Console.LargestWindowWidth >= width && Console.LargestWindowHeight >= height)
            {
                Console.SetWindowSize(width, height);
            }
            else
            {
                Console.Clear();
                UIDraw.TextColor = ConsoleColor.Red;
                UIDraw.DrawCenteredLine("Error: Cannot resize the console window. It's too small for the required dimensions.");
                Console.ResetColor();
            }
        }
        public static void ShowError(string error, bool needsRestart)
        {
            CultureInfo culture = new CultureInfo(Variables.lang);
            ResourceManager rm = new ResourceManager("reShutCLI.Resources.Strings", typeof(Program).Assembly);
            Console.Title = "reShut CLI";
            Console.OutputEncoding = Encoding.UTF8;

            // Prepare all lines of the message
            var lines = new[]
            {
                rm.GetString("ErrorOccurred", culture),
                error,
                needsRestart ? rm.GetString("NeedRestart", culture) : "",
                needsRestart ? rm.GetString("ErrorHandler_PressKeyRestart",culture) : rm.GetString("ErrorHandler_PressKeyBack",culture)
            };

            // Determine the width of the box based on the longest line
            int boxWidth = lines.Max(line => line.Length) + 2; // Adding padding

            // Create the top, middle, and bottom borders of the box
            string topBorder = "╭" + new string('─', boxWidth) + "╮";
            string middleBorder = "├" + new string('─', boxWidth) + "┤";
            string bottomBorder = "╰" + new string('─', boxWidth) + "╯";

            // Print the error box
            Console.Clear();
            UIDraw.TextColor = Variables.LogoColor;
            UIDraw.DrawBoxedMessage(rm.GetString("ErrorHandler_BeingImproved", culture));
            UIDraw.DrawLine(" ");
            UIDraw.TextColor = Variables.SecondaryColor;
            UIDraw.DrawCenteredLine(topBorder);
            UIDraw.DrawCenteredLine("│ " + lines[0].PadRight(boxWidth - 2) + " │");
            UIDraw.DrawCenteredLine(middleBorder);
            UIDraw.TextColor = Variables.SecondaryColor;
            UIDraw.DrawCenteredLine("│ " + lines[1].PadRight(boxWidth - 2) + " │");
            UIDraw.DrawCenteredLine(middleBorder);
            UIDraw.DrawCenteredLine("│ " + lines[2].PadRight(boxWidth - 2) + " │");
            if (needsRestart)
            {
                UIDraw.TextColor = Variables.SecondaryColor;
                UIDraw.DrawCenteredLine("│ " + lines[3].PadRight(boxWidth - 2) + " │");
                UIDraw.DrawCenteredLine(bottomBorder);
                Console.ReadKey();
                AutoRestart.Init();
            }
            else
            {
                UIDraw.TextColor = Variables.SecondaryColor;
                UIDraw.DrawCenteredLine("│ " + lines[3].PadRight(boxWidth - 2) + " │");
                UIDraw.DrawCenteredLine(bottomBorder);
                Console.ReadKey();
                Program.Main(Array.Empty<string>());
            }
        }
    }
}
