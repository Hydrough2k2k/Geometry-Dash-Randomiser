using System.Collections.Generic;
using System.Linq;

namespace Geometry_Dash_Randomiser {

      public static class FontSerializer {

            public static PropertyPair[] ParsePropertyPairs(string str) {
                  // Remove everything before the first space character
                  str = str.Substring(str.IndexOf(' ') == -1 ? 0 : str.IndexOf(' ')).Trim();

                  // Get all data points after splittting the string, where the string is not empty
                  List<string> dataPoints = new List<string>();

                  for (int startIndex = 0; startIndex < str.Length; startIndex++) {
                        // Search for the next equals
                        int equalsIndex = str.IndexOf('=', startIndex);
                        int endIndex = -1;

                        if (equalsIndex != -1) {
                              // If the next character after the equals is a quote
                              if (str.Length > equalsIndex + 2 && str[equalsIndex + 1] == '\"') {
                                    // Search for the next data point
                                    int nextEqualsIndex = str.IndexOf('=', equalsIndex + 1);
                                    if (nextEqualsIndex == -1)
                                          nextEqualsIndex = str.Length - 1;

                                    // Scan backwards for the last space
                                    int spaceIndex = str.LastIndexOf(' ', nextEqualsIndex);
                                    if (spaceIndex == -1)
                                          spaceIndex = str.Length - 1;

                                    // Then the last quote to mark the end of the string
                                    endIndex = str.LastIndexOf('\"', spaceIndex);
                                    if (endIndex != -1)
                                          endIndex++;

                              } else {
                                    endIndex = str.IndexOf(' ', equalsIndex);
                              }
                        }

                        if (endIndex == -1)
                              endIndex = str.Length;

                        string sub = str.Substring(startIndex, endIndex - startIndex).Trim();
                        dataPoints.Add(sub);
                        startIndex = endIndex;
                  }

                  PropertyPair[] pairs = new PropertyPair[dataPoints.Count];

                  for (int i = 0; i < dataPoints.Count; i++) {
                        // Search for the first (and generally only) equals symbol
                        int index = dataPoints[i].IndexOf('=');
                        if (index != -1) {
                              // Trim the name before the equals symbol
                              string name = dataPoints[i].Substring(0, index);
                              string data = dataPoints[i].Substring(index + 1);
                              // If the first and last chars of the data are quotes, remove them
                              if (data[0] == '\"' && data.Last() == '\"') {
                                    //NOTE: data.Trim('\"'); is not going to work if the string contains a quote at the start or end of the encased string
                                    data = data.Substring(1, data.Length - 2);
                              }
                              pairs[i] = new PropertyPair(name, data);

                        } else {
                              // If no equals symbols were found, assume the entire string is a name, and set the data property as an empty string
                              pairs[i] = new PropertyPair(dataPoints[i], string.Empty);
                        }
                  }
                  return pairs;
            }

            public static Font Deserialise(string[] fileStream) {
                  Font font = new Font();
                  List<FontChar> chars = new List<FontChar>();
                  List<FontKerning> kernings = new List<FontKerning>();

                  for (int i = 0; i < fileStream.Length; i++) {
                        string line = fileStream[i];
                        PropertyPair[] pairs = ParsePropertyPairs(line);

                        if (line.StartsWith("info ")) {
                              DeserialiseInfoData(ref font, pairs);

                        } else if (line.StartsWith("common ")) {
                              DeserialiseCommonData(ref font, pairs);

                        } else if (line.StartsWith("page ")) {
                              DeserialisePageData(ref font, pairs);

                        } else if (line.StartsWith("char ")) {
                              chars.Add(DeserialiseFontChar(pairs));

                        } else if (line.StartsWith("kerning ")) {
                              kernings.Add(DeserialiseFontKerning(pairs));
                        }
                  }

                  font.Chars = chars.ToArray();
                  font.Kernings = kernings.ToArray();

                  return font;
            }

