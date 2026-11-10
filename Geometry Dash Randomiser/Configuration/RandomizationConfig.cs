using Geometry_Dash_Randomiser.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace Geometry_Dash_Randomiser {

      /// <summary>
      /// Singleton class for storing this application's simpler and user-facing configurations<br/>
      /// These are exclusively randomisation settings<br/>
      /// Everything in this class is exported and imported via the export and import configurations buttons<br/>
      /// </summary>
      public class RandomizationConfig {

            private static readonly RandomizationConfig _instance = new RandomizationConfig();

            [EditorBrowsable(EditorBrowsableState.Never)]
            public RandomizationConfig() {
                  _created = DateTime.Now;

                  _iconTextures.Updated += SaveConfigAfterDelay;
                  _menuTextures.Updated += SaveConfigAfterDelay;
                  _shopTextures.Updated += SaveConfigAfterDelay;
                  _editorTextures.Updated += SaveConfigAfterDelay;
                  _tileTextures.Updated += SaveConfigAfterDelay;
                  _portalTextures.Updated += SaveConfigAfterDelay;
                  _orbTextures.Updated += SaveConfigAfterDelay;
                  _padTextures.Updated += SaveConfigAfterDelay;
                  _particleTextures.Updated += SaveConfigAfterDelay;
                  _effectTextures.Updated += SaveConfigAfterDelay;
                  _miscTextures.Updated += SaveConfigAfterDelay;
                  _fontRand.Updated += SaveConfigAfterDelay;
            }

            public static RandomizationConfig Instance {
                  get => _instance;
            }

            public IconRandSettings IconTextures {
                  get => _iconTextures;
                  set => UpdateSetting(_iconTextures, value, _defaultIconTextures);
            }

            public RandomizationSetting MenuTextures {
                  get => _menuTextures;
                  set => UpdateSetting(_menuTextures, value, _defaultMenuTextures);
            }

            public RandomizationSetting ShopTextures {
                  get => _shopTextures;
                  set => UpdateSetting(_shopTextures, value, _defaultShopTextures);
            }

            public RandomizationSetting EditorTextures {
                  get => _editorTextures;
                  set => UpdateSetting(_editorTextures, value, _defaultEditorTextures);
            }

            public RandomizationSetting TileTextures {
                  get => _tileTextures;
                  set => UpdateSetting(_tileTextures, value, _defaultTileTextures);
            }

            public RandomizationSetting PortalTextures {
                  get => _portalTextures;
                  set => UpdateSetting(_portalTextures, value, _defaultPortalTextures);
            }

            public RandomizationSetting OrbTextures {
                  get => _orbTextures;
                  set => UpdateSetting(_orbTextures, value, _defaultOrbTextures);
            }

            public RandomizationSetting PadTextures {
                  get => _padTextures;
                  set => UpdateSetting(_padTextures, value, _defaultPadTextures);
            }

            public RandomizationSetting ParticleTextures {
                  get => _particleTextures;
                  set => UpdateSetting(_particleTextures, value, _defaultParticleTextures);
            }

            public RandomizationSetting EffectTextures {
                  get => _effectTextures;
                  set => UpdateSetting(_effectTextures, value, _defaultEffectTextures);
            }

            public RandomizationSetting MiscTextures {
                  get => _miscTextures;
                  set => UpdateSetting(_miscTextures, value, _defaultMiscTextures);
 
            }

            public FontRandomisationSettings FontRand {
                  get => _fontRand;
                  set => UpdateSetting(_fontRand, value, _defaultFontRand);
            }

            public float MaxSpriteMultiplier {
                  get => _maxSpriteMultiplier;
                  set {
                        _maxSpriteMultiplier = Math.Clamp(value, MinSpriteMultiplier, UnlimitedSpriteMultiplier);
                        SaveConfigAfterDelay();
                  }
            }

            public bool AllowDuplicates {
                  get => _allowDuplicates;
                  set {
                        _allowDuplicates = value;
                        SaveConfigAfterDelay();
                  }
            }

            public int Seed {
                  get => _seed;
                  set {
                        _seed = value;
                        SaveConfigAfterDelay();
                  }
            }

            private void UpdateSetting<T>(T target, T source, T defaults) where T : ICopyableSetting<T> {
                  if (source == null)
                        source = defaults;

                  target.CopyDataFrom(source);
                  SaveConfigAfterDelay();
            }

            public int GetEnabledSettingsCount() {
                  return (IconTextures.Enabled ? 1 : 0) +
                         (MenuTextures.Enabled ? 1 : 0) +
                         (ShopTextures.Enabled ? 1 : 0) +
                         (EditorTextures.Enabled ? 1 : 0) +
                         (TileTextures.Enabled ? 1 : 0) +
                         (PortalTextures.Enabled ? 1 : 0) +
                         (OrbTextures.Enabled ? 1 : 0) +
                         (PadTextures.Enabled ? 1 : 0) +
                         (ParticleTextures.Enabled ? 1 : 0) +
                         (EffectTextures.Enabled ? 1 : 0) +
                         (MiscTextures.Enabled ? 1 : 0) +
                         (FontRand.Enabled ? 1 : 0);
            }

            public IEnumerable<int> GetDistinctGroups() {
                  return GetAllRandSettings().Select(g => g.Group).Distinct();
            }

            public RandomizationSetting[] GetAllRandSettings() {
                  return new RandomizationSetting[] {
                        IconTextures.Cube,
                        IconTextures.Ship,
                        IconTextures.Ball,
                        IconTextures.Ufo,
                        IconTextures.Wave,
                        IconTextures.Robot,
                        IconTextures.Spider,
                        IconTextures.Swing,
                        IconTextures.Jetpack,

                        MenuTextures,
                        ShopTextures,
                        EditorTextures,
                        TileTextures,
                        PortalTextures,
                        OrbTextures,
                        PadTextures,
                        ParticleTextures,
                        EffectTextures,
                        MiscTextures,
                  };
            }

            public static void ReadFile() {
                  if (File.Exists(configFileName)) {
                        string inStream = File.ReadAllText(configFileName);
                        RandomizationConfig rc = Deserialise(inStream, out bool success);
                        if (success) {
                              _instance.ApplyFrom(rc);
                        }

                  } else {
                        Logger.WriteToFile(Logger.Mode.Info, $"The file \"{configFileName}\" does not exist. Loading default configuration.");
                  }
            }

            public void SaveConfigAfterDelay() => SaveConfigAfterDelay(this, null);

            public void SaveConfigAfterDelay(object sender, EventArgs e) {
                  TimeSpan elapsedSinceCreated = DateTime.Now - _created;
                  if (elapsedSinceCreated.TotalSeconds < 1)
                        return;

                  if (_fileSaveQueued == true)
                        return;

                  _fileSaveQueued = true;
                  Task.Delay(new TimeSpan(0, 0, AdvancedConfig.Instance.ConfigSaveDelay)).ContinueWith(o => { WriteFile(); });
            }

            public void WriteFile() {
                  Logger.Write(Logger.Mode.Debug, "Saving Config File");

                  string outStream = this.Serialize();

                  try {
                        File.WriteAllText(configFileName, outStream);

                  } catch (IOException ioExcept) {
                        Logger.Write(Logger.Mode.Error, $"Failed to write config file. Reason: {ioExcept}");
                  }

                  _fileSaveQueued = false;
            }

            public string Serialize() => Serialize(FileSerializationOptions);

            public string SerializeExportData() => Serialize(ExportSerializationOptions);

            string Serialize(JsonSerializerOptions jso) {
                  return JsonSerializer.Serialize(this, jso);
            }

            public static RandomizationConfig Deserialise(string data, out bool success) {
                  success = false;

                  RandomizationConfig ret;
                  try {
                        ret = JsonSerializer.Deserialize<RandomizationConfig>(data);
                        success = (ret != null);

                  } catch (JsonException JSON_Except) {
                        Logger.Write(Logger.Mode.Error, $"Failed to convert the config file from JSON. Reason: {JSON_Except}");
                        ret = null;
                  }

                  return ret;
            }

            public string GetExportConfigData() => DataStream.Compress(this.SerializeExportData());

            /// <returns>If the import was successful</returns>
            public bool ImportConfigData(string data) {
                  if (data.Length == 0) {
                        return false;
                  }

                  string decompressedStream = DataStream.Decompress(data);

                  if (decompressedStream.Length == 0) {
                        return false;
                  }

                  RandomizationConfig importedConfig = null;

                  try {
                        importedConfig = JsonSerializer.Deserialize<RandomizationConfig>(decompressedStream);
                  }
                  catch (JsonException JSON_Except) {
                        Logger.Write(Logger.Mode.Error, $"Failed to convert the config file from JSON. Reason: {JSON_Except}");
                        return false;
                  }

                  if (importedConfig == null)
                        return false;

                  this.ApplyFrom(importedConfig);

                  return true;
            }

            private void ApplyFrom(RandomizationConfig other) {
                  if (other == null) {
                        Logger.Write(Logger.Mode.Error, "The parameter \"other\" was passed as null in Config.cs.");
                        return;
                  }

                  this.IconTextures = other.IconTextures;
                  this.MenuTextures = other.MenuTextures;
                  this.ShopTextures = other.ShopTextures;
                  this.EditorTextures = other.EditorTextures;
                  this.TileTextures = other.TileTextures;
                  this.PortalTextures = other.PortalTextures;
                  this.OrbTextures = other.OrbTextures;
                  this.PadTextures = other.PadTextures;
                  this.ParticleTextures = other.ParticleTextures;
                  this.EffectTextures = other.EffectTextures;
                  this.MiscTextures = other.MiscTextures;
                  this.FontRand = other.FontRand;

                  this.MaxSpriteMultiplier = other.MaxSpriteMultiplier;
                  this.AllowDuplicates = other.AllowDuplicates;
                  this.Seed = other.Seed;
            }

            // Private members
            private readonly IconRandSettings _iconTextures = _defaultIconTextures;
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
            private readonly FontRandomisationSettings _fontRand = _defaultFontRand;

            private float _maxSpriteMultiplier = 1.10f;
            private bool _allowDuplicates = false;
            private int _seed = 0;

            const string configFileName = "config.ini";
            public const int maxTextureGroups = 100;

            private bool _fileSaveQueued = false;

            // This exists to make it so upon creation it doesn't immediately try to save the config file
            private DateTime _created;

            private static readonly JsonSerializerOptions FileSerializationOptions = new JsonSerializerOptions() {
                  WriteIndented = true
            };

            // Serialization control logic. This will cut down on the size of the exported config data substantially
            // Disclaimer: The core of this is mostly AI generated, then expanded manually
            private static readonly JsonSerializerOptions ExportSerializationOptions = new JsonSerializerOptions() {
                  TypeInfoResolver = new DefaultJsonTypeInfoResolver {
                        Modifiers = {
                              typeInfo => {
                                    if (typeInfo.Type == typeof(RandomizationSetting)) {
                                          foreach (var property in typeInfo.Properties) {
                                                if (property.Name == nameof(RandomizationSetting.PercentageRandomised)) {
                                                      property.ShouldSerialize = (_, value) => (int)value != DefaultPercentageRandomised;

                                                } else if (property.Name == nameof(RandomizationSetting.Group)) {
                                                      property.ShouldSerialize = (_, value) => (int)value != DefaultGroup;

                                                } else if (property.Name == nameof(RandomizationSetting.Enabled)) {
                                                      property.ShouldSerialize = (_, value) => (bool)value != true;
                                                }
                                          }
                                    }
                              }
                        }
                  }
            };

            // Default values for all settings
            private static IconRandSettings _defaultIconTextures => IconRandSettings.Default;
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
            private static FontRandomisationSettings _defaultFontRand => FontRandomisationSettings.Default;

            public const float MinSpriteMultiplier = 1.01f;
            public const float UnlimitedSpriteMultiplier = 1_000_000f;

            public const int DefaultPercentageRandomised = 100;
            public const int DefaultGroup = 0;
      }
}
