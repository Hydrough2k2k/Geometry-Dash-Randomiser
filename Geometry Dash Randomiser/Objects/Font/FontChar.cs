using RectpackSharp;
using System.Drawing;
using System.Text.Json.Serialization;

namespace Geometry_Dash_Randomiser {

      public class FontChar {

            public int CharID { get; set; } = -1;
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
            public Bitmap Texture { get; set; }

            [JsonIgnore]
            public Rectangle Rectangle => new Rectangle(X, Y, Width, Height);

            public FontChar() { }

            public PackingRectangle GetPackingRect(int ID = 0) {
                  return new PackingRectangle((uint)X, (uint)Y, (uint)Width, (uint)Height, ID);
            }

            public void ReplaceTexture(Bitmap newTexture) {
                  Texture = (Bitmap)newTexture.Clone();

                  this.Width = newTexture.Width;
                  this.Height = newTexture.Height;
            }

            public void SwapIDWith(FontChar other) {
                  (this.CharID, other.CharID) = (other.CharID, this.CharID);
            }

            public string Serialise() {
                  return "char id=" + this.CharID +
                        " x=" + this.X +
                        " y=" + this.Y +
                        " width=" + this.Width +
                        " height=" + this.Height +
                        " xoffset=" + this.X_Offset +
                        " yoffset=" + this.Y_Offset +
                        " xadvance=" + this.X_Advance +
                        " page=" + this.Page +
                        " chnl=" + this.Channel +
                        " letter=\"" + (this.Letter == ' ' ? "space" : this.Letter.ToString()) + "\"";
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
      }
}
