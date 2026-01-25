using System;
using System.Drawing;

namespace Geometry_Dash_Randomiser {

      public class Theme {

            private const int maxNameLength = 20;

            public Theme() { }

            public Theme(string name, Color backgroundColour, Color textColour, Color objectBackColour, Color objectTextColour, Color beamColour) {
                  this.name = name.Substring(0, Math.Min(maxNameLength, name.Length));
                  this.backgroundColour = backgroundColour;
                  this.textColour = textColour;
                  this.objectBackColour = objectBackColour;
                  this.objectTextColour = objectTextColour;
                  this.beamColour = beamColour;
            }

            public string name { get; set; } = string.Empty;

            public Color backgroundColour { get; set; }
            public Color textColour { get; set; }
            public Color objectBackColour { get; set; }
            public Color objectTextColour { get; set; }
            public Color beamColour { get; set; }

            public string Serialize() {
                  return
                        $"Name: {name}\n" +
                        $"Background Colour: {backgroundColour.Serialize()}\n" +
                        $"Text Colour: {backgroundColour.Serialize()}\n" +
                        $"Object Back Colour: {backgroundColour.Serialize()}\n" +
                        $"Object Text Colour: {backgroundColour.Serialize()}\n" +
                        $"Beam Colour: {backgroundColour.Serialize()}\n" +
                        $"End\n";
            }
      }
}
