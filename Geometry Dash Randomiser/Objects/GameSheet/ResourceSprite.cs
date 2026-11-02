using System.Drawing;
using System.Text.Json.Serialization;

namespace Geometry_Dash_Randomiser {

      public class ResourceSprite {

            public ResourceSprite() { }

            /// <summary>
            /// Name of the sprite, for example: "bird_01_001.png"
            /// </summary>
            public string Name {
                  get => _name;
                  set => _name = value ?? string.Empty;
            }

            /// <summary>
            /// The type of the resource. Icon and Resource typer are merged right now, but that might change in the future
            /// </summary>
            public ResourceType Type {
                  get => _type;
                  set => _type = value;
            }

            /// <summary>
            /// The X and Y offset for the sprite.<br/>
            /// This is most likely used by GD internally and is not needed for reading files.<br/>
            /// This also should not be altered except for randomisation purposes
            /// </summary>
            public PointF Offset {
                  get => _offset;
                  set => _offset = value;
            }

            /// <summary>
            /// The real size of the sprite
            /// </summary>
            public Size Size {
                  get => _size;
                  set => _size = value;
            }

            /// <summary>
            /// This value does not sem to change between animation frames, so it's safe to assume it is used by GD internally to determine how big an animation will be.
            /// </summary>
            public Size SourceSize {
                  get => _sourceSize;
                  set => _sourceSize = value;
            }

            /// <summary>
            /// The location and size of the sprite. The Width and Height are always the same as <param name="Size">
            /// </summary>
            public Rectangle TextureRect {
                  get => _textureRect;
                  set {
                        _textureRect = value;
                        _renderRect = GetRenderRect();
                  }
            }

            /// <summary>
            /// Gets the rectangle we need to crop from the full gamesheet.<br/>
            /// If the texture is rotated, we swap the Width and Height values.
            /// </summary>
            public Rectangle CropRect {
                  get => _renderRect;
                  set => _renderRect = value;
            }

            /// <summary>
            /// Whether the texture is rotated in the spritesheet. Don't alter unless you know what you're doing
            /// </summary>
            public bool Rotated {
                  get => _rotated;
                  set {
                        _rotated = value;
                        _renderRect = GetRenderRect();
                  }
            }

            /// <summary>
            /// This is used when reassembling the gamesheet after randomisation
            /// </summary>
            public Rectangle DrawRect {
                  get => _drawRect;
                  set => _drawRect = value;
            }

            /// <summary>
            /// The cropped bitmap for the sprite
            /// </summary>
            public Bitmap Texture { get; set; } = null;

            /// <summary>
            /// Gets the rectangle we need to crop from the full gamesheet.<br/>
            /// If the texture is rotated, we swap the Width and Height values.<br/><br/>
            /// Also adds 1 pixel of padding to each side of the rectangle, since some sprites are missing 1 pixel on either side.<br/>
            /// This seems like the easiest solution for now
            /// </summary>
            private Rectangle GetRenderRect() {
                  if (Rotated)
                        return new Rectangle(TextureRect.X - 1, TextureRect.Y - 1, TextureRect.Height + 2, TextureRect.Width + 2);
                  return new Rectangle(TextureRect.X - 1, TextureRect.Y - 1, TextureRect.Width + 2, TextureRect.Height + 2);
            }

            private string _name = string.Empty;
            private ResourceType _type = ResourceType.Unknown;
            private PointF _offset = new PointF();
            private Size _size = new Size();
            private Size _sourceSize = new Size();
            private Rectangle _textureRect = new Rectangle();
            private Rectangle _renderRect = new Rectangle();
            private bool _rotated = false;
            private Rectangle _drawRect = new Rectangle();
      }
}
