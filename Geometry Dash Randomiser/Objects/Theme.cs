using System;
using System.Drawing;

namespace Geometry_Dash_Randomiser {

      public class Theme {

            private const int maxNameLength = 30;

            public Theme() { }

            public Theme(string Name) {
                  this.Name = Name.Substring(0, Math.Min(maxNameLength, Name.Length));
            }

            public Theme(string Name, Color BackgroundColour, Color TextColour, Color ObjectBackColour, Color ObjectTextColour, Color BeamColour) {
                  this.Name = Name.Substring(0, Math.Min(maxNameLength, Name.Length));
                  this.BackgroundColour = BackgroundColour;
                  this.TextColour = TextColour;
                  this.ObjectBackColour = ObjectBackColour;
                  this.ObjectTextColour = ObjectTextColour;
                  this.BeamColour = BeamColour;
            }

            public string Name { get; set; } = string.Empty;

            public Color BackgroundColour { get; set; } = Color.FromArgb(0, 0, 0);
            public Color TextColour { get; set; } = Color.FromArgb(0, 0, 0);
            public Color ObjectBackColour { get; set; } = Color.FromArgb(0, 0, 0);
            public Color ObjectTextColour { get; set; } = Color.FromArgb(0, 0, 0);
            public Color BeamColour { get; set; } = Color.FromArgb(0, 0, 0);

            public static Theme Default => new Theme();

            public static Theme CreateRandom() => CreateRandom(new Random(Guid.NewGuid().GetHashCode()));

            public static Theme CreateRandom(Random random) {
                  return new Theme(
                        Name: ThemeController.RandomThemeName,
                        BackgroundColour: random.GetRandomRGBColor(),
                        TextColour: random.GetRandomRGBColor(),
                        ObjectBackColour: random.GetRandomRGBColor(),
                        ObjectTextColour: random.GetRandomRGBColor(),
                        BeamColour: random.GetRandomRGBColor()
                  );
            }

            public void CopyColoursFrom(Theme theme) {
                  this.BackgroundColour = theme.BackgroundColour;
                  this.TextColour = theme.TextColour;
                  this.ObjectBackColour = theme.ObjectBackColour;
                  this.ObjectTextColour = theme.ObjectTextColour;
                  this.BeamColour = theme.BeamColour;
            }

            public string Serialize() {
                  return
                        $"Name: {Name}\n" +
                        $"Background Colour: {BackgroundColour.Serialize()}\n" +
                        $"Text Colour: {BackgroundColour.Serialize()}\n" +
                        $"Object Back Colour: {BackgroundColour.Serialize()}\n" +
                        $"Object Text Colour: {BackgroundColour.Serialize()}\n" +
                        $"Beam Colour: {BackgroundColour.Serialize()}\n" +
                        $"End\n";
            }
      }
}
