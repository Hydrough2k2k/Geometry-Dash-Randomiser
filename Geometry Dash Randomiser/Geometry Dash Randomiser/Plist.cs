using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.Drawing;
using Newtonsoft.Json.Linq;
using RectpackSharp;

namespace Geometry_Dash_Randomiser {

      internal static class Plist {

            static readonly string[] fileBegin = {
                  "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
                  "<!DOCTYPE plist PUBLIC \"-//Apple Computer//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">",
                  "<plist version=\"1.0\">",
                  "\t<dict>",
                  "\t\t<key>frames</key>",
                  "\t\t<dict>"
            };

            static readonly string[] spriteBase = {
                  "\t\t\t", // To be populated by <string>textureName.png</string>
                  "\t\t\t<dict>",
                  "\t\t\t\t<key>aliases</key>",
                  "\t\t\t\t<array/>", // Really though, what is the point of this?
                  "\t\t\t\t<key>spriteOffset</key>",
                  "\t\t\t\t",
                  "\t\t\t\t<key>spriteSize</key>",
                  "\t\t\t\t",
                  "\t\t\t\t<key>spriteSourceSize</key>",
                  "\t\t\t\t",
                  "\t\t\t\t<key>textureRect</key>",
                  "\t\t\t\t",
                  "\t\t\t\t<key>textureRotated</key>",
                  "\t\t\t\t",
                  "\t\t\t</dict>"
            };

            static readonly string[] fileEnd = {
                  "\t\t</dict>",
                  "\t\t<key>metadata</key>",
                  "\t\t<dict>",
                  "\t\t\t<key>format</key>",
                  "\t\t\t<integer>3</integer>",
                  "\t\t\t<key>pixelFormat</key>",
                  "\t\t\t<string>RGBA8888</string>",
                  "\t\t\t<key>premultiplyAlpha</key>",
                  "\t\t\t<false/>",
                  "\t\t\t<key>realTextureFileName</key>",
                  "\t\t\t", // To be populated by <string>fileName.png</string>
                  "\t\t\t<key>size</key>",
                  "\t\t\t", // To be populated by <string>{img.Width,img.Height}</string>
                  "\t\t\t<key>smartupdate</key>",
                  "\t\t\t<string>$TexturePacker:SmartUpdate:f04f37b874d8b8386f838f35c29e543d:8606c60fb83119d8c73595d14512ae5f:cd3ab045ff8ec89044c566bf3c4878b7$</string>",
                  "\t\t\t<key>textureFileName</key>",
                  "\t\t\t", // To be populated by <string>fileName.png</string> again
                  "\t\t</dict>",
                  "\t</dict>",
                  "</plist>"
            };

            static readonly string dictStart = "<dict>";
            static readonly string dictEnd = "</dict>";

            internal enum dataType { Unknown, Key, String, Bool }

            public static List<Sprite> BulkDeserialise(List<string> source) {
                  return BulkDeserialise(source);
            }

            public static List<Sprite> BulkDeserialise(string[] source) {
                  List<Sprite> sprites = new List<Sprite>();
                  int i = 0;

                  // Keep searching for the "frames" keyword
                  while (i < source.Length && !source[i].Contains("frames")) {
                        i++;
                  }
                  // Add two more lines to start reading the array of items
                  i += 2;

                  int startLine = i;
                  int endLine = i;

                  int indents = 0;
                  for (; i < source.Length; i++) {
                        string line = source[i].Trim();

                        if (line.StartsWith("<dict>")) {
                              indents++;

                        } else if (line.StartsWith("</dict>")) {
                              indents--;

                              if (indents < 0)
                                    break;

                              endLine = i;

                              // End of the item is here
                              string[] data = source.Trim(startLine, endLine);

                              Sprite sprite = Deserialise(data);
                              sprites.Add(sprite);

                              startLine = i + 1;
                        }
                  }

                  return sprites;
            }

