using System;
using System.Drawing;

namespace Geometry_Dash_Randomiser {

      public class Theme {

            private const int maxNameLength = 20;

            public Theme() { }

            public Theme(string name, Color formBackColour, Color defaultTextColour, Color menuElementBackColour, Color menuElementTextColour, Color beamColour) {
                  this.name = name.Substring(0, Math.Min(maxNameLength, name.Length));
                  this.formBackColour = formBackColour;
                  this.defaultTextColour = defaultTextColour;
                  this.menuElementBackColour = menuElementBackColour;
                  this.menuElementTextColour = menuElementTextColour;
                  this.beamColour = beamColour;
            }

            public string name { get; set; }
            public Color formBackColour { get; set; }
            public Color defaultTextColour { get; set; }
            public Color menuElementBackColour { get; set; }
            public Color menuElementTextColour { get; set; }
            public Color beamColour { get; set; }
      }
}
