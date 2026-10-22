using System.Linq;
using System.Text.Json.Serialization;

namespace Geometry_Dash_Randomiser {

      public class IconRandSettings : IToggleableSetting {

            // Array for storing references to all settings for easy iteration
            private readonly RandomizationSetting[] _randomisationSettings;

            private readonly RandomizationSetting _cube = new RandomizationSetting(0, true);
            private readonly RandomizationSetting _ship = new RandomizationSetting(0, true);
            private readonly RandomizationSetting _ball = new RandomizationSetting(0, true);
            private readonly RandomizationSetting _ufo = new RandomizationSetting(0, true);
            private readonly RandomizationSetting _wave = new RandomizationSetting(0, true);
            private readonly RandomizationSetting _robot = new RandomizationSetting(0, true);
            private readonly RandomizationSetting _spider = new RandomizationSetting(0, true);
            private readonly RandomizationSetting _swing = new RandomizationSetting(0, true);
            private readonly RandomizationSetting _jetpack = new RandomizationSetting(0, true);

            public IconRandSettings() {
                  _randomisationSettings = new RandomizationSetting[] {
                        _cube, _ship, _ball, _ufo, _wave, _robot, _spider, _swing, _jetpack
                  };
            }

            [JsonIgnore]
            public RandomizationSetting[] RandomisationSettings => _randomisationSettings;

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
            public RandomizationSetting Cube { get => _cube; set => _cube.CopyDataFrom(value); }
            public RandomizationSetting Ship { get => _ship; set => _ship.CopyDataFrom(value); }
            public RandomizationSetting Ball { get => _ball; set => _ball.CopyDataFrom(value); }
            public RandomizationSetting Ufo { get => _ufo; set => _ufo.CopyDataFrom(value); }
            public RandomizationSetting Wave { get => _wave; set => _wave.CopyDataFrom(value); }
            public RandomizationSetting Robot { get => _robot; set => _robot.CopyDataFrom(value); }
            public RandomizationSetting Spider { get => _spider; set => _spider.CopyDataFrom(value); }
            public RandomizationSetting Swing { get => _swing; set => _swing.CopyDataFrom(value); }
            public RandomizationSetting Jetpack { get => _jetpack; set => _jetpack.CopyDataFrom(value); }

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

            public RandomizationSetting ToRandSetting() {
                  return new RandomizationSetting(Group, Enabled);
            }
      }
}
