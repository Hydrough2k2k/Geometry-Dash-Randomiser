using System;
using static Geometry_Dash_Randomiser.Font;

namespace Geometry_Dash_Randomiser {

      public class LetterSpacingSettings : IUpdateNotify {

            public event EventHandler Updated;

            public LetterSpacingSettings() {
                  _xOffset.Updated += InvokeConfigUpdateEvent;
                  _kerning.Updated += InvokeConfigUpdateEvent;
            }

            public static LetterSpacingSettings Default => new LetterSpacingSettings();

            public bool Enabled {
                  get => _enabled;
                  set {
                        _enabled = value;
                        InvokeEvent(Updated, null);
                  }
            }

            public SpacingMode Mode {
                  get => _mode;
                  set {
                        _mode = (SpacingMode)(Math.Clamp((int)value, 0, maxMode));
                        InvokeEvent(Updated, null);
                  }
            }

            // Simple Mode
            public int Level {
                  get => _level;
                  set {
                        _level = Math.Clamp(value, minLevel, maxLevel);
                        InvokeEvent(Updated, null);
                  }
            }
            
            // Advanced Mode
            public CappedRange X_Offset {
                  get => _xOffset;
                  set {
                        _xOffset.CopyDataFrom(value ?? CappedRange.Default);
                        InvokeEvent(Updated, null);
                  }
            }

            public CappedRange Kerning {
                  get => _kerning;
                  set {
                        _kerning.CopyDataFrom(value ?? CappedRange.Default);
                        InvokeEvent(Updated, null);
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

            public KerningData GetKerningData() {
                  switch (this.Mode) {
                        case LetterSpacingSettings.SpacingMode.Simple:
                              return KerningData.GetSimpleKerningDataFromLevel(this.Level);

                        case LetterSpacingSettings.SpacingMode.Advanced:
                              return this.GetAdvancedKerningData();

                        default:
                              Logger.Warn($"Unrecognised Letter Spacing Mode: {this.Mode.ToString()}. Defaulting to Simple");
                              goto case LetterSpacingSettings.SpacingMode.Simple;
                  }
            }

            public KerningData GetAdvancedKerningData() {
                  return new KerningData(X_Offset, Kerning, 100);
            }

            private void InvokeConfigUpdateEvent(object sender, EventArgs e) => InvokeEvent(Updated, null);

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

            public class KerningData {

                  public int xOffsetMin;
                  public int xOffsetMax;
                  public int kerningMin;
                  public int kerningMax;
                  public int probability;

                  public KerningData() { }

                  public KerningData(int x_OffsetMin, int x_OffsetMax, int kerningMin, int kerningMax, int probabbility) {
                        this.xOffsetMin = x_OffsetMin;
                        this.xOffsetMax = x_OffsetMax;
                        this.kerningMin = kerningMin;
                        this.kerningMax = kerningMax;
                        probability = probabbility;
                  }

                  public KerningData(CappedRange x_Offset, CappedRange kerning, int probabbility) {
                        xOffsetMin = x_Offset.Min;
                        xOffsetMax = x_Offset.Max;
                        kerningMin = kerning.Min;
                        kerningMax = kerning.Max;
                        probability = probabbility;
                  }

                  public static KerningData Default => new KerningData();

                  public static KerningData GetSimpleKerningDataFromLevel(int level) {
                        if (level < 0 || level > letterKerningLevels.Length) {
                              Logger.Error($"Invalid kerning level was passed: {level}");
                              return KerningData.Default;
                        }

                        return letterKerningLevels[level];
                  }

                  private static KerningData[] letterKerningLevels = new KerningData[] {
                        new KerningData(-3, 5, -3, 5, 10),
                        new KerningData(-5, 8, -5, 8, 25),
                        new KerningData(-10, 15, -10, 15, 40),
                        new KerningData(-20, 30, -20, 30, 55),
                        new KerningData(-35, 50, -35, 50, 70),
                        new KerningData(-40, 60, -40, 60, 80),
                        new KerningData(-50, 75, -50, 75, 90),
                        new KerningData(-65, 100, -65, 100, 100),
                        new KerningData(-80, 120, -80, 120, 100),
                        new KerningData(-100, 150, -100, 150, 100)
                  };
            }
      }
}
