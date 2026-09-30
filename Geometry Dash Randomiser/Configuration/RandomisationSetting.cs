using System;

namespace Geometry_Dash_Randomiser {

      [Serializable]
      public class RandomisationSetting : IToggleableSetting {

            private bool _enabled;
            private int _group;

            public RandomisationSetting() { }

            public RandomisationSetting(int group, bool enabled) {
                  this.Group = group;
                  this.Enabled = enabled;
            }

            internal virtual int MaxGroupNumber {get; set; } = RandomisationConfig.maxTextureGroups;

            public virtual bool Enabled {
                  get {
                        return _enabled;
                  }
                  set {
                        _enabled = value;
                  }
            }

            public virtual int Group {
                  get {
                        return _group;
                  }
                  set {
                        if (value < 0)
                              _group = 0;
                        if (value > MaxGroupNumber)
                              _group = MaxGroupNumber;
                        else
                              _group = value;
                  }
            }

            public virtual int TotalSettingsCount => 1;

            public virtual int GetEnabledSettingsCount() {
                  return Convert.ToInt32(Enabled);
            }

            public virtual bool AnySettingEnabled() {
                  return Enabled;
            }

            public void CopyDataFrom(RandomisationSetting source) {
                  this.Group = source.Group;
                  this.Enabled = source.Enabled;
            }

            public bool IsEnabledAndGroupIsZero() => Enabled == true && Group == 0;

            public bool IsEnabledAndGroupIs(int group) {
                  return Enabled == true && group == this.Group;
            }
      }
}
