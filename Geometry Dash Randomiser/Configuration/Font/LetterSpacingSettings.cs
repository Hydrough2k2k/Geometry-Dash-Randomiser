using System;

namespace Geometry_Dash_Randomiser {

      public class LetterSpacingSettings {

            public enum SpacingMode { Simple = 0, Advanced = 1 }

            public LetterSpacingSettings() { }

            public bool Enabled { get; set; }

            public SpacingMode Mode {
                  get { return _mode; }
                  set {
                        if ((int)value < 0) {
                              _mode = SpacingMode.Simple;

                        } else if (value > maxSpacingMode) {
                              _mode = maxSpacingMode;

                        } else {
                              _mode = value;
                        }
                  }
            }

            public SimpleMode Simple {
                  get { return _simple; }
                  set { _simple = value ?? new SimpleMode(); }
            }

            public AdvancedMode Advanced {
                  get { return _advanced; }
                  set { _advanced = value ?? new AdvancedMode(); }
            }

            public void SetMode(int mode) {
                  Mode = (SpacingMode)mode;
            }

            public Font.LetterSpacing GetAdvancedLetterSpacing() {
                  return new Font.LetterSpacing(Advanced.X_Offset.Min, Advanced.X_Offset.Max, Advanced.Kerning.Min, Advanced.Kerning.Max, 100);
            }

            /// <summary>
            /// Sets 4 different values based on the index:<br/>
            /// 2: X_Offset.Min<br/>
            /// 3: X_Offset.Max
            /// 0: Kerning.Min<br/>
            /// 1: Kerning.Max<br/>
            /// </summary>
            public void SetRangeValue(string value, int index) {
                  bool success = Int32.TryParse(value, out Int32 castValue);
                  if (!success) {
                        Logger.Write(Logger.Mode.Warn, $"Casting value {value} to Int32 failed in \"LetterSpacingSettings\".");
                        return;
                  }

                  switch (index) {
                        case 0:
                              Advanced.X_Offset.Min = castValue;
                              break;

                        case 1:
                              Advanced.X_Offset.Max = castValue;
                              break;

                        case 2:
                              Advanced.Kerning.Min = castValue;
                              break;

                        case 3:
                              Advanced.Kerning.Max = castValue;
                              break;

                        default:
                              Logger.Write(Logger.Mode.Error, $"Index {index} was out of range for setting range value for \"LetterSpacingSettings\"");
                              break;
                  }
            }

            public class CappedRange {

                  private int _min;
                  private int _max;

                  public CappedRange() { }

                  public CappedRange(int min, int max) {
                        Min = min;
                        Max = max;
                  }

                  public CappedRange(int min, int max, int minCap, int maxCap) {
                        Min = min;
                        Max = max;
                        _minimumCap = minCap;
                        _maximumCap = maxCap;
                  }

                  public static CappedRange Default => new CappedRange();

                  public int Min {
                        get { return _min; }
                        set {
                              _min = value.Clamp(_minimumCap, _maximumCap);

                              if (_min > _max)
                                    _min = _max;
                        }
                  }

                  public int Max {
                        get { return _max; }
                        set {
                              _max = value.Clamp(_minimumCap, _maximumCap);

                              if (_max < _min)
                                    _max = _min;
                        }
                  }

                  private readonly int _minimumCap = -128;
                  private readonly int _maximumCap = 127;
            }

            public class SimpleMode {

                  public SimpleMode() { }

                  public int Level {
                        get { return _level; }
                        set {
                              if (value < minInsanityLevel) {
                                    Logger.Warn("LSS.Level was smaller than the minimum allowed level");
                                    _level = minInsanityLevel;

                              } else if (value > maxInsanityLevel) {
                                    Logger.Warn("LSS.Level was greater than the maximum allowed level");
                                    _level = maxInsanityLevel;

                              } else {
                                    _level = value;
                              }
                        }
                  }

                  private int _level = minInsanityLevel;

                  private const int maxInsanityLevel = 10;
                  private const int minInsanityLevel = 1;
            }

            public class AdvancedMode {

                  public AdvancedMode() { }

                  public CappedRange X_Offset {
                        get { return _x_Offset; }
                        set {
                              if (value == null) {
                                    Logger.Warn("LSS.Advanced.X_Offset set was passed as null");
                                    _x_Offset = CappedRange.Default;

                              } else {
                                    _x_Offset = value;
                              }
                        }
                  }

                  public CappedRange Kerning {
                        get { return _kerning; }
                        set {
                              if (value == null) {
                                    Logger.Warn("LSS.Advanced.Kerning set was passed as null");
                                    _kerning = CappedRange.Default;

                              } else {
                                    _kerning = value;
                              }
                        }
                  }

                  private CappedRange _kerning = new CappedRange();
                  private CappedRange _x_Offset = new CappedRange();
            }

            private SpacingMode _mode = SpacingMode.Simple;

            private SimpleMode _simple = new SimpleMode();
            private AdvancedMode _advanced = new AdvancedMode();

            private const SpacingMode maxSpacingMode = SpacingMode.Advanced;
      }
}
