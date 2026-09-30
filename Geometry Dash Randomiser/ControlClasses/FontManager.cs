using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using static Geometry_Dash_Randomiser.FontStyleShuffleSettings;

namespace Geometry_Dash_Randomiser {

      public class FontManager {

            private GameFileManager gameFileManager;
            private RandomisationConfig config = RandomisationConfig.Instance;

            public FontManager(GameFileManager creator) {
                  gameFileManager = creator;
            }

            // Ideas:
            // Add a field for what percentage of characters should be randomised. Int 0 -> 100
            // AlterCharacterScale
            // RandomiseCharacterScale
            // RealignCharacters
            // FixCharacterSpacing
            // AllowDuplicates

            string[] fontFileNames = Array.Empty<string>();
            Font[] fonts = Array.Empty<Font>();

            public string[] FontFileNames => this.fontFileNames;
            public int fontCount => fonts.Length;
            public Font[] Fonts => fonts;

            public void ReadAllFontFiles(string path, Quality quality) {
                  if (fontFileNames.Length != 0 || fonts.Length != 0) {
                        return;
                  }

                  fontFileNames = GetAllFileNames(path, quality);
                  fonts = new Font[fontFileNames.Length];

                  Log.Write(Log.Mode.Info, $"Unpacking {fontFileNames.Length} font files from {PathManager.GetPath(GDR_Path.BackupResourcesFolder)}");

                  gameFileManager.progressState.NewFileBatch(fontFileNames.Length);

                  string outputPath = PathManager.BackupResourcesFolder;

                  for (int i = 0; i < fontFileNames.Length; i++) {
                        gameFileManager.progressState.CurrentFile = fontFileNames[i];

                        string textFilePath = Path.Combine(outputPath, fontFileNames[i] + ".fnt");
                        string gamesheetFilePath = Path.Combine(outputPath, fontFileNames[i] + ".png");

                        // If both files exist, read and unpack them
                        if (File.Exists(textFilePath) && File.Exists(gamesheetFilePath)) {
                              // Deserialise all the fonts from text files
                              string[] fileData = File.ReadAllLines(textFilePath);
                              fontFileNames[i] = fontFileNames[i];
                              fonts[i] = Font.Deserialise(fileData);

                              // Extract and crop all characters from all fontsheets
                              Rectangle[] cropRects = fonts[i].GetCharRects();
                              Bitmap fontsheet = new Bitmap(gamesheetFilePath);
                              Bitmap[] croppedChars = fontsheet.Multicrop(cropRects);

                              for (int j = 0; j < croppedChars.Length; j++) {
                                    fonts[i].Chars[j].Texture = croppedChars[j].GetUniqueClone();
                              }

                              fontsheet.Dispose();
                              croppedChars.Dispose();

                        } else {
                              string missingFile;
                              if (File.Exists(textFilePath) == false) {
                                    missingFile = textFilePath;
                              } else {
                                    missingFile = gamesheetFilePath;
                              }
                              Log.Write(Log.Mode.Warn, $"The file \"{missingFile}\" could not be located in the local backup resources folder");
                        }
                        gameFileManager.progressState.CompletedFiles++;
                  }
            }

            public Font[] RandomiseFiles(int seed) => RandomiseFiles(new Random(seed));

            public Font[] RandomiseFiles(Random random = null) {
                  if (config.FontRand.RandomisationNeeded == false) {
                        return fonts.ToArray();
                  }

                  if (random == null)
                        new Random(Guid.NewGuid().GetHashCode());

                  // Create new array for the randomised fonts
                  Font[] newFonts = Array.Empty<Font>();

                  if (config.FontRand.ShuffleSettings.Enabled) {
                        newFonts = GetNewFontsAfterShufflingFontStyles(random);

                  } else {
                        newFonts = new Font[fonts.Length];
                        for (int i = 0; i < fonts.Length; i++) {
                              newFonts[i] = fonts[i].DeepCopy();
                        }
                  }

                  // Shuffle the chars around within all fonts separately if ShuffleLetters is found
                  if (config.FontRand.CharacterRandSettings.Enabled) {
                        for (int i = 0; i < newFonts.Length; i++) {
                              ReorderCharIDsInFont(ref newFonts[i], random);
                        }
                  }

                  // Finally rearrange the boxes for where the chars will go on the assembled gamesheet
                  RepackFonts(ref newFonts);

                  return newFonts;
            }

