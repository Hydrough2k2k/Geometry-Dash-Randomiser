using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using static Geometry_Dash_Randomiser.FontRandomisationSettings;

namespace Geometry_Dash_Randomiser {

      public class FontManager {

<<<<<<< Updated upstream
            private GameFileManager gameFileManager;
=======
            private GameFileManager _gameFileManager;
            private RandomisationConfig _config = RandomisationConfig.Instance;

            private List<Font> _fonts = new List<Font>();
>>>>>>> Stashed changes

            public FontManager(GameFileManager creator) {
                  _gameFileManager = creator;
            }

<<<<<<< Updated upstream
            // Ideas: add a field for what percentage of characters should be randomised. Int 0 -> 100

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
=======
            // Ideas:
            // Add a field for what percentage of characters should be randomised. Int 0 -> 100
            // AlterCharacterScale
            // RandomiseCharacterScale
            // RealignCharacters
            // FixCharacterSpacing
            // AllowDuplicates
            // Allow negative kerning and Xoffset setting for simple random letter spacing
>>>>>>> Stashed changes

            public int fontCount => _fonts.Count;
            public List<Font> Fonts => _fonts;

            public void ReadAllFontFiles(string path, Quality quality) {
                  if (_fonts.Count != 0) {
                        Logger.Write(Logger.Mode.Debug, "The font files have already been read, skipping this step.");
                        return;
                  }

                  List<string> fontFileNames = GetAllFileNames(path, quality);

                  Logger.Write(Logger.Mode.Info, $"Unpacking {fontFileNames.Count} font files from {PathManager.GetPath(GDR_Path.BackupResourcesFolder)}");

                  _gameFileManager.progressState.NewFileBatch(fontFileNames.Count);

                  string outputPath = PathManager.BackupResourcesFolder;

                  for (int i = 0; i < fontFileNames.Count; i++) {
                        _gameFileManager.progressState.CurrentFile = fontFileNames[i];

                        // Get the full fine path names for the text file and the gamesheet file
                        string textFilePath = Path.Combine(outputPath, fontFileNames[i] + ".fnt");
                        string gamesheetFilePath = Path.Combine(outputPath, fontFileNames[i] + ".png");

                        // Check and store whether the files exist
                        bool foundTextFile = File.Exists(textFilePath);
                        bool foundGamesheetFile = File.Exists(gamesheetFilePath);

                        // If both files exist, read and unpack them
                        if (foundTextFile && foundGamesheetFile) {
                              // Deserialise all the fonts from text files
                              string[] fileData = File.ReadAllLines(textFilePath);

                              // Create a new Font object from the deserialised data
                              Font newFont = Font.Deserialise(fileData);

                              // Extract and crop all characters from all fontsheets
                              Rectangle[] cropRects = newFont.GetCharRects();
                              Bitmap fontsheet = new Bitmap(gamesheetFilePath);
                              Bitmap[] croppedChars = fontsheet.Multicrop(cropRects);

                              for (int j = 0; j < croppedChars.Length; j++) {
<<<<<<< Updated upstream
                                    fonts[i].chars[j].texture = croppedChars[j].GetClone();
=======
                                    // Unique cloning might not be necessary. Just keeping it here in case it would break things
                                    newFont.Chars[j].Texture = croppedChars[j];
>>>>>>> Stashed changes
                              }

                              // Clear the bitmap from memory to prevent memory leaks
                              fontsheet.Dispose();

                              newFont.Name = fontFileNames[i];

                              // Add it to the list
                              _fonts.Add(newFont);

                        } else {
                              // Log the missing file(s)
                              if (foundTextFile == false) {
                                    Logger.Write(Logger.Mode.Warn, $"The file \"{textFilePath}\" could not be located in the local backup resources folder");
                              }
                              if (foundGamesheetFile == false) {
                                    Logger.Write(Logger.Mode.Warn, $"The file \"{gamesheetFilePath}\" could not be located in the local backup resources folder");
                              }

                              // Finally remove the font name from the list, since it is incomplete
                              fontFileNames.RemoveAt(i);
                              // We need to decrement 'i' since we just removed an item from the list
                              i--;
                        }
                        _gameFileManager.progressState.CompletedFiles++;
                  }
            }

