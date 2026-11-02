using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Geometry_Dash_Randomiser {

      public class IconRandSettings : IToggleableSetting, IUpdateNotify {

            public event EventHandler Updated;

            // Array for storing references to all settings for easy iteration
            private readonly RandomizationSetting[] _randomisationSettings;

            public IconRandSettings() {
                  _randomisationSettings = new RandomizationSetting[] {
                        _cube, _ship, _ball, _ufo, _wave, _robot, _spider, _swing, _jetpack
                  };

                  _cube.Updated += InvokeUpdateEvent;
                  _ship.Updated += InvokeUpdateEvent;
                  _ball.Updated += InvokeUpdateEvent;
                  _ufo.Updated += InvokeUpdateEvent;
                  _wave.Updated += InvokeUpdateEvent;
                  _robot.Updated += InvokeUpdateEvent;
                  _spider.Updated += InvokeUpdateEvent;
                  _swing.Updated += InvokeUpdateEvent;
                  _jetpack.Updated += InvokeUpdateEvent;
            }

            public static IconRandSettings Default => new IconRandSettings();

            [JsonIgnore]
            public RandomizationSetting[] RandomisationSettings => _randomisationSettings;

            public bool Enabled {
                  get {
                        return AnySettingEnabled();
                  }
                  set {
                        SetAllEnabled(value);
                        InvokeEvent(Updated, null);
                  }
            }

            public int Group {
                  get => _group;
                  set {
                        _group = Math.Clamp(value, 0, RandomizationConfig.maxTextureGroups);
                        SetAllGroups(_group);
                        InvokeEvent(Updated, null);
                  }
            }

            public int PercentageRandomised {
                  get => _percentageRandomized;
                  set {
                        _percentageRandomized = Math.Clamp(value, 0, 100);
                        SetAllPercentages(_percentageRandomized);
                        InvokeEvent(Updated, null);
                  }
            }

            // These only copy the setters' data to the existing readonly fields, thus the reference stays, but the values don't
            public RandomizationSetting Cube {
                  get => _cube;
                  set {
                        _cube.CopyDataFrom(value ?? _defaultCube);
                        InvokeEvent(Updated, null);
                  }
            }

            public RandomizationSetting Ship {
                  get => _ship;
                  set {
                        _ship.CopyDataFrom(value ?? _defaultShip);
                        InvokeEvent(Updated, null);
                  }
            }

            public RandomizationSetting Ball {
                  get => _ball;
                  set {
                        _ball.CopyDataFrom(value ?? _defaultBall);
                        InvokeEvent(Updated, null);
                  }
            }

            public RandomizationSetting Ufo {
                  get => _ufo;
                  set {
                        _ufo.CopyDataFrom(value ?? _defaultUfo);
                        InvokeEvent(Updated, null);
                  }
            }

            public RandomizationSetting Wave {
                  get => _wave;
                  set {
                        _wave.CopyDataFrom(value ?? _defaultWave);
                        InvokeEvent(Updated, null);
                  }
            }

            public RandomizationSetting Robot {
                  get => _robot;
                  set {
                        _robot.CopyDataFrom(value ?? _defaultRobot);
                        InvokeEvent(Updated, null);
                  }
            }

            public RandomizationSetting Spider {
                  get => _spider;
                  set {
                        _spider.CopyDataFrom(value ?? _defaultSpider);
                        InvokeEvent(Updated, null);
                  }
            }

            public RandomizationSetting Swing {
                  get => _swing;
                  set {
                        _swing.CopyDataFrom(value ?? _defaultSwing);
                        InvokeEvent(Updated, null);
                  }
            }

            public RandomizationSetting Jetpack {
                  get => _jetpack;
                  set {
                        _jetpack.CopyDataFrom(value ?? _defaultJetpack);
                        InvokeEvent(Updated, null);
                  }
            }

            [JsonIgnore]
            public int TotalSettingsCount => RandomisationSettings.Length;

            private void SetAllEnabled(bool value) {
                  _randomisationSettings.ToList().ForEach(r => r.Enabled = value);
            }

            private void SetAllGroups(int value) {
                  _randomisationSettings.ToList().ForEach(r => r.Group = value);
            }

            private void SetAllPercentages(int value) {
                  _randomisationSettings.ToList().ForEach(r => r.PercentageRandomised = value);
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

            private void InvokeUpdateEvent(object sender, EventArgs e) => InvokeEvent(Updated, null);

            private void InvokeEvent(EventHandler @event, EventArgs e) {
                  @event?.Invoke(this, e);
            }

            public void CopyDataFrom(IconRandSettings source) {
                  Group = source.Group;
                  PercentageRandomised = source.PercentageRandomised;

                  Cube = source.Cube;
                  Ship = source.Ship;
                  Ball = source.Ball;
                  Ufo = source.Ufo;
                  Wave = source.Wave;
                  Robot = source.Robot;
                  Spider = source.Spider;
                  Swing = source.Swing;
                  Jetpack = source.Jetpack;
            }

            public static explicit operator RandomizationSetting(IconRandSettings icrs) => icrs.ToRandSetting();

            private RandomizationSetting ToRandSetting() {
                  return new RandomizationSetting(Group, Enabled, PercentageRandomised, ResourceType.Unknown);
            }

            private int _group = 0;
            private int _percentageRandomized = 0;

            private readonly RandomizationSetting _cube = _defaultCube;
            private readonly RandomizationSetting _ship = _defaultShip;
            private readonly RandomizationSetting _ball = _defaultBall;
            private readonly RandomizationSetting _ufo = _defaultUfo;
            private readonly RandomizationSetting _wave = _defaultWave;
            private readonly RandomizationSetting _robot = _defaultRobot;
            private readonly RandomizationSetting _spider = _defaultSpider;
            private readonly RandomizationSetting _swing = _defaultSwing;
            private readonly RandomizationSetting _jetpack = _defaultJetpack;

            private static RandomizationSetting _defaultCube => new RandomizationSetting(group: 0, enabled: true, associatedType: ResourceType.Cube);
            private static RandomizationSetting _defaultShip => new RandomizationSetting(group: 0, enabled: true, associatedType: ResourceType.Ship);
            private static RandomizationSetting _defaultBall => new RandomizationSetting(group: 0, enabled: true, associatedType: ResourceType.Ball);
            private static RandomizationSetting _defaultUfo => new RandomizationSetting(group: 0, enabled: true, associatedType: ResourceType.UFO);
            private static RandomizationSetting _defaultWave => new RandomizationSetting(group: 0, enabled: true, associatedType: ResourceType.Wave);
            private static RandomizationSetting _defaultRobot => new RandomizationSetting(group: 0, enabled: true, associatedType: ResourceType.Robot);
            private static RandomizationSetting _defaultSpider => new RandomizationSetting(group: 0, enabled: true, associatedType: ResourceType.Spider);
            private static RandomizationSetting _defaultSwing => new RandomizationSetting(group: 0, enabled: true, associatedType: ResourceType.Swing);
            private static RandomizationSetting _defaultJetpack => new RandomizationSetting(group: 0, enabled: true, associatedType: ResourceType.Jetpack);
      }
}
