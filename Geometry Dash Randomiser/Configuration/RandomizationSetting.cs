using System;
using System.Text.Json.Serialization;

namespace Geometry_Dash_Randomiser {

      [Serializable]
      public class RandomizationSetting : IToggleableSetting {

            public event EventHandler ConfigUpdated;

            private bool _enabled;
            private int _group;
            private int _percentageRandomised = 100;

            public RandomizationSetting() { }

            public RandomizationSetting(int group, bool enabled) {
                  this.Group = group;
                  this.Enabled = enabled;
            }
            
            public RandomizationSetting(int group, bool enabled, int percentageRandomised) {
                  this.Group = group;
                  this.Enabled = enabled;
                  this.PercentageRandomised = percentageRandomised;
            }

            internal virtual int MaxGroupNumber {get; set; } = RandomisationConfig.maxTextureGroups;

            public virtual int Group {
                  get { return _group; }
                  set {
                        _group = MathExt.Clamp(value, 0, MaxGroupNumber);
                        ConfigUpdated?.Invoke(this, null);
                  }
            }

            public virtual bool Enabled {
                  get { return _enabled; }
                  set {
                        _enabled = value;
                        ConfigUpdated?.Invoke(this, null);
                  }
            }

            public virtual int PercentageRandomised {
                  get { return _percentageRandomised; }
                  set {
                        _percentageRandomised = MathExt.Clamp(value, 0, 100);
                        ConfigUpdated?.Invoke(this, null);
                  }
            }

            [JsonIgnore]
            public virtual int TotalSettingsCount => 1;

            public virtual int GetEnabledSettingsCount() {
                  return Convert.ToInt32(Enabled);
            }

            public virtual bool AnySettingEnabled() {
                  return Enabled;
            }

            public void CopyDataFrom(RandomizationSetting source) {
                  this.Group = source.Group;
                  this.Enabled = source.Enabled;
                  this.PercentageRandomised = source.PercentageRandomised;
            }

            public bool IsEnabledAndGroupIsZero() => Enabled == true && Group == 0;

            public bool IsEnabledAndGroupIs(int group) {
                  return Enabled == true && group == this.Group;
            }
      }
}
