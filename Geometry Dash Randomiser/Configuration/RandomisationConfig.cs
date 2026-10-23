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
                  _iconTextures.ConfigUpdated += SaveConfigAfterDelay;
                  _menuTextures.ConfigUpdated += SaveConfigAfterDelay;
                  _shopTextures.ConfigUpdated += SaveConfigAfterDelay;
                  _editorTextures.ConfigUpdated += SaveConfigAfterDelay;
                  _tileTextures.ConfigUpdated += SaveConfigAfterDelay;
                  _portalTextures.ConfigUpdated += SaveConfigAfterDelay;
                  _orbTextures.ConfigUpdated += SaveConfigAfterDelay;
                  _padTextures.ConfigUpdated += SaveConfigAfterDelay;
                  _particleTextures.ConfigUpdated += SaveConfigAfterDelay;
                  _effectTextures.ConfigUpdated += SaveConfigAfterDelay;
                  _miscTextures.ConfigUpdated += SaveConfigAfterDelay;
                  _fontRand.ConfigUpdated += SaveConfigAfterDelay;
            }

            public static RandomisationConfig Instance {
                  get => _instance;
            }

            public IconRandSettings IconTextures {
                  get => _iconTextures;
                  set {
                        _iconTextures.CopyDataFrom(value ?? IconRandSettings.Default);
                        SaveConfigAfterDelay();
                  }
            }

            public RandomizationSetting MenuTextures {
                  get => _menuTextures;
                  set {
                        _menuTextures.CopyDataFrom(value ?? RandomizationSetting.Default);
                        SaveConfigAfterDelay();
                  }
            }

            public RandomizationSetting ShopTextures {
                  get => _shopTextures;
                  set {
                        _shopTextures.CopyDataFrom(value ?? RandomizationSetting.Default);
                        SaveConfigAfterDelay();
                  }
            }

            public RandomizationSetting EditorTextures {
                  get => _editorTextures;
                  set {
                        _editorTextures.CopyDataFrom(value ?? RandomizationSetting.Default);
                        SaveConfigAfterDelay();
                  }
            }

            public RandomizationSetting TileTextures {
                  get => _tileTextures;
                  set {
                        _tileTextures.CopyDataFrom(value ?? RandomizationSetting.Default);
                        SaveConfigAfterDelay();
                  }
            }

            public RandomizationSetting PortalTextures {
                  get => _portalTextures;
                  set {
                        _portalTextures.CopyDataFrom(value ?? RandomizationSetting.Default);
                        SaveConfigAfterDelay();
                  }
            }

            public RandomizationSetting OrbTextures {
                  get => _orbTextures;
                  set {
                        _orbTextures.CopyDataFrom(value ?? RandomizationSetting.Default);
                        SaveConfigAfterDelay();
                  }
            }

            public RandomizationSetting PadTextures {
                  get => _padTextures;
                  set {
                        _padTextures.CopyDataFrom(value ?? RandomizationSetting.Default);
                        SaveConfigAfterDelay();
                  }
            }

            public RandomizationSetting ParticleTextures {
                  get => _particleTextures;
                  set {
                        _particleTextures.CopyDataFrom(value ?? RandomizationSetting.Default);
                        SaveConfigAfterDelay();
                  }
            }

            public RandomizationSetting EffectTextures {
                  get => _effectTextures;
                  set {
                        _effectTextures.CopyDataFrom(value ?? RandomizationSetting.Default);
                        SaveConfigAfterDelay();
                  }
            }

            public RandomizationSetting MiscTextures {
                  get => _miscTextures;
                  set {
                        _miscTextures.CopyDataFrom(value ?? RandomizationSetting.Default);
                        SaveConfigAfterDelay();
                  }
            }

            public FontRandomisationSettings FontRand {
                  get => _fontRand;
                  set {
                        _fontRand.CopyDataFrom(value ?? FontRandomisationSettings.Default);
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
                        _instance = Deserialise(inStream);

                  } else {
                        Logger.WriteToFile(Logger.Mode.Info, $"The file \"{configFileName}\" does not exist. Loading default configuration.");
                  }
            }

            public void SaveConfigAfterDelay() => SaveConfigAfterDelay(this, null);

            public void SaveConfigAfterDelay(object sender, EventArgs e) {
                  if (_fileSaveNeeded == true)
                        return;

                  Console.WriteLine("Saving Task started");
                  _fileSaveNeeded = true;
                  Task.Delay(new TimeSpan(0, 0, 5)).ContinueWith(o => { WriteFile(); });
            }

            public void WriteFile() {
                  Logger.Write(Logger.Mode.Verbose, "Saving Config File");

                  string outStream = this.Serialize();

                  try {
                        File.WriteAllText(configFileName, outStream);

                  } catch (IOException ioExcept) {
                        Logger.Write(Logger.Mode.Error, $"Failed to write config file. Reason: {ioExcept}");
                  }

                  _fileSaveNeeded = false;
            }

            string Serialize() {
                  return JsonSerializer.Serialize(_instance, SerializationOptions);
            }

            public static RandomisationConfig Deserialise(string data) {
                  RandomisationConfig ret;
                  try {
                        ret = JsonSerializer.Deserialize<RandomisationConfig>(data);

                  } catch (JsonException JSON_Except) {
                        Logger.Write(Logger.Mode.Error, $"Failed to convert the config file from JSON. Reason: {JSON_Except}");
                        ret = _instance;
                  }

                  if (ret.MaxSpriteMultiplier < 1.01f) {
                        ret.MaxSpriteMultiplier = 1.10f;
                  }

                  return ret;
            }

            public string GetExportConfigData() => DataStream.Compress(this.Serialize());

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

            private readonly IconRandSettings _iconTextures = new IconRandSettings { Group = 0 };
            private readonly RandomizationSetting _menuTextures = new RandomizationSetting(group: 1, enabled: true);
            private readonly RandomizationSetting _shopTextures = new RandomizationSetting(group: 1, enabled: true);
            private readonly RandomizationSetting _editorTextures = new RandomizationSetting();
            private readonly RandomizationSetting _tileTextures  = new RandomizationSetting();
            private readonly RandomizationSetting _portalTextures = new RandomizationSetting(group: 0, true);
            private readonly RandomizationSetting _orbTextures = new RandomizationSetting(group: 3, enabled: true);
            private readonly RandomizationSetting _padTextures = new RandomizationSetting(group: 0, enabled: true);
            private readonly RandomizationSetting _particleTextures = new RandomizationSetting(group: 3, enabled: true);
            private readonly RandomizationSetting _effectTextures = new RandomizationSetting(group: 2, enabled: true);
            private readonly RandomizationSetting _miscTextures = new RandomizationSetting();
            private readonly FontRandomisationSettings _fontRand = new FontRandomisationSettings();

            private float _maxSpriteMultiplier = 1.10f;
            private bool _allowDuplicates = false;
            private int _seed = 0;

            const string configFileName = "config.ini";
            public const int maxTextureGroups = 100;

            private bool _fileSaveNeeded = false;

            private static readonly JsonSerializerOptions SerializationOptions = new JsonSerializerOptions() {
                  WriteIndented = true,
                  TypeInfoResolver = new DefaultJsonTypeInfoResolver {
                        Modifiers = {
                              typeInfo => {
                                    if (typeInfo.Type == typeof(RandomizationSetting)) {
                                          foreach (var property in typeInfo.Properties) {
                                                if (property.Name == nameof(RandomizationSetting.PercentageRandomised)) {
                                                      // Don't serialise PercentageRandomised if the value is 100
                                                      property.ShouldSerialize = (_, value) => (int)value != 100;
                                                }
                                          }
                                    }
                              }
                        }
                  }
            };
      }
}
