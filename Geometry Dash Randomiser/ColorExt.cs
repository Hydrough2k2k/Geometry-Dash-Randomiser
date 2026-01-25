using System;
using System.Drawing;

namespace Geometry_Dash_Randomiser {

      public static class ColorExt {

            public static Color Interpolate(Color previous, Color next, float value) {
                  if (value >= 1) return next;

                  int deltaA = (previous.A - next.A) * -1;
                  int deltaR = (previous.R - next.R) * -1;
                  int deltaG = (previous.G - next.G) * -1;
                  int deltaB = (previous.B - next.B) * -1;

                  return Color.FromArgb(
                        previous.A + (int)Math.Ceiling(deltaA * value),
                        previous.R + (int)Math.Ceiling(deltaR * value),
                        previous.G + (int)Math.Ceiling(deltaG * value),
                        previous.B + (int)Math.Ceiling(deltaB * value)
                  );
            }

            public static Color FromHex(string hex) {
                  if (hex == null)
                        throw new ArgumentNullException("Input string was null");

                  if (hex.Length < 8) {
                        hex = "FF" + hex + new string('0', 6 - hex.Length);
                  }

                  uint value = (uint)int.Parse(hex, System.Globalization.NumberStyles.HexNumber);

                  int a = (int)(value / 16_777_216);
                  int r = (int)(value / 65_536 % 256);
                  int g = (int)(value / 256 % 256);
                  int b = (int)(value % 256);

                  return Color.FromArgb(a, r, g, b);
            }

            public static string ToHex(this Color c) {
                  return $"{c.R:X2}{c.G:X2}{c.B:X2}";
            }

            public static string Serialize(this Color c) {
                  return $"[\n\tR: {c.R}\n\tG: {c.G}\n\tB: {c.B}\n\tHex: {c.ToHex()}\n]";
            }

            public static Color Deserialize(string[] text) {
                  int r = 0;
                  int g = 0;
                  int b = 0;
                  for (int i = 0; i < text.Length; i++) {
                        string line = text[i];
                        
                        string data = line.Trim();
                        if (line.Contains(":")) {
                              data = line.Substring(line.IndexOf(':') + 1).Trim();
                        }

                        if (line.StartsWith("\tR")) {
                              int.TryParse(data, out r);

                        } else if (line.StartsWith("\tG")) {
                              int.TryParse(data, out g);

                        } else if (line.StartsWith("\tB")) {
                              int.TryParse(data, out b);

                        } else if (line.StartsWith("\tHex")) {
                              Color c = FromHex(data);

                              // Override RGB values if hex is present
                              return FromHex(data);

                        } else if (line.StartsWith("]")) {
                              break;
                        }
                  }
                  return Color.FromArgb(255, r, g, b);
            }

            public static Color AdjustBrightness(this Color colour, float multiplier) {
                  return Color.FromArgb(
                        colour.A,
                        Math.Min((int)(colour.R * multiplier), 255),
                        Math.Min((int)(colour.G * multiplier), 255),
                        Math.Min((int)(colour.B * multiplier), 255)
                  );
            }
      }
}
