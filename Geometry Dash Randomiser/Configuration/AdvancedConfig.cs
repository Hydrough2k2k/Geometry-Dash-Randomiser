using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
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
                        _configSaveDelay = Math.Clamp(value, 1, 60);
                        SaveConfigAfterDelay();
                  }
            }

            public int MaxLogFileCount {
                  get => _maxLogFileCount;
                  set {
                        _maxLogFileCount = Math.Clamp(value, 1, 255);
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

            public int TextureCropSize {
                  get => _textureCropSize;
                  set {
                        if (value < 1) {
                              _textureCropSize = 256;
                        } else {
                              _textureCropSize = value;
                        }
                  }
            }

            //public bool IncludeExplanations {
            //      get => _includeExplanations;
            //      set => _includeExplanations = value;
            //}

            public static void ReadFile() {
                  if (File.Exists(_configFileName)) {
                        string inStream = File.ReadAllText(_configFileName);
                        _instance.ApplyAllValues(Deserialise(inStream));

                  } else {
                        Logger.WriteToFile(Logger.Mode.Info, $"The file \"{_configFileName}\" does not exist. Loading default configuration.");
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
                  Logger.Write(Logger.Mode.Debug, "Saving Advanced Config File");

                  string outStream = this.Serialize();

                  // Very unfinished
                  //if (_includeExplanations) {
                  //      outStream = string.Join("\n", AddExplanationsToJSON_Data(outStream.Split('\n').Where(s => s != string.Empty).ToList()));
                  //}

                  try {
                        File.WriteAllText(_configFileName, outStream);

                  }
                  catch (IOException ioExcept) {
                        Logger.Write(Logger.Mode.Error, $"Failed to write config file. Reason: {ioExcept}");
                  }

                  _fileSaveQueued = false;
            }

            // Very unfinished
            //private string[] AddExplanationsToJSON_Data(List<string> dataIn) {
            //      List<string> ret = new List<string>();
            //      ret.Add("{");

            //      JSON_Parser jp = new JSON_Parser(dataIn);

            //      for (int i = 0; i < _explanationStrings.Length; i++) {
            //            jp.GetProperty(_explanationStrings[i].PropertyName, true, out string[] result);

            //      }



            //      return ret;
            //}

            private string Serialize() {
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
                  TextureCropSize = config.TextureCropSize;
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

            private int _textureCropSize = 256;

            //private bool _includeExplanations = true;

            // ------------------------------------------------

            private bool _fileSaveQueued = false;

            // This exists to make it so upon creation it doesn't immediately try to save the config file
            private DateTime _created;

            private const string _configFileName = "adv_config.ini";

            private static ExplanationPair[] _explanationStrings = new ExplanationPair[] {
                  new ExplanationPair(
                        nameof(GameDirectory),
                        "This is where the game's executable GeometryDash.exe and it's folders, \"Resources\" and \"Resources\\icons\" are"),
                  new ExplanationPair(
                        nameof(AutoOverwriteFiles),
                        "This will make the application copy the randomized files into the game's directory immediately"),
                  new ExplanationPair(
                        nameof(Quality),
                        "The quality of texture file to randomize"),
                  new ExplanationPair(
                        nameof(ThemeID),
                        "The ID of the theme to use for the application"),
                  new ExplanationPair(
                        nameof(EnableRandomTheme),
                        "Whether \"Random Theme\" is enabled. It colours every UI elements randomly"),
                  new ExplanationPair(
                        nameof(EnableSystemTheme),
                        "Whether \"System Theme\" is enabled. This one is work in progress"),
                  new ExplanationPair(
                        nameof(ConfigSaveDelay),
                        "The delay for saving config files after any setting in the application has been changed"),
                  new ExplanationPair(
                        nameof(MaxLogFileCount),
                        "How many log files will be stored in the \"Logs\" folder. 1 to 255 inclusive"),
                  new ExplanationPair(
                        nameof(DebugMode),
                        "If enabled, there will be a console window to show if there are any warnings or errors"),
                  new ExplanationPair(
                        nameof(TextureCropSize),
                        "Controls image cropping. Change this value, if the unpacking of texture files is too slow")
            };

            private class ExplanationPair {

                  public ExplanationPair(string propertyName, string explanation) {
                        PropertyName = propertyName;
                        Explanation = explanation;
                  }

                  internal ExplanationPair() { }

                  internal string PropertyName { get; set; }
                  internal string Explanation { get; set; }
            }
      }
}
