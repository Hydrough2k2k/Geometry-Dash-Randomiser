using System;
using System.ComponentModel;

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

            private string _fileName = string.Empty;
            private ResourceSprite[] _sprites = Array.Empty<ResourceSprite>();
            private GameFileType _fileType = GameFileType.None;
            private Quality _quality = Quality.Low;
      }
}
