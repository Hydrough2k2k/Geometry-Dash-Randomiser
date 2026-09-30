using System;

namespace Geometry_Dash_Randomiser {

      public class FontStyleShuffleSettings {

            public enum ShufflingMode {
                  /// <summary>
                  /// Each character gets a random sprite from a different font
                  /// </summary>
                  PerLetter,

                  /// <summary>
                  /// Each font's style remains consistent between it's characters when possible, but the styles will be shuffled around
                  /// </summary>
                  PerFont,
            }

            private ShufflingMode _mode = ShufflingMode.PerLetter;

            public FontStyleShuffleSettings() { }

            public bool Enabled { get; set; } = true;

            public ShufflingMode Mode {
                  get { return _mode; }
                  set {
                        _mode = value;

                        int totalValues = Enum.GetValues(typeof(ShufflingMode)).Length;
                        if ((int)Mode > totalValues) {
                              Mode = 0;
                        }
                  }
            }

            public void SetMode(string modeStr) {
                  switch (modeStr) {
                        case "Per Letter":
                              Mode = ShufflingMode.PerLetter;
                              break;

                        case "Per Font":
                              Mode = ShufflingMode.PerFont;
                              break;

                        default:
                              Log.Write(Log.Mode.Error, $"Unrecognised text shuffling mode was received: \"{modeStr}\". Applying default value");
                              Mode = ShufflingMode.PerLetter;
                              break;
                  }
            }
      }
}
