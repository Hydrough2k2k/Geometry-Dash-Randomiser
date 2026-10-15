using System;
using System.Text.Json.Serialization;

namespace Geometry_Dash_Randomiser {

      public class FontRandomisationSettings : IToggleableSetting {

            public FontRandomisationSettings() { }

            [JsonIgnore]
            public bool Enabled {
                  get {
                        return AnySettingEnabled();
                  }
                  set {
                        SetAll(value);
                  }
            }

            public FontStyleShuffleSettings StyleShuffleSettings { get; set; } = new FontStyleShuffleSettings();
            public CharacterShuffleSettings CharacterShuffleSettings { get; set; } = new CharacterShuffleSettings();
            public LetterSpacingSettings LetterSpacingSettings { get; set; } = new LetterSpacingSettings();

            public int TotalSettingsCount => 3;

            public bool AnySettingEnabled() {
                  return GetEnabledSettingsCount() > 0;
            }

            public int GetEnabledSettingsCount() {
                  return Convert.ToInt32(StyleShuffleSettings.Enabled) +
                        Convert.ToInt32(CharacterShuffleSettings.Enabled) +
                        Convert.ToInt32(LetterSpacingSettings.Enabled);
            }

            public void SetAll(bool state) {
                  StyleShuffleSettings.Enabled = state;

                  CharacterShuffleSettings.Enabled = state;
                  CharacterShuffleSettings.SetAll(state);

                  LetterSpacingSettings.Enabled = state;
            }
      }
}
