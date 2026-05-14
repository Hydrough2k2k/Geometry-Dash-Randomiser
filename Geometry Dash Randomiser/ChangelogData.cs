using System;
using System.Collections.Generic;

namespace Geometry_Dash_Randomiser {

      public class ChangelogData {

            private readonly string _version;
            private readonly string[] _newStuff = Array.Empty<string>();
            private readonly string[] _bugfixes = Array.Empty<string>();
            private readonly string[] _knownBugs = Array.Empty<string>();

            public string Version => _version;
            public string[] NewStuff => _newStuff;
            public string[] Bugfixes => _bugfixes;
            public string[] KnownBugs => _knownBugs;

            public ChangelogData() { }

            public ChangelogData(string version) {
                  _version = version;
            }

            public ChangelogData(string version, string[] newStuff, string[] bugfixes, string[] knownBugs) {
                  _version = version;
                  _newStuff = newStuff;
                  _bugfixes = bugfixes;
                  _knownBugs = knownBugs;
            }

            public static ChangelogData Default => new ChangelogData(
                  version: "V 2.4.0.3",

                  newStuff: new string[] {
                        " - Changed header and version text font",
                        " - Added a way to import and export the app configuration",
                        " - Added symbols to fields or areas that prevent the app from working",
                        " - Set the maximum randomisation group to 100 because idk",
                        " - Added a debug console window that you can enable in the config.txt file",
                        " - You can now make custom themes, open the \"Themes\" folder after launching the application",
                        " - Added an extra \"Strawberry\" theme",
                        " - Made a fancy Changelog window. You are looking at it right now! (maybe)"
                  },

                  bugfixes: new string[] {
                        " - Reduced the amount of configuration file writes",
                        " - On bootup the settings for icon randomisations settings were not loaded properly"
                  },

                  knownBugs: new string[] {
                        " - Texture Size Multiplier slider is always visually at 0 at startup. The setting is saved, but not reflected visually on the slider",
                        " - Some sawblades get their hitboxes resized when a smaller or bigger texture replaces it's sprite. This can make levels easier or impossible in some cases",
                        " - Sometimes some fonts do not render at all",
                        " - Mystery bugs that I haven't discovered yet"
                  }
            );

            public static ChangelogData ConvertFromData(List<string> rawData) => ConvertFromData(rawData.ToArray());

            public static ChangelogData ConvertFromData(string[] rawData) {
                  string version = string.Empty;
                  List<string> newStuff = new List<string>();
                  List<string> bugfixes = new List<string>();
                  List<string> knownBugs = new List<string>();

                  for (int i = 0; i < rawData.Length; i++) {
                        string line = rawData[i].Trim();

                        if (line.StartsWith("Changelog for ", StringComparison.OrdinalIgnoreCase)) {
                              int index = line.LastIndexOf(' ');
                              if (index != -1) {
                                    line = line.Substring(index + 1).Trim(' ').Trim(':');
                              }
                              version = line;

                        } else if (line.StartsWith("What's New?", StringComparison.OrdinalIgnoreCase)) {
                              i++;
                              while (i < rawData.Length && !string.IsNullOrWhiteSpace(rawData[i])) {
                                    newStuff.Add(rawData[i]);
                                    i++;
                              }

                        } else if (line.StartsWith("Bugfixes:", StringComparison.OrdinalIgnoreCase)) {
                              i++;
                              while (i < rawData.Length && !string.IsNullOrWhiteSpace(rawData[i])) {
                                    bugfixes.Add(rawData[i]);
                                    i++;
                              }

                        } else if (line.StartsWith("Known Bugs:", StringComparison.OrdinalIgnoreCase)) {
                              i++;
                              while (i < rawData.Length && !string.IsNullOrWhiteSpace(rawData[i])) {
                                    knownBugs.Add(rawData[i]);
                                    i++;
                              }
                        }
                  }

                  return new ChangelogData(version, newStuff.ToArray(), bugfixes.ToArray(), knownBugs.ToArray());
            }
      }
}
