using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Geometry_Dash_Randomiser {

      public class IconRandSettings : RandomisationSetting {

            // Array for storing references to all settings for easy iteration
            private readonly RandomisationSetting[] randomisationSettings;

            private readonly RandomisationSetting _cube = new RandomisationSetting(0, true);
            private readonly RandomisationSetting _ship = new RandomisationSetting(0, true);
            private readonly RandomisationSetting _ball = new RandomisationSetting(0, true);
            private readonly RandomisationSetting _ufo = new RandomisationSetting(0, true);
            private readonly RandomisationSetting _wave = new RandomisationSetting(0, true);
            private readonly RandomisationSetting _robot = new RandomisationSetting(0, true);
            private readonly RandomisationSetting _spider = new RandomisationSetting(0, true);
            private readonly RandomisationSetting _swing = new RandomisationSetting(0, true);
            private readonly RandomisationSetting _jetpack = new RandomisationSetting(0, true);

            public IconRandSettings() {
                  randomisationSettings = new RandomisationSetting[] {
                        Base, _cube, _ship, _ball, _ufo, _wave, _robot, _spider, _swing, _jetpack
                  };
            }

            // These setters only copy the setters' data to the existing readonly fields, thus the reference stays, but the values don't
            public RandomisationSetting Cube { get => _cube; set => _cube.CopyDataFrom(value); }
            public RandomisationSetting Ship { get => _ship; set => _ship.CopyDataFrom(value); }
            public RandomisationSetting Ball { get => _ball; set => _ball.CopyDataFrom(value); }
            public RandomisationSetting Ufo { get => _ufo; set => _ufo.CopyDataFrom(value); }
            public RandomisationSetting Wave { get => _wave; set => _wave.CopyDataFrom(value); }
            public RandomisationSetting Robot { get => _robot; set => _robot.CopyDataFrom(value); }
            public RandomisationSetting Spider { get => _spider; set => _spider.CopyDataFrom(value); }
            public RandomisationSetting Swing { get => _swing; set => _swing.CopyDataFrom(value); }
            public RandomisationSetting Jetpack { get => _jetpack; set => _jetpack.CopyDataFrom(value); }

            [JsonIgnore]
            public RandomisationSetting Base => this as RandomisationSetting;

            public int GetEnabledSettingsCount() {
                  // Go through all settings, but skip the first one (base)
                  // If the base is enabled, but no others are, then the base being on does not matter
                  // I will find a way to change this later
                  return randomisationSettings.Skip(1).Where(s => s.enabled).Count();
            }

            public new string GetStatusHex() {
                  return string.Concat(randomisationSettings.Select(s => s.GetStatusHex()));
            }

            public new void ApplyConfigFromHex(string hex) {
                  int settingIndex = 0;

                  while (hex.Length >= 2 && settingIndex < 10) {
                        // Read the incoming data stream 2 characters at a time, and convert them to an int for later use
                        Int32.TryParse(hex.Substring(0, 2), style: System.Globalization.NumberStyles.HexNumber, null, out int nextValue);

                        // Then remove the 2 parsed characters
                        hex = hex.Substring(2);

                        // Pass the data to the class to apply it fully
                        randomisationSettings[settingIndex].ApplyConfigFromValue(nextValue);

                        settingIndex++;
                  }

                  // Finally make sure the data is within expected range
                  this.Validate();
            }

            public new void Validate() {
                  foreach (var setting in randomisationSettings) {
                        setting.Validate();
                  }
            }
      }
}
