using System;

namespace Geometry_Dash_Randomiser {

      public class LetterSpacingSettings {

            public enum SpacingMode { Simple, Advanced }

            public class Range {

                  private const int _minimum = -128;
                  private const int _maximum = 127;

                  private int _min;
                  private int _max;

                  public Range(int min, int max) {
                        Min = min;
                        Max = max;
                  }

                  public int Min {
                        get { return _min; }
                        set {
                              _min = value.Clamp(_minimum, _maximum);

                              if (_min > _max)
                                    _min = _max;
                        }
                  }

                  public int Max {
                        get { return _max; }
                        set {
                              _max = value.Clamp(_minimum, _maximum);

                              if (_max < _min)
                                    _max = _min;
                        }
                  }
            }

            public LetterSpacingSettings() { }

            public bool Enabled { get; set; }
            public SpacingMode Mode { get; set; }

            // Simple Mode
            public int Level { get; set; }
            
            // Advanced Mode
            public Range Kerning { get; set; }
            public Range X_Offset { get; set; }

            public void SetMode(string modeStr) {
                  switch (modeStr) {
                        case "Simple":
                              Mode = SpacingMode.Simple;
                              break;

                        case "Advanced":
                              Mode = SpacingMode.Advanced;
                              break;

                        default:
                              Log.Write(Log.Mode.Error, $"Unrecognised text spacing mode was received: \"{modeStr}\". Applying default value");
                              Mode = SpacingMode.Simple;
                              break;
                  }
            }

            /// <summary>
            /// Sets 4 different values based on the index:<br/>
            /// 0: Kerning.Min<br/>
            /// 1: Kerning.Max<br/>
            /// 2: X_Offset.Min<br/>
            /// 3: X_Offset.Max
            /// </summary>
            public void SetRangeValue(string value, int index) {
                  bool success = Int32.TryParse(value, out Int32 castValue);
                  if (!success) {
                        Log.Write(Log.Mode.Warn, $"Casting value {value} to Int32 failed in \"LetterSpacingSettings\".");
                        return;
                  }

                  switch (index) {
                        case 0:
                              Kerning.Min = castValue;
                              break;

                        case 1:
                              Kerning.Max = castValue;
                              break;

                        case 2:
                              X_Offset.Min = castValue;
                              break;

                        case 3:
                              X_Offset.Max = castValue;
                              break;

                        default:
                              Log.Write(Log.Mode.Error, $"Index {index} was out of range for setting range value for \"LetterSpacingSettings\"");
                              break;
                  }
            }
      }
}
