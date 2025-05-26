using System;
using System.Text.RegularExpressions;

namespace Geometry_Dash_Randomiser {

      internal struct int2 {

            public int2(int x, int y) {
                  this.x = x;
                  this.y = y;
            }

            int x { get; set; }
            int y { get; set; }

            public static int2 Parse(string data) {
                  data = Regex.Replace(data, "[^0-9-,]+", "", RegexOptions.Compiled);
                  string[] vals = data.Split(',');
                  Array.Resize(ref vals, 2);

                  for (int i = 0; i < vals.Length; i++) {
                        if (string.IsNullOrEmpty(vals[i]))
                              vals[i] = "0";
                  }

                  return new int2(Int32.Parse(vals[0]), Int32.Parse(vals[1]));
            }
      }
}
