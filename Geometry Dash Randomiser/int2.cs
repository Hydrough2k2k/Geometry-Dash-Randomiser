using System;
using System.Text.RegularExpressions;

namespace Geometry_Dash_Randomiser {

      public struct int2 {

            public int2(int x, int y) {
                  this.x = x;
                  this.y = y;
            }

            public int x { get; set; }
            public int y { get; set; }

            public int2(string data) {
                  data = Regex.Replace(data, "[^0-9-,]+", "", RegexOptions.Compiled);
                  string[] vals = data.Split(',');
                  Array.Resize(ref vals, 2);

                  for (int i = 0; i < vals.Length; i++) {
                        if (string.IsNullOrEmpty(vals[i]))
                              vals[i] = "0";
                  }

                  this.x = Int32.Parse(vals[0]);
                  this.y = Int32.Parse(vals[1]);
            }
      }
}
