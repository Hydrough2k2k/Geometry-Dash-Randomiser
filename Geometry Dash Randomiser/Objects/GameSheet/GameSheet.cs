using Geometry_Dash_Randomiser.Properties;
using RectpackSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Geometry_Dash_Randomiser {

      public partial class GameSheet {

            [EditorBrowsable(EditorBrowsableState.Never)]
            public GameSheet() { }

            public GameSheet(string fileName) {
                  _fileName = fileName;
                  _quality = GameFilesExtension.GetQualityFromFileName(_fileName);
            }

            public string FileName => _fileName;

            public ResourceSprite[] Sprites {
                  get => _sprites;
                  set {
                        if (value == null) {
                              Logger.Error($"Tried to assign null to Sprite array in {FileName} GameSheet.");

                        } else {
                              _sprites = value;
                              AssignAllSpriteTypes();
                        }
                  }
            }

            public GameFileType FileType {
                  get => _fileType;
                  set {
                        if (value == GameFileType.None) {
                              Logger.Warn($"Tried to assign \"None\" to \"FileType\" in file {FileName}.");

                        } else {
                              _fileType = value;
                        }
                  }
            }

            public Quality Quality => _quality;

            public Size Size {
                  get => _size;
                  set => _size = value;
            }

            /// <summary>
            /// Denotes whether this gamesheet's contents have been altered during randomization<br/>
            /// If it has, it will be repacked and reassembled before being saved to disk<br/>
            /// Currently unused
            /// </summary>
            public bool Altered {
                  get => _altered;
                  set => _altered = value;
            }

            public ResourceSprite[] GetSpritesByType(ResourceType type, int probability, Random random) {
                  return GetSpritesByType(type)
                        .Where(s => random.NextTrue(probability))
                        .ToArray();
            }

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

            public Bitmap AssembleGameSheet() {
                  return GameSheetAssembler.Assemble(this);
            }

            public string[] Serialize() => Plist.Serialise(_sprites, FileName, Size);

            private readonly string _fileName = string.Empty;
            private ResourceSprite[] _sprites = Array.Empty<ResourceSprite>();
            private GameFileType _fileType = GameFileType.None;
            private Quality _quality = Quality.Low;
            private Size _size = new Size();

            private bool _altered = false;
      }
}