            public static Sprite Deserialise(string[] source) {
                  if (source == null || source.Length == 0)
                        return new Sprite();

                  Sprite sprite = new Sprite();
                  sprite.spriteName = source[0].Trim().GetKey();

                  source = source.Select(s => s.Trim()).ToArray();

                  for (int i = 0; i < source.Length; i++) {
                        string line = source[i].Trim();

                        dataType type = dataType.Unknown;
                        if (line.StartsWith("<key>")) {
                              line = line.GetKey();
                              type = dataType.Key;
                        }

                        if (type == dataType.Key) {
                              if (line.StartsWith("spriteOffset")) {
                                    sprite.spriteOffset = PointExtension.Parse(source[i + 1].GetString());

                              } else if (line.StartsWith("spriteSize")) {
                                    sprite.spriteSize = PointExtension.Parse(source[i + 1].GetString());

                              } else if (line.StartsWith("spriteSourceSize")) {
                                    sprite.spriteSourceSize = PointExtension.Parse(source[i + 1].GetString());

                              } else if (line.StartsWith("textureRect")) {
                                    string data = source[i + 1].GetString();
                                    data = Regex.Replace(data, "[^0-9,]+", "", RegexOptions.Compiled);

                                    string[] vals = data.Split(',');
                                    Array.Resize(ref vals, 4);

                                    int[] values = vals.Select(v => Int32.Parse(v)).ToArray();

                                    sprite.textureRect = new System.Drawing.Rectangle(new System.Drawing.Point(values[0], values[1]), new System.Drawing.Size(values[2], values[3]));

                              } else if (line.StartsWith("textureRotated")) {
                                    if (source[i + 1] == "<true/>")
                                          sprite.textureRotated = true;
                              }
                        }
                  }

                  if (sprite.textureRotated) {
                        sprite.textureRect = new System.Drawing.Rectangle(sprite.textureRect.X, sprite.textureRect.Y, sprite.textureRect.Height, sprite.textureRect.Width);
                  }

                  return sprite;
            }

            static string GetKey(this string line) {
                  return line.Substring(5, line.Length - 11);
            }

            static string GetString(this string line) {
                  return line.Substring(8, line.Length - 17);
            }

            static string ConvertToString(this string value) {
                  return "<string>" + value + "</string>";
            }

            static string ConvertToKey(this string value) {
                  return "<key>" + value + "</key>";
            }

            static string ToStringSimple(this Point value) {
                  return "{" + value.X + "," + value.Y + "}";
            }

            static string ToStringSimple(this Size value) {
                  return "{" + value.Width + "," + value.Height + "}";
            }

            static string ToStringSimple(this Rectangle value) {
                  return "{" + new Point(value.X, value.Y).ToStringSimple() + "," + new Size(value.Width, value.Height).ToStringSimple() + "}";
            }

            static string ToStringSimple(this PackingRectangle value) {
                  return "{" + new Point((int)value.X, (int)value.Y).ToStringSimple() + "," + new Size((int)value.Width, (int)value.Height).ToStringSimple() + "}";
            }

            static string ConvertToString(this Point value) {
                  return "<string>" + value.ToStringSimple() + "</string>";
            }

            static string ConvertToString(this Size value) {
                  return "<string>" + value.ToStringSimple() + "</string>";
            }

            static string ConvertToString(this Rectangle value) {
                  return "<string>" + value.ToStringSimple() + "</string>";
            }

            static string ConvertToString(this PackingRectangle value) {
                  return "<string>" + value.ToStringSimple() + "</string>";
            }

            public static string[] Serialise(Sprite[] sprites, PackingRectangle[] newRects, string fileName, Size bitmapSize) {
                  List<string> ret = new List<string>();
                  ret.AddRange(fileBegin);

                  for (int i = 0; i < sprites.Length; i++) {
                        string[] data = (string[])spriteBase.Clone();

                        data[0] += sprites[i].spriteName.ConvertToKey();
                        data[5] += sprites[i].spriteOffset.ConvertToString();
                        data[7] += sprites[i].spriteSize.ConvertToString();
                        data[9] += sprites[i].spriteSourceSize.ConvertToString();

                        if (sprites[i].textureRotated == true) {
                              data[13] +=  "<true/>";
                              Point point = new Point((int)newRects[i].X, (int)newRects[i].Y);
                              Size size = new Size((int)newRects[i].Height, (int)newRects[i].Width);

                              data[11] += new Rectangle(point, size).ConvertToString();

                        } else {
                              data[13] += "<false/>";
                              data[11] += newRects[i].ConvertToString();
                        }
                        ret.AddRange(data);
                  }

                  // Add the custom metadata
                  string[] ending = (string[]) fileEnd.Clone();
                  ending[11] += (fileName + ".png").ConvertToString();
                  ending[13] += bitmapSize.ConvertToString();
                  ending[18] += (fileName + ".png").ConvertToString();

                  ret.AddRange(ending);
                  return ret.ToArray();
            }
      }
}