            public RandomisationMode GetRandomisationMode() {
                  RandomisationMode mode = RandomisationMode.None;

<<<<<<< Updated upstream
                  if (Config.Instance.fontRand.enabled == false) {
                        return mode;
=======
            public Font[] RandomiseFiles(Random random = null) {
                  if (_config.FontRand.Enabled == false) {
                        return _fonts.ToArray();
>>>>>>> Stashed changes
                  }
                  if (Config.Instance.fontRand.shuffleFontStyles == true) {
                        mode |= RandomisationMode.ShuffleFontStyles;
                  }
                  if (Config.Instance.fontRand.shufflingMode == FontStyleShufflingMode.PerLetter) {
                        mode |= RandomisationMode.PerCharacterStyleShuffling;
                  }
                  if (Config.Instance.fontRand.randomiseLetters == true) {
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
                  Font[] newFonts = GetFontFileCopies(random);

<<<<<<< Updated upstream
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
=======
                  if (_config.FontRand.CharacterShuffleSettings.Enabled) {
                        ShuffleCharactersInFonts(random, in newFonts);
                  }

                  if (_config.FontRand.LetterSpacingSettings.Enabled) {
                        AlterLetterSpacing(random, in newFonts);
>>>>>>> Stashed changes
                  }

                  // Finally rearrange the boxes for where the chars will go on the assembled gamesheet
                  RepackFonts(ref newFonts);

                  return newFonts;
            }

<<<<<<< Updated upstream
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
=======
            /// <summary>
            /// Get copies of the original font files, either shuffled or not depending on the config settings
            /// </summary>
            private Font[] GetFontFileCopies(Random random) {
                  if (_config.FontRand.StyleShuffleSettings.Enabled) {
                        return ShuffleFontStyles(random);
>>>>>>> Stashed changes

                  } else {
                        // Just straight up copy all of the files
                        return DeepCopyFonts();
                  }
            }

            private Font[] DeepCopyFonts() {
                  return _fonts.Select(f => f.DeepCopy()).ToArray();
            }

            private void ShuffleCharactersInFonts(Random random, in Font[] fonts) {
                  for (int i = 0; i < fonts.Length; i++) {
                        fonts[i].ShuffleLetters(random);
                  }
            }

<<<<<<< Updated upstream
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
=======
            private Font[] ShuffleFontStyles(Random random) {
                  switch (_config.FontRand.StyleShuffleSettings.Mode) {
                        case ShufflingMode.PerLetter:
                              return ShuffleFontStlesPerLetter(random).ToArray();

                        case ShufflingMode.PerFont:
                              return ShuffleFontStylesPerFont(random).ToArray();

                        default:
                              Logger.Write(Logger.Mode.Error, $"Unrecognised font style shuffling mode was received: \"{_config.FontRand.StyleShuffleSettings.Mode}\". Cloning fonts");
                              return DeepCopyFonts();
                  }
            }

            private List<Font> ShuffleFontStlesPerLetter(Random random) {
                  // Create a partial copy of all fonts
                  List<Font> newFonts = _fonts.Select(f => f.DeepCopy()).ToList();

                  // Clear the 'chars' arrays in the new fonts, since we will be adding them back in a randomised order
                  newFonts.ForEach(f => f.Chars = new FontChar[f.Chars.Length]);

                  // Keep track of how many chartacters have been added to each font thus far
                  int[] addedCharsCount = new int[_fonts.Count];

                  // Get all character IDs that exist in all font files
                  int[] allCharIDs = GetAllDistinctCharIDs();

                  // Iterate through every charID that exists in all font files
                  for (int charID = 0; charID < allCharIDs.Length; charID++) {
                        int currentCharID = allCharIDs[charID];

                        // Get all fontIDs that contain the charID we are looking for
                        int[] fontIDs = _fonts
                              .Select((font, index) => new { font, index })
                              .Where(pair => pair.font.HasCharID(currentCharID))
                              .Select(pair => pair.index)
                              .ToArray();

                        // Create a copy of the array
                        int[] newCharacterOrder = fontIDs.Select(v => v).ToArray();

                        // Shuffle the order of the fonts that contain the character
                        random.Shuffle(newCharacterOrder);

                        for (int fontID = 0; fontID < fontIDs.Length; fontID++) {
                              int originalFontIndex = fontIDs[fontID];
                              int newFontIndex = newCharacterOrder[fontID];
                              int addedCharacters = addedCharsCount[newFontIndex];

                              // Get a clone of the character from the original font
                              FontChar originalChar = _fonts[originalFontIndex].GetChar(currentCharID).DeepCopy();

                              // Add this character to this font
                              newFonts[newFontIndex].Chars[addedCharacters] = originalChar;

                              // Signal that one character was added to the font
                              addedCharsCount[newFontIndex]++;
>>>>>>> Stashed changes
                        }
                  }

                  return newFonts;
            }

            private List<Font> ShuffleFontStylesPerFont(Random random) {
                  // Create a deep copy of all fonts
                  List<Font> newFonts = _fonts.Select(f => f.PartialCopy()).ToList();

<<<<<<< Updated upstream
                  for (int i = 0; i < font.chars.Length; i++) {
                        font.chars[i].charID = newCharIDs[i];
=======
                  // Generate an order in which the fonts will be shuffled
                  int[] newFontOrder = random.GetShuffledIntRange(_fonts.Count).ToArray();

                  for (int fontID = 0; fontID < _fonts.Count; fontID++) {
                        int newFontID = newFontOrder[fontID];

                        Font originalFontRef = _fonts[fontID];
                        Font newFontRef = _fonts[newFontID];

                        int[] charIDs = originalFontRef.Chars.Select(c => c.CharID).ToArray();

                        List<FontChar> newCharList = new List<FontChar>();

                        for (int charID = 0; charID < charIDs.Length; charID++) {
                              // Try and get the char from the new font
                              FontChar @char = newFontRef.GetChar(charIDs[charID], out bool found);

                              if (found) {
                                    // Add it if it does exist
                                    newCharList.Add(@char.DeepCopy());

                              } else {
                                    // If it doesn't, copy it from the original font instead. This is important, otherwise GD will shit itself
                                    newCharList.Add(originalFontRef.GetChar(charIDs[charID]).DeepCopy());
                              }
                        }

                        newFonts[fontID].Chars = newCharList.ToArray();

                        // Get the kernings from the font we are copying to keep it consistent with the new font style
                        newFonts[fontID].Kernings = newFontRef.Kernings.Select(k => k.DeepCopy()).ToArray();
                  }

                  return newFonts;
            }

            private void AlterLetterSpacing(Random random, in Font[] fonts) {
                  switch (_config.FontRand.LetterSpacingSettings.Mode) {
                        case LetterSpacingSettings.SpacingMode.Simple:
                              AlterLetterSpacing_Simple(random, in fonts);
                              break;
                        case LetterSpacingSettings.SpacingMode.Advanced:
                              AlterLetterSpacing_Advanced(random, in fonts);
                              break;
                        default:
                              Logger.Write(Logger.Mode.Error, $"Unrecognised letter spacing mode was received: \"{_config.FontRand.LetterSpacingSettings.Mode}\". Skipping step");
                              break;
                  }
            }

            private void AlterLetterSpacing_Simple(Random random, in Font[] fonts) {
                  Font.LetterSpacing ls = Font.LetterSpacing.Get(_config.FontRand.LetterSpacingSettings.Simple.Level);
                  AlterLetterSpacing(random, in fonts, ls);
            }

            private void AlterLetterSpacing_Advanced(Random random, in Font[] fonts) {
                  Font.LetterSpacing ls = _config.FontRand.LetterSpacingSettings.GetAdvancedLetterSpacing();
                  AlterLetterSpacing(random, in fonts, ls);
            }

            private void AlterLetterSpacing(Random random, in Font[] fonts, Font.LetterSpacing ls) {
                  for (int i = 0; i < fonts.Length; i++) {
                        fonts[i].AlterLetterSpacing(random, ls);
>>>>>>> Stashed changes
                  }
            }

            public static void RepackFonts(ref Font[] fonts) {
                  for (int i = 0; i < fonts.Length; i++) {
                        fonts[i].Repack();
                  }
            }

            public void WriteFontsToDisk(Font[] fonts) => WriteFontsToDisk(GDR_Path.LocalResourcesOutputFolder, fonts);

            public void WriteFontsToDisk(GDR_Path path, Font[] fonts) => WriteFontsToDisk(PathManager.GetPath(path), fonts);

            public void WriteFontsToDisk(string path, Font[] fonts) {
                  _gameFileManager.progressState.NewFileBatch(fonts.Length);

                  if (Directory.Exists(path) == false) {
                        Directory.CreateDirectory(path);
                  }

                  for (int i = 0; i < fonts.Length; i++) {
                        _gameFileManager.progressState.CurrentFile = _fonts[i].Name;

                        string textFileName = Path.Combine(path, _fonts[i].Name + ".fnt");
                        string gamesheetFileName = Path.Combine(path, _fonts[i].Name + ".png");

                        File.WriteAllText(textFileName, fonts[i].Serialise());
                        Bitmap gamesheet = fonts[i].AssembleGamesheet();
                        gamesheet.Save(gamesheetFileName);

                        _gameFileManager.progressState.CompletedFiles++;
                  }
            }

<<<<<<< Updated upstream
            public static void Dispose(ref Font font) {
                  for (int ch = 0; ch < font.chars.Length; ch++) {
                        font.chars[ch].texture.Dispose();
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
                  return GetAllFileNames(PathManager.GetPath(source), quality);
            }

            public string[] GetAllFileNames(string path, Quality quality) {
=======
            public int[] GetAllDistinctCharIDs() {
                  HashSet<int> distinctCharIDs = new HashSet<int>();
                  for (int i = 0; i < _fonts.Count; i++) {
                        for (int j = 0; j < _fonts[i].Chars.Length; j++) {
                              distinctCharIDs.Add(_fonts[i].Chars[j].CharID);
                        }
                  }

                  return distinctCharIDs.ToArray();
            }

            public static List<string> GetAllFileNames(GDR_Path source, Quality quality) {
                  return GetAllFileNames(PathManager.GetPath(source), quality);
            }

            public static List<string> GetAllFileNames(string path, Quality quality) {
>>>>>>> Stashed changes
                  return Directory.GetFiles(path)
                        .Where(f => Path.GetExtension(f) == ".fnt")
                        .FilterFilesByQuality(quality)
                        .Where(f => File.Exists(f + ".png") && File.Exists(f + ".fnt"))
                        .Select(f => Path.GetFileName(f))
                        .ToList();
            }
      }
}
