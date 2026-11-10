using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Geometry_Dash_Randomiser {

      public class IconRandSettings : IToggleableSetting, IUpdateNotify, ICopyableSetting<IconRandSettings> {

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

            public static IconRandSettings Default => new IconRandSettings { Group = 0, Enabled = true };

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
                  set => UpdateSetting(_cube, value, _defaultCube);
            }

            public RandomizationSetting Ship {
                  get => _ship;
                  set => UpdateSetting(_ship, value, _defaultShip);
            }

            public RandomizationSetting Ball {
                  get => _ball;
                  set => UpdateSetting(_ball, value, _defaultBall);
            }

            public RandomizationSetting Ufo {
                  get => _ufo;
                  set => UpdateSetting(_ufo, value, _defaultUfo);
            }

            public RandomizationSetting Wave {
                  get => _wave;
                  set => UpdateSetting(_wave, value, _defaultWave);
            }

            public RandomizationSetting Robot {
                  get => _robot;
                  set => UpdateSetting(_robot, value, _defaultRobot);
            }

            public RandomizationSetting Spider {
                  get => _spider;
                  set => UpdateSetting(_spider, value, _defaultSpider);
            }

            public RandomizationSetting Swing {
                  get => _swing;
                  set => UpdateSetting(_swing, value, _defaultSwing);
            }

            public RandomizationSetting Jetpack {
                  get => _jetpack;
                  set => UpdateSetting(_jetpack, value, _defaultJetpack);
            }

            private void UpdateSetting<T>(T target, T source, T defaults) where T : ICopyableSetting<T> {
                  if (source == null)
                        source = defaults;

                  target.CopyDataFrom(source);
                  InvokeEvent(Updated, null);
            }

            [JsonIgnore]
            public int TotalSettingsCount => RandomisationSettings.Length;

            private void SetAllEnabled(bool value) {
                  foreach (RandomizationSetting setting in _randomisationSettings) {
                        setting.Enabled = value;
                  }
            }

            private void SetAllGroups(int value) {
                  foreach (RandomizationSetting setting in _randomisationSettings) {
                        setting.Group = value;
                  }
            }

            private void SetAllPercentages(int value) {
                  foreach (RandomizationSetting setting in _randomisationSettings) {
                        setting.PercentageRandomised = value;
                  }
            }

            public int GetEnabledSettingsCount() {
                  return _randomisationSettings.Count(s => s.Enabled);
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
                  if (source == null) {
                        return;
                  }

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
            private int _percentageRandomized = 100;

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
