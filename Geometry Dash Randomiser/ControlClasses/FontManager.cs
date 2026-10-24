using Microsoft.WindowsAPICodePack.Dialogs.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using static Geometry_Dash_Randomiser.FontStyleShuffleSettings;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using static System.Net.Mime.MediaTypeNames;

namespace Geometry_Dash_Randomiser {

      public class FontManager {

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

            public int fontCount => _fonts.Length;
            public Font[] Fonts => _fonts;

            public void ReadAllFontFiles(string path, Quality quality) {
                  if (_fonts.Length != 0) {
                        return;
                  }

                  List<string> fontFileNames = GetAllFileNames(path, quality).ToList();
                  List<Font> newFontsList = new List<Font>();

                  Logger.Write(Logger.Mode.Info, $"Unpacking {fontFileNames.Count} font files from {PathManager.GetPath(GDR_Path.BackupResourcesFolder)}");

                  gameFileManager.progressState.NewFileBatch(fontFileNames.Count);

                  string outputPath = PathManager.BackupResourcesFolder;

                  for (int i = 0; i < fontFileNames.Count; i++) {
                        gameFileManager.progressState.CurrentFile = fontFileNames[i];

                        string textFilePath = Path.Combine(outputPath, fontFileNames[i] + ".fnt");
                        string gamesheetFilePath = Path.Combine(outputPath, fontFileNames[i] + ".png");

                        bool textFileFound = File.Exists(textFilePath);
                        bool gamesheetFileFound = File.Exists(gamesheetFilePath);

                        // If both files exist, read and unpack them
                        if (textFileFound && gamesheetFileFound) {
                              // Deserialise the new font file
                              Font newFont = Font.DeserialiseFromPath(textFilePath);
                              newFont.Name = fontFileNames[i];

                              // Extract and crop all characters from all fontsheets
                              Rectangle[] cropRects = newFont.GetCharRects();
                              Bitmap fontsheet = new Bitmap(gamesheetFilePath);
                              Bitmap[] croppedChars = fontsheet.Multicrop(cropRects);

                              for (int j = 0; j < croppedChars.Length; j++) {
                                    newFont.Chars[j].Texture = croppedChars[j];
                              }

                              fontsheet.Dispose();

                              gameFileManager.progressState.CompletedFiles++;
                              newFontsList.Add(newFont);

                        } else {
                              // Log the missing files
                              if (!textFileFound)
                                    Logger.Write(Logger.Mode.Warn, $"The file \"{textFilePath}\" could not be located in the local backup resources folder");
                              if (!gamesheetFileFound)
                                    Logger.Write(Logger.Mode.Warn, $"The file \"{gamesheetFilePath}\" could not be located in the local backup resources folder");

                              // Remove 1 from the total expected files list
                              gameFileManager.progressState.TotalFiles -= 1;

                              // Finally remove the not found file from the list
                              fontFileNames.RemoveAt(i);
                              i--;
                        }
                  }

                  _fonts = newFontsList.ToArray();
            }

            public Font[] RandomiseFiles(int seed) => RandomiseFiles(new Random(seed));

            public Font[] RandomiseFiles(Random random = null) {
                  if (config.FontRand.Enabled == false) {
                        return _fonts.ToArray();
                  }

                  if (random == null)
                        new Random(Guid.NewGuid().GetHashCode());

                  // Create new array for the randomised fonts
                  Font[] newFonts;

                  if (config.FontRand.ShuffleSettings.Enabled) {
                        newFonts = _fonts.Select(f => f.PartialCopy()).ToArray();
                        ShuffleFontStyles(random, ref newFonts, config.FontRand.ShuffleSettings.Mode);

                  } else {
                        newFonts = _fonts.Select(f => f.DeepCopy()).ToArray();
                  }

                  if (config.FontRand.CharacterRandSettings.Enabled) {
                        ShuffleCharactersInFonts(random, ref newFonts);
                  }

                  if (config.FontRand.LetterSpacingSettings.Enabled) {
                        AlterLetterSpacing(random, ref newFonts);
                  }

                  // Finally rearrange the boxes for where the chars will go on the assembled gamesheet
                  RepackFonts(ref newFonts);

                  return newFonts;
            }

