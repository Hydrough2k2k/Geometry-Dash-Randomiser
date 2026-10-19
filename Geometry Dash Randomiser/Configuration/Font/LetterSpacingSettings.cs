using System;
using static Geometry_Dash_Randomiser.FontStyleShuffleSettings;

namespace Geometry_Dash_Randomiser {

      public class LetterSpacingSettings {

            public enum SpacingMode { Simple, Advanced }

            public LetterSpacingSettings() { }

            public bool Enabled { get; set; }
            public SpacingMode Mode {
                  get => _mode;
                  set {
                        if ((int)Mode > maxMode) {
                              Logger.Warn($"The setting for LSS.Mode was out of expected range");
                              _mode = 0;
                        } else {
                              _mode = value;
                        }
                  }
            }

            // Simple Mode
            public int Level {
                  get => _level;
                  set => value.Clamp(minLevel, maxLevel);
            }
            
            // Advanced Mode
            public CappedRange X_Offset {
                  get => _xOffset;
                  set {
                        if (value == null) {
                              Logger.Error("LSS.X_Offset was passed as null");
                              value = CappedRange.Default;

                        } else {
                              _xOffset = value;
                        }
                  }
            }
            public CappedRange Kerning {
                  get => _kerning;
                  set {
                        if (value == null) {
                              Logger.Error("LSS.Kerning was passed as null");
                              value = CappedRange.Default;

                        } else {
                              _kerning = value;
                        }
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
                        Logger.Write(Logger.Mode.Warn, $"Casting value {value} to Int32 failed in \"LetterSpacingSettings\".");
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

            private SpacingMode _mode = SpacingMode.Simple;

            private int _level = 1;

            private CappedRange _xOffset = CappedRange.Default;
            private CappedRange _kerning = CappedRange.Default;

            private static int maxMode = Enum.GetValues(typeof(SpacingMode)).Length;

            private static int minLevel = 1;
            private static int maxLevel = 10;
      }
}
