using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Geometry_Dash_Randomiser {

      public class GameTextureConfig : IUpdateNotify, ICopyableSetting<GameTextureConfig>, IEquatable<GameTextureConfig> {

            public enum SettingType { Menu, Shop, Editor, Tile, Portal, Orb, Pad, Particle, Effect, Misc }

            public event EventHandler Updated;

            public IReadOnlyList<RandomizationSetting> RandomizationSettings;

            public GameTextureConfig() {
                  _randomisationSettings = new RandomizationSetting[] {
                        _menuTextures, _shopTextures, _editorTextures, _tileTextures, _portalTextures,
                        _orbTextures, _padTextures, _particleTextures, _effectTextures, _miscTextures, 
                  };

                  RandomizationSettings = Array.AsReadOnly(_randomisationSettings);

                  for (int i = 0; i < _randomisationSettings.Length; i++) {
                        _randomisationSettings[i].Updated += InvokeUpdateEvent;
                  }
            }

            [JsonIgnore]
            public static GameTextureConfig Default => new GameTextureConfig();

            // These only copy the setters' data to the existing readonly fields, thus the reference stays, but the values don't
            public RandomizationSetting Menu {
                  get => _menuTextures;
                  set => UpdateSetting(_menuTextures, value, _defaultMenuTextures);
            }

            public RandomizationSetting Shop {
                  get => _shopTextures;
                  set => UpdateSetting(_shopTextures, value, _defaultShopTextures);
            }

            public RandomizationSetting Editor {
                  get => _editorTextures;
                  set => UpdateSetting(_editorTextures, value, _defaultEditorTextures);
            }

            public RandomizationSetting Tile {
                  get => _tileTextures;
                  set => UpdateSetting(_tileTextures, value, _defaultTileTextures);
            }

            public RandomizationSetting Portal {
                  get => _portalTextures;
                  set => UpdateSetting(_portalTextures, value, _defaultPortalTextures);
            }

            public RandomizationSetting Orb {
                  get => _orbTextures;
                  set => UpdateSetting(_orbTextures, value, _defaultOrbTextures);
            }

            public RandomizationSetting Pad {
                  get => _padTextures;
                  set => UpdateSetting(_padTextures, value, _defaultPadTextures);
            }

            public RandomizationSetting Particle {
                  get => _particleTextures;
                  set => UpdateSetting(_particleTextures, value, _defaultParticleTextures);
            }

            public RandomizationSetting Effect {
                  get => _effectTextures;
                  set => UpdateSetting(_effectTextures, value, _defaultEffectTextures);
            }

            public RandomizationSetting Misc {
                  get => _miscTextures;
                  set => UpdateSetting(_miscTextures, value, _defaultMiscTextures);
            }

            private void UpdateSetting<T>(T target, T source, T defaults) where T : ICopyableSetting<T> {
                  if (source == null)
                        source = defaults;

                  target.CopyDataFrom(source);
                  InvokeEvent(Updated, null);
            }

            public void UpdateSetting<T>(T source, SettingType setting) where T : ICopyableSetting<T> {
                  int settingID = (int)setting;

                  UpdateSetting(
                        (T)(ICopyableSetting<RandomizationSetting>)_randomisationSettings[settingID],
                        source,
                        (T)(ICopyableSetting<RandomizationSetting>)_defaultValues[settingID]
                  );
            }

            private void InvokeUpdateEvent(object sender, EventArgs e) => InvokeEvent(Updated, null);

            private void InvokeEvent(EventHandler @event, EventArgs e) {
                  @event?.Invoke(this, e);
            }

            public int GetEnabledSettingsCount() {
                  return (Menu.Enabled ? 1 : 0) +
                         (Shop.Enabled ? 1 : 0) +
                         (Editor.Enabled ? 1 : 0) +
                         (Tile.Enabled ? 1 : 0) +
                         (Portal.Enabled ? 1 : 0) +
                         (Orb.Enabled ? 1 : 0) +
                         (Pad.Enabled ? 1 : 0) +
                         (Particle.Enabled ? 1 : 0) +
                         (Effect.Enabled ? 1 : 0) +
                         (Misc.Enabled ? 1 : 0);
            }

            public void CopyDataFrom(GameTextureConfig source) {
                  if (source == null) {
                        Logger.Write(Logger.Mode.Error, "The parameter \"source\" was passed as null in GameTextureConfig.CopyDataFrom");
                        return;
                  }

                  Menu = source.Menu;
                  Shop = source.Shop;
                  Editor = source.Editor;
                  Tile = source.Tile;
                  Portal = source.Portal;
                  Orb = source.Orb;
                  Pad = source.Pad;
                  Particle = source.Particle;
                  Effect = source.Effect;
                  Misc = source.Misc;
            }

            public bool Equals(GameTextureConfig other) {
                  if (other == null) {
                        Logger.Write(Logger.Mode.Error, "The parameter \"other\" was passed as null in GameTextureConfig.Equals");
                        return false;
                  }

                  // Compare each Randomization setting
                  for (int i = 0; i < _randomisationSettings.Length; i++) {
                        // If any of them don't match, return false
                        if (this._randomisationSettings[i].Equals(other.RandomizationSettings[i]) == false) {
                              return false;
                        }
                  }

                  return true;
            }

            private readonly RandomizationSetting _menuTextures = _defaultMenuTextures;
            private readonly RandomizationSetting _shopTextures = _defaultShopTextures;
            private readonly RandomizationSetting _editorTextures = _defaultEditorTextures;
            private readonly RandomizationSetting _tileTextures = _defaultTileTextures;
            private readonly RandomizationSetting _portalTextures = _defaultPortalTextures;
            private readonly RandomizationSetting _orbTextures = _defaultOrbTextures;
            private readonly RandomizationSetting _padTextures = _defaultPadTextures;
            private readonly RandomizationSetting _particleTextures = _defaultParticleTextures;
            private readonly RandomizationSetting _effectTextures = _defaultEffectTextures;
            private readonly RandomizationSetting _miscTextures = _defaultMiscTextures;

            // Array for storing references to all settings for easy iteration
            private readonly RandomizationSetting[] _randomisationSettings;

            private static RandomizationSetting _defaultMenuTextures => new RandomizationSetting(group: 1, enabled: true, associatedType: ResourceType.Menu);
            private static RandomizationSetting _defaultShopTextures => new RandomizationSetting(group: 1, enabled: true, associatedType: ResourceType.Shop);
            private static RandomizationSetting _defaultEditorTextures => new RandomizationSetting(associatedType: ResourceType.Editor);
            private static RandomizationSetting _defaultTileTextures => new RandomizationSetting(associatedType: ResourceType.Block);
            private static RandomizationSetting _defaultPortalTextures => new RandomizationSetting(group: 0, enabled: true, associatedType: ResourceType.Portal);
            private static RandomizationSetting _defaultOrbTextures => new RandomizationSetting(group: 3, enabled: true, associatedType: ResourceType.Orb);
            private static RandomizationSetting _defaultPadTextures => new RandomizationSetting(group: 0, enabled: true, associatedType: ResourceType.Pad);
            private static RandomizationSetting _defaultParticleTextures => new RandomizationSetting(group: 3, enabled: true, associatedType: ResourceType.Particle);
            private static RandomizationSetting _defaultEffectTextures => new RandomizationSetting(group: 2, enabled: true, associatedType: ResourceType.Effect);
            private static RandomizationSetting _defaultMiscTextures => new RandomizationSetting(associatedType: ResourceType.Miscellaneous);

            private static readonly RandomizationSetting[] _defaultValues = new RandomizationSetting[] {
                        _defaultMenuTextures,
                        _defaultShopTextures,
                        _defaultEditorTextures,
                        _defaultTileTextures,
                        _defaultPortalTextures,
                        _defaultOrbTextures,
                        _defaultPadTextures,
                        _defaultParticleTextures,
                        _defaultEffectTextures,
                        _defaultMiscTextures
            };
      }
}