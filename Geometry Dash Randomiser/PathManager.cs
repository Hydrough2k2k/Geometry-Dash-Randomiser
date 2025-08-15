using System.IO;
using static Geometry_Dash_Randomiser.GameFileManager;

namespace Geometry_Dash_Randomiser {

      public static class PathManager {

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

            public static string GetQualityFolderName(Quality quality) {
                  switch (quality) {
                        case Quality.Low:
                              return lowQualityName;
                        case Quality.Medium:
                              return mediumQualityName;
                        case Quality.High:
                              return highQualityName;
                        default:
                              return string.Empty;
                  }
            }

            public static string GetPath(GDR_Path folder) {
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

            const string resourcesFolderName = "Resources";
            const string iconsFolderName = "icons";

            const string randomisedFiles = "Randomised Files";
            const string unalteredFiles = "Backup Files";

            public const string lowQualityName = "Low Quality";
            public const string mediumQualityName = "Medium Quality";
            public const string highQualityName = "High Quality";

            // -------------------------------------------------------------------------------------

            /// <summary> Resources folder where the game is installed </summary>
            public static string gameResourcesFolder => Path.Combine(Config.gameDirectory, resourcesFolderName);

            /// <summary> Icons folder where the game is installed </summary>
            public static string gameIconsFolder => Path.Combine(Config.gameDirectory, resourcesFolderName, iconsFolderName);

            // -------------------------------------------------------------------------------------

            /// <summary> Resources folder in the application's folder. This stores the unaltered files </summary>
            public static string backupResourcesFolder => Path.Combine(unalteredFiles, GetQualityFolderName(Config.quality), resourcesFolderName);

            /// <summary> Icons folder in the application's folder. This stores the unaltered files </summary>
            public static string backupIconsFolder => Path.Combine(unalteredFiles, GetQualityFolderName(Config.quality), resourcesFolderName, iconsFolderName);

            /// <summary> This is where the randomised Resources files will go by default </summary>
            public static string localResourcesOutputFolder => Path.Combine(randomisedFiles, GetQualityFolderName(Config.quality), resourcesFolderName);

            /// <summary> This is where the randomised Icons files will go by default </summary>
            public static string localIconsOutputFolder => Path.Combine(randomisedFiles, GetQualityFolderName(Config.quality), resourcesFolderName, iconsFolderName);
      }
}
