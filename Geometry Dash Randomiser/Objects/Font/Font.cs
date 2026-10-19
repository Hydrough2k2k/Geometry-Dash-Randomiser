using RectpackSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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

            public static Font DeserialiseFromPath(string path) => Font.Deserialise(System.IO.File.ReadAllLines(path));

            public static Font Deserialise(string[] fileStream) {
                  Font font = FontSerializer.Deserialise(fileStream);

                  // Will be important for the binary search
                  Font.SortArrays(font);

                  return font;
            }

            public static void SortArrays(Font font) => font.SortArrays();

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

            private int GetCharPositionInArray(int charID) {
                  for (int i = 0; i < this.Chars.Length; i++) {
                        if (this.Chars[i].CharID == charID) {
                              return i;
                        }
                  }
                  return -1;
            }

            public FontChar GetChar(int charID, out bool found) {
                  int ID = GetCharPositionInArray(charID);
                  if (ID != -1) {
                        found = true;
                        return this.Chars[ID];
                  } else {
                        found = false;
                        return null;
                  }
            }

            public Rectangle[] GetCharRects() {
                  Rectangle[] rects = new Rectangle[this.Chars.Length];
                  for (int i = 0; i < this.Chars.Length; i++) {
                        rects[i] = this.Chars[i].Rectangle;
                  }
                  return rects;
            }

            public void Repack() {
                  PackingRectangle[] packingRects = GetPackingRects().ToArray();

                  // Get the new position for the images, then sort the array of packing rects
                  RectanglePacker.Pack(packingRects, out PackingRectangle bounds);
                  Array.Sort(packingRects, (a, b) => a.Id.CompareTo(b.Id));

                  int maxDimentionSize = Math.Max((int)bounds.Height, (int)bounds.Width);
                  // Snap it to the next number divisible by 256
                  maxDimentionSize = ((maxDimentionSize - 1) / 256 + 1) * 256;

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

            public IEnumerable<PackingRectangle> GetPackingRects() {
                  return Chars
                        .Select((ch, index) => new { ch, index })
                        .Where(pair => pair.ch.Width != 0 && pair.ch.Height != 0)
                        .Select(pair => pair.ch.GetPackingRect(pair.index));
            }

            public Bitmap AssembleGamesheet() {
                  return GameSheetAssembler.Assemble(this);
            }

            public Font PartialCopy() {
                  Font copy = new Font();

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

                  return copy;
            }

            public Font DeepCopy() {
                  Font copy = this.PartialCopy();

                  copy.Chars = this.Chars.Select(c => c.DeepCopy()).ToArray();
                  copy.Kernings = this.Kernings.Select(c => c.DeepCopy()).ToArray();

                  return copy;
            }
      }
}
