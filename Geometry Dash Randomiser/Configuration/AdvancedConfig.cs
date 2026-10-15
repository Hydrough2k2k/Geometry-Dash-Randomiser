using System.ComponentModel;
using System.IO;
using System.Text.Json;

namespace Geometry_Dash_Randomiser {

      /// <summary>
      /// Singleton class for storing this application's more advanced and less user friendly configurations
      /// </summary>
      public class AdvancedConfig {

            private readonly static AdvancedConfig _instance = new AdvancedConfig();

            private int _themeID = 0;

            // This has to be public for JSON deserialization sadly
            [EditorBrowsable(EditorBrowsableState.Never)]
            public AdvancedConfig() { }

            public static AdvancedConfig Instance => _instance;

            public string GameDirectory { get; set; } = "";
            public bool AutoOverwriteFiles { get; set; } = true;

            public Quality Quality { get; set; } = Quality.High;

            public int ThemeID {
                  get {
                        return _themeID;
                  }
                  set {
                        if (value >= 0) {
                              _themeID = value;
                        } else {
                              Logger.Write(Logger.Mode.Error, $"The value \"{value}\" was passed to the property \"ThemeID\" in AdvancedConfig.cs. This is not a valid value.");
                              _themeID = 0;
                        }
                  }
            }

            public bool EnableRandomTheme { get; set; } = true;
            public bool EnableSystemTheme { get; set; } = false;

            // More Log related settings for minimum log value for file and console writing.

            public int MaxLogFileCount { get; set; } = 20;

            // Planned features
            //public int TextureGroupScrollValueChange { get; set; } = 3;
            //public int LetterGroupScrollValueChange { get; set; } = 1;

            public bool DebugMode { get; set; } = false;

            public static void ReadFile() {
                  if (File.Exists(configFileName)) {
                        string inStream = File.ReadAllText(configFileName);
                        _instance.ApplyAllProperties(Deserialise(inStream));

                  } else {
                        Logger.WriteToFile(Logger.Mode.Info, $"The file \"{configFileName}\" does not exist. Loading default configuration.");
                  }
            }

            public void WriteFile() {
                  Logger.Write(Logger.Mode.Verbose, "Saving Config File");

                  string outStream = this.Serialize();

                  try {
                        File.WriteAllText(configFileName, outStream);

                  }
                  catch (IOException ioExcept) {
                        Logger.Write(Logger.Mode.Error, $"Failed to write config file. Reason: {ioExcept}");
                  }
            }

            string Serialize() {
                  JsonSerializerOptions options = new JsonSerializerOptions {
                        WriteIndented = true
                  };

                  return JsonSerializer.Serialize(_instance, options);
            }

            public static AdvancedConfig Deserialise(string data) {
                  AdvancedConfig ret;
                  try {
                        ret = JsonSerializer.Deserialize<AdvancedConfig>(data);

                  }
                  catch (JsonException JSON_Except) {
                        Logger.Write(Logger.Mode.Error, $"Failed to convert the config file from JSON. Reason: {JSON_Except}");
                        ret = _instance;
                  }

                  return ret;
            }

            private void ApplyAllProperties(AdvancedConfig other) {
                  if (other == null) {
                        Logger.Write(Logger.Mode.Error, "The parameter \"other\" was passed as null in AdvancedConfig.cs.");
                        return;
                  }

                  _instance.GameDirectory = other.GameDirectory;
                  _instance.AutoOverwriteFiles = other.AutoOverwriteFiles;
                  _instance.Quality = other.Quality;
                  _instance.ThemeID = other.ThemeID;
                  _instance.EnableRandomTheme = other.EnableRandomTheme;
                  _instance.EnableSystemTheme = other.EnableSystemTheme;
                  _instance.MaxLogFileCount = other.MaxLogFileCount;
                  _instance.DebugMode = other.DebugMode;
            }

            const string configFileName = "adv_config.ini";
      }
}
