using RectpackSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json.Serialization;
using static Geometry_Dash_Randomiser.Logger;

namespace Geometry_Dash_Randomiser {

      public class Font {

<<<<<<< Updated upstream:Geometry Dash Randomiser/Objects/Font.cs
            internal struct PropertyPair {
                  public string name;
                  public string data;

                  public PropertyPair(string name, string data) {
                        this.name = name;
                        this.data = data;
                  }
            }

            public string infoFace { get; set; } = string.Empty;
            public int size { get; set; }
            public int bold { get; set; }
            public int italic { get; set; }
            public string charSet { get; set; } = string.Empty;
            public int unicode { get; set; }
            public int stretchH { get; set; }
            public int smooth { get; set; }
            public int aa { get; set; }
            public int4 padding { get; set; }
            public int2 spacing { get; set; }
            public int lineHeight { get; set; }
            public int baseVal { get; set; }
            public int scaleW { get; set; }
            public int scaleH { get; set; }
            public int pages { get; set; }
            public int packed { get; set; }
            public int pageID { get; set; }
            public string file { get; set; } = string.Empty;
            public FontChar[] chars { get; set; } = Array.Empty<FontChar>();
            public FontKerning[] kernings { get; set; } = Array.Empty<FontKerning>();
=======
            public string Name { get; set; } = string.Empty;
            public string InfoFace { get; set; } = string.Empty;
            public int Size { get; set; }
            public int Bold { get; set; }
            public int Italic { get; set; }
            public string CharSet { get; set; } = string.Empty;
            public int Unicode { get; set; }
            public int Stretch_H { get; set; }
            public int Smooth { get; set; }
            public int AA { get; set; }
            public Int4 Padding { get; set; }
            public Int2 Spacing { get; set; }
            public int LineHeight { get; set; }
            public int BaseVal { get; set; }
            public int Scale_W { get; set; }
            public int Scale_H { get; set; }
            public int Pages { get; set; }
            public int Packed { get; set; }
            public int PageID { get; set; }
            public string File { get; set; } = string.Empty;
            public FontChar[] Chars { get; set; } = Array.Empty<FontChar>();
            public FontKerning[] Kernings { get; set; } = Array.Empty<FontKerning>();
>>>>>>> Stashed changes:Geometry Dash Randomiser/Objects/Font/Font.cs

            [JsonIgnore]
            public int[] CharIDs => this.Chars.Select(c => c.CharID).ToArray();

            public static Font Deserialise(string[] fileStream) {
                  Font font = FontSerializer.Deserialise(fileStream);

<<<<<<< Updated upstream:Geometry Dash Randomiser/Objects/Font.cs
                  // Important for the binary search
                  Sort(font.chars);
                  Sort(font.kernings);
=======
                  // Will be important for binary search later
                  Sort(font.Chars);
                  Sort(font.Kernings);
>>>>>>> Stashed changes:Geometry Dash Randomiser/Objects/Font/Font.cs

                  return font;
            }

            public void SortArrays() {
                  Sort(this.chars);
                  Sort(this.kernings);
            }

            static void Sort(FontChar[] chars) {
                  Array.Sort(chars, delegate (FontChar x, FontChar y) { return x.charID.CompareTo(y.charID); });
            }

            static void Sort(FontKerning[] kernings) {
                  Array.Sort(kernings, delegate (FontKerning x, FontKerning y) { return x.first.CompareTo(y.first); });
            }

            public char[] GetCharSet() {
                  return this.chars.Select(c => c.letter).ToArray();
            }

<<<<<<< Updated upstream:Geometry Dash Randomiser/Objects/Font.cs
            public int[] GetCharIDs() {
                  return this.chars.Select(c => c.charID).ToArray();
            }

