using System.Drawing;
using System.Linq;

namespace Geometry_Dash_Randomiser {

      public class Theme {

            public Theme() { }

            public Theme(string name, Color formBackColour, Color defaultTextColour, Color menuElementBackColour, Color menuElementForeColour, Color beamColour) {
                  this.name = name;
                  this.formBackColour = formBackColour;
                  this.defaultTextColour = defaultTextColour;
                  this.menuElementBackColour = menuElementBackColour;
                  this.menuElementForeColour = menuElementForeColour;
                  this.beamColour = beamColour;
            }

            public string name { get; set; }
            public Color formBackColour { get; set; }
            public Color defaultTextColour { get; set; }
            public Color menuElementBackColour { get; set; }
            public Color menuElementForeColour { get; set; }
            public Color beamColour { get; set; }
      }
}
