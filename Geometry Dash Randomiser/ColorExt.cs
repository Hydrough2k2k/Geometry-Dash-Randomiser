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
                  if (hex == null) throw new ArgumentNullException();
                  if (hex.Length < 8) {
                        hex = "FF" + hex + new string('0', 6 - hex.Length);
                  }
                  uint value = (uint)int.Parse(hex.Substring(0, 8), System.Globalization.NumberStyles.HexNumber);

                  int a = (int)(value / 16_777_216);
                  int r = (int)(value / 65_536 % 256);
                  int g = (int)(value / 256 % 256);
                  int b = (int)(value % 256);

                  return Color.FromArgb(a, r, g, b);
            }
      }
}
