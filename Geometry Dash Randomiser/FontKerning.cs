using System;
using System.Collections.Generic;
using System.IO;

namespace Geometry_Dash_Randomiser {

      internal class FontKerning {

            public FontKerning(int first, int second, int amount) {
                  this.first = first;
                  this.second = second;
                  this.amount = amount;
            }

            public int first { get; set; }
            public int second { get; set; }
            public int amount { get; set; }

            public FontKerning(Font.PropertyPair[] pairs) {

                  for (int i = 0; i < pairs.Length; i++) {
                        string filtered = pairs[i].data.FilterDigits();
                        int parsed = 0;
                        if (filtered.Length != 0) {
                              parsed = Int32.Parse(filtered);
                        }

                        if (pairs[i].name == "first") {
                              this.first = parsed;
                        } else if (pairs[i].name == "second") {
                              this.second = parsed;
                        } else if (pairs[i].name == "amount") {
                              this.amount = parsed;
                        }
                  }
            }
      }
}
