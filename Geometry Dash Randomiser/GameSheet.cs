using RectpackSharp;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Geometry_Dash_Randomiser {

      internal static class GameSheet {

            public static Bitmap Assemble(Sprite[] sprites, PackingRectangle[] rects, PackingRectangle bounds) {

                  Bitmap gamesheet = new Bitmap((int)bounds.Width, (int)bounds.Height);

                  for (int i = 0; i < sprites.Length; i++) {
                        // Add 1 pixels to both X and Y axes to account for the 1 pixel added around every sprite to avoid images flowing into each other
                        gamesheet.CopyTo(sprites[i].texture, (int)rects[i].X + 1, (int)rects[i].Y + 1);
                  }

                  return gamesheet;
            }
      }
}
