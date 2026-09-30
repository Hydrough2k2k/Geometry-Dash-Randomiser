using System.Linq;
using System.Text.Json.Serialization;

namespace Geometry_Dash_Randomiser {

      public class IconRandSettings : IToggleableSetting {

            // Array for storing references to all settings for easy iteration
            private readonly RandomisationSetting[] _randomisationSettings;

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
                  _randomisationSettings = new RandomisationSetting[] {
                        _cube, _ship, _ball, _ufo, _wave, _robot, _spider, _swing, _jetpack
                  };
            }

            [JsonIgnore]
            public RandomisationSetting[] RandomisationSettings => _randomisationSettings;

            public bool Enabled {
                  get {
                        return AnySettingEnabled();
                  }
                  set {
                        SetAll(value);
                  }
            }

            public int Group { get; set; } = 0;

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
            public int TotalSettingsCount => RandomisationSettings.Length;

            public void SetAll(bool state) {
                  _randomisationSettings.ToList().ForEach(r => r.Enabled = state);
            }

            public int GetEnabledSettingsCount() {
                  return _randomisationSettings.Where(s => s.Enabled).Count();
            }

            public bool AnySettingEnabled() {
                  for (int i = 0; i < _randomisationSettings.Length; i++) {
                        if (_randomisationSettings[i].Enabled == true) {
                              return true;
                        }
                  }
                  return false;
            }

            public RandomisationSetting ToRandSetting() {
                  return new RandomisationSetting(Group, Enabled);
            }
      }
}
