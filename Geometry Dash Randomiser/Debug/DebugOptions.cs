using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_Randomiser {

      internal static class DebugOptions {

            internal static void WriteFontCharacterFilesToDisk(Font[] fonts) {
                  Directory.CreateDirectory(DebugFontCharacterFolderPath);

                  for (int f = 0; f < fonts.Length; f++) {
                        for (int c = 0; c < fonts[f].Chars.Length; c++) {
                              if (fonts[f].Chars[c].Texture != null) {
                                    fonts[f].Chars[c].Texture.Save(Path.Combine(DebugFontCharacterFolderPath, fonts[f].Name + " - " + fonts[f].Chars[c].CharID + ".png"));

                              } else {
                                    Logger.Error($"Character {fonts[f].Chars[c].CharID} in Font {fonts[f].Name} has a null texture");
                              }
                        }
                  }
            }

            private static string DebugFontCharacterFolderPath = Path.Combine("Debug", "Font", "Characters");
      }
}
