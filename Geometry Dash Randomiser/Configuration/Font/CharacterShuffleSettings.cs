using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Geometry_Dash_Randomiser {

      public class CharacterShuffleSettings : IToggleableSetting {

            // Array for storing references to all settings for easy iteration
            private readonly RandomisationSetting[] _randomisationSettings;

            private RandomisationSetting _letter = new RandomisationSetting(0, false);
            private RandomisationSetting _number = new RandomisationSetting(1, true);
            private RandomisationSetting _symbol = new RandomisationSetting(2, true);

            public CharacterShuffleSettings() {
                  _randomisationSettings = new RandomisationSetting[] {
                        _letter, _number, _symbol
                  };

                  _letter.MaxGroupNumber = 10;
                  _number.MaxGroupNumber = 10;
                  _symbol.MaxGroupNumber = 10;
            }

            public RandomisationSetting Letter {
                  get => _letter;
                  set {
                        _letter = value;
                        _randomisationSettings[0] = value;
                  }
            }

            public RandomisationSetting Number {
                  get => _number;
                  set {
                        _number = value;
                        _randomisationSettings[1] = value;
                  }
            }

            public RandomisationSetting Symbol {
                  get => _symbol;
                  set {
                        _symbol = value;
                        _randomisationSettings[2] = _symbol;
                  }
            }

            #region IToggleableSetting Implementation

            [JsonIgnore]
            public bool Enabled {
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

            #endregion

            public void SetAll(bool state) {
                  _randomisationSettings.ToList().ForEach(r => r.Enabled = state);
            }

            public int[] GetDistinctNonZeroGroupValues() {
                  return _randomisationSettings
                        .Where(s => s.Enabled && s.Group != 0)
                        .Select(s => s.Group)
                        .Distinct()
                        .ToArray();
            }
      }
}
