using System;

namespace Geometry_Dash_Randomiser {

      [Serializable] public class RandomisationGroup {

            public int group { get; set; } = 0;
            public bool enabled { get; set; } = false;

            public RandomisationGroup() { }

            public RandomisationGroup(int group, bool enabled) {
                  this.group = group;
                  this.enabled = enabled;
            }
      }
}
