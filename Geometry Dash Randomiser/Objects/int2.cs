using System;
using System.Text.RegularExpressions;

namespace Geometry_Dash_Randomiser {

      public struct Int2 {

            public Int2(int x, int y) {
                  this.x = x;
                  this.y = y;
            }

            public int x { get; set; }
            public int y { get; set; }

            public Int2(string data) {
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

            public string ToString(FormatMode format) {
                  switch (format) {
                        case FormatMode.Default:
                              return base.ToString();
                        case FormatMode.Plist:
                              return x + "," + y;
                        case FormatMode.Json:
                              return Json.Serialise(this);
                        default:
                              Log.Write(Log.Mode.Error, $"Format Mode \"{format}\" does not exist for object type \"{this.GetType()}\"");
                              return string.Empty;
                  }
            }
      }
}
