using System.Collections.Generic;
using System;
using System.Drawing;
using System.Linq;
using RectpackSharp;

namespace Geometry_Dash_Randomiser {

      public class Font {

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

            public static Font Deserialise(string[] fileStream) {
                  Font font = FontSerialiser.Deserialise(fileStream);

                  // Important for the binary search
                  Sort(font.chars);
                  Sort(font.kernings);

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

            public int[] GetCharIDs() {
                  return this.chars.Select(c => c.charID).ToArray();
            }

            public int GetKerningBetween(int first, int second) {
                  for (int i = 0; i < this.kernings.Length; i++) {
                        if (kernings[i].first == first && kernings[i].second == second) {
                              return this.kernings[i].amount;
                        }
                  }
                  return 0;
            }

            public string Serialise() {
                  return FontSerialiser.SerialiseTextFile(this);
            }

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
                        this.chars[i].xAdvance = (int)(this.chars[i].xAdvance * RandomExt.NextFloat(random, minMult, maxMult));
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

            public FontChar GetChar(int charID) {
                  int ID = GetCharPositionInArray(charID);
                  return ID == -1 ? new FontChar() : this.chars[ID];
            }

            public FontChar GetChar(char letter) {
                  int ID = GetCharPositionInArray(letter);
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
            }

            public bool HasCharID(int ID) {
                  for (int i = 0; i < this.chars.Length; i++) {
                        if (this.chars[i].charID == ID)
                              return true;
                  }
                  return false;
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
            }

            public PackingRectangle[] GetPackingRects() {
                  return chars
                        .Select((ch, index) => new { ch, index })
                        .Where(pair => pair.ch.width != 0 && pair.ch.height != 0)
                        .Select(pair => pair.ch.GetPackingRect(pair.index))
                        .ToArray();
            }

            public Bitmap AssembleGamesheet() {
                  return GameSheet.Assemble(this);
            }

            public Font PartialCopy() {
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

                  return copy;
            }

            public Font DeepCopy() {
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

                  copy.chars = this.chars.Select(c => c.DeepCopy()).ToArray();
                  copy.kernings = this.kernings.Select(c => c.DeepCopy()).ToArray();

                  return copy;
            }
      }
}
