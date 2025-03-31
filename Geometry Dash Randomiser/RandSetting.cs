using System;

namespace Geometry_Dash_Randomiser {

      [Serializable] public class RandSetting {

            public int group { get; set; } = 0;
            public bool enabled { get; set; } = false;

            public RandSetting() { }

            public RandSetting(int group, bool enabled) {
                  this.group = group;
                  this.enabled = enabled;
            }

            public RandSetting(bool enabled, int group) {
                  this.group = group;
                  this.enabled = enabled;
            }

            public bool isEnabledAndGroupIsZero() => enabled == true && group == 0;

            public bool IsEnabledAndGroupMatches(int group) {
                  return enabled == true && group == this.group;
            }
      }
}
