using System;

namespace Geometry_Dash_Randomiser {

      public class LetterSpacingSettings {

            public event EventHandler ConfigUpdated;

            public LetterSpacingSettings() {
                  _xOffset.ConfigUpdated += InvokeConfigUpdateEvent;
                  _kerning.ConfigUpdated += InvokeConfigUpdateEvent;
            }

            public bool Enabled {
                  get => _enabled;
                  set {
                        _enabled = value;
                        InvokeEvent(ConfigUpdated, null);
                  }
            }

            public SpacingMode Mode {
                  get => _mode;
                  set {
                        if ((int)Mode > maxMode) {
                              Logger.Warn($"The setting for LSS.Mode was out of expected range");
                              _mode = 0;
                        } else {
                              _mode = value;
                        }
                        InvokeEvent(ConfigUpdated, null);
                  }
            }

            // Simple Mode
            public int Level {
                  get => _level;
                  set {
                        value.Clamp(minLevel, maxLevel);
                        InvokeEvent(ConfigUpdated, null);
                  }
            }
            
            // Advanced Mode
            public CappedRange X_Offset {
                  get => _xOffset;
                  set {
                        if (value != null) {
                              _xOffset.CopyDataFrom(value);

                        } else {
                              Logger.Error("LSS.X_Offset was passed as null. Setting value to default");
                              value = CappedRange.Default;
                        }
                        InvokeEvent(ConfigUpdated, null);
                  }
            }

            public CappedRange Kerning {
                  get => _kerning;
                  set {
                        if (value != null) {
                              _kerning.CopyDataFrom(value);

                        } else {
                              Logger.Error("LSS.Kerning was passed as null. Setting value to default");
                              value = CappedRange.Default;
                        }
                        InvokeEvent(ConfigUpdated, null);
                  }
            }

            public void SetMode(int modeID) {
                  Mode = (SpacingMode)modeID;
            }

            /// <summary>
            /// Sets 4 different values based on the index:<br/>
            /// 0: X_Offset.Min<br/>
            /// 1: X_Offset.Max<br/>
            /// 2: Kerning.Min<br/>
            /// 3: Kerning.Max
            /// </summary>
            public void SetRangeValue(string value, int index) {
                  bool success = Int32.TryParse(value, out Int32 castValue);
                  if (!success) {
                        Logger.Write(Logger.Mode.Warn, $"Casting value {value} to Int32 failed in \"LetterSpacingSettings\". Index: {index}");
                        return;
                  }

                  switch (index) {
                        case 0:
                              X_Offset.Min = castValue;
                              break;

                        case 1:
                              X_Offset.Max = castValue;
                              break;

                        case 2:
                              Kerning.Min = castValue;
                              break;

                        case 3:
                              Kerning.Max = castValue;
                              break;

                        default:
                              Logger.Write(Logger.Mode.Error, $"Index {index} was out of range for setting range value for \"LetterSpacingSettings\"");
                              break;
                  }
            }

            public void CopyDataFrom(LetterSpacingSettings source) {
                  X_Offset.CopyDataFrom(source.X_Offset);
                  Kerning.CopyDataFrom(source.Kerning);
            }

            private void InvokeConfigUpdateEvent(object sender, EventArgs e) => InvokeEvent(ConfigUpdated, null);

            private void InvokeEvent(EventHandler @event, EventArgs e) {
                  @event?.Invoke(this, e);
            }

            private bool _enabled = true;
            private SpacingMode _mode = SpacingMode.Simple;

            private int _level = 1;

            private readonly CappedRange _xOffset = CappedRange.Default;
            private readonly CappedRange _kerning = CappedRange.Default;

            private static int maxMode = Enum.GetValues(typeof(SpacingMode)).Length;

            private static int minLevel = 1;
            private static int maxLevel = 10;

            public enum SpacingMode { Simple, Advanced }
      }
}
