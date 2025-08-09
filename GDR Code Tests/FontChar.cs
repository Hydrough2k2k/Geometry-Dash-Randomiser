using RectpackSharp;
using System.Drawing;

namespace GDR_Code_Tests {

      public class FontChar {

            public FontChar() { }

            public int charID { get; set; }
            public int x { get; set; }
            public int y { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public int xOffset { get; set; }
            public int yOffset { get; set; }
            public int xAdvance { get; set; }
            public int page { get; set; }
            public int channel { get; set; }
            public char letter { get; set; }
            public Bitmap texture { get; set; } = null;

            public Rectangle rectangle => new Rectangle(x, y, width, height);

            public string Serialise() {
                  return FontSerialiser.SerialiseFontChar(this);
            }

            public FontChar DeepCopy() {
                  FontChar copy = new FontChar();

                  copy.charID = this.charID;
                  copy.x = this.x;
                  copy.y = this.y;
                  copy.width = this.width;
                  copy.height = this.height;
                  copy.xOffset = this.xOffset;
                  copy.yOffset = this.yOffset;
                  copy.xAdvance = this.xAdvance;
                  copy.page = this.page;
                  copy.channel = this.channel;
                  copy.letter = this.letter;
                  copy.texture = this.texture;

                  return copy;
            }

            public PackingRectangle GetPackingRect(int ID = 0) {
                  return new PackingRectangle((uint)x, (uint)y, (uint)width, (uint)height, ID);
            }

            public void ReplaceTexture(Bitmap newTexture) {
                  texture = (Bitmap)newTexture.Clone();

                  this.width = newTexture.Width;
                  this.height = newTexture.Height;
            }
      }
}
