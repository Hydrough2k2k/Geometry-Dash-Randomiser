using System.Collections.Generic;
using System.Linq;

namespace Geometry_Dash_Randomiser {

      public static class FontSerialiser {

            public struct PropertyPair {
                  public string name, data;

                  public PropertyPair(string name, string data) {
                        this.name = name;
                        this.data = data;
                  }
            }

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

                  font.chars = chars.ToArray();
                  font.kernings = kernings.ToArray();

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
                              font.infoFace = pairs[i].data;
                        } else if (pairs[i].name == "size") {
                              font.size = parsed;
                        } else if (pairs[i].name == "bold") {
                              font.bold = parsed;
                        } else if (pairs[i].name == "italic") {
                              font.italic = parsed;
                        } else if (pairs[i].name == "charset") {
                              font.charSet = pairs[i].data;
                        } else if (pairs[i].name == "unicode") {
                              font.unicode = parsed;
                        } else if (pairs[i].name == "stretchH") {
                              font.stretchH = parsed;
                        } else if (pairs[i].name == "smooth") {
                              font.smooth = parsed;
                        } else if (pairs[i].name == "aa") {
                              font.aa = parsed;
                        } else if (pairs[i].name == "padding") {
                              font.padding = new int4(pairs[i].data);
                        } else if (pairs[i].name == "spacing") {
                              font.spacing = new int2(pairs[i].data);
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
                              font.lineHeight = parsed;
                        } else if (pairs[i].name == "base") {
                              font.baseVal = parsed;
                        } else if (pairs[i].name == "scaleW") {
                              font.scaleW = parsed;
                        } else if (pairs[i].name == "scaleH") {
                              font.scaleH = parsed;
                        } else if (pairs[i].name == "pages") {
                              font.pages = parsed;
                        } else if (pairs[i].name == "packed") {
                              font.packed = parsed;
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
                              font.pageID = parsed;
                        } else if (pairs[i].name == "file") {
                              font.file = pairs[i].data;
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
                              fontChar.charID = parsed;
                        } else if (pairs[i].name == "x") {
                              fontChar.x = parsed;
                        } else if (pairs[i].name == "y") {
                              fontChar.y = parsed;
                        } else if (pairs[i].name == "width") {
                              fontChar.width = parsed;
                        } else if (pairs[i].name == "height") {
                              fontChar.height = parsed;
                        } else if (pairs[i].name == "xoffset") {
                              fontChar.xOffset = parsed;
                        } else if (pairs[i].name == "yoffset") {
                              fontChar.yOffset = parsed;
                        } else if (pairs[i].name == "xadvance") {
                              fontChar.xAdvance = parsed;
                        } else if (pairs[i].name == "page") {
                              fontChar.page = parsed;
                        } else if (pairs[i].name == "chnl") {
                              fontChar.channel = parsed;
                        } else if (pairs[i].name == "letter") {

                              if (pairs[i].data == "space") {
                                    fontChar.letter = ' ';
                              } else {
                                    fontChar.letter = pairs[i].data[0];
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
                              fontKerning.first = parsed;
                        } else if (pairs[i].name == "second") {
                              fontKerning.second = parsed;
                        } else if (pairs[i].name == "amount") {
                              fontKerning.amount = parsed;
                        }
                  }
                  return fontKerning;
            }

            public static string SerialiseTextFile(Font font) {
                  int arrayLength = 4 + font.chars.Length + 1 + font.kernings.Length;

                  string[] serialised = new string[arrayLength];
                  serialised[0] = SerialiseInfoLine(font);
                  serialised[1] = SerialiseCommonLine(font);
                  serialised[2] = SerialisePageLine(font);
                  serialised[3] = "chars count=" + font.chars.Length;

                  int line = 4;
                  for (int i = 0; line < arrayLength && i < font.chars.Length; line++, i++) {
                        serialised[line] = SerialiseFontChar(font.chars[i]);
                  }
                  serialised[line++] = "kernings count=" + font.kernings.Length;

                  for (int i = 0; line < arrayLength && i < font.kernings.Length; line++, i++) {
                        serialised[line] = SerialiseFontKerning(font.kernings[i]);
                  }

                  return string.Join("\n", serialised);
            }

            static string SerialiseInfoLine(Font font) {
                  return "info face=\"" + font.infoFace + "\"" +
                        " size=" + font.size +
                        " bold=" + font.bold +
                        " italic=" + font.italic +
                        " charset=\"" + font.charSet + "\"" +
                        " unicode=" + font.unicode +
                        " stretchH=" + font.stretchH +
                        " smooth=" + font.smooth +
                        " aa=" + font.aa +
                        " padding=" + font.padding.x + "," + font.padding.y + "," + font.padding.z + "," + font.padding.w +
                        " spacing=" + font.spacing.x + "," + font.spacing.y;
            }

            static string SerialiseCommonLine(Font font) {
                  return "common lineHeight=" + font.lineHeight +
                        " base=" + font.baseVal +
                        " scaleW=" + font.scaleW +
                        " scaleH=" + font.scaleH +
                        " pages=" + font.pages +
                        " packed=" + font.packed;
            }

            static string SerialisePageLine(Font font) {
                  return "page id=" + font.pageID +
                        " file=\"" + font.file + "\"";
            }

            public static string SerialiseFontChar(FontChar fontChar) {
                  return "char id=" + fontChar.charID +
                        " x=" + fontChar.x +
                        " y=" + fontChar.y +
                        " width=" + fontChar.width +
                        " height=" + fontChar.height +
                        " xoffset=" + fontChar.xOffset +
                        " yoffset=" + fontChar.yOffset +
                        " xadvance=" + fontChar.xAdvance +
                        " page=" + fontChar.page +
                        " chnl=" + fontChar.channel +
                        " letter=\"" + (fontChar.letter == ' ' ? "space" : fontChar.letter.ToString()) + "\"";
            }

            public static string SerialiseFontKerning(FontKerning fontKerning) {
                  return "kerning first=" + fontKerning.first +
                        " second=" + fontKerning.second +
                        " amount=" + fontKerning.amount;
            }
      }
}
