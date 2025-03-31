using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_Randomiser {


      internal class FontChar {

            public int charID { get; set; } = 0;
            public int x { get; set; } = 0;
            public int y { get; set; } = 0;
            public int width { get; set; } = 0;
            public int height { get; set; } = 0;
            public int xOffset { get; set; } = 0;
            public int yOffset { get; set; } = 0;
            public int xAdvance { get; set; } = 0;
            public int page { get; set; } = 0;
            public int channel { get; set; } = 0;
            public int letter { get; set; } = 0;

            public FontChar (Font.PropertyPair[] pairs) {

                  for (int i = 0; i < pairs.Length; i++) {
                        string filtered = pairs[i].data.FilterDigits();
                        int parsed = 0;
                        if (filtered.Length != 0) {
                              parsed = Int32.Parse(filtered);
                        }

                        if (pairs[i].name == "id") {
                              this.charID = parsed;
                        } else if (pairs[i].name == "x") {
                              this.x = parsed;
                        } else if (pairs[i].name == "y") {
                              this.y = parsed;
                        } else if (pairs[i].name == "width") {
                              this.width = parsed;
                        } else if (pairs[i].name == "height") {
                              this.height = parsed;
                        } else if (pairs[i].name == "xoffset") {
                              this.xOffset = parsed;
                        } else if (pairs[i].name == "yoffset") {
                              this.yOffset = parsed;
                        } else if (pairs[i].name == "xadvance") {
                              this.xAdvance = parsed;
                        } else if (pairs[i].name == "page") {
                              this.page = parsed;
                        } else if (pairs[i].name == "chnl") {
                              this.channel = parsed;
                        } else if (pairs[i].name == "letter") {
                              if (pairs[i].data == "space") {
                                    this.letter = (int)' ';
                              } else {
                                    this.letter = (int)pairs[i].data[0];
                              }
                        }
                  }
            }
      }
}
