using System;
using System.Text.Json.Serialization;

namespace Geometry_Dash_Randomiser {

      public class FontRandomisationSettings : IToggleableSetting, IUpdateNotify, ICopyableSetting<FontRandomisationSettings>, IEquatable<FontRandomisationSettings> {

            public event EventHandler Updated;

            public FontRandomisationSettings() {
                  _shuffleSettings.Updated += InvokeUpdateEvent;
                  _characterRandSettings.Updated += InvokeUpdateEvent;
                  _letterSpacingSettings.Updated += InvokeUpdateEvent;
            }

            [JsonIgnore]
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
                        _letterSpacingSettings.CopyDataFrom(value ?? LetterSpacingSettings.Default);
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
                  if (source == null) {
                        Logger.Write(Logger.Mode.Error, "The parameter \"source\" was passed as null in FontRandomisationSettings.CopyDataFrom");
                        return;
                  }

                  ShuffleSettings = source.ShuffleSettings;
                  CharacterRandSettings = source.CharacterRandSettings;
                  LetterSpacingSettings = source.LetterSpacingSettings;
            }

            public bool Equals(FontRandomisationSettings other) {
                  if (other == null) {
                        Logger.Write(Logger.Mode.Error, "The parameter \"other\" was passed as null in FontRandomisationSettings.Equals");
                        return false;
                  }

                  if (this.ShuffleSettings.Equals(other.ShuffleSettings) == false ||
                        this.CharacterRandSettings.Equals(other.CharacterRandSettings) == false ||
                        this.LetterSpacingSettings.Equals(other.LetterSpacingSettings) == false) {

                        return false;
                  }

                  return true;
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
