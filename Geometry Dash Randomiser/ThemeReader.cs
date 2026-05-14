using System;
using System.Collections.Generic;
using System.Drawing;

namespace Geometry_Dash_Randomiser {

      internal class ThemeReader {

            internal static int unnamedThemeCounter = 1;

            internal static List<Theme> ReadThemesFromText(string[] text, string fileName) {
                  List<Theme> themes = new List<Theme>();
                  int startLine = 0;
                  for (int i = 0; i < text.Length; i++) {
                        string line = text[i];
                        if (line.StartsWith("#"))
                              continue;

                        if (line.StartsWith("End")) {
                              if (startLine >= 0) {
                                    string[] data = new string[i - startLine];
                                    int l = 0;
                                    for (int j = startLine; j < i; j++, l++) {
                                          data[l] = text[j];
                                    }

                                    Theme theme = DeserializeTheme(data, fileName, text[startLine]);
                                    if (theme != null) {
                                          themes.Add(theme);
                                    }
                              }
                              startLine = i + 1;
                        }
                  }
                  return themes;
            }

            public static Theme DeserializeTheme(string[] text, string fileName, string startLine) {
                  Theme ret = new Theme();

                  string colourChannel = string.Empty;
                  int colourStartLine = -1;
                  int colourEndLine = -1;

                  for (int i = 0; i < text.Length; i++) {
                        string line = text[i];
                        string data = line;
                        
                        if (data.Contains(":")) {
                              data = line.Substring(line.IndexOf(':') + 1).Trim();
                        }

                        if (line.StartsWith("#"))
                              continue;

                        if (line.StartsWith("Name")) {
                              ret.Name = data;

                        } else if (line.Contains("Color") || line.Contains("Colour")) {
                              colourStartLine = i;
                              colourChannel = line;
                        }

                        if (line.Contains("]")) {
                              colourEndLine = i;

                              if (colourStartLine == -1) {
                                    Console.WriteLine($"Malformed colour data in {fileName} at line {startLine + i}. Colour channel {colourChannel} could not be decoded");
                                    continue;
                              }

                              string[] colourData = new string[colourEndLine - colourStartLine + 1];
                              for (int j = colourStartLine; j <= colourEndLine; j++) {
                                    colourData[j - colourStartLine] = text[j];
                              }

                              Color colour = ColorExt.Deserialize(colourData);

                              if (colourChannel.Contains("Background")) {
                                    ret.BackgroundColour = colour;

                              } else if (colourChannel.Contains("Text") && !line.Contains("Object")) {
                                    ret.TextColour = colour;

                              } else if (colourChannel.Contains("Object Back")) {
                                    ret.ObjectBackColour = colour;

                              } else if (colourChannel.Contains("Object Text")) {
                                    ret.ObjectTextColour = colour;

                              } else if (colourChannel.Contains("Beam")) {
                                    ret.BeamColour = colour;

                              } else {
                                    Console.WriteLine($"Unknown colour channel \"{colourChannel}\" in \"{fileName}\" at line {startLine + i}");
                              }
                        }
                  }

                  if (ret.Name == string.Empty) {
                        ret.Name = "Unnamed Theme " + unnamedThemeCounter.ToString();
                        unnamedThemeCounter++;

                  } else if (ret.Name == "Sample Theme") {
                        Console.WriteLine($"Ignoring theme called \"{ret.Name}\" from file \"{fileName}\"");
                        return null;
                  }

                  return ret;
            }
      }
}
