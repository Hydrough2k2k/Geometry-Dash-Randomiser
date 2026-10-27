using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Geometry_Dash_Randomiser {

      /// <summary>
      /// Singleton class for storing this application's more advanced and less user friendly configurations
      /// </summary>
      public class AdvancedConfig {

            private static AdvancedConfig _instance = new AdvancedConfig();

            // This has to be public for JSON deserialization sadly
            [EditorBrowsable(EditorBrowsableState.Never)]
            public AdvancedConfig() {
                  _created = DateTime.Now;
            }

            public static AdvancedConfig Instance {
                  get {
                        if (_instance == null)
                              _instance = new AdvancedConfig();
                        return _instance;
                  }
            }

            public string GameDirectory {
                  get => _gameDirectory;
                  set {
                        _gameDirectory = value ?? string.Empty;
                        SaveConfigAfterDelay();
                  }
            }

            public bool AutoOverwriteFiles {
                  get => _autoOverwriteFiles;
                  set {
                        _autoOverwriteFiles = value;
                        SaveConfigAfterDelay();
                  }
            }

            public Quality Quality {
                  get => _quality;
                  set {
                        _quality = value;
                        SaveConfigAfterDelay();
                  }
            }

            public int ThemeID {
                  get => _themeID;
                  set {
                        if (value < 0)
                              _themeID = 0; 

                        _themeID = value;
                        SaveConfigAfterDelay();
                  }
            }

            public bool EnableRandomTheme {
                  get => _enableRandomTheme;
                  set {
                        _enableRandomTheme = value;
                        SaveConfigAfterDelay();
                  }
            }

            public bool EnableSystemTheme {
                  get => _enableSystemTheme;
                  set {
                        _enableSystemTheme = value;
                        SaveConfigAfterDelay();
                  }
            }

            public int ConfigSaveDelay {
                  get => _configSaveDelay;
                  set {
                        _configSaveDelay = value;
                        SaveConfigAfterDelay();
                  }
            }

            public int MaxLogFileCount {
                  get => _maxLogFileCount;
                  set {
                        _maxLogFileCount = value;
                        SaveConfigAfterDelay();
                  }
            }

            public bool DebugMode {
                  get => _debugMode;
                  set {
                        _debugMode = value;
                        SaveConfigAfterDelay();
                  }
            }

            public static void ReadFile() {
                  if (File.Exists(configFileName)) {
                        string inStream = File.ReadAllText(configFileName);
                        _instance.ApplyAllValues(Deserialise(inStream));

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
                  Task.Delay(new TimeSpan(0, 0, ConfigSaveDelay)).ContinueWith(o => { WriteFile(); });
            }

            public void WriteFile() {
                  Logger.Write(Logger.Mode.Verbose, "Saving Advanced Config File");

                  string outStream = this.Serialize();

                  try {
                        File.WriteAllText(configFileName, outStream);

                  }
                  catch (IOException ioExcept) {
                        Logger.Write(Logger.Mode.Error, $"Failed to write config file. Reason: {ioExcept}");
                  }

                  _fileSaveQueued = false;
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

            public void ApplyAllValues(AdvancedConfig config) {
                  GameDirectory = config.GameDirectory;
                  AutoOverwriteFiles = config.AutoOverwriteFiles;
                  Quality = config.Quality;
                  ThemeID = config.ThemeID;
                  EnableRandomTheme = config.EnableRandomTheme;
                  EnableSystemTheme = config.EnableSystemTheme;
                  ConfigSaveDelay = config.ConfigSaveDelay;
                  MaxLogFileCount = config.MaxLogFileCount;
                  DebugMode = config.DebugMode;
            }

            private string _gameDirectory = string.Empty;
            private bool _autoOverwriteFiles = true;

            private Quality _quality = Quality.High;

            private int _themeID = 0;
            private bool _enableRandomTheme = true;
            private bool _enableSystemTheme = false;

            private int _configSaveDelay = 5;

            // More Log related settings for minimum log value for file and console writing.
            private int _maxLogFileCount = 20;

            private bool _debugMode = false;

            private bool _fileSaveQueued = false;

            // This exists to make it so upon creation it doesn't immediately try to save the config file
            private DateTime _created;

            const string configFileName = "adv_config.ini";
      }
}
