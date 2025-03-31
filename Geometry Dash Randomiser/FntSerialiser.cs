using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Geometry_Dash_Randomiser {

      internal static class FntSerialiser {

            public static Font Deserialise(string path) {
                  string[] lines = File.ReadAllLines(path);

                  return Deserialise(lines);
            }

            public static Font Deserialise(string[] fileStream) {
                  Font font = new Font();
                  List<FontChar> chars = new List<FontChar>();
                  List<FontKerning> kernings = new List<FontKerning>();

                  for (int i = 0; i < fileStream.Length; i++) {
                        string line = fileStream[i];
                        Font.PropertyPair[] pairs = GetPropertyPairs(line);

                        if (line.StartsWith("info ")) {
                              font.ApplyInfoData(pairs);

                        } else if (line.StartsWith("common ")) {
                              font.ApplyCommonData(pairs);

                        } else if (line.StartsWith("page ")) {
                              font.ApplyPageData(pairs);

                        } else if (line.StartsWith("chars ")) {
                              font.charsCount = Int32.Parse(pairs[0].data);

                        } else if (line.StartsWith("char ")) {
                              chars.Add(new FontChar(pairs));

                        } else if (line.StartsWith("kernings ")) {
                              font.kerningsCount = Int32.Parse(pairs[0].data);

                        } else if (line.StartsWith("kerning ")) {
                              kernings.Add(new FontKerning(pairs));
                        }
                  }

                  font.chars = chars.ToArray();
                  font.kernings = kernings.ToArray();                  
                  return font;
            }

            static Font.PropertyPair[] GetPropertyPairs(string str) {

                  // Get all indexes of where equals signs appear in the string
                  List<int> foundIndexes = new List<int>();
                  for (int i = str.IndexOf('='); i > -1; i = str.IndexOf('=', i + 1)) {
                        if (str[i - 1] == '"' && str[i + 1] == '"') {
                              // If both chars on either side of the equals are ", don't add them
                              // This prevents equals symbol in quotation marks causing errors when it's a character in the font sheet
                        } else {
                              foundIndexes.Add(i);
                        }
                  }
                  Font.PropertyPair[] pairs = new Font.PropertyPair[foundIndexes.Count];

                  for (int i = 0; i < foundIndexes.Count; i++) {

                        // Search for the previous equals
                        if (i == 0) {
                              // If there is no previous one, search from the start of the string then crop until the equals sign
                              int index = str.IndexOf(' ');
                              pairs[i].name = str.Substring(index, foundIndexes[i] - index).Trim();
                        } else {
                              // Search for the next space from the previous equals sign
                              int index = str.IndexOf(' ', foundIndexes[i - 1]);
                              pairs[i].name = str.Substring(index, foundIndexes[i] - index).Trim();
                        }

                        // Find the next space after the equals symbol
                        int index2 = str.IndexOf(' ', foundIndexes[i]);
                        if (index2 == -1) {
                              // If none were found trim until the end of the string
                              pairs[i].data = str.Substring(foundIndexes[i] + 1).Trim();
                        } else {
                              pairs[i].data = str.Substring(foundIndexes[i] + 1, index2 - foundIndexes[i]).Trim();
                        }

                        // If the string starts and ends with quotes remove them
                        if (pairs[i].data.StartsWith("\"") && pairs[i].data.EndsWith("\"")) {
                              pairs[i].data = pairs[i].data.Substring(1, pairs[i].data.Length - 2);
                        }
                  }
                  return pairs;
            }
      }
}
