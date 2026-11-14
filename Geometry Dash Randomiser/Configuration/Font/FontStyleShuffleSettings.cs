using System;

namespace Geometry_Dash_Randomiser {

      public class FontStyleShuffleSettings : IUpdateNotify, IEquatable<FontStyleShuffleSettings> {

            public event EventHandler Updated;

            public FontStyleShuffleSettings() { }

            public static FontStyleShuffleSettings Default => new FontStyleShuffleSettings();

            public bool Enabled {
                  get => _enabled;
                  set {
                        _enabled = value;
                        InvokeEvent(Updated, null);
                  }
            }

            public ShufflingMode Mode {
                  get => _mode;
                  set {
                        if ((int)value < 0 || (int)value >= maxMode) {
                              Logger.Warn($"The setting for FSSS.Mode was out of expected range");
                              _mode = 0;

                        } else {
                              _mode = value;
                        }

                        InvokeEvent(Updated, null);
                  }
            }

            public void SetMode(int modeID) {
                  Mode = (ShufflingMode)modeID;
            }

            public void CopyDataFrom(FontStyleShuffleSettings source) {
                  if (source == null) {
                        Logger.Write(Logger.Mode.Error, "The parameter \"source\" was passed as null in FontStyleShuffleSettings.CopyDataFrom");
                        return;
                  }

                  Enabled = source.Enabled;
                  Mode = source.Mode;
            }
            
            public bool Equals(FontStyleShuffleSettings other) {
                  if (other == null) {
                        Logger.Write(Logger.Mode.Error, "The parameter \"other\" was passed as null in IconRandomizationSettings.Equals");
                        return false;
                  }

                  if (this.Enabled != other.Enabled ||
                        this.Mode != other.Mode) {

                        return false;
                  }

                  return true;
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
