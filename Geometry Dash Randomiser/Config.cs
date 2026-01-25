using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using static Geometry_Dash_Randomiser.FontRandomisationSettings;

namespace Geometry_Dash_Randomiser {

      public class Config {

            private static Config _instance;

            [EditorBrowsable(EditorBrowsableState.Never)]
            public Config() { }

            public static Config Instance {
                  get {
                        if (_instance == null)
                              _instance = new Config();
                        return _instance;
                  }
            }

            public string gameDirectory { get; set; } = "";
            public bool autoOverwriteFiles { get; set; } = true;

            public IconRandSettings iconTextures { get; set; } = new IconRandSettings { group = 0, enabled = true };
            public RandomisationSetting menuTextures { get; set; } = new RandomisationSetting(group: 1, enabled: true);
            public RandomisationSetting shopTextures { get; set; } = new RandomisationSetting(group: 1, enabled: true);
            public RandomisationSetting editorTextures { get; set; } = new RandomisationSetting();
            public RandomisationSetting tileTextures { get; set; } = new RandomisationSetting();
            public RandomisationSetting portalTextures { get; set; } = new RandomisationSetting(group: 0, true);
            public RandomisationSetting orbTextures { get; set; } = new RandomisationSetting(group: 3, enabled: true);
            public RandomisationSetting padTextures { get; set; } = new RandomisationSetting(group: 0, enabled: true);
            public RandomisationSetting particleTextures { get; set; } = new RandomisationSetting(group: 3, enabled: true);
            public RandomisationSetting effectTextures { get; set; } = new RandomisationSetting(group: 2, enabled: true);
            public RandomisationSetting miscTextures { get; set; } = new RandomisationSetting();
            public FontRandomisationSettings fontRand { get; set; } = new FontRandomisationSettings(
                  enabled: true,
                  shuffleFontStyles: true,
                  shufflingMode: FontStyleShufflingMode.PerLetter,
                  randomiseLetters: false
            );

            public float maxSpriteMultiplier { get; set; } = 1.10f;
            public bool allowDuplicates { get; set; } = false;

            public Quality quality { get; set; } = Quality.High;
            public int seed { get; set; } = 0;

            public int themeID { get; set; } = 0;

            // Not in use at the moment, this is a future feature
            public int configurationID = 0;
            public bool debugMode { get; set; } = false;

            public int GetEnabledSettingsCount() {
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
                        Convert.ToInt32(fontRand.enabled);
            }

            public static void ReadFile() {
                  if (File.Exists(configFileName)) {
                        string[] inStream = File.ReadAllLines(configFileName);
                        _instance = Config.Deserialise(inStream);
                  }
            }

            public void WriteFile() {
                  Console.WriteLine("Saving Config File");

                  string outStream = this.Serialize();

                  try {
                        File.WriteAllText(configFileName, outStream);
                  } catch (IOException ioExcept) {
                        Console.WriteLine($"Failed to write config file. Reason: {ioExcept}");
                  }
            }

            string Serialize() {
                  JsonSerializerOptions options = new JsonSerializerOptions {
                        WriteIndented = true
                  };

                  return JsonSerializer.Serialize(_instance, options);
            }

            public static Config Deserialise(string[] data) {
                  Config ret;
                  try {
                        ret = JsonSerializer.Deserialize<Config>(string.Join("\n", data));

                  } catch (JsonException JSON_Except) {
                        Console.WriteLine($"Failed to convert the config file from JSON. Reason: {JSON_Except}");
                        ret = null;
                  }

                  if (ret.themeID < 0)
                        ret.themeID = 0;

                  ret.iconTextures.Validate();

                  ret.menuTextures.Validate();
                  ret.shopTextures.Validate();
                  ret.editorTextures.Validate();
                  ret.tileTextures.Validate();
                  ret.portalTextures.Validate();
                  ret.orbTextures.Validate();
                  ret.padTextures.Validate();
                  ret.particleTextures.Validate();
                  ret.effectTextures.Validate();
                  ret.miscTextures.Validate();
                  
                  if (ret.maxSpriteMultiplier < 1.01) {
                        ret.maxSpriteMultiplier = 1.10f;
                  }

                  return ret;
            }

