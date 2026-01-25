using System;
using System.Linq;
using System.Text;

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
                        _cube, _ship, _ball, _ufo, _wave, _robot, _spider, _swing, _jetpack
                  };
            }

            public RandomisationSetting Cube => _cube;
            public RandomisationSetting Ship => _ship;
            public RandomisationSetting Ball => _ball;
            public RandomisationSetting Ufo => _ufo;
            public RandomisationSetting Wave => _wave;
            public RandomisationSetting Robot => _robot;
            public RandomisationSetting Spider => _spider;
            public RandomisationSetting Swing => _swing;
            public RandomisationSetting Jetpack => _jetpack;
            public RandomisationSetting Base => this as RandomisationSetting;

            public int GetEnabledSettingsCount() {
                  return randomisationSettings.Where(s => s.enabled).Count();
            }

            public override void Validate() {
                  foreach (var setting in randomisationSettings) {
                        setting.Validate();
                  }

                  if (group > Config.maxGroups)
                        group = 0;
            }

            public override string GetStatusHex() {
                  StringBuilder statusHex = new StringBuilder();

                  // Add the main setting status
                  statusHex.Append(base.GetStatusHex());

                  // Add each icon setting status
                  foreach (var setting in randomisationSettings) {
                        statusHex.Append(setting.GetStatusHex());
                  }

                  return statusHex.ToString();
            }

            public override void ApplyConfigFromHex(string hex) {
                  int index = 0;

                  while (hex.Length >= 2) {
                        // Read the incoming data stream 2 characters at a time, and convert them to an int for later use
                        Int32.TryParse(hex.Substring(0, 2), style: System.Globalization.NumberStyles.HexNumber, null, out int nextValue);

                        // Then remove the 2 parsed characters
                        hex = hex.Substring(2);

                        if (index == 0) {
                              // First value is for the base setting
                              Base.ApplyConfigFromValue(nextValue);

                        } else if (index < 10) {
                              // Subsequent values are for each icon setting
                              // We need to subtract one, since the base value is the first, and that is missing from the array
                              randomisationSettings[index - 1].ApplyConfigFromValue(nextValue);

                        } else {
                              Console.WriteLine("Something went terribly wrong. Error code: 1");
                              break;
                        }

                        index++;
                  }

                  // Finally make sure the data is within expected range
                  this.Validate();
            }
      }
}