            static void DeserialiseInfoData(ref Font font, PropertyPair[] pairs) {

                  for (int i = 0; i < pairs.Length; i++) {
                        string filtered = pairs[i].data.FilterDigits();
                        int parsed = 0;
                        if (filtered.Length != 0) {
                              parsed = Parse.Int(filtered);
                        }

                        if (pairs[i].name == "face") {
                              font.InfoFace = pairs[i].data;
                        } else if (pairs[i].name == "size") {
                              font.Size = parsed;
                        } else if (pairs[i].name == "bold") {
                              font.Bold = parsed;
                        } else if (pairs[i].name == "italic") {
                              font.Italic = parsed;
                        } else if (pairs[i].name == "charset") {
                              font.CharSet = pairs[i].data;
                        } else if (pairs[i].name == "unicode") {
                              font.Unicode = parsed;
                        } else if (pairs[i].name == "stretchH") {
                              font.Stretch_H = parsed;
                        } else if (pairs[i].name == "smooth") {
                              font.Smooth = parsed;
                        } else if (pairs[i].name == "aa") {
                              font.AA = parsed;
                        } else if (pairs[i].name == "padding") {
                              font.Padding = new Int4(pairs[i].data);
                        } else if (pairs[i].name == "spacing") {
                              font.Spacing = new Int2(pairs[i].data);
                        }
                  }
            }

            static void DeserialiseCommonData(ref Font font, PropertyPair[] pairs) {

                  for (int i = 0; i < pairs.Length; i++) {
                        string filtered = pairs[i].data.FilterDigits();
                        int parsed = 0;
                        if (filtered.Length != 0) {
                              parsed = Parse.Int(filtered);
                        }

                        if (pairs[i].name == "lineHeight") {
                              font.LineHeight = parsed;
                        } else if (pairs[i].name == "base") {
                              font.BaseVal = parsed;
                        } else if (pairs[i].name == "scaleW") {
                              font.Scale_W = parsed;
                        } else if (pairs[i].name == "scaleH") {
                              font.Scale_H = parsed;
                        } else if (pairs[i].name == "pages") {
                              font.Pages = parsed;
                        } else if (pairs[i].name == "packed") {
                              font.Packed = parsed;
                        }
                  }
            }

            static void DeserialisePageData(ref Font font, PropertyPair[] pairs) {

                  for (int i = 0; i < pairs.Length; i++) {
                        string filtered = pairs[i].data.FilterDigits();
                        int parsed = 0;
                        if (filtered.Length != 0) {
                              parsed = Parse.Int(filtered);
                        }

                        if (pairs[i].name == "id") {
                              font.PageID = parsed;
                        } else if (pairs[i].name == "file") {
                              font.File = pairs[i].data;
                        }
                  }
            }

            static FontChar DeserialiseFontChar(PropertyPair[] pairs) {
                  FontChar fontChar = new FontChar();

                  for (int i = 0; i < pairs.Length; i++) {
                        string filtered = pairs[i].data.FilterDigits();
                        int parsed = 0;
                        if (filtered.Length != 0) {
                              parsed = Parse.Int(filtered);
                        }

                        if (pairs[i].name == "id") {
                              fontChar.CharID = parsed;
                        } else if (pairs[i].name == "x") {
                              fontChar.X = parsed;
                        } else if (pairs[i].name == "y") {
                              fontChar.Y = parsed;
                        } else if (pairs[i].name == "width") {
                              fontChar.Width = parsed;
                        } else if (pairs[i].name == "height") {
                              fontChar.Height = parsed;
                        } else if (pairs[i].name == "xoffset") {
                              fontChar.X_Offset = parsed;
                        } else if (pairs[i].name == "yoffset") {
                              fontChar.Y_Offset = parsed;
                        } else if (pairs[i].name == "xadvance") {
                              fontChar.X_Advance = parsed;
                        } else if (pairs[i].name == "page") {
                              fontChar.Page = parsed;
                        } else if (pairs[i].name == "chnl") {
                              fontChar.Channel = parsed;
                        } else if (pairs[i].name == "letter") {

                              if (pairs[i].data == "space") {
                                    fontChar.Letter = ' ';
                              } else {
                                    fontChar.Letter = pairs[i].data[0];
                              }
                        }
                  }
                  return fontChar;
            }