            private void ShuffleFontStyles(Random random, ref Font[] newFonts, ShufflingMode mode) {
                  switch (mode) {
                        case ShufflingMode.PerFont:
                              ShuffleFontStylesByFont(random, ref newFonts);
                              break;

                        case ShufflingMode.PerCharacter:
                              ShuffleFontStylesByLetter(random, ref newFonts);
                              break;

                        default:
                              Logger.Warn($"Unrecognised Font Shuffling Mode: {mode}. Defaulting to Per Font mode");
                              goto case ShufflingMode.PerFont;
                  }
            }

            private void ShuffleFontStylesByFont(Random random, ref Font[] newFonts) {
                  int[] newOrder = random.GetShuffledIntRange(newFonts.Length);

                  for (int fontID = 0; fontID < newFonts.Length; fontID++) {
                        int newFontStyleID = newOrder[fontID];

                        // Get references to the original and the replacement font styles
                        Font originalFontStyle = _fonts[fontID];
                        Font newFontStyle = _fonts[newFontStyleID];

                        List<FontChar> newChars = new List<FontChar>();

                        for (int @char = 0; @char < originalFontStyle.Chars.Length; @char++) {
                              int charID = originalFontStyle.Chars[@char].CharID;

                              // Try and get the character with the given ID from the new font stlye
                              FontChar newChar = newFontStyle.GetChar(charID, out bool found);

                              if (!found) {
                                    // If it doesn't exist, get it from the original one
                                    newChar = originalFontStyle.GetChar(charID, out found);
                              }

                              // Add it to the temporary list
                              newChars.Add(newChar.DeepCopy());
                        }

                        newFonts[fontID].Chars = newChars.ToArray();
                  }
            }

            private void ShuffleFontStylesByLetter(Random random, ref Font[] newFonts) {
                  for (int f = 0; f < newFonts.Length; f++) {
                        // Copy only the characterIDs from the old array into the new one. The randomised characters will replace the current ones based on their ID
                        newFonts[f].Chars = _fonts[f].Chars.Select(c => new FontChar { CharID = c.CharID }).ToArray();

                        // Clone the Kernings arrar in it's entirety
                        newFonts[f].Kernings = _fonts[f].Kernings.Select(k => k.DeepCopy()).ToArray();
                  }

                  // Get all common character IDs
                  int[] commonCharIDs = FontChar.CommonIDs;

                  for (int i = 0; i < commonCharIDs.Length; i++) {
                        int currentCharID = commonCharIDs[i];

                        // Get all fonts from the new array that contain this characterID
                        Font[] newFontRefs = newFonts
                              .Where(f => f.HasCharID(currentCharID))
                              .ToArray();

                        // Get the FontChar from each font with the current charID
                        FontChar[] characters = _fonts
                              .Select(f => f.GetChar(currentCharID))
                              .Where(c => c != null)
                              .ToArray();

                        // Shuflle the resulting characters
                        random.Shuffle<FontChar>(characters);

                        for (int l = 0; l < newFontRefs.Length; l++) {
                              // Clone and insert them into the font
                              newFontRefs[l].ReplaceCharByID(characters[l].DeepCopy());
                        }
                  }

                  for (int f = 0; f < newFonts.Length; f++) {
                        Font font = newFonts[f];
                        for (int c = 0; c < font.Chars.Length; c++) {
                              // Every character that was not copied before will get copied now
                              if (font.Chars[c].Texture == null) {
                                    font.ReplaceCharByID(_fonts[f].Chars[c].DeepCopy());
                              }
                        }
                  }
            }

