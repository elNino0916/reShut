using System;
using System.Linq;

namespace reShutCLI
{
    internal class Theme
    {
        public static string RandomColor()
        {
            var consoleColors = ((ConsoleColor[])Enum.GetValues(typeof(ConsoleColor)))
                .Where(color => color != ConsoleColor.Black) // Exclude black color
                .ToArray();

            var random = new Random();
            var randomIndex = random.Next(consoleColors.Length);
            return consoleColors[randomIndex].ToString();
        }
        public static ConsoleColor Parsed
        {
            get
            {
                var colorName = RandomColor();
                if (Enum.TryParse<ConsoleColor>(colorName, out var color))
                {
                    return color;
                }
                return ConsoleColor.White;
            }
        }

    }
}