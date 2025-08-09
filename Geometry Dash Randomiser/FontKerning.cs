
namespace Geometry_Dash_Randomiser {

      public class FontKerning {

            public FontKerning(int first, int second, int amount) {
                  this.first = first;
                  this.second = second;
                  this.amount = amount;
            }

            public FontKerning() { }

            public int first { get; set; }
            public int second { get; set; }
            public int amount { get; set; }

            internal FontKerning(Font.PropertyPair[] pairs) {

                  for (int i = 0; i < pairs.Length; i++) {
                        string filtered = pairs[i].data.FilterDigits();
                        int parsed = 0;
                        if (filtered.Length != 0) {
                              parsed = Parse.Int(filtered);
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

            public string Serialise() {
                  return "kerning first=" + this.first +
                        " second=" + this.second +
                        " amount=" + this.amount;
            }

            public FontKerning DeepCopy() {
                  return new FontKerning(this.first, this.second, this.amount);
            }
      }
}
