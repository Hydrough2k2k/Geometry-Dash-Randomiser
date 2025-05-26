using System;
using System.Drawing;

namespace Geometry_Dash_Randomiser {

      internal static class ColorExtension {

            public static Color Interpolate(Color previous, Color next, float value) {
                  int deltaA = (previous.A - next.A) * -1;
                  int deltaR = (previous.R - next.R) * -1;
                  int deltaG = (previous.G - next.G) * -1;
                  int deltaB = (previous.B - next.B) * -1;

                  return Color.FromArgb(
                        previous.A + (int)Math.Ceiling(deltaA * value),
                        previous.R + (int)Math.Ceiling(deltaR * value),
                        previous.G + (int)Math.Ceiling(deltaG * value),
                        previous.B + (int)Math.Ceiling(deltaB * value));
            }
      }
}
