using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Geometry_Dash_Randomiser {

      public class ChangelogData {

            public string Version => _version;
            public string ReleaseDate => _releaseDate;
            //public string[] NewStuff => _newStuff;
            //public string[] Changes => _changes;
            //public string[] Bugfixes => _bugfixes;
            //public string[] KnownBugs => _knownBugs;
            //public string[] Notes => _notes;

            public ChangeLogProperty NewStuff => _newStuff;
            public ChangeLogProperty Changes => _changes;
            public ChangeLogProperty Bugfixes => _bugfixes;
            public ChangeLogProperty KnownBugs => _knownBugs;
            public ChangeLogProperty Notes => _notes;

            public ChangelogData() { }

            public ChangelogData(string version) {
                  _version = version;
            }

            public ChangelogData(string version, string releaseDate, string[] newStuff, string[] changes, string[] bugfixes, string[] knownBugs, string[] notes) {
                  this._version = version;
                  this._releaseDate = releaseDate;
                  this._newStuff = new ChangeLogProperty(newStuff, ChangeLogProperty.Type.NewStuff);
                  this._changes = new ChangeLogProperty(changes, ChangeLogProperty.Type.Changes);
                  this._bugfixes = new ChangeLogProperty(bugfixes, ChangeLogProperty.Type.Bugfixes);
                  this._knownBugs = new ChangeLogProperty(knownBugs, ChangeLogProperty.Type.KnownBugs);
                  this._notes = new ChangeLogProperty(notes, ChangeLogProperty.Type.Notes);
            }

            public static ChangelogData Default => new ChangelogData(
                  version: "V2.5.1.0",
                  releaseDate: "10 Nov 2026",

                  newStuff: new string[] {
                        " - The UI has been made a lot more responsive",
                        " - Texture Size Multiplier slider's position is updated correctly when the application launches",
                        " - Added a setting button next to the theme selector for enabling and disabling the Random and System themes",
                        " - Added a logger system. It is only be partially implemented for now, but will be expanded in the future"
                  },

                  changes: new string[] {

                  },

                  bugfixes: new string[] {
                        " - Fixed the Icon Settings group value not changing",
                        " - On bootup the settings for icon randomisations settings were not loaded properly",
                        " - Fixed the Debug Console not printing anything sometimes. You can enable it in the config.txt file for debugging purposes",
                        " - Fixed a crash when refreshing themes and there were fewer themes than the current theme index",
                        " - Potentially fixed fonts not rendering sometimes. More testing is required"
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
            private readonly ChangeLogProperty _newStuff = new ChangeLogProperty();
            private readonly ChangeLogProperty _changes = new ChangeLogProperty();
            private readonly ChangeLogProperty _bugfixes = new ChangeLogProperty();
            private readonly ChangeLogProperty _knownBugs = new ChangeLogProperty();
            private readonly ChangeLogProperty _notes = new ChangeLogProperty();

            public class ChangeLogProperty {

                  public enum Type { None, NewStuff, Changes, Bugfixes, KnownBugs, Notes }

                  public ChangeLogProperty() { }

                  public ChangeLogProperty(string[] data, Type type) {
                        _type = type;
                        Data = data;
                  }

                  public ChangeLogProperty(Type type, string[] data) {
                        _type = type;
                        Data = data;
                  }

                  public string[] Data { get; set; }
                  public string TypeName => TryGetTypeName();

                  private string TryGetTypeName() {
                        bool found = _typeNameDict.TryGetValue(this._type, out string value);
                        return found ? value : string.Empty;
                  }

                  private static Dictionary<Type, string> _typeNameDict = new Dictionary<Type, string>() {
                         { Type.None, string.Empty },
                         { Type.NewStuff, "What's New?" },
                         { Type.Changes, "Changes:" },
                         { Type.Bugfixes, "Bugfixes:" },
                         { Type.KnownBugs, "Known Bugs:" },
                         { Type.Notes, "Notes:" }
                  };

                  private Type _type;
            }
      }
}
