using System;
using System.IO;
using System.Text.Json;
using static Geometry_Dash_Randomiser.FontRandomisationSettings;
using static Geometry_Dash_Randomiser.GameFileManager;

namespace Geometry_Dash_Randomiser {

      // Convert to singleton later after doing some testing in another project. I'm not sure how deserialisation works with singletons
      public static class Config {

            const string configFileName = "config.txt";

            // This is where the default values are defined
            public static string gameDirectory = "";
            public static bool autoOverwriteFiles = true;

            // Configs for every randomisation type
            // Default: group 0, disabled
            public static IconRandSettings iconTextures = new IconRandSettings();
            public static RandomisationSetting menuTextures = new RandomisationSetting(group: 2, enabled: true);
            public static RandomisationSetting shopTextures = new RandomisationSetting();
            public static RandomisationSetting editorTextures = new RandomisationSetting();
            public static RandomisationSetting tileTextures = new RandomisationSetting();
            public static RandomisationSetting portalTextures = new RandomisationSetting(group: 0, true);
            public static RandomisationSetting orbTextures = new RandomisationSetting(group: 3, enabled: true);
            public static RandomisationSetting padTextures = new RandomisationSetting(group: 0, enabled: true);
            public static RandomisationSetting particleTextures = new RandomisationSetting(group: 4, enabled: true);
            public static RandomisationSetting effectTextures = new RandomisationSetting();
            public static RandomisationSetting miscTextures = new RandomisationSetting();
            public static FontRandomisationSettings fontRand = new FontRandomisationSettings(
                  enabled: true,
                  shuffleFontStyles: true,
                  shufflingMode: FontStyleShufflingMode.PerFont,
                  randomiseLetters: false
            );

            public static float maxSpriteMultiplier = 1.10f;
            public static bool allowDuplicates = false;

            public static Quality quality = Quality.High;
            public static int seed = 0;

            public static int themeID = 0;

            public static void ApplySettings(Serialised_Config config) {
                  gameDirectory = config.gameDirectory;
                  autoOverwriteFiles = config.autoOverwriteFiles;

                  iconTextures = config.iconTextures;
                  menuTextures = config.menuTextures;
                  shopTextures = config.shopTextures;
                  editorTextures = config.editorTextures;
                  tileTextures = config.tileTextures;
                  portalTextures = config.portalTextures;
                  orbTextures = config.orbTextures;
                  padTextures = config.padTextures;
                  particleTextures = config.particleTextures;
                  effectTextures = config.effectTextures;
                  miscTextures = config.miscTextures;
                  fontRand = config.fontRand;

                  maxSpriteMultiplier = config.maxSpriteMultiplier;
                  allowDuplicates = config.allowDuplicates;

                  quality = config.quality;
                  seed = config.seed;

                  themeID = config.themeID;
            }

            public static void ReadFile() {
                  if (File.Exists(configFileName)) {
                        string[] inStream = File.ReadAllLines(configFileName);
                        Serialised_Config config = Serialised_Config.Deserialise(inStream);

                        if (config != null) {
                              Config.ApplySettings(config);
                        } else {
                              WriteFile();
                        }
                  } else {
                        // If the file doesn't exist, create it with default settings
                        WriteFile();
                  }
            }

            public static void WriteFile() {
                  Serialised_Config config = new Serialised_Config();

                  JsonSerializerOptions options = new JsonSerializerOptions();
                  options.WriteIndented = true;
                  string outStream = JsonSerializer.Serialize(config, options);

                  try {
                        File.WriteAllText(configFileName, outStream);
                  } catch (IOException) {

                  }
            }

            // Unused atm, the 0 settings enabled warning is temporarily disabled
            public static int GetEnabledSettingsCount() {
                  return Convert.ToInt32(iconTextures.enabled) +
                        Convert.ToInt32(menuTextures.enabled) +
                        Convert.ToInt32(shopTextures.enabled) +
                        Convert.ToInt32(editorTextures.enabled) +
                        Convert.ToInt32(tileTextures.enabled) +
                        Convert.ToInt32(portalTextures.enabled) +
                        Convert.ToInt32(orbTextures.enabled) +
                        Convert.ToInt32(padTextures.enabled) +
                        Convert.ToInt32(particleTextures.enabled) +
                        Convert.ToInt32(effectTextures.enabled) +
                        Convert.ToInt32(miscTextures.enabled) +
                        Convert.ToInt32(fontRand.enabled) +
                        Convert.ToInt32(fontRand.shuffleFontStyles) +
                        Convert.ToInt32(fontRand.randomiseLetters);
            }
      }

      public class Serialised_Config {

            // Move to different config later
            public string gameDirectory { get; set; } = string.Empty;
            // Move to different config later
            public bool autoOverwriteFiles { get; set; }

            public IconRandSettings iconTextures { get; set; }
            public RandomisationSetting menuTextures { get; set; }
            public RandomisationSetting shopTextures { get; set; }
            public RandomisationSetting editorTextures { get; set; }
            public RandomisationSetting tileTextures { get; set; }
            public RandomisationSetting portalTextures { get; set; }
            public RandomisationSetting orbTextures { get; set; }
            public RandomisationSetting padTextures { get; set; }
            public RandomisationSetting particleTextures { get; set; }
            public RandomisationSetting effectTextures { get; set; }
            public RandomisationSetting miscTextures { get; set; }
            public FontRandomisationSettings fontRand { get; set; }

            public float maxSpriteMultiplier { get; set; }
            public bool allowDuplicates { get; set; }

            // Move to different config later
            public Quality quality { get; set; }
            public int seed { get; set; }

            // Move to different config later
            public int themeID { get; set; }

            public Serialised_Config() {
                  this.gameDirectory = Config.gameDirectory;
                  this.autoOverwriteFiles = Config.autoOverwriteFiles;

                  this.iconTextures = Config.iconTextures;
                  this.menuTextures = Config.menuTextures;
                  this.shopTextures = Config.shopTextures;
                  this.editorTextures = Config.editorTextures;
                  this.tileTextures = Config.tileTextures;
                  this.portalTextures = Config.portalTextures;
                  this.orbTextures = Config.orbTextures;
                  this.padTextures = Config.padTextures;
                  this.particleTextures = Config.particleTextures;
                  this.effectTextures = Config.effectTextures;
                  this.miscTextures = Config.miscTextures;
                  this.fontRand = Config.fontRand;

                  this.maxSpriteMultiplier = Config.maxSpriteMultiplier;
                  this.allowDuplicates = Config.allowDuplicates;

                  this.quality = Config.quality;
                  this.seed = Config.seed;

                  this.themeID = Config.themeID;
            }

            public static Serialised_Config Deserialise(string[] data) {
                  Serialised_Config ret;
                  try {
                        ret = JsonSerializer.Deserialize<Serialised_Config>(string.Join("\n", data));

                  } catch (JsonException) {
                        // If there was an error while deserialising the data, return null
                        ret = null;
                  }
                  return ret;
            }
      }
}
