using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Geometry_Dash_Randomiser {

      public class CharacterRandomisationSettings : IToggleableSetting, IUpdateNotify {

            public event EventHandler Updated;

            public CharacterRandomisationSettings() {
                  _randomisationSettings = new RandomizationSetting[] {
                        _letter, _number, _symbol, _special
                  };

                  _letter.MaxGroupNumber = 10;
                  _number.MaxGroupNumber = 10;
                  _symbol.MaxGroupNumber = 10;
                  _special.MaxGroupNumber = 10;

                  _letter.Updated += InvokeUpdateEvent;
                  _number.Updated += InvokeUpdateEvent;
                  _symbol.Updated += InvokeUpdateEvent;
                  _special.Updated += InvokeUpdateEvent;
            }

            public static CharacterRandomisationSettings Default => new CharacterRandomisationSettings();

            [JsonIgnore]
            public virtual bool Enabled {
                  get => AnySettingEnabled();
                  set {
                        SetAll(value);
                        InvokeEvent(Updated, null);
                  }
            }

            public RandomizationSetting Letter {
                  get => _letter;
                  set => UpdateSetting(_letter, value, _defaultLetter);
            }

            public RandomizationSetting Number {
                  get => _number;
                  set => UpdateSetting(_number, value, _defaultNumber);
            }

            public RandomizationSetting Symbol {
                  get => _symbol;
                  set => UpdateSetting(_symbol, value, _defaultSymbol);
            }

            public RandomizationSetting Special {
                  get => _special;
                  set => UpdateSetting(_special, value, _defaultSpecial);
            }

            private void UpdateSetting<T>(T target, T source, T defaults) where T : ICopyableSetting<T> {
                  if (source == null)
                        source = defaults;

                  target.CopyDataFrom(source);
                  InvokeEvent(Updated, null);
            }

            [JsonIgnore]
            public int TotalSettingsCount => _randomisationSettings.Length;

            public int[] GetAllGroups() {
                  HashSet<int> groups = new HashSet<int>();
                  for (int i = 0; i < _randomisationSettings.Length; i++) {
                        groups.Add(_randomisationSettings[i].Group);
                  }
                  return groups.ToArray();
            }

            public int GetEnabledSettingsCount() {
                  return _randomisationSettings.Count(s => s.Enabled);
            }

            public bool AnySettingEnabled() {
                  return GetEnabledSettingsCount() > 0;
            }

            private void SetAll(bool state) {
                  foreach (RandomizationSetting setting in _randomisationSettings) {
                        setting.Enabled = state;
                  }
            }

            public void CopyDataFrom(CharacterRandomisationSettings source) {
                  Letter = source.Letter;
                  Number = source.Number;
                  Symbol = source.Symbol;
                  Special = source.Special;
            }

            private void InvokeUpdateEvent(object sender, EventArgs e) => InvokeEvent(Updated, null);

            private void InvokeEvent(EventHandler @event, EventArgs e) {
                  @event?.Invoke(this, e);
            }

            private readonly RandomizationSetting _letter = _defaultLetter;
            private readonly RandomizationSetting _number = _defaultNumber;
            private readonly RandomizationSetting _symbol = _defaultSymbol;
            private readonly RandomizationSetting _special = _defaultSpecial;

            // Array for storing references to all settings for easy iteration
            private readonly RandomizationSetting[] _randomisationSettings;

            private static RandomizationSetting _defaultLetter => new RandomizationSetting(group: 1, enabled: true, associatedType: ResourceType.Letter);
            private static RandomizationSetting _defaultNumber => new RandomizationSetting(group: 2, enabled: true, associatedType: ResourceType.Number);
            private static RandomizationSetting _defaultSymbol => new RandomizationSetting(group: 0, enabled: false, associatedType: ResourceType.Symbol);
            private static RandomizationSetting _defaultSpecial => new RandomizationSetting(group: 0, enabled: false, associatedType: ResourceType.Special);
      }
}
