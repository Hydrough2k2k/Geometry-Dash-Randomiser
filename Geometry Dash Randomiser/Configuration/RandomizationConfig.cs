using Geometry_Dash_Randomiser.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace Geometry_Dash_Randomiser {

      /// <summary>
      /// Singleton class for storing this application's simpler and user-facing configurations<br/>
      /// These are exclusively randomisation settings<br/>
      /// Everything in this class is exported and imported via the export and import configurations buttons<br/>
      /// </summary>
      public class RandomizationConfig : IEquatable<RandomizationConfig> {

            private static readonly RandomizationConfig _instance = new RandomizationConfig();

            [EditorBrowsable(EditorBrowsableState.Never)]
            public RandomizationConfig() {
                  _created = DateTime.Now;

                  _iconTextures.Updated += SaveConfigAfterDelay;
                  _gameTextureConfig.Updated += SaveConfigAfterDelay;
                  _fontRand.Updated += SaveConfigAfterDelay;
            }

            public static RandomizationConfig Instance {
                  get => _instance;
            }

            public IconRandomizationSettings IconTextures {
                  get => _iconTextures;
                  set => UpdateSetting(_iconTextures, value, _defaultIconTextures);
            }

            public GameTextureConfig GameTextureConfig {
                  get => _gameTextureConfig;
                  set => UpdateSetting(_gameTextureConfig, value, _defaultGameTextureConfig);
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
                         GameTextureConfig.GetEnabledSettingsCount() +
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

                        GameTextureConfig.Menu,
                        GameTextureConfig.Shop,
                        GameTextureConfig.Editor,
                        GameTextureConfig.Tile,
                        GameTextureConfig.Portal,
                        GameTextureConfig.Orb,
                        GameTextureConfig.Pad,
                        GameTextureConfig.Particle,
                        GameTextureConfig.Effect,
                        GameTextureConfig.Misc,

                        // The font ones are missing from this on purpose for now. Later they will be added when font and game textures can be mixed around
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
                        Logger.Write(Logger.Mode.Error, "The parameter \"other\" was passed as null in RandomizationConfig.ApplyFrom");
                        return;
                  }

                  this.IconTextures = other.IconTextures;
                  this.GameTextureConfig = other.GameTextureConfig;
                  this.FontRand = other.FontRand;

                  this.MaxSpriteMultiplier = other.MaxSpriteMultiplier;
                  this.AllowDuplicates = other.AllowDuplicates;
                  this.Seed = other.Seed;
            }

            public bool Equals(RandomizationConfig other) {
                  if (other == null) {
                        Logger.Write(Logger.Mode.Error, "The parameter \"other\" was passed as null in RandomizationConfig.Equals");
                        return false;
                  }

                  if (this.IconTextures.Equals(other.IconTextures) == false ||
                        this.GameTextureConfig.Equals(other.GameTextureConfig) == false ||
                        this.FontRand.Equals(other.FontRand) == false) {

                        return false;
                  }

                  return true;
            }

            // Private members
            private readonly IconRandomizationSettings _iconTextures = _defaultIconTextures;
            private readonly GameTextureConfig _gameTextureConfig = _defaultGameTextureConfig;
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
            private static IconRandomizationSettings _defaultIconTextures => IconRandomizationSettings.Default;
            private static GameTextureConfig _defaultGameTextureConfig => GameTextureConfig.Default;
            private static FontRandomisationSettings _defaultFontRand => FontRandomisationSettings.Default;

            public const float MinSpriteMultiplier = 1.01f;
            public const float UnlimitedSpriteMultiplier = 1_000_000f;

            public const int DefaultPercentageRandomised = 100;
            public const int DefaultGroup = 0;

            #region Legacy Fields

            // Legacy Fields for reading JSON data

            [EditorBrowsable(EditorBrowsableState.Never)]
            public RandomizationSetting MenuTextures {
                  set => GameTextureConfig.UpdateSetting(value, GameTextureConfig.SettingType.Menu);
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public RandomizationSetting ShopTextures {
                  set => GameTextureConfig.UpdateSetting(value, GameTextureConfig.SettingType.Shop);
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public RandomizationSetting EditorTextures {
                  set => GameTextureConfig.UpdateSetting(value, GameTextureConfig.SettingType.Editor);
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public RandomizationSetting TileTextures {
                  set => GameTextureConfig.UpdateSetting(value, GameTextureConfig.SettingType.Tile);
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public RandomizationSetting PortalTextures {
                  set => GameTextureConfig.UpdateSetting(value, GameTextureConfig.SettingType.Portal);
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public RandomizationSetting OrbTextures {
                  set => GameTextureConfig.UpdateSetting(value, GameTextureConfig.SettingType.Orb);
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public RandomizationSetting PadTextures {
                  set => GameTextureConfig.UpdateSetting(value, GameTextureConfig.SettingType.Pad);
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public RandomizationSetting ParticleTextures {
                  set => GameTextureConfig.UpdateSetting(value, GameTextureConfig.SettingType.Particle);
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public RandomizationSetting EffectTextures {
                  set => GameTextureConfig.UpdateSetting(value, GameTextureConfig.SettingType.Effect);
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public RandomizationSetting MiscTextures {
                  set => GameTextureConfig.UpdateSetting(value, GameTextureConfig.SettingType.Misc);
            }

            #endregion
      }
}
