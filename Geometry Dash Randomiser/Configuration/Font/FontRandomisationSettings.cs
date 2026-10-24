using System;

namespace Geometry_Dash_Randomiser {

      public class FontRandomisationSettings : IToggleableSetting, IUpdateNotify {

            public event EventHandler Updated;

            public FontRandomisationSettings() {
                  _shuffleSettings.Updated += InvokeUpdateEvent;
                  _characterRandSettings.Updated += InvokeUpdateEvent;
                  _letterSpacingSettings.Updated += InvokeUpdateEvent;
            }

            public static FontRandomisationSettings Default => new FontRandomisationSettings();

            public bool Enabled {
                  get => AnySettingEnabled();
                  set {
                        SetAll(value);
                        InvokeEvent(Updated, null);
                  }
            }

            public FontStyleShuffleSettings ShuffleSettings {
                  get => _shuffleSettings;
                  set {
                        _shuffleSettings.CopyDataFrom(value ?? FontStyleShuffleSettings.Default);
                        InvokeEvent(Updated, null);
                  } 
            }

            public CharacterRandomisationSettings CharacterRandSettings {
                  get => _characterRandSettings;
                  set {
                        _characterRandSettings.CopyDataFrom(value ?? CharacterRandomisationSettings.Default);
                        InvokeEvent(Updated, null);
                  }
            }

            public LetterSpacingSettings LetterSpacingSettings {
                  get => _letterSpacingSettings;
                  set {
                        _letterSpacingSettings.CopyDataFrom(value ?? Geometry_Dash_Randomiser.LetterSpacingSettings.Default);
                        InvokeEvent(Updated, null);
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

            private void SetAll(bool state) {
                  ShuffleSettings.Enabled = state;
                  CharacterRandSettings.Enabled = state;
                  LetterSpacingSettings.Enabled = state;
            }

            public void CopyDataFrom(FontRandomisationSettings source) {
                  ShuffleSettings = source.ShuffleSettings;
                  CharacterRandSettings = source.CharacterRandSettings;
                  LetterSpacingSettings = source.LetterSpacingSettings;
            }

            private void InvokeUpdateEvent(object sender, EventArgs e) => InvokeEvent(Updated, null);

            private void InvokeEvent(EventHandler @event, EventArgs e) {
                  @event?.Invoke(this, e);
            }

            private readonly FontStyleShuffleSettings _shuffleSettings = new FontStyleShuffleSettings();
            private readonly CharacterRandomisationSettings _characterRandSettings = new CharacterRandomisationSettings();
            private readonly LetterSpacingSettings _letterSpacingSettings = new LetterSpacingSettings();
      }
}
