using Geometry_Dash_Randomiser.Properties;
using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using System.Security.Policy;
using static Geometry_Dash_Randomiser.Logger;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

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
                  version: "V2.5.0.0",

                  newStuff: new string[] {
                        " - Reworked and massively expanded font randomisation settings. Go wild",
                        " - Made the connector beams have a different colour when the setting it branched away from is disabled",
                        " - Split the config file into \"congif.ini\" and \"adv_config.ini\". Randomisation settings are all stored in the first one",
                        " - Made the parent checkboxes (like the icon randomisation one) have a square instead of a check mark when not all child settings are enabled",
                        " - Added milliseconds to the elapsed time display, because why not"
                  },

                  bugfixes: new string[] {
                        " - When you closed GDR during a randomisation, it would keep working on it until it was done",
                        " - Made the application remove old log files. The default limit can be adjusted in the advanced config file"
                  },

                  knownBugs: new string[] {
                        " - Some sawblades get their hitboxes resized when a smaller or bigger texture replaces it's sprite. This can make levels easier or impossible in some cases",
                        " - Mystery bugs that I haven't discovered yet"
<<<<<<< Updated upstream
=======
                  },

                  notes: new string[] {
                        " - This update breaks old settings exported via the \"Export Config\" button as the system was completely overhauled"
>>>>>>> Stashed changes
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
