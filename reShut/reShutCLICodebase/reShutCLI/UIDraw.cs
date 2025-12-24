using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A fancy class that simplifies drawing messages and just a fancy wrapper around Console methods xD
/// </summary>
namespace reShutCLI
{
    internal class UIDraw
    {
        public static void DrawBoxedMessage(string message)
        {
            DrawBoxedMessages(new List<string> { message });
        }
        private static CliColor _textColor;
        public static CliColor TextColor
        {
            get => _textColor;
            set
            {
                _textColor = value;
                _textColor.ApplyForeground();
            }
        }
        private static CliColor _backgroundColor;
        public static CliColor BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                _backgroundColor.ApplyBackground();
            }
        }
        public static void DrawLine(string Text)
        {
            Console.WriteLine(Text);
        }
        public static void Draw(string Text)
        {
            Console.Write(Text);
        }
        public static void DrawBoxedMessages(IEnumerable<string> messages)
        {
            var lines = messages.ToList();
            if (!lines.Any()) return;

            int maxContentWidth = lines.Max(line => line.Length);
            int totalWidth = maxContentWidth + 4;

            string topBorder = "╭" + new string('─', totalWidth - 2) + "╮";
            string bottomBorder = "╰" + new string('─', totalWidth - 2) + "╯";

            DrawCenteredLine(topBorder);

            foreach (string line in lines)
            {
                string paddedLine = $"│ {line.PadRight(maxContentWidth)} │";
                DrawCenteredLine(paddedLine);
            }

            DrawCenteredLine(bottomBorder);
        }
        public static void DrawCenteredLine(string message)
        {
            var consoleWidth = Console.WindowWidth;
            var padding = (consoleWidth - message.Length) / 2;

            UIDraw.DrawLine(new string(' ', padding) + message);
        }
        public static void DrawCentered(string message)
        {
            var consoleWidth = Console.WindowWidth;
            var padding = (consoleWidth - message.Length) / 2;

            UIDraw.Draw(new string(' ', padding) + message);
        }
    }
}
