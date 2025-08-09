using RectpackSharp;
using System.Drawing;
using System.Linq;

namespace GDR_Code_Tests {

      public static class GameSheet {

            public static Bitmap Assemble(Font font) {

                  return BitmapExtensions.Assemble(
                        font.chars.Select(c => c.texture).ToArray(),
                        font.chars.Select(c => c.rectangle).ToArray(),
                        new Size(font.scaleW, font.scaleH));
            }
      }
}
