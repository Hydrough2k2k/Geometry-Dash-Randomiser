using System;

namespace Geometry_Dash_Randomiser {

      public class FontRandomisationSettings : IToggleableSetting {

            public event EventHandler ConfigUpdated;

            public FontRandomisationSettings() {
                  _shuffleSettings.ConfigUpdated += InvokeConfigUpdateEvent;
                  _characterRandSettings.ConfigUpdated += InvokeConfigUpdateEvent;
                  _letterSpacingSettings.ConfigUpdated += InvokeConfigUpdateEvent;
            }

            public bool Enabled {
                  get => AnySettingEnabled();
                  set {
                        SetAll(value);
                        InvokeEvent(ConfigUpdated, null);
                  }
            }

            public FontStyleShuffleSettings ShuffleSettings {
                  get => _shuffleSettings;
                  set {
                        _shuffleSettings.CopyDataFrom(value);
                        InvokeEvent(ConfigUpdated, null);
                  } 
            }

            public CharacterRandomisationSettings CharacterRandSettings {
                  get => _characterRandSettings;
                  set {
                        _characterRandSettings.CopyDataFrom(value);
                        InvokeEvent(ConfigUpdated, null);
                  }
            }

            public LetterSpacingSettings LetterSpacingSettings {
                  get => _letterSpacingSettings;
                  set {
                        _letterSpacingSettings.CopyDataFrom(value);
                        InvokeEvent(ConfigUpdated, null);
                  }
            }

            public int TotalSettingsCount => 3;

            public bool AnySettingEnabled() {
                  return GetEnabledSettingsCount() > 0;
            }

            public int GetEnabledSettingsCount() {
                  return Convert.ToInt32(ShuffleSettings.Enabled) +
                        Convert.ToInt32(CharacterRandSettings.Enabled) +
                        Convert.ToInt32(LetterSpacingSettings.Enabled);
            }

            public void SetAll(bool state) {
                  ShuffleSettings.Enabled = state;
                  CharacterRandSettings.Enabled = state;
                  LetterSpacingSettings.Enabled = state;
            }

            public void CopyDataFrom(FontRandomisationSettings source) {
                  ShuffleSettings = source.ShuffleSettings;
                  CharacterRandSettings = source.CharacterRandSettings;
                  LetterSpacingSettings = source.LetterSpacingSettings;
            }

            private void InvokeConfigUpdateEvent(object sender, EventArgs e) => InvokeEvent(ConfigUpdated, null);

            private void InvokeEvent(EventHandler @event, EventArgs e) {
                  @event?.Invoke(this, e);
            }

            private readonly FontStyleShuffleSettings _shuffleSettings = new FontStyleShuffleSettings();
            private readonly CharacterRandomisationSettings _characterRandSettings = new CharacterRandomisationSettings();
            private readonly LetterSpacingSettings _letterSpacingSettings = new LetterSpacingSettings();
      }
}
