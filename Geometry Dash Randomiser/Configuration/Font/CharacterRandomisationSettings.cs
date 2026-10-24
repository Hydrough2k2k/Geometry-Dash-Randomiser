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
                  set {
                        _letter.CopyDataFrom(value ?? RandomizationSetting.Default);
                        InvokeEvent(Updated, null);
                  }
            }

            public RandomizationSetting Number {
                  get => _number;
                  set {
                        _number.CopyDataFrom(value ?? RandomizationSetting.Default);
                        InvokeEvent(Updated, null);
                  }
            }

            public RandomizationSetting Symbol {
                  get => _symbol;
                  set {
                        _symbol.CopyDataFrom(value ?? RandomizationSetting.Default);
                        InvokeEvent(Updated, null);
                  }
            }

            public RandomizationSetting Special {
                  get => _special;
                  set {
                        _special.CopyDataFrom(value ?? RandomizationSetting.Default);
                        InvokeEvent(Updated, null);
                  } 
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
                  return _randomisationSettings.Where(s => s.Enabled).Count();
            }

            public bool AnySettingEnabled() {
                  return GetEnabledSettingsCount() > 0;
            }

            private void SetAll(bool state) {
                  _randomisationSettings.ToList().ForEach(s => s.Enabled = state);
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

            private readonly RandomizationSetting _letter = new RandomizationSetting(1, true);
            private readonly RandomizationSetting _number = new RandomizationSetting(2, true);
            private readonly RandomizationSetting _symbol = new RandomizationSetting(0, false);
            private readonly RandomizationSetting _special = new RandomizationSetting(0, false);

            // Array for storing references to all settings for easy iteration
            private readonly RandomizationSetting[] _randomisationSettings;
      }
}
