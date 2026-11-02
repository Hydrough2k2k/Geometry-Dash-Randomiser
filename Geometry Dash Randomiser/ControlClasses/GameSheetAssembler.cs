using System.Drawing;
using System.Linq;

namespace Geometry_Dash_Randomiser {

      public static class GameSheetAssembler {

            public static Bitmap Assemble(GameSheet gameSheet) {
                  return BitmapExtensions.Assemble(
                        gameSheet.Sprites.Select(s => s.Texture).ToArray(),
                        gameSheet.Sprites.Select(s => s.TextureRect).ToArray(),
                        gameSheet.Size
                  );
            }

            public static Bitmap Assemble(Font font) {
                  return BitmapExtensions.Assemble(
                        font.Chars.Select(c => c.Texture).ToArray(),
                        font.Chars.Select(c => c.Rectangle).ToArray(),
                        new Size(font.Scale_W, font.Scale_H)
                  );
            }
      }
}