            private Font[] GetNewFontsAfterShufflingFontStyles(Random random) {
                  Font[] newFonts = new Font[fonts.Length];

                  for (int i = 0; i < fonts.Length; i++) {
                        newFonts[i] = this.fonts[i].PartialCopy();

                        // Set 'chars' array size, but kernings remains empty for now
                        newFonts[i].Chars = new FontChar[this.fonts[i].Chars.Length];
                        newFonts[i].Kernings = Array.Empty<FontKerning>();
                  }

                  int[] allCharIDs = GetAllDistinctCharIDs();

                  if (config.FontRand.ShuffleSettings.Mode == ShufflingMode.PerLetter) {
                        // Contains how many chars have been added to each randomised font
                        int[] addedCharsCount = new int[fonts.Length];

                        // Iterate through every charID that exists in all font files
                        for (int i = 0; i < allCharIDs.Length; i++) {
                              int currentCharID = allCharIDs[i];

                              // Get all fontIDs that contain the charID we are looking for
                              int[] fontIDs = fonts
                                    .Select((font, index) => new { font, index })
                                    .Where(pair => pair.font.HasCharID(currentCharID))
                                    .Select(pair => pair.index)
                                    .ToArray();

                              int[] newCharacterOrder = random.GetShuffledIntRange(fontIDs.Length).ToArray();

                              for (int j = 0; j < fontIDs.Length; j++) {
                                    int oldFontIndex = fontIDs[j];
                                    int charPosition = fonts[oldFontIndex].GetCharPositionInArray(currentCharID);

                                    // Which font in the array we are adding the character to
                                    int newFontIndex = fontIDs[newCharacterOrder[j]];
                                    int fontAddedCharsCount = addedCharsCount[newFontIndex];

                                    // Add the character and the bitmap clone
                                    newFonts[newFontIndex].Chars[fontAddedCharsCount] = fonts[oldFontIndex].Chars[charPosition].DeepCopy();

                                    // Signal that one character was added to the font
                                    addedCharsCount[newFontIndex]++;
                              }
                        }

                  } else {
                        // If there is no per-character shuffling
                        int[] newFontOrder = random.GetShuffledIntRange(fonts.Length).ToArray();

                        for (int i = 0; i < this.fonts.Length; i++) {
                              Font newFont = newFonts[i];
                              int newFontID = newFontOrder[i];
                              Font newFontStyle = this.fonts[newFontID];

                              // If the ID matches, copy the font, randomisation will achieve nothing else
                              if (i == newFontID) {
                                    newFonts[i] = this.fonts[i].DeepCopy();

                              } else {
                                    for (int ch = 0; ch < newFont.Chars.Length; ch++) {
                                          int targetCharID = this.fonts[i].Chars[ch].CharID;
                                          FontChar newFontChar = newFontStyle.GetChar(targetCharID);

                                          if (newFontChar != null) {
                                                newFont.Chars[ch] = newFontChar.DeepCopy();

                                          } else {
                                                newFont.Chars[ch] = this.fonts[i].GetChar(targetCharID).DeepCopy();
                                          }
                                    }
                              }
                        }
                  }
                  return newFonts;
            }

            void ReorderCharIDsInFont(ref Font font, Random random) {
                  int[] newCharIDs = font.GetCharIDs();
                  random.Shuffle(newCharIDs);

                  for (int i = 0; i < font.Chars.Length; i++) {
                        font.Chars[i].CharID = newCharIDs[i];
                  }
            }

            public static void RepackFonts(ref Font[] fonts) {
                  for (int i = 0; i < fonts.Length; i++) {
                        fonts[i].Repack();
                  }
            }

            public void WriteFontsToDisk(Font[] fonts) {
                  WriteFontsToDisk(GDR_Path.LocalResourcesOutputFolder, fonts);
            }

            public void WriteFontsToDisk(GDR_Path path, Font[] fonts) => WriteFontsToDisk(PathManager.GetPath(path), fonts);

            public void WriteFontsToDisk(string path, Font[] fonts) {
                  gameFileManager.progressState.NewFileBatch(fonts.Length);

                  for (int i = 0; i < fonts.Length; i++) {
                        gameFileManager.progressState.CurrentFile = fontFileNames[i];

                        string textFileName = Path.Combine(path, fontFileNames[i] + ".fnt");
                        string gamesheetFileName = Path.Combine(path, fontFileNames[i] + ".png");

                        File.WriteAllText(textFileName, fonts[i].Serialise());
                        Bitmap gamesheet = fonts[i].AssembleGamesheet();
                        gamesheet.Save(gamesheetFileName);

                        gameFileManager.progressState.CompletedFiles++;
                  }
            }

            public int[] GetAllDistinctCharIDs() {
                  List<int> allCharIDs = new List<int>();
                  for (int i = 0; i < fonts.Length; i++) {
                        allCharIDs.AddRange(fonts[i].GetCharIDs());
                  }
                  allCharIDs = allCharIDs.Distinct().ToList();
                  allCharIDs.Sort();
                  return allCharIDs.ToArray();
            }

            public static void Dispose(ref Font font) {
                  for (int ch = 0; ch < font.Chars.Length; ch++) {
                        font.Chars[ch].Texture.Dispose();
                  }
                  font = null;
            }

            public static string[] SerialiseTextFiles(Font[] fonts) {
                  string[] ret = new string[fonts.Length];
                  for (int i = 0; i < fonts.Length; i++) {
                        ret[i] = SerialiseTextFile(fonts[i]);
                  }
                  return ret;
            }

            public static string SerialiseTextFile(Font font) {
                  return font.Serialise();
            }

            public static string[] GetAllFileNames(GDR_Path source, Quality quality) {
                  return GetAllFileNames(PathManager.GetPath(source), quality);
            }

            public static string[] GetAllFileNames(string path, Quality quality) {
                  return Directory.GetFiles(path)
                        .Where(f => Path.GetExtension(f) == ".fnt")
                        .FilterFilesByQuality(quality)
                        .Where(f => File.Exists(f + ".png") && File.Exists(f + ".fnt"))
                        .Select(f => Path.GetFileName(f))
                        .ToArray();
            }
      }
}
