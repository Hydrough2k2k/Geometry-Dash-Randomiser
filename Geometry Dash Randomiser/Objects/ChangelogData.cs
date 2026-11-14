using System;
using System.Collections.Generic;

namespace Geometry_Dash_Randomiser {

      public class ChangelogData {

            public string Version => _version;
            public string ReleaseDate => _releaseDate;
            public string[] NewStuff => _newStuff;
            public string[] Changes => _changes;
            public string[] Bugfixes => _bugfixes;
            public string[] KnownBugs => _knownBugs;
            public string[] Notes => _notes;

            public ChangelogData() { }

            public ChangelogData(string version) {
                  _version = version;
            }

            public ChangelogData(string version, string releaseDate, string[] newStuff, string[] changes, string[] bugfixes, string[] knownBugs, string[] notes) {
                  this._version = version;
                  this._releaseDate = releaseDate;
                  this._newStuff = newStuff;
                  this._changes = changes;
                  this._bugfixes = bugfixes;
                  this._knownBugs = knownBugs;
                  this._notes = notes;
            }

            public static ChangelogData Default => new ChangelogData(
                  version: "V2.5.1.1",
                  releaseDate: "14 Nov 2026",

                  newStuff: new string[] {
                        
                  },

                  changes: new string[] {

                  },

                  bugfixes: new string[] {
                        " - Fixed texture randomization not working at all in the last release. Oops."
                  },

                  knownBugs: new string[] {
                        " - Some sawblades get their hitboxes resized when a smaller or bigger texture replaces it's sprite. This can make levels easier or impossible in some cases",
                        " - Mystery bugs that I haven't discovered yet"
                  },

                  notes: new string[] {
                        
                  }
            );

            public static ChangelogData ConvertFromData(List<string> rawData) => ConvertFromData(rawData.ToArray());

            public static ChangelogData ConvertFromData(string[] rawData) {
                  string version = string.Empty;
                  string releaseDate = string.Empty;
                  List<string> newStuff = new List<string>();
                  List<string> changes = new List<string>();
                  List<string> bugfixes = new List<string>();
                  List<string> knownBugs = new List<string>();
                  List<string> notes = new List<string>();

                  for (int i = 0; i < rawData.Length; i++) {
                        string line = rawData[i].Trim();

                        if (line.StartsWith("Changelog", StringComparison.OrdinalIgnoreCase)) {
                              version = GetPropertyAfter(' ', line, ':');

                        } else if (line.StartsWith("Release Date:", StringComparison.OrdinalIgnoreCase)) {
                              releaseDate = GetPropertyAfter(':', line, ' ');

                        } else if (line.StartsWith("What's New?", StringComparison.OrdinalIgnoreCase)) {
                              AddLinesUntilNullOrWhiteSpace(newStuff, rawData, ref i);

                        } else if (line.StartsWith("Changes:", StringComparison.OrdinalIgnoreCase)) {
                              AddLinesUntilNullOrWhiteSpace(changes, rawData, ref i);

                        } else if (line.StartsWith("Bugfixes:", StringComparison.OrdinalIgnoreCase)) {
                              AddLinesUntilNullOrWhiteSpace(bugfixes, rawData, ref i);

                        } else if (line.StartsWith("Known Bugs:", StringComparison.OrdinalIgnoreCase)) {
                              AddLinesUntilNullOrWhiteSpace(knownBugs, rawData, ref i);

                        } else if (line.StartsWith("Notes:", StringComparison.OrdinalIgnoreCase)) {
                              AddLinesUntilNullOrWhiteSpace(notes, rawData, ref i);
                        }
                  }

                  return new ChangelogData(version, releaseDate, newStuff.ToArray(), changes.ToArray(), bugfixes.ToArray(), knownBugs.ToArray(), notes.ToArray());
            }

            private static void AddLinesUntilNullOrWhiteSpace(List<string> to, string[] from, ref int index) {
                  index++;
                  while (index < from.Length && !string.IsNullOrWhiteSpace(from[index])) {
                        to.Add(from[index]);
                        index++;
                  }
            }

            private static string GetPropertyAfter(char ch, string from, char trim) {
                  int index = from.IndexOf(ch);
                  if (index != -1) {
                        from = from.Substring(index + 1).Trim(trim).Trim(' ');
                  }
                  return from;
            }

            private readonly string _version = string.Empty;
            private readonly string _releaseDate = string.Empty;
            private readonly string[] _newStuff = Array.Empty<string>();
            private readonly string[] _changes = Array.Empty<string>();
            private readonly string[] _bugfixes = Array.Empty<string>();
            private readonly string[] _knownBugs = Array.Empty<string>();
            private readonly string[] _notes = Array.Empty<string>();
      }
}
