using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_Randomiser {

      public class ThemeController {

            public enum Theme { Dark, Light }

            public Theme theme = Theme.Dark;

            public ThemeController() { }

            public ThemeController(Theme theme) {
                  this.theme = theme;
            }

            readonly Color[] formBackColours = new Color[] {
                  Color.FromArgb(30, 30, 30),
                  Color.FromArgb(175, 175, 175)
            };

            readonly Color[] fontColours = new Color[] {
                  Color.FromArgb(255, 255, 255),
                  Color.FromArgb(0, 0, 0),
            };

            readonly Color[] menuElementBackColours = new Color[] {
                  Color.FromArgb(65, 65, 65),
                  Color.FromArgb(255, 255, 255),
            };

            readonly Color[] menuElementForeColours = new Color[] {
                  Color.FromArgb(255, 255, 255),
                  Color.FromArgb(0, 0, 0),
            };

            public Color GetFormBackgroundColour() {
                  return formBackColours[(int)this.theme];
            }

            public Color GetTextColour() {
                  return fontColours[(int)this.theme];
            }

            public Color GetMenuElementBackColour() {
                  return menuElementBackColours[(int)this.theme];
            }

            public Color GetMenuElementForeColour() {
                  return menuElementForeColours[(int)this.theme];
            }
      }
}
