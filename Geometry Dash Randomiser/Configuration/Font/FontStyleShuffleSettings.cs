using System;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      public class FontStyleShuffleSettings {

            public event EventHandler ConfigUpdated;

            public FontStyleShuffleSettings() { }

            public static FontStyleShuffleSettings Default => new FontStyleShuffleSettings();

            public bool Enabled {
                  get => _enabled;
                  set {
                        _enabled = value;
                        InvokeEvent(ConfigUpdated, null);
                  }
            }

            public ShufflingMode Mode {
                  get => _mode;
                  set {
                        if ((int)Mode > maxMode) {
                              Logger.Warn($"The setting for FSSS.Mode was out of expected range");
                              _mode = 0;
                        } else {
                              _mode = value;
                        }

                        InvokeEvent(ConfigUpdated, null);
                  }
            }

            public void SetMode(int modeID) {
                  Mode = (ShufflingMode)modeID;
            }

            public void CopyDataFrom(FontStyleShuffleSettings source) {
                  Enabled = source.Enabled;
                  Mode = source.Mode;
            }

            private void InvokeEvent(EventHandler @event, EventArgs e) {
                  @event?.Invoke(this, e);
            }

            private bool _enabled = true;
            private ShufflingMode _mode = ShufflingMode.PerCharacter;
            private static int maxMode = Enum.GetValues(typeof(ShufflingMode)).Length;

            public enum ShufflingMode {
                  /// <summary>
                  /// Each font's style remains consistent between it's characters when possible, but the styles will be shuffled around
                  /// </summary>
                  PerFont,

                  /// <summary>
                  /// Each character gets a random sprite from a different font
                  /// </summary>
                  PerCharacter,
            }
      }
}