            static FontKerning DeserialiseFontKerning(PropertyPair[] pairs) {
                  FontKerning fontKerning = new FontKerning();
                  for (int i = 0; i < pairs.Length; i++) {
                        string filtered = pairs[i].data.FilterDigits();
                        int parsed = 0;
                        if (filtered.Length != 0) {
                              parsed = Parse.Int(filtered);
                        }

                        if (pairs[i].name == "first") {
                              fontKerning.First = parsed;
                        } else if (pairs[i].name == "second") {
                              fontKerning.Second = parsed;
                        } else if (pairs[i].name == "amount") {
                              fontKerning.Amount = parsed;
                        }
                  }
                  return fontKerning;
            }

            public static string SerialiseTextFile(Font font) {
                  int arrayLength = 4 + font.Chars.Length + 1 + font.Kernings.Length;

                  string[] serialised = new string[arrayLength];
                  serialised[0] = SerialiseInfoLine(font);
                  serialised[1] = SerialiseCommonLine(font);
                  serialised[2] = SerialisePageLine(font);
                  serialised[3] = "chars count=" + font.Chars.Length;

                  int line = 4;
                  for (int i = 0; line < arrayLength && i < font.Chars.Length; line++, i++) {
                        serialised[line] = SerialiseFontChar(font.Chars[i]);
                  }
                  serialised[line++] = "kernings count=" + font.Kernings.Length;

                  for (int i = 0; line < arrayLength && i < font.Kernings.Length; line++, i++) {
                        serialised[line] = SerialiseFontKerning(font.Kernings[i]);
                  }

                  return string.Join("\n", serialised);
            }

            static string SerialiseInfoLine(Font font) {
                  return "info face=\"" + font.InfoFace + "\"" +
                        " size=" + font.Size +
                        " bold=" + font.Bold +
                        " italic=" + font.Italic +
                        " charset=\"" + font.CharSet + "\"" +
                        " unicode=" + font.Unicode +
                        " stretchH=" + font.Stretch_H +
                        " smooth=" + font.Smooth +
                        " aa=" + font.AA +
                        " padding=" + font.Padding.ToString(FormatMode.Plist) +
                        " spacing=" + font.Spacing.ToString(FormatMode.Plist);
            }

            static string SerialiseCommonLine(Font font) {
                  return "common lineHeight=" + font.LineHeight +
                        " base=" + font.BaseVal +
                        " scaleW=" + font.Scale_W +
                        " scaleH=" + font.Scale_H +
                        " pages=" + font.Pages +
                        " packed=" + font.Packed;
            }

            static string SerialisePageLine(Font font) {
                  return "page id=" + font.PageID +
                        " file=\"" + font.File + "\"";
            }

            public static string SerialiseFontChar(FontChar fontChar) {
                  return "char id=" + fontChar.CharID +
                        " x=" + fontChar.X +
                        " y=" + fontChar.Y +
                        " width=" + fontChar.Width +
                        " height=" + fontChar.Height +
                        " xoffset=" + fontChar.X_Offset +
                        " yoffset=" + fontChar.Y_Offset +
                        " xadvance=" + fontChar.X_Advance +
                        " page=" + fontChar.Page +
                        " chnl=" + fontChar.Channel +
                        " letter=\"" + (fontChar.Letter == ' ' ? "space" : fontChar.Letter.ToString()) + "\"";
            }

            public static string SerialiseFontKerning(FontKerning fontKerning) {
                  return "kerning first=" + fontKerning.First +
                        " second=" + fontKerning.Second +
                        " amount=" + fontKerning.Amount;
            }
      }
}
