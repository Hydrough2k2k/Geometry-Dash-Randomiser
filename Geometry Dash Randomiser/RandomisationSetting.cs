using System;

namespace Geometry_Dash_Randomiser {

      [Serializable]
      public class RandomisationSetting {

            public int group { get; set; } = 0;
            public bool enabled { get; set; } = false;

            public RandomisationSetting() { }

            public RandomisationSetting(int group, bool enabled) {
                  this.group = group;
                  this.enabled = enabled;
            }

            public void CopyDataFrom(RandomisationSetting source) {
                  this.group = source.group;
                  this.enabled = source.enabled;
            }

            public bool isEnabledAndGroupIsZero() => enabled == true && group == 0;

            public bool IsEnabledAndGroupIs(int group) {
                  return enabled == true && group == this.group;
            }

            public virtual void Validate() {
                  if (this.group > Config.maxGroups)
                        this.group = Config.maxGroups;
            }

            public virtual string GetStatusHex() {
                  return ((Convert.ToInt32(enabled) << 7) + group).ToString("X2");
            }

            public virtual void ApplyConfigFromHex(string hex) {
                  Int32.TryParse(hex, style: System.Globalization.NumberStyles.HexNumber, null, out int result);
                  ApplyConfigFromValue(result);
            }

            public virtual void ApplyConfigFromValue(int input) {
                  // Get the first bit and apply it to enabled, the rest to group
                  enabled = (input & 0x80) != 0;
                  group = input & 0x7F;
            }
      }
}
