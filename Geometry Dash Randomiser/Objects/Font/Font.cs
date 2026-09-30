using System.Collections.Generic;
using System;
using System.Drawing;
using System.Linq;
using RectpackSharp;

namespace Geometry_Dash_Randomiser {

      public class Font {

            public struct PropertyPair {
                  public readonly string name;
                  public readonly string data;

                  public PropertyPair(string name, string data) {
                        this.name = name;
                        this.data = data;
                  }
            }

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

            public static Font Deserialise(string[] fileStream) {
                  Font font = FontSerializer.Deserialise(fileStream);

                  // Important for the binary search
                  Sort(font.Chars);
                  Sort(font.Kernings);

                  return font;
            }

            public void SortArrays() {
                  Sort(this.Chars);
                  Sort(this.Kernings);
            }

            static void Sort(FontChar[] chars) {
                  Array.Sort(chars, delegate (FontChar x, FontChar y) { return x.CharID.CompareTo(y.CharID); });
            }

            static void Sort(FontKerning[] kernings) {
                  Array.Sort(kernings, delegate (FontKerning x, FontKerning y) { return x.First.CompareTo(y.First); });
            }

            public char[] GetCharSet() {
                  return this.Chars.Select(c => c.Letter).ToArray();
            }

            public int[] GetCharIDs() {
                  return this.Chars.Select(c => c.CharID).ToArray();
            }

            public int GetKerningBetween(int first, int second) {
                  for (int i = 0; i < this.Kernings.Length; i++) {
                        if (Kernings[i].First == first && Kernings[i].Second == second) {
                              return this.Kernings[i].Amount;
                        }
                  }
                  return 0;
            }

            public string Serialise() {
                  return FontSerializer.SerialiseTextFile(this);
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
                  for (int i = 0; i < this.Chars.Length; i++) {
                        this.Chars[i].X_Advance += random.Next(min, max);
                  }
            }

            public void RandomiseCharacterProperties(float minMult, float maxMult, Random random) {
                  for (int i = 0; i < this.Chars.Length; i++) {
                        this.Chars[i].X_Advance = (int)(this.Chars[i].X_Advance * RandomExtensions.NextFloat(random, minMult, maxMult));
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

                  for (int j = 0; j < this.Chars.Length; j++) {
                        for (int i = 0; i < this.Chars.Length; i++) {

                              if (random.Next(100) > probabilityPercent) {
                                    int first = j, second = i, kerning = random.Next(min, max);
                                    newKernings.Add(new FontKerning(first, second, kerning));
                              }
                        }
                  }

                  this.Kernings = newKernings.ToArray();
            }

            public int GetCharPositionInArray(int charID) {
                  for (int i = 0; i < this.Chars.Length; i++) {
                        if (this.Chars[i].CharID == charID) {
                              return i;
                        }
                  }
                  return -1;
            }

            public int GetCharPositionInArray(char letter) {
                  for (int i = 0; i < this.Chars.Length; i++) {
                        if (this.Chars[i].Letter == letter) {
                              return i;
                        }
                  }
                  return -1;
            }

            public FontChar GetChar(int charID) {
                  int ID = GetCharPositionInArray(charID);
                  return ID == -1 ? new FontChar() : this.Chars[ID];
            }

            public FontChar GetChar(char letter) {
                  int ID = GetCharPositionInArray(letter);
                  return ID == -1 ? new FontChar() : this.Chars[ID];
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
                  return firstChar == -1 ? 0 : this.Chars[firstChar].X_Advance + GetKerningBetween(first, second);
            }

            public bool HasCharID(int ID) {
                  for (int i = 0; i < this.Chars.Length; i++) {
                        if (this.Chars[i].CharID == ID)
                              return true;
                  }
                  return false;
            }

            public Rectangle[] GetCharRects() {
                  Rectangle[] rects = new Rectangle[this.Chars.Length];
                  for (int i = 0; i < this.Chars.Length; i++) {
                        rects[i] = this.Chars[i].Rectangle;
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
                  Scale_W = maxDimentionSize;
                  Scale_H = maxDimentionSize;

                  // Finally set the new coordinates, widths and heights for the new characters
                  for (int j = 0; j < packingRects.Length; j++) {
                        int charID = packingRects[j].Id;
                        FontChar ch = Chars[charID];

                        ch.X = (int)packingRects[j].X;
                        ch.Y = (int)packingRects[j].Y;
                  }
            }

            public PackingRectangle[] GetPackingRects() {
                  return Chars
                        .Select((ch, index) => new { ch, index })
                        .Where(pair => pair.ch.Width != 0 && pair.ch.Height != 0)
                        .Select(pair => pair.ch.GetPackingRect(pair.index))
                        .ToArray();
            }

            public Bitmap AssembleGamesheet() {
                  return GameSheetAssembler.Assemble(this);
            }

            public Font PartialCopy() {
                  Font copy = new Font();

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

                  return copy;
            }

            public Font DeepCopy() {
                  Font copy = new Font();

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

                  copy.Chars = this.Chars.Select(c => c.DeepCopy()).ToArray();
                  copy.Kernings = this.Kernings.Select(c => c.DeepCopy()).ToArray();

                  return copy;
            }
      }
}
