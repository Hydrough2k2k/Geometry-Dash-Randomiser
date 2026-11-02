using System.Drawing;
using System.Linq;

namespace Geometry_Dash_Randomiser {

      public static class GameSheetAssembler {

            public static Bitmap Assemble(GameSheet gameSheet) {
                  ResourceSprite[] sprites = gameSheet.Sprites.ToArray();
                  Rectangle[] rects = sprites.Select(s => s.DrawRect).ToArray();
                  Size size = gameSheet.Size;

                  Bitmap img = new Bitmap(size.Width, size.Height);

                  for (int i = 0; i < sprites.Length; i++) {
                        img.PasteImage(sprites[i].Texture, rects[i].X, rects[i].Y);
                  }

                  return img;

                  //return BitmapExtensions.Assemble(
                  //      gameSheet.Sprites.Select(s => s.Texture).ToArray(),
                  //      gameSheet.Sprites.Select(s => s.DrawRect).ToArray(),
                  //      gameSheet.Size
                  //);
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
