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

            public FontStyleShuffleSettings() { }

            public bool Enabled { get; set; } = true;

            public ShufflingMode Mode {
                  get => _mode;
                  set {
                        if ((int)Mode > maxMode) {
                              Logger.Warn($"The setting for FSSS.Mode was out of expected range");
                              _mode = 0;
                        } else {
                              _mode = value;
                        }
                  }
            }

            public void SetMode(int modeID) {
                  Mode = (ShufflingMode)modeID;
            }

            private ShufflingMode _mode = ShufflingMode.PerLetter;

            private static int maxMode = Enum.GetValues(typeof(ShufflingMode)).Length;
      }
}
