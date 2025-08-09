using System;
using System.Text.RegularExpressions;

namespace GDR_Code_Tests {

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
