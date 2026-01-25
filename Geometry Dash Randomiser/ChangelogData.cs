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
                  "Unknown Version"
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
