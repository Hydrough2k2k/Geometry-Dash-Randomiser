using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_Randomiser {

      internal class Font {

            internal struct PropertyPair {
                  public string name;
                  public string data;

                  public PropertyPair(string name, string data) {
                        this.name = name;
                        this.data = data;
                  }
            }

            public string infoFace { get; set; } = string.Empty;
            public int size { get; set; } = 0;
            public int bold { get; set; } = 0;
            public int italic { get; set; } = 0;
            public string charSet { get; set; } = string.Empty;
            public int unicode { get; set; } = 0;
            public int stretchH { get; set; } = 0;
            public int smooth { get; set; } = 0;
            public int aa { get; set; } = 0;
            public int4 padding { get; set; }
            public int2 spacing { get; set; }
            public int lineHeight { get; set; }
            public int baseVal { get; set; }
            public int scaleW { get; set; }
            public int scaleH { get; set; }
            public int pages { get; set; }
            public int packed { get; set; }
            public int pageID { get; set; }
            public string file { get; set; } = string.Empty;
            public int charsCount { get; set; }
            public FontChar[] chars { get; set; }
            public int kerningsCount { get; set; }
            public FontKerning[] kernings { get; set; }

            public void ApplyInfoData(PropertyPair[] pairs) {

                  for (int i = 0; i < pairs.Length; i++) {
                        string filtered = pairs[i].data.FilterDigits();
                        int parsed = 0;
                        if (filtered.Length != 0) {
                              parsed = Int32.Parse(filtered);
                        }

                        if (pairs[i].name == "face") {
                              this.infoFace = pairs[i].data;
                        } else if (pairs[i].name == "size") {
                              this.size = parsed;
                        } else if (pairs[i].name == "bold") {
                              this.bold = parsed;
                        } else if (pairs[i].name == "italic") {
                              this.italic = parsed;
                        } else if (pairs[i].name == "charset") {
                              this.charSet = pairs[i].data;
                        } else if (pairs[i].name == "unicode") {
                              this.unicode = parsed;
                        } else if (pairs[i].name == "stretchH") {
                              this.stretchH = parsed;
                        } else if (pairs[i].name == "smooth") {
                              this.smooth = parsed;
                        } else if (pairs[i].name == "aa") {
                              this.aa = parsed;
                        } else if (pairs[i].name == "padding") {
                              this.padding = new int4(pairs[i].data);
                        } else if (pairs[i].name == "spacing") {
                              this.spacing = new int2(pairs[i].data);
                        }
                  }
            }

            public void ApplyCommonData(PropertyPair[] pairs) {

                  for (int i = 0; i < pairs.Length; i++) {
                        string filtered = pairs[i].data.FilterDigits();
                        int parsed = 0;
                        if (filtered.Length != 0) {
                              parsed = Int32.Parse(filtered);
                        }

                        if (pairs[i].name == "lineHeight") {
                              this.lineHeight = parsed;
                        } else if (pairs[i].name == "base") {
                              this.baseVal = parsed;
                        } else if (pairs[i].name == "scaleW") {
                              this.scaleW = parsed;
                        } else if (pairs[i].name == "scaleH") {
                              this.scaleH = parsed;
                        } else if (pairs[i].name == "pages") {
                              this.pages = parsed;
                        } else if (pairs[i].name == "packed") {
                              this.packed = parsed;
                        }
                  }
            }

            public void ApplyPageData(PropertyPair[] pairs) {

                  for (int i = 0; i < pairs.Length; i++) {
                        string filtered = pairs[i].data.FilterDigits();
                        int parsed = 0;
                        if (filtered.Length != 0) {
                              parsed = Int32.Parse(filtered);
                        }

                        if (pairs[i].name == "id") {
                              this.pageID = parsed;
                        } else if (pairs[i].name == "file") {
                              this.file = pairs[i].data;
                        }
                  }
            }
      }
}
