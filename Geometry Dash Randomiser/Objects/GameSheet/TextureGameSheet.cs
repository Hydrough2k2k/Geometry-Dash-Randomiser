using System;
using System.Collections.Generic;

namespace Geometry_Dash_Randomiser.Objects.GameSheet {

      internal class TextureGameSheet : GameSheet {

            internal TextureGameSheet(GameFileType type) { }

            public List<Sprite> sprites { get; private set; } = new List<Sprite>();
      }
}
