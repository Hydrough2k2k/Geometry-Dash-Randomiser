using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Geometry_Dash_Randomiser {

      partial class PointExtension {

            public static Point Parse(string data) {
                  data = Regex.Replace(data, "[^0-9-,]+", "", RegexOptions.Compiled);
                  string[] values = data.Split(',');
                  Array.Resize(ref values, 2);

                  for (int i = 0; i < values.Length; i++) {
                        if (string.IsNullOrEmpty(values[i]))
                              values[i] = "0";
                  }

                  return new Point(Int32.Parse(values[0]), Int32.Parse(values[1]));
            }
      }
}
