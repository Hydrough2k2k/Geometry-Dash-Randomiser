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
                              ret.name = data;

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
                                    ret.backgroundColour = colour;

                              } else if (colourChannel.Contains("Text") && !line.Contains("Object")) {
                                    ret.textColour = colour;

                              } else if (colourChannel.Contains("Object Back")) {
                                    ret.objectBackColour = colour;

                              } else if (colourChannel.Contains("Object Text")) {
                                    ret.objectTextColour = colour;

                              } else if (colourChannel.Contains("Beam")) {
                                    ret.beamColour = colour;

                              } else {
                                    Console.WriteLine($"Unknown colour channel \"{colourChannel}\" in \"{fileName}\" at line {startLine + i}");
                              }
                        }
                  }

                  if (ret.name == string.Empty) {
                        ret.name = "Unnamed Theme " + unnamedThemeCounter.ToString();
                        unnamedThemeCounter++;

                  } else if (ret.name == "Sample Theme") {
                        Console.WriteLine($"Ignoring theme called \"{ret.name}\" from file \"{fileName}\"");
                        return null;
                  }

                  return ret;
            }
      }
}
