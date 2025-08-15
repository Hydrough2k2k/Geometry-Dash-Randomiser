using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using static Geometry_Dash_Randomiser.GameFileManager;
using static Geometry_Dash_Randomiser.GameFilesExtension;
using static Geometry_Dash_Randomiser.PathManager;
using static Geometry_Dash_Randomiser.FontRandomisationSettings;

namespace Geometry_Dash_Randomiser {

      public class FontManager {

            private GameFileManager gameFileManager;
            private PathManager pathManager;

            public FontManager(GameFileManager creator, PathManager pathManager) {
                  gameFileManager = creator;
                  this.pathManager = pathManager;
            }

            public enum RandomisationMode {
                  None = 0,

                  /// <summary> Shuffle letters around within the same font, swap letter 'a' and 'c' and their parameters for example </summary>
                  ShuffleLetters = 1,

                  /// <summary> Shuffle textures around between fonts, making the letter 'a' and 'c' sprites come from different fonts for example </summary>
                  ShuffleFontStyles = 2,

                  /// <summary> Denotes whether the textures for a given font all come from the same font, or if every character is replaced from a different font </summary>
                  PerCharacterStyleShuffling = 4,

                  // Unimplemented
                  AlterCharacterScale = 8,

                  /// <summary>
                  /// If a small character is put into a big scale font (or vica versa) the character will be rescaled to match the other characters in size
                  /// Works only ShuffleFontsCharacters is enabled, since otherwise all characters come from the same font and no rescaling is necessary
                  /// </summary>
                  // Unimplemented
                  NormaliseCharacterScale = AlterCharacterScale,

                  /// <summary> When shuffling a font, the scale of characters will be randomly altered on the X and Y axes </summary>
                  // Unimplemented
                  RandomiseCharacterScale = 16,

                  /// <summary>
                  /// Some characters might be higher up compared to others. This will attempt to fix that
                  /// </summary>
                  // Unimplemented
                  RealignCharacters = 32,

                  /// <summary>
                  /// Some characters might be too close or too far. This will attempt to fix that and make spacing more consistent
                  /// </summary>
                  // Unimplemented
                  FixCharacterSpacing = 64,

                  /// <summary> When randomising characters, do we want the same characters appearing multiple times, and some not at all </summary>
                  // Unimplemented
                  AllowDuplicates = 128,

                  ShuffleEverything = ShuffleLetters | ShuffleFontStyles
            }

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

                  string outputPath = pathManager.backupResourcesFolder;

