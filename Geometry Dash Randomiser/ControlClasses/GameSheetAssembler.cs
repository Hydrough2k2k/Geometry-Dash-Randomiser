using RectpackSharp;
using System.Drawing;
using System.Linq;

namespace Geometry_Dash_Randomiser {

      public static class GameSheetAssembler {

            // Idea:
            // This will be the class to contain a set of Sprites. After extracting them, they will be added to a List<Sprite>.
            // The sprites get swapped between GameSheets, then whn calling Assemble, the sprites will be packed into a single Bitmap and returned.
            // The Packing rects will be calculated when the gamesheet will be assembled to make working with this less of a nightmare
            // Yeah, I should probably do this

            public static Bitmap Assemble(Sprite[] sprites, PackingRectangle[] rects, PackingRectangle bounds) {

                  Bitmap gamesheet = new Bitmap((int)bounds.Width, (int)bounds.Height);

                  for (int i = 0; i < sprites.Length; i++) {
                        // Add 1 pixels to both X and Y axes to account for the 1 pixel added around every sprite to avoid images flowing into each other
                        gamesheet.CopyTo(sprites[i].texture, (int)rects[i].X + 1, (int)rects[i].Y + 1);
                  }

                  return gamesheet;
            }

            public static Bitmap Assemble(Font font) {

                  return BitmapExtensions.Assemble(
                        font.chars.Select(c => c.texture).ToArray(),
                        font.chars.Select(c => c.rectangle).ToArray(),
                        new Size(font.scaleW, font.scaleH)
                  );
            }
      }
}
