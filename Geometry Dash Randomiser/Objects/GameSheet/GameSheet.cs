using RectpackSharp;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace Geometry_Dash_Randomiser {

      public partial class GameSheet : IDisposable {

            [EditorBrowsable(EditorBrowsableState.Never)]
            public GameSheet() { }

            public GameSheet(string fileName) {
                  _fileName = fileName;
            }

            public string FileName => _fileName;

            public ResourceSprite[] Sprites {
                  get => _sprites;
                  set {
                        if (value == null) {
                              Logger.Error($"Tried to assign null to Sprite array in {FileName} GameSheet.");
                              return;
                        }
                        _sprites = value;
                        AssignAllSpriteTypes();
                  }
            }

            /// <summary>
            /// The type of gamesheet this is. It can only be Icon or Resource type
            /// </summary>
            public GameFileType FileType {
                  get => _fileType;
                  set {
                        if (value == GameFileType.None || value == GameFileType.Font) {
                              Logger.Warn($"Tried to assign {value.ToString()} to \"FileType\" in GameSheet file {FileName}.");
                              return;
                        } 
                        _fileType = value;
                        AssignAllSpriteTypes();
                  }
            }

            /// <summary>
            /// The size of the full gamesheet image
            /// </summary>
            public Size Size {
                  get => _size;
                  set => _size = value;
            }

            /// <summary>
            /// Get every sprite from the sprite array by type and with a certain probability
            /// </summary>
            /// <param name="type">The type of sprite to filter for</param>
            /// <param name="probability">The probability percentage that each sprite will be returned</param>
            /// <param name="random">The random number generator used to get the probability</param>
            /// <returns></returns>
            public ResourceSprite[] GetSpritesByType(ResourceType type, int probability, Random random) {
                  return GetSpritesByType(type)
                        .Where(s => random.NextTrue(probability))
                        .ToArray();
            }

            /// <summary>
            /// Get every sprite from the sprite array by type
            /// </summary>
            /// <param name="type">The type of sprite to filter for</param>
            /// <returns></returns>
            public ResourceSprite[] GetSpritesByType(ResourceType type) {
                  return Sprites
                        .Where(s => s.Type == type)
                        .ToArray(); 
            }

            public static GameSheet Deserialize(string gameSheetName, GameFileType fileType, string[] fileData, Bitmap gameSheetImage) {
                  GameSheet gameSheet = new GameSheet(gameSheetName);
                  gameSheet.FileType = fileType;
                  gameSheet.Sprites = Plist.BulkDeserialize(fileData);
                  gameSheet.Size = new Size(gameSheetImage.Width, gameSheetImage.Height);

                  Rectangle[] rects = gameSheet.Sprites.Select(s => s.CropRect).ToArray();

                  Bitmap[] cropped = gameSheetImage.Multicrop(rects, AdvancedConfig.Instance.TextureCropSize);
                  gameSheetImage.Dispose();

                  for (int i = 0; i < gameSheet.Sprites.Length; i++) {
                        gameSheet.Sprites[i].Texture = cropped[i];
                  }

                  return gameSheet;
            }

            /// <summary>
            /// Using Thomas Miz's algorithm, every sprite in this GameSheet will get relocated, and the sprites' coordinates will get updated as well
            /// </summary>
            public void Repack() {
                  PackingRectangle[] rects = Sprites
                        .Select((s, i) => new { s, i })
                        .Select(pair => new PackingRectangle(
                              (uint)pair.s.CropRect.X, (uint)pair.s.CropRect.Y,
                              (uint)pair.s.CropRect.Width, (uint)pair.s.CropRect.Height,
                              pair.i)
                        )
                        .ToArray();

                  RectanglePacker.Pack(rects, out PackingRectangle bounds);
                  Size = new Size((int)bounds.Width, (int)bounds.Height);

                  for (int s = 0; s < Sprites.Length; s++) {
                        Sprites[s].DrawRect = new Rectangle(
                              (int)rects[s].X, (int)rects[s].Y,
                              (int)rects[s].Width, (int)rects[s].Height
                        );
                  }
            }

            /// <summary>
            /// Assemble the Bitmap associated with this gamesheet. It will contain all sprite's textures and put them onto one image file
            /// </summary>
            /// <returns>The assembled Bitmap</returns>
            public Bitmap AssembleGameSheet() {
                  this.Repack();
                  return GameSheetAssembler.Assemble(this);
            }

            /// <summary>
            /// Gets a clone of the GameSheet instance
            /// </summary>
            /// <param name="cloneTextures">This determines if the Sprite's textures will be made into unique clones.<br/>
            /// Doing so will make the cloning slower</param>
            /// <returns>The cloned bitmap</returns>
            public GameSheet DeepCopy(bool cloneTextures = false) {
                  GameSheet gs = new GameSheet(this.FileName);

                  gs.Sprites = this.Sprites.Select(s => s.DeepCopy(cloneTextures)).ToArray();
                  gs.FileType = this.FileType;
                  gs.Size = this.Size;

                  return gs;
            }

            /// <summary>
            /// Dispose of all texture Bitmaps to clear them from memory. This will set them all to null
            /// </summary>
            public void Dispose() {
                  for (int i = 0; i < _sprites.Length; i++) {
                        _sprites[i].Dispose();
                  }
            }

            /// <summary>
            /// Serialize this instance of GameSheet to text form
            /// </summary>
            /// <returns>The serialized gamesheet. The data will be returned as "plist" data</returns>
            public string[] Serialize() => Plist.Serialise(_sprites, FileName, Size);

            private readonly string _fileName = string.Empty;
            private ResourceSprite[] _sprites = Array.Empty<ResourceSprite>();
            private GameFileType _fileType = GameFileType.None;
            private Size _size = new Size();
      }
}
