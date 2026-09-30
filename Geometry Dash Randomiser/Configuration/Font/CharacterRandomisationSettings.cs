using System.Linq;
using System.Text.Json.Serialization;

namespace Geometry_Dash_Randomiser {

      public class CharacterRandomisationSettings : IToggleableSetting {

            // Array for storing references to all settings for easy iteration
            private readonly RandomisationSetting[] _randomisationSettings;

            private readonly RandomisationSetting _letter = new RandomisationSetting(1, true);
            private readonly RandomisationSetting _number = new RandomisationSetting(2, true);
            private readonly RandomisationSetting _symbol = new RandomisationSetting(0, false);

            public CharacterRandomisationSettings() {
                  _randomisationSettings = new RandomisationSetting[] {
                        _letter, _number, _symbol
                  };

                  _letter.MaxGroupNumber = 10;
                  _number.MaxGroupNumber = 10;
                  _symbol.MaxGroupNumber = 10;

                  Enabled = true;
            }

            public RandomisationSetting Letter {
                  get => _letter;
                  set => _letter.CopyDataFrom(value);
            }

            public RandomisationSetting Number {
                  get => _number;
                  set => _number.CopyDataFrom(value);
            }

            public RandomisationSetting Symbol {
                  get => _symbol;
                  set => _symbol.CopyDataFrom(value);
            }

            public virtual bool Enabled {
                  get {
                        return AnySettingEnabled();
                  }
                  set {
                        SetAll(value);
                  }
            }

            [JsonIgnore]
            public int TotalSettingsCount => _randomisationSettings.Length;

            public int GetEnabledSettingsCount() {
                  return _randomisationSettings.Where(s => s.Enabled).Count();
            }

            public bool AnySettingEnabled() {
                  return GetEnabledSettingsCount() > 0;
            }

            public void SetAll(bool state) {
                  _randomisationSettings.ToList().ForEach(r => r.Enabled = state);
            }
      }
}
