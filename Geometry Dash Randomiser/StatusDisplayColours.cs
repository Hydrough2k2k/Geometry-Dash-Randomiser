using System.Drawing;

namespace Geometry_Dash_Randomiser {

      public class StatusDisplayColours {

            public StatusDisplayColours() { }

            public StatusDisplayColours(Color unready, Color ready, Color randomising, Color complete, Color backColour) {
                  this.unready = unready;
                  this.ready = ready;
                  this.randomising = randomising;
                  this.complete = complete;
                  this.backColour = backColour;
            }

            public Color unready { get; set; } = Color.FromArgb(255, 0, 0);
            public Color ready { get; set; } = Color.FromArgb(0, 255, 0);
            public Color randomising { get; set; } = Color.FromArgb(0, 127, 255);
            public Color complete { get; set; } = Color.FromArgb(0, 0, 255);
            public Color backColour { get; set; } = Color.FromArgb(0, 0, 0, 0);
      }
}
