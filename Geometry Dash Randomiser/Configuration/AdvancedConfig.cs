using System.ComponentModel;
using System.IO;
using System.Text.Json;

namespace Geometry_Dash_Randomiser {

      /// <summary>
      /// Singleton class for storing this application's more advanced and less user friendly configurations
      /// </summary>
      public class AdvancedConfig {

            private static AdvancedConfig _instance;

            // This has to be public for JSON deserialization sadly
            [EditorBrowsable(EditorBrowsableState.Never)]
            public AdvancedConfig() { }

            public static AdvancedConfig Instance {
                  get {
                        if (_instance == null)
                              _instance = new AdvancedConfig();
                        return _instance;
                  }
            }

            public string GameDirectory { get; set; } = "";
            public bool AutoOverwriteFiles { get; set; } = true;

            public Quality Quality { get; set; } = Quality.High;

            public int ThemeID { get; set; } = 0;
            public bool EnableRandomTheme { get; set; } = true;
            public bool EnableSystemTheme { get; set; } = false;

            // More Log related settings for minimum log value for file and console writing.

            public int MaxLogFileCount { get; set; } = 20;

            public bool DebugMode { get; set; } = false;

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

                  }
                  catch (IOException ioExcept) {
                        Log.Write(Log.Mode.Error, $"Failed to write config file. Reason: {ioExcept}");
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
                        Log.Write(Log.Mode.Error, $"Failed to convert the config file from JSON. Reason: {JSON_Except}");
                        ret = _instance;
                  }

                  if (ret.ThemeID < 0) {
                        ret.ThemeID = 0;
                  }

                  return ret;
            }

            const string configFileName = "adv_config.ini";
      }
}
