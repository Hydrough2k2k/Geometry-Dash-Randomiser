using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Geometry_Dash_Randomiser {

      [Serializable]
      public class RandomizationSetting : IToggleableSetting, IUpdateNotify, ICopyableSetting<RandomizationSetting>, IEquatable<RandomizationSetting> {

            public event EventHandler Updated;

            [EditorBrowsable(EditorBrowsableState.Never)]
            public RandomizationSetting() { }

            public RandomizationSetting(ResourceType associatedType) {
                  this._associatedType = associatedType;
            }

            public RandomizationSetting(int group, bool enabled, ResourceType associatedType) {
                  this.Group = group;
                  this.Enabled = enabled;
                  this._associatedType = associatedType;
            }
            
            public RandomizationSetting(int group, bool enabled, int percentageRandomised, ResourceType associatedType) {
                  this.Group = group;
                  this.Enabled = enabled;
                  this.PercentageRandomised = percentageRandomised;
                  this._associatedType = associatedType;
            }

            internal virtual int MaxGroupNumber {get; set; } = RandomizationConfig.maxTextureGroups;

            public virtual int Group {
                  get { return _group; }
                  set {
                        _group = Math.Clamp(value, 0, MaxGroupNumber);
                        Updated?.Invoke(this, null);
                  }
            }

            public virtual bool Enabled {
                  get { return _enabled; }
                  set {
                        _enabled = value;
                        Updated?.Invoke(this, null);
                  }
            }

            public virtual int PercentageRandomised {
                  get { return _percentageRandomised; }
                  set {
                        _percentageRandomised = Math.Clamp(value, 0, 100);
                        Updated?.Invoke(this, null);
                  }
            }

            [JsonIgnore]
            public virtual ResourceType AssociatedType => _associatedType;

            [JsonIgnore]
            public virtual int TotalSettingsCount => 1;

            public virtual int GetEnabledSettingsCount() {
                  return Convert.ToInt32(Enabled);
            }

            public virtual bool AnySettingEnabled() {
                  return Enabled;
            }

            public void CopyDataFrom(RandomizationSetting source) {
                  if (source == null) {
                        Logger.Write(Logger.Mode.Error, "The parameter \"source\" was passed as null in RandomizationSetting.CopyDataFrom");
                        return;
                  }

                  this.Group = source.Group;
                  this.Enabled = source.Enabled;
                  this.PercentageRandomised = source.PercentageRandomised;
            }

            public bool Equals(RandomizationSetting other) {
                  if (other == null) {
                        Logger.Write(Logger.Mode.Error, "The parameter \"other\" was passed as null in RandomizationSetting.Equals");
                        return false;
                  }

                  if (this.Enabled != other.Enabled ||
                        this.Group != other.Group ||
                        this.PercentageRandomised != other._percentageRandomised) {

                        return false;
                  }
                  return true;
            }

            public bool IsEnabledAndGroupIsZero() => Enabled == true && Group == 0;

            public bool IsEnabledAndGroupIs(int group) {
                  return Enabled == true && group == this.Group;
            }

            private bool _enabled = true;
            private int _group = 0;
            private int _percentageRandomised = 100;
            private readonly ResourceType _associatedType = ResourceType.Unknown;
      }
}
