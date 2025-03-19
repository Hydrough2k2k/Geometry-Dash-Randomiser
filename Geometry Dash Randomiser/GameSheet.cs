using RectpackSharp;
using System;
using System.Drawing;

namespace Geometry_Dash_Randomiser {

      internal static class GameSheet {

            public delegate void ChangeDisplayedText(object sender, string s);
            public static event ChangeDisplayedText changeDisplayedTextEvent;

            public static Bitmap Assemble(Sprite[] sprites, PackingRectangle[] rects, PackingRectangle bounds) {

                  Bitmap gamesheet = new Bitmap((int)bounds.Width, (int)bounds.Height);

                  for (int i = 0; i < sprites.Length; i++) {
                        gamesheet.CopyTo(sprites[i].texture, (int)rects[i].X, (int)rects[i].Y);
                  }

                  return gamesheet;
            }
      }
}
