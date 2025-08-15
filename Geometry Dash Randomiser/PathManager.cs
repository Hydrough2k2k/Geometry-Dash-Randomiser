using System.IO;
using static Geometry_Dash_Randomiser.GameFileManager;

namespace Geometry_Dash_Randomiser {
      public class PathManager {

            readonly GameFileManager gameFileManager;

            public PathManager(GameFileManager creator) {
                  gameFileManager = creator;
            }

            public enum GDR_Path {
                  /// <summary> Resources folder where the game is installed </summary>
                  GameResourcesFolder,

                  /// <summary> Icons folder where the game is installed </summary>
                  GameIconsFolder,

                  /// <summary> Resources folder in the application's folder. This stores the unaltered files </summary>
                  BackupResourcesFolder,

                  /// <summary> Icons folder in the application's folder. This stores the unaltered files </summary>
                  BackupIconsFolder,

                  /// <summary> This is where the randomised Resources files will go by default </summary>
                  LocalResourcesOutputFolder,

                  /// <summary> This is where the randomised Icons files will go by default </summary>
                  LocalIconsOutputFolder
            }

            public void SetQuality(Quality quality) {
                  switch (quality) {
                        case Quality.Low:
                              currentQualityFolder = lowQualityName;
                              break;
                        case Quality.Medium:
                              currentQualityFolder = mediumQualityName;
                              break;
                        case Quality.High:
                              currentQualityFolder = highQualityName;
                              break;
                        default:
                              break;
                  }
            }

            public string GetPath(GDR_Path folder) {
                  switch (folder) {
                        case GDR_Path.GameResourcesFolder:
                              return gameResourcesFolder;

                        case GDR_Path.GameIconsFolder:
                              return gameIconsFolder;

                        case GDR_Path.BackupResourcesFolder:
                              return backupResourcesFolder;

                        case GDR_Path.BackupIconsFolder:
                              return backupIconsFolder;

                        case GDR_Path.LocalResourcesOutputFolder:
                              return localResourcesOutputFolder;

                        case GDR_Path.LocalIconsOutputFolder:
                              return localIconsOutputFolder;

                        default:
                              return string.Empty;
                  }
            }

            public string currentQualityFolder;

            const string resourcesFolderName = "Resources";
            const string iconsFolderName = "icons";

            const string randomisedFiles = "Randomised Files";
            const string unalteredFiles = "Unaltered Files";

            public const string lowQualityName = "Low Quality";
            public const string mediumQualityName = "Medium Quality";
            public const string highQualityName = "High Quality";

            // -------------------------------------------------------------------------------------

            /// <summary> Resources folder where the game is installed </summary>
            public string gameResourcesFolder => Path.Combine(Config.gameDirectory, resourcesFolderName);

            /// <summary> Icons folder where the game is installed </summary>
            public string gameIconsFolder => Path.Combine(Config.gameDirectory, resourcesFolderName, iconsFolderName);

            // -------------------------------------------------------------------------------------

            /// <summary> Resources folder in the application's folder. This stores the unaltered files </summary>
            public string backupResourcesFolder => Path.Combine(unalteredFiles, currentQualityFolder, resourcesFolderName);

            /// <summary> Icons folder in the application's folder. This stores the unaltered files </summary>
            public string backupIconsFolder => Path.Combine(unalteredFiles, currentQualityFolder, resourcesFolderName, iconsFolderName);

            /// <summary> This is where the randomised Resources files will go by default </summary>
            public string localResourcesOutputFolder => Path.Combine(randomisedFiles, currentQualityFolder, resourcesFolderName);

            /// <summary> This is where the randomised Icons files will go by default </summary>
            public string localIconsOutputFolder => Path.Combine(randomisedFiles, currentQualityFolder, resourcesFolderName, iconsFolderName);
      }
}
