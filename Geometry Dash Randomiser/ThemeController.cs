using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Geometry_Dash_Randomiser {

      public class ThemeController {

            private const string themeFolderPath = "Themes";
            private const string sampleThemeFileName = "Theme_Example.txt";

            public ThemeController(bool getDefaultThemes = true) {

                  CreateThemeFolderAndDefaultContents();

                  string[] themeFiles = Directory.GetFiles(themeFolderPath, "*.txt");

                  for (int i = 0; i < themeFiles.Length; i++) {
                        string[] fileData = File.ReadAllLines(themeFiles[i]);
                        List<Theme> readThemes = ThemeReader.ReadThemesFromText(fileData, themeFiles[i]).Where(t => t != null).ToList();
                        themes.AddRange(readThemes);
                  }

                  if (getDefaultThemes) {
                        themes.AddRange(GetDefaultThemes());
                  }

                  Console.WriteLine($"Total themes loaded: {this.GetThemeCount()}");
                  Console.WriteLine(string.Join("\n", this.GetAllThemeNames()) + "\n");
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

            private void CreateThemeFolderAndDefaultContents() {

                  if (Directory.Exists(themeFolderPath) == false) {
                        Console.WriteLine("Theme folder does not exist, time to create it");

                        Directory.CreateDirectory(themeFolderPath);
                  }

                  JsonSerializerOptions options = new JsonSerializerOptions {
                        WriteIndented = true
                  };

                  string sampleThemePath = Path.Combine(themeFolderPath, sampleThemeFileName);
                  if (File.Exists(sampleThemePath)) {
                        Console.WriteLine($"Sample theme file \"{sampleThemeFileName}\" already exists, skipping creation");
                        return;
                  }

                  string sampleThemeData =
                        "# This is a sample theme file for this application\n" +
                        "# The sample theme won't be read because of the \"Ignore Theme\" section, but all others in this file will be\n" +
                        "# The alpha (A) channel is ignored, as transparent objects can't exist\n" +
                        "# Hex values override the RGB values, so you can just skip them if you like\n";

                  sampleThemeData += "\n" + sampleTheme.Serialize();

                  File.WriteAllText(sampleThemePath, sampleThemeData);

                  Console.WriteLine($"Finished writing \"{sampleThemeFileName}\"");
            }

            public Theme GetActiveTheme() {
                  if (activeThemeID >= themes.Count) {
                        Console.WriteLine($"Failed to get Active Theme ID {activeThemeID}, ID parameter was out of range. Total themes count: {this.themes.Count}");
                        activeThemeID = activeThemeID % themes.Count;
                        Config.Instance.themeID = activeThemeID;
                        return themes[0];

                  } else if (activeThemeID < 0) {
                        Console.WriteLine($"The theme ID was negative");
                        Config.Instance.themeID = 0;
                        this.activeThemeID = 0;
                        return themes[0];
                  }
                  return themes[activeThemeID];
            }

            public Theme GetThemeByID(int ID) {
                  if (ID > themes.Count) {
                        Console.WriteLine($"Failed to get Theme ID {ID}, ID parameter was out of range. Total themes count: {this.themes.Count}");
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
                  return current.backgroundColour;
            }

            public Color GetDefaultTextColour() {
                  return current.textColour;
            }

            public Color GetMenuElementBackColour() {
                  return current.objectBackColour;
            }

            public Color GetMenuElementForeColour() {
                  return current.objectTextColour;
            }

            public Color GetBeamColour() {
                  return current.beamColour;
            }

            public static Theme[] GetDefaultThemes() {
                  return new Theme[] {
                        new Theme(
                              name: "Wisteria",
                              backgroundColour: ColorExt.FromHex("7B60AC"),
                              textColour: ColorExt.FromHex("F9E6FF"),
                              objectBackColour: ColorExt.FromHex("664D91"),
                              objectTextColour: ColorExt.FromHex("F9E6FF"),
                              beamColour: ColorExt.FromHex("D2BEE6")
                        ),
                        new Theme(
                              name: "Night Theme",
                              backgroundColour: Color.FromArgb(0, 7, 33),
                              textColour: Color.FromArgb(200, 225, 255),
                              objectBackColour: Color.FromArgb(60, 71, 115),
                              objectTextColour: Color.FromArgb(200, 225, 255),
                              beamColour: Color.FromArgb(200, 225, 255)
                        ),
                        new Theme(
                              name: "Dark Theme",
                              backgroundColour: ColorExt.FromHex("121212"),
                              textColour: ColorExt.FromHex("E6E6E6"),
                              objectBackColour: ColorExt.FromHex("222222"),
                              objectTextColour: ColorExt.FromHex("FFFFFF"),
                              beamColour: ColorExt.FromHex("888888")
                        ),
                        new Theme(
                              name: "Light Theme",
                              backgroundColour: Color.FromArgb(175, 175, 175),
                              textColour: Color.FromArgb(0, 0, 0),
                              objectBackColour: Color.FromArgb(255, 255, 255),
                              objectTextColour: Color.FromArgb(0, 0, 0),
                              beamColour: Color.FromArgb(0, 0, 0)
                        ),
                        new Theme(
                              name: "Random Theme",
                              backgroundColour: Color.FromArgb(0, 0, 0),
                              textColour: Color.FromArgb(0, 0, 0),
                              objectBackColour: Color.FromArgb(0, 0, 0),
                              objectTextColour: Color.FromArgb(0, 0, 0),
                              beamColour: Color.FromArgb(0, 0, 0)
                        ),
                        new Theme(
                            name: "Strawberry",
                            backgroundColour: ColorExt.FromHex("E8B8C2"),
                            textColour: ColorExt.FromHex("2A1419"),
                            objectBackColour: ColorExt.FromHex("FF6B86"),
                            objectTextColour: ColorExt.FromHex("FFFFFF"),
                            beamColour: ColorExt.FromHex("FF9FB0")
                        )
                  };
            }

            private static Theme sampleTheme = new Theme(
                  name: "Sample Theme",
                  backgroundColour: Color.FromArgb(50, 50, 50),
                  textColour: Color.FromArgb(200, 200, 200),
                  objectBackColour: Color.FromArgb(80, 80, 80),
                  objectTextColour: Color.FromArgb(220, 220, 220),
                  beamColour: Color.FromArgb(150, 150, 150)
            );
      }
}
