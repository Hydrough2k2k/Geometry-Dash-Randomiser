using Geometry_Dash_Randomiser.Helpers;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

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
            public RandomisationSetting MenuTextures { get; set; } = new RandomisationSetting(group: 1, enabled: true);
            public RandomisationSetting ShopTextures { get; set; } = new RandomisationSetting(group: 1, enabled: true);
            public RandomisationSetting EditorTextures { get; set; } = new RandomisationSetting();
            public RandomisationSetting TileTextures { get; set; } = new RandomisationSetting();
            public RandomisationSetting PortalTextures { get; set; } = new RandomisationSetting(group: 0, true);
            public RandomisationSetting OrbTextures { get; set; } = new RandomisationSetting(group: 3, enabled: true);
            public RandomisationSetting PadTextures { get; set; } = new RandomisationSetting(group: 0, enabled: true);
            public RandomisationSetting ParticleTextures { get; set; } = new RandomisationSetting(group: 3, enabled: true);
            public RandomisationSetting EffectTextures { get; set; } = new RandomisationSetting(group: 2, enabled: true);
            public RandomisationSetting MiscTextures { get; set; } = new RandomisationSetting();
            public FontRandomisationSettings FontRand { get; set; } = new FontRandomisationSettings();

            public float MaxSpriteMultiplier { get; set; } = 1.10f;
            public bool AllowDuplicates { get; set; } = false;
            public int Seed { get; set; } = 0;

            private void SaveConfigFileAfterDelay(int seconds = 0) {
                  Task.Delay(new TimeSpan(0, 0, seconds)).ContinueWith(o => { WriteFile(); });
            }

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
                        Log.WriteToFile(Log.Mode.Info, $"The file \"{configFileName}\" does not exist. Loading default configuration.");
                  }
            }

            public void WriteFile() {
                  Log.Write(Log.Mode.Verbose, "Saving Config File");

                  string outStream = this.Serialize();

                  try {
                        File.WriteAllText(configFileName, outStream);

                  } catch (IOException ioExcept) {
                        Log.Write(Log.Mode.Error, $"Failed to write config file. Reason: {ioExcept}");
                  }
            }

            string Serialize() {
                  JsonSerializerOptions options = new JsonSerializerOptions {
                        WriteIndented = true
                  };
                  return Serialize(options);
            }

            string Serialize(JsonSerializerOptions options) {
                  return JsonSerializer.Serialize(_instance, options);
            }

            public static RandomisationConfig Deserialise(string data) {
                  RandomisationConfig ret;
                  try {
                        ret = JsonSerializer.Deserialize<RandomisationConfig>(data);

                  } catch (JsonException JSON_Except) {
                        Log.Write(Log.Mode.Error, $"Failed to convert the config file from JSON. Reason: {JSON_Except}");
                        ret = _instance;
                  }

                  if (ret.MaxSpriteMultiplier < 1.01f) {
                        ret.MaxSpriteMultiplier = 1.10f;
                  }

                  return ret;
            }

            public string GetExportConfigData() {
                  JsonSerializerOptions options = new JsonSerializerOptions {
                        WriteIndented = false
                  };

                  return DataStream.Compress(this.Serialize(options));
            }

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
                        Log.Write(Log.Mode.Error, $"Failed to convert the config file from JSON. Reason: {JSON_Except}");
                        return false;
                  }

                  this.ApplyAllProperties(importedConfig);

                  return true;
            }

            private void ApplyAllProperties(RandomisationConfig other) {
                  if (other == null) {
                        Log.Write(Log.Mode.Error, "The parameter \"other\" was passed as null in Config.cs.");
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
      }
}
