using Geometry_Dash_Randomiser.Properties;
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

            public Quality Quality {
                  get => _quality;
            }

            public Size Size {
                  get => _size;
                  set => _size = value;
            }

            public Bitmap Resource { get; set; }

            public static GameSheet Deserialize(string gameSheetName, GameFileType fileType, string[] fileData, Bitmap gameSheetImage) {
                  GameSheet gameSheet = new GameSheet(gameSheetName);
                  gameSheet.FileType = fileType;
                  gameSheet.Sprites = Plist.BulkDeserialize(fileData);
                  gameSheet.Size = new Size(gameSheetImage.Width, gameSheetImage.Height);

                  Rectangle[] rects = gameSheet.Sprites.Select(s => s.CroppingRect).ToArray();
                  Bitmap[] cropped = gameSheetImage.Multicrop(rects, AdvancedConfig.Instance.TextureCropSize);
                  gameSheetImage.Dispose();

                  for (int i = 0; i < gameSheet.Sprites.Length; i++) {
                        gameSheet.Sprites[i].Texture = cropped[i];
                  }

                  return gameSheet;
            }

            public Bitmap AssembleGameSheet() {
                  return GameSheetAssembler.Assemble(this);
            }

            public string[] Serialize() => Plist.Serialise(_sprites, FileName, Size);

            private string _fileName = string.Empty;
            private ResourceSprite[] _sprites = Array.Empty<ResourceSprite>();
            private GameFileType _fileType = GameFileType.None;
            private Quality _quality = Quality.Low;
            private Size _size = new Size();
      }
}
