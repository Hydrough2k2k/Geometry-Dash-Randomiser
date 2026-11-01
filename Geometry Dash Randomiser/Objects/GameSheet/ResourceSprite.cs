using System.Drawing;

namespace Geometry_Dash_Randomiser {

      public class ResourceSprite {

            public ResourceSprite() { }

            /// <summary>
            /// Name of the sprite, for example: "bird_01_001.png"
            /// </summary>
            public string Name { get; set; } = string.Empty;

            /// <summary>
            /// The type of the resource. Icon and Resource typer are merged right now, but that might change in the future
            /// </summary>
            public ResourceType Type { get; set; } = ResourceType.Unknown;

            /// <summary>
            /// The X and Y offset for the sprite. The usage of this is unlear to me at the moment
            /// </summary>
            public PointF Offset { get; set; } = new Point();

            /// <summary>
            /// The real size of the sprite
            /// </summary>
            public Size Size { get; set; } = new Size();

            /// <summary>
            /// This value does not sem to change between animation frames, so it's safe to assume it is used by GD internally to determine how big an animation will be.
            /// </summary>
            public Size SourceSize { get; set; } = new Size();

            /// <summary>
            /// The location and size of the sprite. The Width and Height are always the same as <param name="Size">
            /// </summary>
            public Rectangle TextureRect { get; set; } = new Rectangle();

            /// <summary>
            /// Gets the rectangle we need to crop from the full gamesheet.<br/>
            /// If the texture is rotated, we swap the Width and Height values
            /// </summary>
            public Rectangle CroppingRect => GetCroppingRect();

            /// <summary>
            /// Whether the texture is rotated in the spritesheet. Don't alter unless you know what you're doing
            /// </summary>
            public bool Rotated { get; set; } = false;

            /// <summary>
            /// The cropped bitmap for the sprite
            /// </summary>
            public Bitmap Texture = null;

            /// <summary>
            /// Gets the rectangle we need to crop from the full gamesheet.<br/>
            /// If the texture is rotated, we swap the Width and Height values
            /// </summary>
            private Rectangle GetCroppingRect() {
                  if (Rotated)
                        return new Rectangle(TextureRect.X, TextureRect.Y, TextureRect.Height, TextureRect.Width);
                  return TextureRect;
            }
      }
}
