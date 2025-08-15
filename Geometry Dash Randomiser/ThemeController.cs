using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Geometry_Dash_Randomiser {

      public class ThemeController {

            private const string themeFolderPath = "Themes";

            public ThemeController(bool getDefaultThemes = true) {
                  if (getDefaultThemes) {
                        themes.AddRange(GetDefaultThemes());
                  }
            }

            List<Theme> themes = new List<Theme>();

            public int activeThemeID { get; set; }
            public int themeCount => themes.Count;

            public Theme current => GetActiveTheme();

            public void AddTheme(Theme theme) {
                  themes.Add(theme);
            }

            public void AddThemes(Theme[] theme) {
                  themes.AddRange(theme);
            }

            public void AddThemes(List<Theme> theme) {
                  themes.AddRange(theme);
            }

            public Theme GetActiveTheme() {
                  if (activeThemeID >= themes.Count) {
                        Console.WriteLine("Failed to get Active Theme ID {0}, ID parameter was out of range. Total themes count: {1}", activeThemeID, this.themes.Count);
                        activeThemeID = activeThemeID % themes.Count;
                        Config.themeID = activeThemeID;
                        return themes[0];
                  }
                  return themes[activeThemeID];
            }

            public Theme GetThemeByID(int ID) {
                  if (ID > themes.Count) {
                        Console.WriteLine("Failed to get Theme ID {0}, ID parameter was out of range. Total themes count: {1}", ID, this.themes.Count);
                        return themes[0];
                  }
                  return themes[ID];
            }

            public int GetThemeCount() {
                  return themes.Count;
            }

            public string[] GetAllThemeNames() {
                  return this.themes.Select(t => t.name).ToArray();
            }

            public string GetThemeName() {
                  return current.name;
            }

            public Color GetFormBackgroundColour() {
                  return current.formBackColour;
            }

            public Color GetDefaultTextColour() {
                  return current.defaultTextColour;
            }

            public Color GetMenuElementBackColour() {
                  return current.menuElementBackColour;
            }

            public Color GetMenuElementForeColour() {
                  return current.menuElementTextColour;
            }

            public Color GetBeamColour() {
                  return current.beamColour;
            }

            public static Theme[] GetDefaultThemes() {
                  return new Theme[] {
                        new Theme(
                              name: "Wisteria",
                              formBackColour: ColorExt.FromHex("7B60AC"),
                              defaultTextColour: ColorExt.FromHex("F9E6FF"),
                              menuElementBackColour: ColorExt.FromHex("664D91"),
                              menuElementTextColour: ColorExt.FromHex("F9E6FF"),
                              beamColour: ColorExt.FromHex("D2BEE6")
                        ),
                        new Theme(
                              name: "Night Theme",
                              formBackColour: Color.FromArgb(0, 7, 33),
                              defaultTextColour: Color.FromArgb(200, 225, 255),
                              menuElementBackColour: Color.FromArgb(60, 71, 115),
                              menuElementTextColour: Color.FromArgb(200, 225, 255),
                              beamColour: Color.FromArgb(200, 225, 255)
                        ),
                        new Theme(
                              name: "Dark Theme",
                              formBackColour: ColorExt.FromHex("121212"),
                              defaultTextColour: ColorExt.FromHex("E6E6E6"),
                              menuElementBackColour: ColorExt.FromHex("222222"),
                              menuElementTextColour: ColorExt.FromHex("FFFFFF"),
                              beamColour: ColorExt.FromHex("888888")
                        ),
                        new Theme(
                              name: "Light Theme",
                              formBackColour : Color.FromArgb(175, 175, 175),
                              defaultTextColour: Color.FromArgb(0, 0, 0),
                              menuElementBackColour: Color.FromArgb(255, 255, 255),
                              menuElementTextColour: Color.FromArgb(0, 0, 0),
                              beamColour: Color.FromArgb(0, 0, 0)
                        ),
                        new Theme(
                              name: "Random Theme",
                              formBackColour : Color.FromArgb(0, 0, 0),
                              defaultTextColour: Color.FromArgb(0, 0, 0),
                              menuElementBackColour: Color.FromArgb(0, 0, 0),
                              menuElementTextColour: Color.FromArgb(0, 0, 0),
                              beamColour: Color.FromArgb(0, 0, 0)
                        )
                  };
            }
      }
}
