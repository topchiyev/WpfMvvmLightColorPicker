using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace Gymdata.Accreditation.Util
{
    public static class ColorUtils
    {
        public static Color? HexStringToNullableColor(string hex)
        {
            if (string.IsNullOrEmpty(hex)) return null;

            hex = hex.Replace("#", "").ToLowerInvariant();

            Regex regex = new Regex(@"^[abcdef\d]{6}$");
            if (!regex.IsMatch(hex)) return null;

            var r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

            return Color.FromRgb(r, g, b);
        }

        public static Color HexStringToColor(string hex)
        {
            if (string.IsNullOrEmpty(hex)) throw new ArgumentNullException();

            hex = hex.Replace("#", "").ToLowerInvariant();

            Regex regex = new Regex(@"^[abcdef\d]{6}$");
            if (!regex.IsMatch(hex)) throw new ArgumentOutOfRangeException();

            var r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

            return Color.FromRgb(r, g, b);
        }

        public static Color HexRgbaStringToColor(string hex)
        {
            if (string.IsNullOrEmpty(hex)) throw new ArgumentNullException();

            hex = hex.Replace("#", "").ToLowerInvariant();

            Regex regex = new Regex(@"^[abcdef\d]{8}$");
            if (!regex.IsMatch(hex)) throw new ArgumentOutOfRangeException();

            var r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            var a = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);

            return Color.FromArgb(a, r, g, b);
        }

        public static string ColorToHexString(Color? color)
        {
            if (color == null) return null;

            var hex = $"#{color.Value.R:X2}{color.Value.G:X2}{color.Value.B:X2}";
            return hex;
        }

        public static (double h, double s, double v) RgbToHsv(byte r, byte g, byte b)
        {

            double max = Math.Max(Math.Max(r, g), b);
            double min = Math.Min(Math.Min(r, g), b);

            double delta = max - min;

            double h = 0, s = 0, v = max;

            if (Math.Abs(v) < 0.00001)
            {
                s = 0;
            }
            else
            {
                s = delta / v;
            }

            if (Math.Abs(s) < 0.00001)
            {
                h = 0;
            }

            else
            {
                if (Math.Abs(r - v) < 0.00001)
                {
                    h = (g - b) / delta;
                }
                else if (Math.Abs(g - v) < 0.00001)
                {
                    h = 2 + (b - r) / delta;
                }
                else if (Math.Abs(b - v) < 0.00001)
                {
                    h = 4 + (r - g) / delta;
                }

                h *= 60;

                if (h < 0)
                {
                    h = h + 360;
                }
            }

            v = v / 255;

            return (h: h, s: s, v: v);
        }
    }

}