            public string GetExportConfigData() {
                  List<string> dataPoints = new List<string>();

                  dataPoints.Add(iconTextures.GetStatusHex());
                  dataPoints.Add(menuTextures.GetStatusHex());
                  dataPoints.Add(shopTextures.GetStatusHex());
                  dataPoints.Add(editorTextures.GetStatusHex());
                  dataPoints.Add(tileTextures.GetStatusHex());
                  dataPoints.Add(portalTextures.GetStatusHex());
                  dataPoints.Add(orbTextures.GetStatusHex());
                  dataPoints.Add(padTextures.GetStatusHex());
                  dataPoints.Add(particleTextures.GetStatusHex());
                  dataPoints.Add(effectTextures.GetStatusHex());
                  dataPoints.Add(miscTextures.GetStatusHex());
                  dataPoints.Add(fontRand.GetStatusHex());
                  dataPoints.Add(((int)(maxSpriteMultiplier * 100)).ToString("X"));
                  dataPoints.Add(Convert.ToInt32(allowDuplicates).ToString());
                  dataPoints.Add(seed.ToString("X"));

                  return string.Join("#", dataPoints);
            }

            /// <returns>If the import was successful</returns>
            public bool ImportConfigData(string data) {
                  string[] dataPoints = data.Split('#');

                  if (dataPoints.Length <= 1) {
                        Console.WriteLine("The app received no data points to import");
                        return false;
                  }

                  Console.WriteLine($"Received {dataPoints.Length} datapoints from import string");

                  bool quit = false;

                  for (int i = 0; i < dataPoints.Length; i++) {
                        string dp = dataPoints[i];
                        Console.WriteLine(dp);

                        // If the data for a setting is empty, skip it
                        if (dp.Length == 0)
                              continue;

                        switch (i) {
                              case 0:
                                    this.iconTextures.ApplyConfigFromHex(dp);
                                    break;
                              case 1:
                                    this.menuTextures.ApplyConfigFromHex(dp);
                                    break;
                              case 2:
                                    this.shopTextures.ApplyConfigFromHex(dp);
                                    break;
                              case 3:
                                    this.editorTextures.ApplyConfigFromHex(dp);
                                    break;
                              case 4:
                                    this.tileTextures.ApplyConfigFromHex(dp);
                                    break;
                              case 5:
                                    this.portalTextures.ApplyConfigFromHex(dp);
                                    break;
                              case 6:
                                    this.orbTextures.ApplyConfigFromHex(dp);
                                    break;
                              case 7:
                                    this.padTextures.ApplyConfigFromHex(dp);
                                    break;
                              case 8:
                                    this.particleTextures.ApplyConfigFromHex(dp);
                                    break;
                              case 9:
                                    this.effectTextures.ApplyConfigFromHex(dp);
                                    break;
                              case 10:
                                    this.miscTextures.ApplyConfigFromHex(dp);
                                    break;
                              case 11:
                                    this.fontRand.ApplyConfigFromHex(dp);
                                    break;
                              case 12:
                                    Int32.TryParse(dp.Trim(), System.Globalization.NumberStyles.HexNumber, null, out int convertedMult);
                                    this.maxSpriteMultiplier = convertedMult / 100f;
                                    break;
                              case 13:
                                    Boolean.TryParse(dp.Trim(), out bool convertedAllowDupes);
                                    this.allowDuplicates = convertedAllowDupes;
                                    break;
                              case 14:
                                    Int32.TryParse(dp.Trim(), System.Globalization.NumberStyles.HexNumber, null, out int convertedSeed);
                                    this.seed = convertedSeed;
                                    break;
                              default:
                                    Console.WriteLine($"The app received too many data points. Received: {dataPoints.Length}, Max expected: {maxExpectedDataPoints}");
                                    quit = true;
                                    break;
                        }

                        if (quit)
                              break;
                  }

                  return true;
            }

            enum ExportIdentifiers {
                  IconTextures,
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
                  FontRandomisation,
                  MaxSpriteSizeMultiplier,
                  AllowDuplicates,
                  Seed
            }

            private const int maxExpectedDataPoints = 15;
            const string configFileName = "config.txt";
            public const int maxGroups = 100;
      }
}