            private void ShuffleCharactersInFonts(Random random, ref Font[] newFonts) {
                  CharacterRandomisationSettings config = RandomisationConfig.Instance.FontRand.CharacterRandSettings;
                  int[] groups = config.GetAllGroups();

                  for (int g = 0; g < groups.Length; g++) {
                        if (groups[g] == 0)
                              continue;

                        FontChar.CharacterType charTypeCollection = FontChar.CharacterType.None;

                        // Add each flag to the collection
                        if (config.Letter.IsEnabledAndGroupIs(groups[g]))
                              charTypeCollection |= FontChar.CharacterType.Letter;

                        if (config.Number.IsEnabledAndGroupIs(groups[g]))
                              charTypeCollection |= FontChar.CharacterType.Number;

                        if (config.Symbol.IsEnabledAndGroupIs(groups[g]))
                              charTypeCollection |= FontChar.CharacterType.Symbol;

                        if (config.Special.IsEnabledAndGroupIs(groups[g]))
                              charTypeCollection |= FontChar.CharacterType.Special;

                        Console.WriteLine((int)charTypeCollection);

                        for (int f = 0; f < newFonts.Length; f++) {
                              newFonts[f].ShuffleCharacters(random, charTypeCollection);
                        }
                  }

                  for (int f = 0; f < newFonts.Length; f++) {
                        if (config.Letter.IsEnabledAndGroupIsZero())
                              newFonts[f].ShuffleCharacters(random, FontChar.CharacterType.Letter);

                        if (config.Number.IsEnabledAndGroupIsZero())
                              newFonts[f].ShuffleCharacters(random, FontChar.CharacterType.Number);

                        if (config.Symbol.IsEnabledAndGroupIsZero())
                              newFonts[f].ShuffleCharacters(random, FontChar.CharacterType.Symbol);

                        if (config.Special.IsEnabledAndGroupIsZero())
                              newFonts[f].ShuffleCharacters(random, FontChar.CharacterType.Special);
                  }
            }

            private void AlterLetterSpacing(Random random, ref Font[] newFonts) {
                  LetterSpacingSettings LSS = RandomisationConfig.Instance.FontRand.LetterSpacingSettings;

                  LetterSpacingSettings.KerningData kerningData = LSS.GetKerningData();

                  for (int i = 0; i < newFonts.Length; i++) {
                        newFonts[i].AlterLetterSpacing(random, kerningData);
                  }
            }

            private void ReorderCharIDsInFont(ref Font font, Random random) {
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

            public void WriteFontsToDisk() => WriteFontsToDisk(GDR_Path.LocalResourcesOutputFolder, _fonts);
            public void WriteFontsToDisk(Font[] fonts) => WriteFontsToDisk(GDR_Path.LocalResourcesOutputFolder, fonts);

            public void WriteFontsToDisk(GDR_Path path, Font[] fonts) => WriteFontsToDisk(PathManager.GetPath(path), fonts);

            public void WriteFontsToDisk(string path, Font[] fonts) {
                  Directory.CreateDirectory(path);

                  gameFileManager.progressState.NewFileBatch(fonts.Length);

                  for (int i = 0; i < fonts.Length; i++) {
                        string fontName = fonts[i].Name;

                        gameFileManager.progressState.CurrentFile = fontName;

                        string textFileName = Path.Combine(path, fontName + ".fnt");
                        string gamesheetFileName = Path.Combine(path, fontName + ".png");

                        using (FileStream fs = File.Create(textFileName)) {
                              byte[] encodedText = new UTF8Encoding(true).GetBytes(fonts[i].Serialise());
                              //byte[] encodedText = Encoding.UTF8.GetBytes(fonts[i].Serialise());
                              fs.Write(encodedText, 0, encodedText.Length);
                        }

                        //File.WriteAllText(textFileName, fonts[i].Serialise());
                        Bitmap gamesheet = fonts[i].AssembleGamesheet();
                        gamesheet.Save(gamesheetFileName);

                        gameFileManager.progressState.CompletedFiles++;
                  }
            }

            public int[] GetAllDistinctCharIDs() {
                  List<int> allCharIDs = new List<int>();
                  for (int i = 0; i < _fonts.Length; i++) {
                        allCharIDs.AddRange(_fonts[i].GetCharIDs());
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

            public static IEnumerable<string> GetAllFileNames(GDR_Path source, Quality quality) {
                  return GetAllFileNames(PathManager.GetPath(source), quality);
            }

            public static IEnumerable<string> GetAllFileNames(string path, Quality quality) {
                  return Directory.GetFiles(path)
                        .Where(f => Path.GetExtension(f) == ".fnt")
                        .FilterFilesByQuality(quality)
                        .Where(f => File.Exists(f + ".png") && File.Exists(f + ".fnt"))
                        .Select(f => Path.GetFileName(f));
            }

            private Font[] _fonts = Array.Empty<Font>();

            private GameFileManager gameFileManager;
            private RandomisationConfig config = RandomisationConfig.Instance;
      }
}
