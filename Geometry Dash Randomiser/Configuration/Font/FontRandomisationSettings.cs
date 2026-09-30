using System;

namespace Geometry_Dash_Randomiser {

      public class FontRandomisationSettings : IToggleableSetting {

            public FontRandomisationSettings() { }

            public bool Enabled {
                  get {
                        return AnySettingEnabled();
                  }
                  set {
                        SetAll(value);
                  }
            }

            public FontStyleShuffleSettings ShuffleSettings { get; set; } = new FontStyleShuffleSettings();
            public CharacterRandomisationSettings CharacterRandSettings { get; set; } = new CharacterRandomisationSettings();
            public LetterSpacingSettings LetterSpacingSettings { get; set; } = new LetterSpacingSettings();

            public bool RandomisationNeeded {
                  get {
                        if (Enabled == false)
                              return false;

                        if (ShuffleSettings.Enabled)
                              return true;

                        if (ShuffleSettings.Enabled)
                              return true;

                        return false;
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
                  CharacterRandSettings.SetAll(state);

                  LetterSpacingSettings.Enabled = state;
            }
      }
}
