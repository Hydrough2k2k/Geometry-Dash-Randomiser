using RectpackSharp;
using System;
using System.Drawing;
using System.Linq;
using System.Text.Json.Serialization;

namespace Geometry_Dash_Randomiser {

      public class FontChar {

            [Flags]
            public enum CharacterType { Letter = 1, Number = 2, Symbol = 4, Special = 8 }

            public int CharID { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int X_Offset { get; set; }
            public int Y_Offset { get; set; }
            public int X_Advance { get; set; }
            public int Page { get; set; }
            public int Channel { get; set; }
            public char Letter { get; set; }
            [JsonIgnore]
            public Bitmap Texture { get; set; }

            [JsonIgnore]
            public CharacterType CharType => GetCharType();
            [JsonIgnore]
            public Rectangle Rectangle => new Rectangle(X, Y, Width, Height);

            public FontChar() { }

            public string Serialise() {
                  return FontSerializer.SerialiseFontChar(this);
            }

            public CharacterType GetCharType() {
                  if ((CharID >= 65 && CharID <= 90)
                        || (CharID >= 97 && CharID <= 122)) {
                        return CharacterType.Letter;

                  } else if (CharID >= 48 && CharID <= 57) {
                        return CharacterType.Number;

                  } else if ((CharID >= 30 && CharID <= 47)
                        || (CharID >= 58 && CharID <= 64)
                        || (CharID >= 91 && CharID <= 96)
                        || (CharID >= 123 && CharID <= 127)
                        || CharID == 8226) {
                        return CharacterType.Symbol;

                  } else {
                        return CharacterType.Special;
                  }
            }

            public PackingRectangle GetPackingRect(int ID = 0) {
                  return new PackingRectangle((uint)X, (uint)Y, (uint)Width, (uint)Height, ID);
            }

            public void ReplaceTexture(Bitmap newTexture) {
                  Texture = (Bitmap)newTexture.Clone();

                  this.Width = newTexture.Width;
                  this.Height = newTexture.Height;
            }

            public FontChar DeepCopy() {
                  FontChar copy = new FontChar();

                  copy.CharID = this.CharID;
                  copy.X = this.X;
                  copy.Y = this.Y;
                  copy.Width = this.Width;
                  copy.Height = this.Height;
                  copy.X_Offset = this.X_Offset;
                  copy.Y_Offset = this.Y_Offset;
                  copy.X_Advance = this.X_Advance;
                  copy.Page = this.Page;
                  copy.Channel = this.Channel;
                  copy.Letter = this.Letter;
                  copy.Texture = this.Texture;

                  return copy;
            }

            public static int[] LetterIDs => new int[] {
                  // Upper Case
                  65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90,

                  // Lower Case
                  97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122
            };

            public static int[] NumberIDs => new int[] {
                  // Digits
                  48, 49, 50, 51, 52, 53, 54, 55, 56, 57
            };

            public static int[] SymbolsIDs => new int[] {
                  30, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47,
                  58, 59, 60, 61, 62, 63, 64,
                  91, 92, 93, 94, 95, 96,
                  123, 124, 125, 126, 127,
                  8226
            };

            public static int[] CommonIDs => GetCommonCharcterIDs();

            private static int[] GetCommonCharcterIDs() {
                  int[] commons = LetterIDs.Concat(NumberIDs).Concat(SymbolsIDs).ToArray();
                  Array.Sort(commons);
                  return commons;
            }
      }
}
