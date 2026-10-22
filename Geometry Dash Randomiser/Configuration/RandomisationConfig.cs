using Geometry_Dash_Randomiser.Helpers;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Geometry_Dash_Randomiser {

      /// <summary>
      /// Singleton class for storing this application's simpler and user-facing configurations<br/>
      /// These are exclusively randomisation settings<br/>
      /// Everything in this class is exported and imported via the export and import configurations buttons<br/>
      /// </summary>
      public class RandomisationConfig {

            private static RandomisationConfig _instance;

            // This has to be public for JSON deserialization sadly
            [EditorBrowsable(EditorBrowsableState.Never)]
            public RandomisationConfig() { }

            public static RandomisationConfig Instance {
                  get {
                        if (_instance == null)
                              _instance = new RandomisationConfig();
                        return _instance;
                  }
            }

            public IconRandSettings IconTextures { get; set; } = new IconRandSettings { Group = 0 };
            public RandomizationSetting MenuTextures { get; set; } = new RandomizationSetting(group: 1, enabled: true);
            public RandomizationSetting ShopTextures { get; set; } = new RandomizationSetting(group: 1, enabled: true);
            public RandomizationSetting EditorTextures { get; set; } = new RandomizationSetting();
            public RandomizationSetting TileTextures { get; set; } = new RandomizationSetting();
            public RandomizationSetting PortalTextures { get; set; } = new RandomizationSetting(group: 0, true);
            public RandomizationSetting OrbTextures { get; set; } = new RandomizationSetting(group: 3, enabled: true);
            public RandomizationSetting PadTextures { get; set; } = new RandomizationSetting(group: 0, enabled: true);
            public RandomizationSetting ParticleTextures { get; set; } = new RandomizationSetting(group: 3, enabled: true);
            public RandomizationSetting EffectTextures { get; set; } = new RandomizationSetting(group: 2, enabled: true);
            public RandomizationSetting MiscTextures { get; set; } = new RandomizationSetting();
            public FontRandomisationSettings FontRand { get; set; } = new FontRandomisationSettings();

            public float MaxSpriteMultiplier { get; set; } = 1.10f;
            public bool AllowDuplicates { get; set; } = false;
            public int Seed { get; set; } = 0;

            public int GetEnabledSettingsCount() {
                  return Convert.ToInt32(IconTextures.AnySettingEnabled()) +
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

            public void WriteFile() {
                  Logger.Write(Logger.Mode.Verbose, "Saving Config File");

                  string outStream = this.Serialize();

                  try {
                        File.WriteAllText(configFileName, outStream);

                  } catch (IOException ioExcept) {
                        Logger.Write(Logger.Mode.Error, $"Failed to write config file. Reason: {ioExcept}");
                  }
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

            const string configFileName = "config.ini";
            public const int maxTextureGroups = 100;

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
