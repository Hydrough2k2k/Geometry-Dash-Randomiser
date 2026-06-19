using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Geometry_Dash_Randomiser {

      public class ThemeController {

            private const string themeFolderPath = "Themes";
            private const string sampleThemeFileName = "Theme_Example.txt";

            public ThemeController() {
                  CreateThemeFolder();
            }

            readonly List<Theme> themes = new List<Theme>();

            public int ActiveThemeID { get; set; }
            public int ThemeCount => themes.Count;

            public Theme Current => GetActiveTheme();

            public void GetAllThemesFromFile(bool getDefaultThemes = true) {
                  Theme randomTheme = new Theme(RandomThemeName);
                  if (themes.Count != 0) {
                        randomTheme = themes.Where(t => t.Name == "Random Theme").ToArray()[0];
                  }

                  themes.Clear();

                  string[] themeFiles = Directory.GetFiles(themeFolderPath, "*.txt");

                  for (int i = 0; i < themeFiles.Length; i++) {
                        string[] fileData = File.ReadAllLines(themeFiles[i]);
                        List<Theme> readThemes = ThemeReader.ReadThemesFromText(fileData, themeFiles[i]).Where(t => t != null).ToList();
                        themes.AddRange(readThemes);
                  }

                  if (getDefaultThemes) {
                        themes.AddRange(DefaultThemes);

                        if (Config.Instance.enableRandomTheme) {
                              themes.Add(randomTheme);
                        }

                        if (Config.Instance.enableSystemTheme) {
                              themes.Add(new Theme(SystemThemeName));
                        }
                  }

                  Log.Write(Log.Mode.Info, $"Total themes loaded: {this.GetThemeCount()}");
                  Log.Write(Log.Mode.Verbose, "Loaded themes: \n\t" + string.Join("\n\t", this.GetAllThemeNames()));
            }

            public void AddTheme(Theme theme) {
                  themes.Add(theme);
            }

            public void AddThemes(Theme[] theme) {
                  themes.AddRange(theme);
            }

            public void AddThemes(List<Theme> theme) {
                  themes.AddRange(theme);
            }

            public void CreateThemeFolder() {

                  if (Directory.Exists(themeFolderPath) == false) {
                        Log.Write(Log.Mode.Info, "Theme folder does not exist, time to create it");

                        Directory.CreateDirectory(themeFolderPath);
                  }

                  string sampleThemePath = Path.Combine(themeFolderPath, sampleThemeFileName);
                  if (File.Exists(sampleThemePath)) {
                        Log.Write(Log.Mode.Info, $"Sample theme file \"{sampleThemeFileName}\" already exists, skipping creation");
                        return;
                  }

                  string sampleThemeData =
                        "# This is a sample theme file for this application\n" +
                        "# The sample theme won't be read because of the \"Ignore Theme\" section, but all others in this file will be\n" +
                        "# The alpha (A) channel is ignored, as transparent objects can't exist, as a limitation of WinForms\n" +
                        "# Hex values override the RGB values, so you can just skip them if you like\n";

                  sampleThemeData += "\n" + sampleTheme.Serialize();

                  File.WriteAllText(sampleThemePath, sampleThemeData);

                  Log.Write(Log.Mode.Verbose, $"Finished writing \"{sampleThemeFileName}\"");
            }

            public void ValidateCurrentThemeID() {
                  if (ActiveThemeID >= themes.Count) {
                        ActiveThemeID = 0;
                        Config.Instance.themeID = ActiveThemeID;

                  } else if (ActiveThemeID < 0) {
                        Config.Instance.themeID = 0;
                        this.ActiveThemeID = 0;
                  }
            }

            public Theme GetActiveTheme() {
                  if (ActiveThemeID >= themes.Count) {
                        Log.Write(Log.Mode.Warn, $"Failed to get Active Theme ID {ActiveThemeID}, ID parameter was out of range. Total themes count: {this.themes.Count}");
                        ActiveThemeID = 0;
                        Config.Instance.themeID = ActiveThemeID;
                        return themes[0];

                  } else if (ActiveThemeID < 0) {
                        Log.Write(Log.Mode.Warn, $"The theme ID was negative. Setting it to 0");
                        Config.Instance.themeID = 0;
                        this.ActiveThemeID = 0;
                        return themes[0];
                  }
                  return themes[ActiveThemeID];
            }

            public Theme GetThemeByID(int ID) {
                  if (ID > themes.Count) {
                        Log.Write(Log.Mode.Warn, $"Failed to get Theme ID {ID}, ID parameter was out of range. Total themes count: {this.themes.Count}");
                        return themes[0];
                  }
                  return themes[ID];
            }

            public int GetThemeCount() {
                  return themes.Count;
            }

            public string[] GetAllThemeNames() {
                  return this.themes.Select(t => t.Name).ToArray();
            }

            public string GetThemeName() {
                  return Current.Name;
            }

            public Color GetFormBackgroundColour() {
                  return Current.BackgroundColour;
            }

            public Color GetDefaultTextColour() {
                  return Current.TextColour;
            }

            public Color GetMenuElementBackColour() {
                  return Current.ObjectBackColour;
            }

            public Color GetMenuElementForeColour() {
                  return Current.ObjectTextColour;
            }

            public Color GetBeamColour() {
                  return Current.BeamColour;
            }

            private Theme[] DefaultThemes = new Theme[] {
                  new Theme(
                        Name: "Wisteria",
                        BackgroundColour: ColorExtensions.FromHex("7B60AC"),
                        TextColour: ColorExtensions.FromHex("F9E6FF"),
                        ObjectBackColour: ColorExtensions.FromHex("664D91"),
                        ObjectTextColour: ColorExtensions.FromHex("F9E6FF"),
                        BeamColour: ColorExtensions.FromHex("D2BEE6")
                  ),
                  new Theme(
                        Name: "Night Theme",
                        BackgroundColour: Color.FromArgb(0, 7, 33),
                        TextColour: Color.FromArgb(200, 225, 255),
                        ObjectBackColour: Color.FromArgb(60, 71, 115),
                        ObjectTextColour: Color.FromArgb(200, 225, 255),
                        BeamColour: Color.FromArgb(200, 225, 255)
                  ),
                  new Theme(
                        Name: "Dark Theme",
                        BackgroundColour: ColorExtensions.FromHex("121212"),
                        TextColour: ColorExtensions.FromHex("E6E6E6"),
                        ObjectBackColour: ColorExtensions.FromHex("222222"),
                        ObjectTextColour: ColorExtensions.FromHex("FFFFFF"),
                        BeamColour: ColorExtensions.FromHex("999999")
                  ),
                  new Theme(
                        Name: "Light Theme",
                        BackgroundColour: Color.FromArgb(175, 175, 175),
                        TextColour: Color.FromArgb(0, 0, 0),
                        ObjectBackColour: Color.FromArgb(255, 255, 255),
                        ObjectTextColour: Color.FromArgb(0, 0, 0),
                        BeamColour: Color.FromArgb(0, 0, 0)
                  ),
                  new Theme(
                        Name: "Strawberry",
                        BackgroundColour: ColorExtensions.FromHex("E8B8C2"),
                        TextColour: ColorExtensions.FromHex("2A1419"),
                        ObjectBackColour: ColorExtensions.FromHex("FF6B86"),
                        ObjectTextColour: ColorExtensions.FromHex("FFFFFF"),
                        BeamColour: ColorExtensions.FromHex("FF6890")
                  )
            };

            private static readonly Theme sampleTheme = new Theme(
                  Name: "Sample Theme",
                  BackgroundColour: Color.FromArgb(50, 50, 50),
                  TextColour: Color.FromArgb(200, 200, 200),
                  ObjectBackColour: Color.FromArgb(80, 80, 80),
                  ObjectTextColour: Color.FromArgb(220, 220, 220),
                  BeamColour: Color.FromArgb(150, 150, 150)
            );

            public const string RandomThemeName = "Random Theme";
            public const string SystemThemeName = "System Theme";
      }
}
