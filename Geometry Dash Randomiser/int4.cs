using System;
using System.Text.RegularExpressions;

namespace Geometry_Dash_Randomiser {

      internal struct int4 {

            public int4(int x, int y, int z, int w) {
                  this.x = x;
                  this.y = y;
                  this.z = z;
                  this.w = w;
            }

            int x { get; set; }
            int y { get; set; }
            int z { get; set; }
            int w { get; set; }

            public int4(string data) {
                  data = Regex.Replace(data, "[^0-9-,]+", "", RegexOptions.Compiled);
                  string[] vals = data.Split(',');
                  Array.Resize(ref vals, 4);

                  for (int i = 0; i < vals.Length; i++) {
                        if (string.IsNullOrEmpty(vals[i]))
                              vals[i] = "0";
                  }

                  this.x = Int32.Parse(vals[0]);
                  this.y = Int32.Parse(vals[1]);
                  this.z = Int32.Parse(vals[2]);
                  this.w = Int32.Parse(vals[3]);
            }
      }
}
