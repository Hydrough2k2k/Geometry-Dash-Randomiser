using System;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text.Json;

namespace Geometry_Dash_Randomiser {

      internal static class Config {

            static string configFileName = "config.txt";

            // This is where the default values are defined
            public static string gameDirectory = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Geometry Dash";
            //public static bool ignoreBlacklistedFiles = true; // Not functional, but will work. Also add a warning for "this will cause extra chaos"
            
            // Configs for every randomisation type. More might be added later
            public static RandomisationGroup iconTextures = new RandomisationGroup(1, true);
            public static RandomisationGroup menuTextures = new RandomisationGroup(2, true);
            public static RandomisationGroup shopTextures = new RandomisationGroup(0, false);
            public static RandomisationGroup editorTextures = new RandomisationGroup(0, false);
            public static RandomisationGroup tileTextures = new RandomisationGroup(3, true);
            public static RandomisationGroup portalTextures = new RandomisationGroup(0, false);
            public static RandomisationGroup orbTextures = new RandomisationGroup(0, false);
            public static RandomisationGroup particleTextures = new RandomisationGroup(4, true);
            public static RandomisationGroup effectTextures = new RandomisationGroup(0, false);
            public static RandomisationGroup miscTextures = new RandomisationGroup(0, false);

            public static GameFiles.Quality quality = GameFiles.Quality.High;
            public static int seed = 0;

            public static bool caching = true;

            public static void ApplySettings(Serialised_Config config) {
                  gameDirectory = config.gameDirectory;
                  //Config.ignoreBlacklistedFiles = config.ignoreBlacklistedFiles;

                  iconTextures = config.iconTextures;
                  menuTextures = config.menuTextures;
                  shopTextures = config.shopTextures;
                  editorTextures = config.editorTextures;
                  tileTextures = config.tileTextures;
                  portalTextures = config.portalTextures;
                  orbTextures = config.orbTextures;
                  particleTextures = config.particleTextures;
                  effectTextures = config.effectTextures;
                  miscTextures = config.miscTextures;

                  quality = config.quality;
                  seed = config.seed;
                  caching = config.caching;
            }

            public static void ReadFile() {
                  if (File.Exists(configFileName)) {
                        string inStream = File.ReadAllText(configFileName);
                        Serialised_Config config = JsonSerializer.Deserialize<Serialised_Config>(inStream);
                        Config.ApplySettings(config);

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
                  File.WriteAllText(configFileName, outStream);
            }

            public static int GetEnabledSettingsCount() {
                  return Convert.ToInt32(iconTextures.enabled) +
                        Convert.ToInt32(menuTextures.enabled) +
                        Convert.ToInt32(shopTextures.enabled) +
                        Convert.ToInt32(editorTextures.enabled) +
                        Convert.ToInt32(tileTextures.enabled) +
                        Convert.ToInt32(portalTextures.enabled) +
                        Convert.ToInt32(orbTextures.enabled) +
                        Convert.ToInt32(particleTextures.enabled) +
                        Convert.ToInt32(effectTextures.enabled) +
                        Convert.ToInt32(miscTextures.enabled);
            }
      }

      internal class Serialised_Config {

            public string gameDirectory { get; set; }
            //public bool ignoreBlacklistedFiles { get; set; }

            public RandomisationGroup iconTextures { get; set; }
            public RandomisationGroup menuTextures { get; set; }
            public RandomisationGroup shopTextures { get; set; }
            public RandomisationGroup editorTextures { get; set; }
            public RandomisationGroup tileTextures { get; set; }
            public RandomisationGroup portalTextures { get; set; }
            public RandomisationGroup orbTextures { get; set; }
            public RandomisationGroup particleTextures { get; set; }
            public RandomisationGroup effectTextures { get; set; }
            public RandomisationGroup miscTextures { get; set; }

            public GameFiles.Quality quality { get; set; }
            public int seed { get; set; }

            public bool caching { get; set; }

            public Serialised_Config() {
                  this.gameDirectory = Config.gameDirectory;
                  //this.ignoreBlacklistedFiles = Config.ignoreBlacklistedFiles;

                  this.iconTextures = Config.iconTextures;
                  this.menuTextures = Config.menuTextures;
                  this.shopTextures = Config.shopTextures;
                  this.editorTextures = Config.editorTextures;
                  this.tileTextures = Config.tileTextures;
                  this.portalTextures = Config.portalTextures;
                  this.orbTextures = Config.orbTextures;
                  this.particleTextures = Config.particleTextures;
                  this.effectTextures = Config.effectTextures;
                  this.miscTextures = Config.miscTextures;

                  this.quality = Config.quality;
                  this.seed = Config.seed;
                  this.caching = Config.caching;
            }
      }
}
