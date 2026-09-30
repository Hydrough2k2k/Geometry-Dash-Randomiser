namespace Geometry_Dash_Randomiser {

      public class FontKerning {

            public FontKerning(int first, int second, int amount) {
                  this.First = first;
                  this.Second = second;
                  this.Amount = amount;
            }

            public FontKerning() { }

            public int First { get; set; }
            public int Second { get; set; }
            public int Amount { get; set; }

            public FontKerning(Font.PropertyPair[] pairs) {

                  for (int i = 0; i < pairs.Length; i++) {
                        string filtered = pairs[i].data.FilterDigits();
                        int parsed = 0;
                        if (filtered.Length != 0) {
                              parsed = Parse.Int(filtered);
                        }

                        if (pairs[i].name == "first") {
                              this.First = parsed;
                        } else if (pairs[i].name == "second") {
                              this.Second = parsed;
                        } else if (pairs[i].name == "amount") {
                              this.Amount = parsed;
                        }
                  }
            }

            public string Serialise() {
                  return "kerning first=" + this.First +
                        " second=" + this.Second +
                        " amount=" + this.Amount;
            }

            public FontKerning DeepCopy() {
                  return new FontKerning(this.First, this.Second, this.Amount);
            }
      }
}
