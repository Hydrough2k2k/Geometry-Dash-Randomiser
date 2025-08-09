
namespace GDR_Code_Tests {

      public class FontKerning {

            public FontKerning() { }

            public FontKerning(int first, int second, int amount) {
                  this.first = first;
                  this.second = second;
                  this.amount = amount;
            }

            public FontKerning(FontKerning kerning) {
                  this.first = kerning.first;
                  this.second = kerning.second;
                  this.amount = kerning.amount;
            }

            public int first { get; set; }
            public int second { get; set; }
            public int amount { get; set; }

            public string Serialise() {
                  return FontSerialiser.SerialiseFontKerning(this);
            }

            public FontKerning DeepCopy() {
                  return new FontKerning(this.first, this.second, this.amount);
            }
      }
}
