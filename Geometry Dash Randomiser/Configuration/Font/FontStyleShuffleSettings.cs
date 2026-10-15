using System;

namespace Geometry_Dash_Randomiser {

      public class FontStyleShuffleSettings {

            public enum ShufflingMode {
                  /// <summary>
                  /// Each font's style remains consistent between it's characters when possible, but the styles will be shuffled around
                  /// </summary>
                  PerFont,

                  /// <summary>
                  /// Each character gets a random sprite from a different font
                  /// </summary>
                  PerLetter,
            }

            private ShufflingMode _mode = ShufflingMode.PerFont;

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

            public void SetMode(int mode) {
                  Mode = (ShufflingMode)mode;
            }
      }
}
