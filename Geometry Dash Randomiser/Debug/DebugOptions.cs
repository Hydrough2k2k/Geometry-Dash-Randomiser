using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_Randomiser {

      internal static class DebugOptions {

            internal enum GameSheetImageSorting {
                  Unsorted,
                  ByGameSheet,
                  ByType
            }

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

            internal static void WriteGameSheetImagesToDisk(GameSheet[] gameSheets)
                  => WriteGameSheetImagesToDisk(gameSheets, GameSheetImageSorting.ByGameSheet);

            internal static void WriteGameSheetImagesToDisk(GameSheet[] gameSheets, GameSheetImageSorting sorting) {
                  switch (sorting) {
                        case GameSheetImageSorting.Unsorted:
                              //WriteGameSheetImagesToDiskUnsorted(gameSheets);
                              return;

                        case GameSheetImageSorting.ByGameSheet:
                              WriteGameSheetImagesToDiskByGameSheet(gameSheets);
                              return;

                        case GameSheetImageSorting.ByType:
                              //WriteGameSheetImagesToDiskByType(gameSheets);
                              return;
                  }
            }

            internal static void WriteGameSheetImagesToDiskByGameSheet(GameSheet[] gameSheets) {
                  Directory.CreateDirectory(DebugGameSheetImageFolderPath);

                  for (int i = 0; i < gameSheets.Length; i++) {
                        GameSheet gs = gameSheets[i];
                        string folderPath = Path.Combine(DebugGameSheetImageFolderPath, gs.FileName);
                        Directory.CreateDirectory(folderPath);

                        for (int j = 0; j < gs.Sprites.Length; j++) {
                              gs.Sprites[j].Texture.Save(Path.Combine(folderPath, gs.Sprites[j].Name + ".png"));
                        }
                  }
            }

            private static string DebugGameSheetImageFolderPath = Path.Combine("Debug", "GameSheet", "Sprites");
            private static string DebugFontCharacterFolderPath = Path.Combine("Debug", "Font", "Characters");
      }
}
