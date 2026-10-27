using Geometry_Dash_Randomiser.Helpers;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace Geometry_Dash_Randomiser {

      /// <summary>
      /// Singleton class for storing this application's simpler and user-facing configurations<br/>
      /// These are exclusively randomisation settings<br/>
      /// Everything in this class is exported and imported via the export and import configurations buttons<br/>
      /// </summary>
      public class RandomisationConfig {

            private static RandomisationConfig _instance = new RandomisationConfig();

            [EditorBrowsable(EditorBrowsableState.Never)]
            public RandomisationConfig() {
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

            public static RandomisationConfig Instance {
                  get => _instance;
            }

            public IconRandSettings IconTextures {
                  get => _iconTextures;
                  set {
                        _iconTextures.CopyDataFrom(value ?? _defaultIconTextures);
                        SaveConfigAfterDelay();
                  }
            }

            public RandomizationSetting MenuTextures {
                  get => _menuTextures;
                  set {
                        _menuTextures.CopyDataFrom(value ?? _defaultMenuTextures);
                        SaveConfigAfterDelay();
                  }
            }

            public RandomizationSetting ShopTextures {
                  get => _shopTextures;
                  set {
                        _shopTextures.CopyDataFrom(value ?? _defaultShopTextures);
                        SaveConfigAfterDelay();
                  }
            }

            public RandomizationSetting EditorTextures {
                  get => _editorTextures;
                  set {
                        _editorTextures.CopyDataFrom(value ?? _defaultEditorTextures);
                        SaveConfigAfterDelay();
                  }
            }

            public RandomizationSetting TileTextures {
                  get => _tileTextures;
                  set {
                        _tileTextures.CopyDataFrom(value ?? _defaultTileTextures);
                        SaveConfigAfterDelay();
                  }
            }

            public RandomizationSetting PortalTextures {
                  get => _portalTextures;
                  set {
                        _portalTextures.CopyDataFrom(value ?? _defaultPortalTextures);
                        SaveConfigAfterDelay();
                  }
            }

            public RandomizationSetting OrbTextures {
                  get => _orbTextures;
                  set {
                        _orbTextures.CopyDataFrom(value ?? _defaultOrbTextures);
                        SaveConfigAfterDelay();
                  }
            }

            public RandomizationSetting PadTextures {
                  get => _padTextures;
                  set {
                        _padTextures.CopyDataFrom(value ?? _defaultPadTextures);
                        SaveConfigAfterDelay();
                  }
            }

            public RandomizationSetting ParticleTextures {
                  get => _particleTextures;
                  set {
                        _particleTextures.CopyDataFrom(value ?? _defaultPadTextures);
                        SaveConfigAfterDelay();
                  }
            }

            public RandomizationSetting EffectTextures {
                  get => _effectTextures;
                  set {
                        _effectTextures.CopyDataFrom(value ?? _defaultEffectTextures);
                        SaveConfigAfterDelay();
                  }
            }

            public RandomizationSetting MiscTextures {
                  get => _miscTextures;
                  set {
                        _miscTextures.CopyDataFrom(value ?? _defaultMiscTextures);
                        SaveConfigAfterDelay();
                  }
            }

            public FontRandomisationSettings FontRand {
                  get => _fontRand;
                  set {
                        _fontRand.CopyDataFrom(value ?? _defaultFontRand);
                        SaveConfigAfterDelay();
                  }
            }

            public float MaxSpriteMultiplier {
                  get => _maxSpriteMultiplier;
                  set {
                        _maxSpriteMultiplier = Math.Clamp(value, 1.01f, 1000f);
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

            public int GetEnabledSettingsCount() {
                  return Convert.ToInt32(IconTextures.Enabled) +
                        Convert.ToInt32(MenuTextures.Enabled) +
                        Convert.ToInt32(ShopTextures.Enabled) +
                        Convert.ToInt32(EditorTextures.Enabled) +
                        Convert.ToInt32(TileTextures.Enabled) +
                        Convert.ToInt32(PortalTextures.Enabled) +
                        Convert.ToInt32(OrbTextures.Enabled) +
                        Convert.ToInt32(PadTextures.Enabled) +
                        Convert.ToInt32(ParticleTextures.Enabled) +
                        Convert.ToInt32(EffectTextures.Enabled) +
                        Convert.ToInt32(MiscTextures.Enabled) +
                        Convert.ToInt32(FontRand.Enabled);
            }

            public static void ReadFile() {
                  if (File.Exists(configFileName)) {
                        string inStream = File.ReadAllText(configFileName);
                        RandomisationConfig rc = Deserialise(inStream, out bool success);
                        if (success) {
                              _instance.ApplyAllProperties(rc);
                        }

                  } else {
                        Logger.WriteToFile(Logger.Mode.Info, $"The file \"{configFileName}\" does not exist. Loading default configuration.");
                  }
            }

            public void SaveConfigAfterDelay() => SaveConfigAfterDelay(this, null);

            public void SaveConfigAfterDelay(object sender, EventArgs e) {
                  TimeSpan elapsedSinceCreated = DateTime.Now - _created;
                  if (elapsedSinceCreated.Seconds < 1)
                        return;

                  if (_fileSaveQueued == true)
                        return;

                  _fileSaveQueued = true;
                  Task.Delay(new TimeSpan(0, 0, AdvancedConfig.Instance.ConfigSaveDelay)).ContinueWith(o => { WriteFile(); });
            }

            public void WriteFile() {
                  Logger.Write(Logger.Mode.Verbose, "Saving Config File");

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
                  return JsonSerializer.Serialize(_instance, jso);
            }

            public static RandomisationConfig Deserialise(string data, out bool success) {
                  success = false;

                  RandomisationConfig ret;
                  try {
                        ret = JsonSerializer.Deserialize<RandomisationConfig>(data);
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

                  RandomisationConfig importedConfig = null;

                  try {
                        importedConfig = JsonSerializer.Deserialize<RandomisationConfig>(decompressedStream);
                  }
                  catch (JsonException JSON_Except) {
                        Logger.Write(Logger.Mode.Error, $"Failed to convert the config file from JSON. Reason: {JSON_Except}");
                        return false;
                  }

                  this.ApplyAllProperties(importedConfig);

                  return true;
            }

            private void ApplyAllProperties(RandomisationConfig other) {
                  if (other == null) {
                        Logger.Write(Logger.Mode.Error, "The parameter \"other\" was passed as null in Config.cs.");
                        return;
                  }

                  _instance.IconTextures = other.IconTextures;
                  _instance.MenuTextures = other.MenuTextures;
                  _instance.ShopTextures = other.ShopTextures;
                  _instance.EditorTextures = other.EditorTextures;
                  _instance.TileTextures = other.TileTextures;
                  _instance.PortalTextures = other.PortalTextures;
                  _instance.OrbTextures = other.OrbTextures;
                  _instance.PadTextures = other.PadTextures;
                  _instance.ParticleTextures = other.ParticleTextures;
                  _instance.EffectTextures = other.EffectTextures;
                  _instance.MiscTextures = other.MiscTextures;
                  _instance.FontRand = other.FontRand;

                  _instance.MaxSpriteMultiplier = other.MaxSpriteMultiplier;
                  _instance.AllowDuplicates = other.AllowDuplicates;
                  _instance.Seed = other.Seed;
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
                                                      property.ShouldSerialize = (_, value) => (int)value != 100;

                                                } else if (property.Name == nameof(RandomizationSetting.Group)) {
                                                      property.ShouldSerialize = (_, value) => (int)value != 0;

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
            private static IconRandSettings _defaultIconTextures => new IconRandSettings { Group = 0, Enabled = true };
            private static RandomizationSetting _defaultMenuTextures => new RandomizationSetting { Group = 1, Enabled = true };
            private static RandomizationSetting _defaultShopTextures => new RandomizationSetting { Group = 1, Enabled = true };
            private static RandomizationSetting _defaultEditorTextures => new RandomizationSetting { };
            private static RandomizationSetting _defaultTileTextures => new RandomizationSetting { };
            private static RandomizationSetting _defaultPortalTextures => new RandomizationSetting { Group = 0, Enabled = true };
            private static RandomizationSetting _defaultOrbTextures => new RandomizationSetting { Group = 3, Enabled = true };
            private static RandomizationSetting _defaultPadTextures => new RandomizationSetting { Group = 0, Enabled = true };
            private static RandomizationSetting _defaultParticleTextures => new RandomizationSetting { Group = 3, Enabled = true };
            private static RandomizationSetting _defaultEffectTextures => new RandomizationSetting { Group = 2, Enabled = true };
            private static RandomizationSetting _defaultMiscTextures => new RandomizationSetting { };
            private static FontRandomisationSettings _defaultFontRand => new FontRandomisationSettings { };
      }
}
