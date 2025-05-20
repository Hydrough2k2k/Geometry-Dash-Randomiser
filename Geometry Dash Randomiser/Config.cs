using System;
using System.IO;
using System.Text.Json;

namespace Geometry_Dash_Randomiser {

      internal static class Config {

            public enum OutputFolder { Unknown, Default, Overwritten, Invalid, Creatable };

            static readonly string configFileName = "config.txt";

            // This is where the default values are defined
            public static string gameDirectory = "";
            public static string outputDirectory = "";
            public static bool fileAutoOverwrite = false;
            
            // Configs for every randomisation type
            public static IconRandSettings iconTextures = new IconRandSettings();
            public static RandSetting menuTextures = new RandSetting(2, true);
            public static RandSetting shopTextures = new RandSetting(0, false);
            public static RandSetting editorTextures = new RandSetting(0, false);
            public static RandSetting tileTextures = new RandSetting(3, true);
            public static RandSetting portalTextures = new RandSetting(0, false);
            public static RandSetting orbTextures = new RandSetting(4, true);
            public static RandSetting padTextures = new RandSetting(5, true);
            public static RandSetting particleTextures = new RandSetting(4, true);
            public static RandSetting effectTextures = new RandSetting(0, false);
            public static RandSetting miscTextures = new RandSetting(0, false);

            public static bool ignoreBlacklistedFiles = true; // Not functional. Add a warning for "this will cause extra chaos"
            public static GameFiles.Quality quality = GameFiles.Quality.High;
            public static int seed = 0;

            public static bool caching = true;

            public static void ApplySettings(Serialised_Config config) {
                  gameDirectory = config.gameDirectory;
                  outputDirectory = config.outputDirectory;
                  fileAutoOverwrite = config.fileAutoOverwrite;

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

                  Config.ignoreBlacklistedFiles = config.ignoreBlacklistedFiles;
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
                        Convert.ToInt32(padTextures.enabled) +
                        Convert.ToInt32(particleTextures.enabled) +
                        Convert.ToInt32(effectTextures.enabled) +
                        Convert.ToInt32(miscTextures.enabled);
            }

            public static OutputFolder GetOutputDirectoryStatus() {
                  if (outputDirectory == string.Empty)
                        return OutputFolder.Default;

                  if (Directory.Exists(outputDirectory) == false) {

                        int index = outputDirectory.IndexOf("\\");
                        string dir = outputDirectory.Substring(0, index);
                        if (Directory.Exists(dir) == true) {
                              // Folder entered by user does not exist, but the folder can be created
                              return OutputFolder.Creatable;
                        }

                        return OutputFolder.Invalid;

                  } else {
                        // Folder does exist and was entered by user
                        return OutputFolder.Overwritten;
                  }
            }
      }

      internal class Serialised_Config {

            public string gameDirectory { get; set; }
            public string outputDirectory { get; set; }
            public bool fileAutoOverwrite { get; set; }


            public IconRandSettings iconTextures { get; set; }
            public RandSetting menuTextures { get; set; }
            public RandSetting shopTextures { get; set; }
            public RandSetting editorTextures { get; set; }
            public RandSetting tileTextures { get; set; }
            public RandSetting portalTextures { get; set; }
            public RandSetting orbTextures { get; set; }
            public RandSetting padTextures { get; set; }
            public RandSetting particleTextures { get; set; }
            public RandSetting effectTextures { get; set; }
            public RandSetting miscTextures { get; set; }

            public bool ignoreBlacklistedFiles { get; set; }
            public GameFiles.Quality quality { get; set; }
            public int seed { get; set; }

            public bool caching { get; set; }

            public Serialised_Config() {
                  this.gameDirectory = Config.gameDirectory;
                  this.outputDirectory = Config.outputDirectory;
                  this.fileAutoOverwrite = Config.fileAutoOverwrite;

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

                  this.ignoreBlacklistedFiles = Config.ignoreBlacklistedFiles;
                  this.quality = Config.quality;
                  this.seed = Config.seed;
                  this.caching = Config.caching;
            }
      }
}
