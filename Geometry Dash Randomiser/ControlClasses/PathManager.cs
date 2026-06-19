using System.IO;

namespace Geometry_Dash_Randomiser {

      public static class PathManager {

            public static string GetQualityFolderName(Quality quality) {
                  switch (quality) {
                        case Quality.Low:
                              return lowQualityName;

                        case Quality.Medium:
                              return mediumQualityName;

                        case Quality.High:
                              return highQualityName;

                        default:
                              Log.Write(Log.Mode.Fatal, $"The quality \"{quality}\" does not exist");
                              throw new System.Exception("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                  }
            }

            public static string GetPath(GDR_Path folder) {
                  switch (folder) {
                        case GDR_Path.GameResourcesFolder:
                              return GameResourcesFolder;

                        case GDR_Path.GameIconsFolder:
                              return GameIconsFolder;

                        case GDR_Path.BackupResourcesFolder:
                              return BackupResourcesFolder;

                        case GDR_Path.BackupIconsFolder:
                              return BackupIconsFolder;

                        case GDR_Path.LocalResourcesOutputFolder:
                              return LocalResourcesOutputFolder;

                        case GDR_Path.LocalIconsOutputFolder:
                              return LocalIconsOutputFolder;

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

            /// <summary>
            /// Resources folder where the game is installed
            /// </summary>
            public static string GameResourcesFolder =>
                  Path.Combine(Config.Instance.gameDirectory, resourcesFolderName);

            /// <summary>
            /// Icons folder where the game is installed
            /// </summary>
            public static string GameIconsFolder =>
                  Path.Combine(Config.Instance.gameDirectory, resourcesFolderName, iconsFolderName);

            /// <summary>
            /// Resources folder in the application's folder. This stores the unaltered files
            /// </summary>
            public static string BackupResourcesFolder =>
                  Path.Combine(unalteredFiles, GetQualityFolderName(Config.Instance.quality), resourcesFolderName);

            /// <summary>
            /// Icons folder in the application's folder. This stores the unaltered files
            /// </summary>
            public static string BackupIconsFolder =>
                  Path.Combine(unalteredFiles, GetQualityFolderName(Config.Instance.quality), resourcesFolderName, iconsFolderName);

            /// <summary>
            /// This is where the randomised Resources files will go by default
            /// </summary>
            public static string LocalResourcesOutputFolder =>
                  Path.Combine(randomisedFiles, GetQualityFolderName(Config.Instance.quality), resourcesFolderName);

            /// <summary>
            /// This is where the randomised Icons files will go by default
            /// </summary>
            public static string LocalIconsOutputFolder =>
                  Path.Combine(randomisedFiles, GetQualityFolderName(Config.Instance.quality), resourcesFolderName, iconsFolderName);
      }
}
