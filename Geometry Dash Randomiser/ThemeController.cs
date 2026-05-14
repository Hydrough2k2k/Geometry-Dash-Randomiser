using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Geometry_Dash_Randomiser {

      public class ThemeController {

            private const string themeFolderPath = "Themes";
            private const string sampleThemeFileName = "Theme_Example.txt";

            public ThemeController() {
                  CreateThemeFolderAndDefaultContents();
            }

            public void GetAllThemesFromFile(bool getDefaultThemes = true) {
                  if (themes.Count != 0) {
                        DefaultThemes[DefaultThemes.Length - 1] = themes.Where(t => t.Name == "Random Theme").ToArray()[0];
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
                  }

                  Console.WriteLine($"Total themes loaded: {this.GetThemeCount()}");
                  Console.WriteLine(string.Join("\n", this.GetAllThemeNames()) + "\n");
            }

            readonly List<Theme> themes = new List<Theme>();

            public int ActiveThemeID { get; set; }
            public int ThemeCount => themes.Count;

            public Theme Current => GetActiveTheme();

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

                  string sampleThemePath = Path.Combine(themeFolderPath, sampleThemeFileName);
                  if (File.Exists(sampleThemePath)) {
                        Console.WriteLine($"Sample theme file \"{sampleThemeFileName}\" already exists, skipping creation");
                        return;
                  }

                  string sampleThemeData =
                        "# This is a sample theme file for this application\n" +
                        "# The sample theme won't be read because of the \"Ignore Theme\" section, but all others in this file will be\n" +
                        "# The alpha (A) channel is ignored, as transparent objects can't exist, as a limitation of WinForms\n" +
                        "# Hex values override the RGB values, so you can just skip them if you like\n";

                  sampleThemeData += "\n" + sampleTheme.Serialize();

                  File.WriteAllText(sampleThemePath, sampleThemeData);

                  Console.WriteLine($"Finished writing \"{sampleThemeFileName}\"");
            }

            public Theme GetActiveTheme() {
                  if (ActiveThemeID >= themes.Count) {
                        Console.WriteLine($"Failed to get Active Theme ID {ActiveThemeID}, ID parameter was out of range. Total themes count: {this.themes.Count}");
                        ActiveThemeID %= themes.Count;
                        Config.Instance.themeID = ActiveThemeID;
                        return themes[0];

                  } else if (ActiveThemeID < 0) {
                        Console.WriteLine($"The theme ID was negative");
                        Config.Instance.themeID = 0;
                        this.ActiveThemeID = 0;
                        return themes[0];
                  }
                  return themes[ActiveThemeID];
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
                        BackgroundColour: ColorExt.FromHex("7B60AC"),
                        TextColour: ColorExt.FromHex("F9E6FF"),
                        ObjectBackColour: ColorExt.FromHex("664D91"),
                        ObjectTextColour: ColorExt.FromHex("F9E6FF"),
                        BeamColour: ColorExt.FromHex("D2BEE6")
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
                        BackgroundColour: ColorExt.FromHex("121212"),
                        TextColour: ColorExt.FromHex("E6E6E6"),
                        ObjectBackColour: ColorExt.FromHex("222222"),
                        ObjectTextColour: ColorExt.FromHex("FFFFFF"),
                        BeamColour: ColorExt.FromHex("999999")
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
                        BackgroundColour: ColorExt.FromHex("E8B8C2"),
                        TextColour: ColorExt.FromHex("2A1419"),
                        ObjectBackColour: ColorExt.FromHex("FF6B86"),
                        ObjectTextColour: ColorExt.FromHex("FFFFFF"),
                        BeamColour: ColorExt.FromHex("FF6890")
                  ),
                  new Theme( // The colours of all of these properties are randomised every time this theme is selected
                        Name: "Random Theme",
                        BackgroundColour: Color.FromArgb(0, 0, 0),
                        TextColour: Color.FromArgb(0, 0, 0),
                        ObjectBackColour: Color.FromArgb(0, 0, 0),
                        ObjectTextColour: Color.FromArgb(0, 0, 0),
                        BeamColour: Color.FromArgb(0, 0, 0)
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
      }
}
