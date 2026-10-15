using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_Randomiser {

      public static class DebugOptions {

            public static void WriteFontFileDataToDisk(List<Font> fonts, string rootDirectory)
                  => WriteFontFileDataToDisk(fonts.ToArray(), rootDirectory);

            public static void WriteFontFileDataToDisk(Font[] fonts, string rootDirectory) {

                  Directory.CreateDirectory(rootDirectory);

                  for (int fontID = 0; fontID < fonts.Length; fontID++) {

                        string fontPath = Path.Combine(rootDirectory, fonts[fontID].Name);
                        Directory.CreateDirectory(fontPath);

                        for (int charID = 0; charID < fonts[fontID].Chars.Length; charID++) {
                              string charPath = Path.Combine(fontPath, fonts[fontID].Chars[charID].CharID + ".png");

                              fonts[fontID].Chars[charID].Texture.Save(charPath);
                        }
                  }
            }
      }
}