            public int GetKerningBetween(int first, int second) {
                  for (int i = 0; i < this.kernings.Length; i++) {
                        if (kernings[i].first == first && kernings[i].second == second) {
                              return this.kernings[i].amount;
=======
            public int GetKerningBetween(int first, int second) {
                  FontKerning fk = GetKerning(first, second, out bool found);
                  return found ? fk.Amount : 0;
            }

            public FontKerning GetKerning(int first, int second, out bool found) {
                  for (int i = 0; i < this.Kernings.Length; i++) {
                        if (Kernings[i].First == first && Kernings[i].Second == second) {
                              found = true;
                              return this.Kernings[i];
>>>>>>> Stashed changes:Geometry Dash Randomiser/Objects/Font/Font.cs
                        }
                  }
                  found = false;
                  return null;
            }

            public string Serialise() {
                  return FontSerializer.SerialiseTextFile(this);
            }

<<<<<<< Updated upstream:Geometry Dash Randomiser/Objects/Font.cs
            public void RandomiseProperties(int level, Random random) {
                  if (level <= 0) return;

                  RandomiseCharacterProperties(level, random);
                  RandomiseKerningsProperties(level, random);
            }

            public void RandomiseCharacterProperties(int level, Random random) {
                  switch (level) {
                        case 1:
                              RandomiseCharacterProperties(0.925f, 1.1f, random);
                              break;
                        case 2:
                              RandomiseCharacterProperties(0.825f, 1.2666f, random);
                              break;
                        case 3:
                              RandomiseCharacterProperties(0.625f, 1.6f, random);
                              break;
                        case 4:
                              RandomiseCharacterProperties(0.4f, 2.25f, random);
                              break;
                        case 5:
                              RandomiseCharacterProperties(0.25f, 4f, random);
                              break;
                        default:
                              break;
                  }
            }

            public void RandomiseCharacterProperties(int min, int max, Random random) {
                  for (int i = 0; i < this.chars.Length; i++) {
                        this.chars[i].xAdvance += random.Next(min, max);
                  }
            }

            public void RandomiseCharacterProperties(float minMult, float maxMult, Random random) {
                  for (int i = 0; i < this.chars.Length; i++) {
                        this.chars[i].xAdvance = (int)(this.chars[i].xAdvance * RandomExtensions.NextFloat(random, minMult, maxMult));
                  }
            }

            public void RandomiseKerningsProperties(int level, Random random) {
                  switch (level) {
                        case 1:
                              RandomiseKerningsProperties(20, -15, 25, random);
                              break;
                        case 2:
                              RandomiseKerningsProperties(35, -25, 40, random);
                              break;
                        case 3:
                              RandomiseKerningsProperties(55, -40, 75, random);
                              break;
                        case 4:
                              RandomiseKerningsProperties(80, -75, 100, random);
                              break;
                        case 5:
                              RandomiseKerningsProperties(100, -100, 150, random);
                              break;
                        default:
                              break;
                  }
            }

            /// <param name="probability">What percent chance there is for the kerning between 2 characters to be altered, or for new kerning to be added</param>
            public void RandomiseKerningsProperties(int probabilityPercent, int min, int max, Random random) {
                  List<FontKerning> newKernings = new List<FontKerning>();

                  for (int j = 0; j < this.chars.Length; j++) {
                        for (int i = 0; i < this.chars.Length; i++) {

                              if (random.Next(100) > probabilityPercent) {
                                    int first = j, second = i, kerning = random.Next(min, max);
                                    newKernings.Add(new FontKerning(first, second, kerning));
                              }
                        }
                  }

                  this.kernings = newKernings.ToArray();
            }

=======
>>>>>>> Stashed changes:Geometry Dash Randomiser/Objects/Font/Font.cs
            public int GetCharPositionInArray(int charID) {
                  for (int i = 0; i < this.chars.Length; i++) {
                        if (this.chars[i].charID == charID) {
                              return i;
                        }
                  }
                  return -1;
            }

            public int GetCharPositionInArray(char letter) {
                  for (int i = 0; i < this.chars.Length; i++) {
                        if (this.chars[i].letter == letter) {
                              return i;
                        }
                  }
                  return -1;
            }

            public FontChar GetChar(int charID)
                  => GetChar(charID, out bool found);

            public FontChar GetChar(int charID, out bool found) {
                  int ID = GetCharPositionInArray(charID);
<<<<<<< Updated upstream:Geometry Dash Randomiser/Objects/Font.cs
                  return ID == -1 ? new FontChar() : this.chars[ID];
=======
                  if (ID != -1) {
                        found = true;
                        return this.Chars[ID];

                  } else {
                        found = false;
                        return null;
                  }
>>>>>>> Stashed changes:Geometry Dash Randomiser/Objects/Font/Font.cs
            }

            public FontChar GetChar(char letter)
                  => GetChar(letter, out bool found);

            public FontChar GetChar(char letter, out bool found) {
                  int ID = GetCharPositionInArray(letter);
<<<<<<< Updated upstream:Geometry Dash Randomiser/Objects/Font.cs
                  return ID == -1 ? new FontChar() : this.chars[ID];
            }

            public int GetDistanceBetweenChars(char first, char second) {
                  int firstVal = GetCharPositionInArray(first);
                  int secondVal = GetCharPositionInArray(second);

                  if (firstVal != -1 && secondVal != -1)
                        return GetDistanceBetweenChars(firstVal, secondVal);
                  return 0;
            }

            public int GetDistanceBetweenChars(int first, int second) {
                  int firstChar = GetCharPositionInArray(first);
                  return firstChar == -1 ? 0 : this.chars[firstChar].xAdvance + GetKerningBetween(first, second);
=======
                  if (ID != -1) {
                        found = true;
                        return this.Chars[ID];
                  } else {
                        found = false;
                        return null;
                  }
>>>>>>> Stashed changes:Geometry Dash Randomiser/Objects/Font/Font.cs
            }

            public bool HasCharID(int ID) {
                  for (int i = 0; i < this.chars.Length; i++) {
                        if (this.chars[i].charID == ID)
                              return true;
                  }
                  return false;
            }

            /// <summary>
            /// Get all the letters from this font and shuffle them around
            /// </summary>
            public List<int> GetLetterCharIndexes() {
                  return ArrayExt.GetIndexesFrom(CharIDs, LetterCharIDs);
            }

            /// <summary>
            /// Get all the numbers from this font and shuffle them around
            /// </summary>
            public List<int> GetNumberCharIndexes() {
                  return ArrayExt.GetIndexesFrom(CharIDs, NumberCharIDs);
            }

            /// <summary>
            /// Get all the symbols from this font and shuffle them around
            /// </summary>
            public List<int> GetSymbolCharIndexes() {
                  return ArrayExt.GetIndexesFrom(CharIDs, SymbolCharIDs);
            }

            public void ShuffleLetters(Random random) {
                  CharacterShuffleSettings CSS = RandomisationConfig.Instance.FontRand.CharacterShuffleSettings;

                  // Shuffle all of the enabled and zero based groups separately
                  if (CSS.Letter.IsEnabledAndGroupIsZero()) {
                        ShuffleCharacters(random, GetLetterCharIndexes());
                  }

                  if (CSS.Number.IsEnabledAndGroupIsZero()) {
                        ShuffleCharacters(random, GetNumberCharIndexes());
                  }

                  if (CSS.Symbol.IsEnabledAndGroupIsZero()) {
                        ShuffleCharacters(random, GetSymbolCharIndexes());
                  }

                  int[] groupSettingValues = CSS.GetDistinctNonZeroGroupValues();

                  for (int i = 0; i < groupSettingValues.Length; i++) {
                        List<int> indexes = new List<int>();
                        
                        if (CSS.Letter.IsEnabledAndGroupIs(groupSettingValues[i]))
                              indexes.AddRange(GetLetterCharIndexes());
                        if (CSS.Number.IsEnabledAndGroupIs(groupSettingValues[i]))
                              indexes.AddRange(GetNumberCharIndexes());
                        if (CSS.Symbol.IsEnabledAndGroupIs(groupSettingValues[i]))
                              indexes.AddRange(GetSymbolCharIndexes());

                        ShuffleCharacters(random, indexes);
                  }
            }

            /// <summary>
            /// Get all the characters by index from this font and shuffle them around
            /// </summary>
            public void ShuffleCharacters(Random random, List<int> indexes) {
                  // If there are too few entries for the huffling to be meaningful, just skip this step
                  if (indexes.Count < 2)
                        return;

                  int rnd;
                  for (int i = 0; i < indexes.Count; i++) {
                        // Make sure we don't get the same ID
                        do {
                              rnd = random.Next(0, indexes.Count);
                        } while (rnd == i);

                        // Finally just swap the CharIDs of the letters we rolled
                        Chars[indexes[i]].SwapIDWith(Chars[indexes[rnd]]);
                  }
            }

            public void AlterLetterSpacing(Random random, LetterSpacing ls) {
                  for (int i = 0; i < Chars.Length; i++) {
                        // If the random roll is greater than the probability
                        if (random.Next(1, 100) <= ls.Probability) {

                              // Alter the X_Offset of the character by a random amount
                              Chars[i].X_Offset += random.Next(ls.MinX_Offset, ls.MaxX_Offset);
                        }
                  }

                  List<FontKerning> newKernings = new List<FontKerning>();

                  // Take all character pairs
                  for (int c1 = 0; c1 < Chars.Length; c1++) {
                        for (int c2 = 0; c2 < Chars.Length; c2++) {

                              // If the random roll is greater than the probability
                              if (random.Next(1, 100) <= ls.Probability) {
                                    int charID1 = Chars[c1].CharID;
                                    int charID2 = Chars[c2].CharID;

                                    // Try and get the kerning from the current array
                                    FontKerning fk = GetKerning(charID1, charID2, out bool found);

                                    if (found) {
                                          // Clone it if it's found, so we don't alter the original kerning
                                          fk = fk.DeepCopy();
                                    } else {
                                          // Otherwise create a new kerning with the two character IDs
                                          fk = new FontKerning(charID1, charID2, 0);
                                    }

                                    // Alter the amount
                                    fk.Amount += random.Next(ls.MinKerning, ls.MaxKerning);

                                    // Finally, add it if the result is not zero
                                    if (fk.Amount != 0) {
                                          newKernings.Add(fk);
                                    }
                              }
                        }
                  }

                  // Store the result in the array, and sort them
                  this.Kernings = newKernings.ToArray();
                  Sort(Kernings);
            }

            public Rectangle[] GetCharRects() {
                  Rectangle[] rects = new Rectangle[this.chars.Length];
                  for (int i = 0; i < this.chars.Length; i++) {
                        rects[i] = this.chars[i].rectangle;
                  }
                  return rects;
            }

            public void Repack() {
                  PackingRectangle[] packingRects = GetPackingRects();

                  // Get the new position for the images, then sort the array of packing rects
                  RectanglePacker.Pack(packingRects, out PackingRectangle bounds);
                  Array.Sort(packingRects, (a, b) => a.Id.CompareTo(b.Id));

                  int maxDimentionSize = Math.Max((int)bounds.Height, (int)bounds.Width);

                  // Snap it to the next number divisible by 512
                  maxDimentionSize = ((maxDimentionSize - 1) / 512 + 1) * 512;

                  // Set the width and height of the font image
                  scaleW = maxDimentionSize;
                  scaleH = maxDimentionSize;

                  // Finally set the new coordinates, widths and heights for the new characters
                  for (int j = 0; j < packingRects.Length; j++) {
                        int charID = packingRects[j].Id;
                        FontChar ch = chars[charID];

                        ch.x = (int)packingRects[j].X;
                        ch.y = (int)packingRects[j].Y;
                  }

                  Array.Sort(this.Chars, (a, b) => a.CharID.CompareTo(b.CharID));
            }

            public PackingRectangle[] GetPackingRects() {
                  return chars
                        .Select((ch, index) => new { ch, index })
                        .Where(pair => pair.ch.width != 0 && pair.ch.height != 0)
                        .Select(pair => pair.ch.GetPackingRect(pair.index))
                        .ToArray();
            }

            public Bitmap AssembleGamesheet() {
                  return GameSheetAssembler.Assemble(this);
            }

            public string SerialiseInfoLine() {
                  return "info face=\"" + this.InfoFace + "\"" +
                        " size=" + this.Size +
                        " bold=" + this.Bold +
                        " italic=" + this.Italic +
                        " charset=\"" + this.CharSet + "\"" +
                        " unicode=" + this.Unicode +
                        " stretchH=" + this.Stretch_H +
                        " smooth=" + this.Smooth +
                        " aa=" + this.AA +
                        " padding=" + this.Padding.ToString(FormatMode.Plist) +
                        " spacing=" + this.Spacing.ToString(FormatMode.Plist);
            }

            public string SerialiseCommonLine() {
                  return "common lineHeight=" + this.LineHeight +
                        " base=" + this.BaseVal +
                        " scaleW=" + this.Scale_W +
                        " scaleH=" + this.Scale_H +
                        " pages=" + this.Pages +
                        " packed=" + this.Packed;
            }

            public string SerialisePageLine() {
                  return "page id=" + this.PageID +
                        " file=\"" + this.File + "\"";
            }

            public Font PartialCopy() {
                  Font copy = new Font();

<<<<<<< Updated upstream:Geometry Dash Randomiser/Objects/Font.cs
                  copy.infoFace = this.infoFace;
                  copy.size = this.size;
                  copy.bold = this.bold;
                  copy.italic = this.italic;
                  copy.charSet = this.charSet;
                  copy.unicode = this.unicode;
                  copy.stretchH = this.stretchH;
                  copy.smooth = this.smooth;
                  copy.aa = this.aa;
                  copy.padding = this.padding;
                  copy.spacing = this.spacing;
                  copy.lineHeight = this.lineHeight;
                  copy.baseVal = this.baseVal;
                  copy.scaleW = this.scaleW;
                  copy.scaleH = this.scaleH;
                  copy.pages = this.pages;
                  copy.packed = this.packed;
                  copy.pageID = this.pageID;
                  copy.file = this.file;
=======
                  copy.Name = this.Name;
                  copy.InfoFace = this.InfoFace;
                  copy.Size = this.Size;
                  copy.Bold = this.Bold;
                  copy.Italic = this.Italic;
                  copy.CharSet = this.CharSet;
                  copy.Unicode = this.Unicode;
                  copy.Stretch_H = this.Stretch_H;
                  copy.Smooth = this.Smooth;
                  copy.AA = this.AA;
                  copy.Padding = this.Padding;
                  copy.Spacing = this.Spacing;
                  copy.LineHeight = this.LineHeight;
                  copy.BaseVal = this.BaseVal;
                  copy.Scale_W = this.Scale_W;
                  copy.Scale_H = this.Scale_H;
                  copy.Pages = this.Pages;
                  copy.Packed = this.Packed;
                  copy.PageID = this.PageID;
                  copy.File = this.File;
>>>>>>> Stashed changes:Geometry Dash Randomiser/Objects/Font/Font.cs

                  return copy;
            }

            public Font DeepCopy() {
<<<<<<< Updated upstream:Geometry Dash Randomiser/Objects/Font.cs
                  Font copy = new Font();

                  copy.infoFace = this.infoFace;
                  copy.size = this.size;
                  copy.bold = this.bold;
                  copy.italic = this.italic;
                  copy.charSet = this.charSet;
                  copy.unicode = this.unicode;
                  copy.stretchH = this.stretchH;
                  copy.smooth = this.smooth;
                  copy.aa = this.aa;
                  copy.padding = this.padding;
                  copy.spacing = this.spacing;
                  copy.lineHeight = this.lineHeight;
                  copy.baseVal = this.baseVal;
                  copy.scaleW = this.scaleW;
                  copy.scaleH = this.scaleH;
                  copy.pages = this.pages;
                  copy.packed = this.packed;
                  copy.pageID = this.pageID;
                  copy.file = this.file;
=======
                  Font copy = this.PartialCopy();
>>>>>>> Stashed changes:Geometry Dash Randomiser/Objects/Font/Font.cs

                  copy.chars = this.chars.Select(c => c.DeepCopy()).ToArray();
                  copy.kernings = this.kernings.Select(c => c.DeepCopy()).ToArray();

                  return copy;
            }

            private static int[] LetterCharIDs => new int[] {
                  // Upper case
                  65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90,
                  // Lower case
                  97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122
            };

            private static int[] NumberCharIDs => new int[] {
                  // Digits 0-9
                  48, 49, 50, 51, 52, 53, 54, 55, 56, 57
            };

            private static int[] SymbolCharIDs => new int[] {
                  32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47,
                  58, 59, 60, 61, 62, 63, 64,
                  91, 92, 93, 94, 95, 96,
                  123, 124, 125, 126,
                  // Special Character, circle or something, not sure what it is used for
                  8226
            };

            public struct NamedProperty {
                  public readonly string name;
                  public readonly string data;

                  public NamedProperty(string name, string data) {
                        this.name = name;
                        this.data = data;
                  }
            }

            public class LetterSpacing {
                  // Minimum X_Offset value chage
                  public readonly int MinX_Offset;

                  // Maximum X_Offset value chage
                  public readonly int MaxX_Offset;

                  // Minimum Kerning value chage
                  public readonly int MinKerning;

                  // Maximum Kerning value chage
                  public readonly int MaxKerning;

                  // How likely it is that 2 characters will be spaced apart, as a percentage
                  public readonly int Probability;

                  public LetterSpacing(int minX_Offset, int maxX_Offset, int minKerning, int maxKerning, int probability) {
                        MinX_Offset = minX_Offset;
                        MaxX_Offset = maxX_Offset;
                        MinKerning = minKerning;
                        MaxKerning = maxKerning;
                        Probability = probability;
                  }

                  public static LetterSpacing Get(int level) {

                        // Note: All of these values are subject to change
                        switch (level) {
                              case 0:
                                    return new LetterSpacing(-3, 5, -3, 5, 15);
                              case 1:
                                    return new LetterSpacing(-5, 8, -5, 8, 20);
                              case 2:
                                    return new LetterSpacing(-10, 15, -10, 15, 25);
                              case 3:
                                    return new LetterSpacing(-12, 18, -12, 18, 30);
                              case 4:
                                    return new LetterSpacing(-16, 25, -16, 25, 40);
                              case 5:
                                    return new LetterSpacing(-24, 35, -24, 35, 50);
                              case 6:
                                    return new LetterSpacing(-32, 60, -32, 60, 60);
                              case 7:
                                    return new LetterSpacing(-45, 70, -45, 70, 75);
                              case 8:
                                    return new LetterSpacing(-60, 90, -60, 90, 90);
                              case 9:
                                    return new LetterSpacing(-80, 120, -80, 120, 100);
                              case 10:
                                    return new LetterSpacing(-100, 150, -100, 150, 100);
                              default:
                                    Logger.Write(Logger.Mode.Error, $"Unrecognised letter spacing level was received: \"{level}\". Defaulting to level 5");
                                    goto case 5;
                        }
                  }
            }
      }
}
