using System;

namespace Geometry_Dash_Randomiser {

      [Serializable] public class RandomisationSetting {

            public int group { get; set; } = 0;
            public bool enabled { get; set; } = false;

            public RandomisationSetting() { }

            public RandomisationSetting(int group, bool enabled) {
                  this.group = group;
                  this.enabled = enabled;
            }

            public bool isEnabledAndGroupIsZero() => enabled == true && group == 0;

            public bool IsEnabledAndGroupIs(int group) {
                  return enabled == true && group == this.group;
            }
      }
}
