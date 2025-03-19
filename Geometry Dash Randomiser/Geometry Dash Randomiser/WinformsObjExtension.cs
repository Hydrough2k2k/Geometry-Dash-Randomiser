using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      internal static class WinformsObjExtension {

            public static void ClampValue(this NumericUpDown n, int min, int max) {
                  if (n.Value > max) n.Value = max;
                  else if (n.Value < min) n.Value = min;
            }
      }
}
