using System;
using System.Globalization;

namespace reShutCLI
{
    internal struct CliColor
    {
        private readonly string _hex;
        private readonly ConsoleColor? _consoleColor;

        public CliColor(ConsoleColor consoleColor)
        {
            _consoleColor = consoleColor;
            _hex = null;
        }

        public CliColor(string color)
        {
            if (!string.IsNullOrEmpty(color) && color.StartsWith("#"))
            {
                _hex = color;
                _consoleColor = null;
            }
            else if (Enum.TryParse<ConsoleColor>(color, true, out var cc))
            {
                _consoleColor = cc;
                _hex = null;
            }
            else
            {
                _consoleColor = ConsoleColor.White;
                _hex = null;
            }
        }

        public void ApplyForeground()
        {
            if (_consoleColor.HasValue)
            {
                Console.ForegroundColor = _consoleColor.Value;
            }
            else if (!string.IsNullOrEmpty(_hex))
            {
                var (r, g, b) = ParseHex(_hex);
                Console.Write($"\u001b[38;2;{r};{g};{b}m");
            }
        }

        public void ApplyBackground()
        {
            if (_consoleColor.HasValue)
            {
                Console.BackgroundColor = _consoleColor.Value;
            }
            else if (!string.IsNullOrEmpty(_hex))
            {
                var (r, g, b) = ParseHex(_hex);
                Console.Write($"\u001b[48;2;{r};{g};{b}m");
            }
        }

        private static (int r, int g, int b) ParseHex(string hex)
        {
            hex = hex.TrimStart('#');
            if (hex.Length != 6)
                throw new ArgumentException("Hex color string must be exactly 6 characters long after removing '#'.", nameof(hex));
            int r = int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            int g = int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            int b = int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            return (r, g, b);
        }

        public override string ToString()
        {
            return _consoleColor?.ToString() ?? _hex;
        }

        public static implicit operator CliColor(ConsoleColor color) => new CliColor(color);
        public static implicit operator CliColor(string color) => new CliColor(color);
    }
}