                  for (int i = 0; i < fontFileNames.Length; i++) {

                        Console.WriteLine(fontFileNames[i]);

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
                                    fonts[i].chars[j].texture = croppedChars[j].GetClone();
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
                              Console.WriteLine("\"{0}\" file could not be located in the local unaltered resources folder");
                        }
                  }
            }

            public RandomisationMode GetRandomisationMode() {
                  RandomisationMode mode = RandomisationMode.None;

                  if (Config.fontRand.enabled == false) {
                        return mode;
                  }
                  if (Config.fontRand.shuffleFontStyles == true) {
                        mode |= RandomisationMode.ShuffleFontStyles;
                  }
                  if (Config.fontRand.shufflingMode == FontStyleShufflingMode.PerLetter) {
                        mode |= RandomisationMode.PerCharacterStyleShuffling;
                  }
                  if (Config.fontRand.randomiseLetters == true) {
                        mode |= RandomisationMode.ShuffleLetters;
                  }

                  return mode;
            }

            public Font[] RandomiseFiles(RandomisationMode mode, int seed) => RandomiseFiles(mode, new Random(seed));

            public Font[] RandomiseFiles(RandomisationMode mode, Random random = null) {
                  if (mode == RandomisationMode.None)
                        return fonts.ToArray();

                  if (random == null)
                        new Random(Guid.NewGuid().GetHashCode());

                  // Create new array for the randomised fonts
                  Font[] newFonts = Array.Empty<Font>();

                  if (mode.HasFlag(RandomisationMode.ShuffleFontStyles)) {
                        newFonts = GetNewFontsAfterShufflingFontStyles(mode, random);

                  } else {
                        newFonts = new Font[fonts.Length];
                        for (int i = 0; i < fonts.Length; i++) {
                              newFonts[i] = fonts[i].DeepCopy();
                        }
                  }

                  // Shuffle the chars around within all fonts separately if ShuffleLetters is found
                  if (mode.HasFlag(RandomisationMode.ShuffleLetters)) {
                        for (int i = 0; i < newFonts.Length; i++) {
                              ReorderCharIDsInFont(ref newFonts[i], random);
                        }
                  }

                  // Finally rearrange the boxes for where the chars will go on the assembled gamesheet
                  RepackFonts(ref newFonts);

                  return newFonts;
            }

            Font[] GetNewFontsAfterShufflingFontStyles(RandomisationMode mode, Random random) {
                  Font[] newFonts = new Font[fonts.Length];

                  for (int i = 0; i < fonts.Length; i++) {
                        newFonts[i] = this.fonts[i].PartialCopy();

                        // Set 'chars' array size, but kernings remains empty for now
                        newFonts[i].chars = new FontChar[this.fonts[i].chars.Length];
                        newFonts[i].kernings = Array.Empty<FontKerning>();
                  }

                  int[] allCharIDs = GetAllDistinctCharIDs();

                  if (mode.HasFlag(RandomisationMode.PerCharacterStyleShuffling)) {
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
                                    newFonts[newFontIndex].chars[fontAddedCharsCount] = fonts[oldFontIndex].chars[charPosition].DeepCopy();

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
                                    for (int ch = 0; ch < newFont.chars.Length; ch++) {
                                          int targetCharID = this.fonts[i].chars[ch].charID;
                                          FontChar newFontChar = newFontStyle.GetChar(targetCharID);

                                          if (newFontChar != null) {
                                                newFont.chars[ch] = newFontChar.DeepCopy();

                                          } else {
                                                newFont.chars[ch] = this.fonts[i].GetChar(targetCharID).DeepCopy();
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

                  for (int i = 0; i < font.chars.Length; i++) {
                        font.chars[i].charID = newCharIDs[i];
                  }
            }

            public void RepackAllFonts() {
                  RepackFonts(ref this.fonts);
            }

            public static void RepackFonts(ref List<Font> fonts) {
                  for (int i = 0; i < fonts.Count; i++) {
                        fonts[i].Repack();
                  }
            }

            public static void RepackFonts(ref Font[] fonts) {
                  for (int i = 0; i < fonts.Length; i++) {
                        fonts[i].Repack();
                  }
            }

            public void WriteFontsToDisk(GDR_Path path, Font[] fonts) => WriteFontsToDisk(pathManager.GetPath(path), fonts);

            public void WriteFontsToDisk(string path, Font[] fonts) {

                  for (int i = 0; i < fonts.Length; i++) {
                        string textFileName = Path.Combine(path, fontFileNames[i] + ".fnt");
                        string gamesheetFileName = Path.Combine(path, fontFileNames[i] + ".png");

                        File.WriteAllText(textFileName, fonts[i].Serialise());
                        Bitmap gamesheet = fonts[i].AssembleGamesheet();
                        gamesheet.Save(gamesheetFileName);
                  }
            }

            public static void Dispose(Font[] fonts) {
                  for (int i = 0; i < fonts.Length; i++) {
                        Dispose(ref fonts[i]);
                  }
            }

            public static void Dispose(ref Font font) {
                  for (int ch = 0; ch < font.chars.Length; ch++) {
                        font.chars[ch].texture.Dispose();
                  }
                  font = null;
            }

            public string[] SerialiseAllTextFiles() {
                  return SerialiseTextFiles(this.fonts.ToArray());
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

            public Bitmap[] SetialiseAllGamesheets() {
                  Bitmap[] ret = new Bitmap[fonts.Length];
                  for (int i = 0; i < fonts.Length; i++) {
                        ret[i] = GameSheet.Assemble(fonts[i]);
                  }
                  return ret;
            }

            public static Bitmap[] SetialiseAllGamesheets(Font[] fonts) {
                  Bitmap[] ret = new Bitmap[fonts.Length];
                  for (int i = 0; i < fonts.Length; i++) {
                        ret[i] = GameSheet.Assemble(fonts[i]);
                  }
                  return ret;
            }

            public static Bitmap SerialiseGamesheet(Font font) {
                  return GameSheet.Assemble(font);
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

            public string[] GetAllFileNames(GDR_Path source, Quality quality) {
                  return GetAllFileNames(pathManager.GetPath(source), quality);
            }

            public string[] GetAllFileNames(string path, Quality quality) {
                  return Directory.GetFiles(path)
                        .Where(f => Path.GetExtension(f) == ".fnt")
                        .FilterFilesByQuality(quality)
                        .Where(f => File.Exists(f + ".png") && File.Exists(f + ".fnt"))
                        .Select(f => Path.GetFileName(f))
                        .ToArray();
            }
      }
}
