using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using static Geometry_Dash_Randomiser.PathManager;

namespace Geometry_Dash_Randomiser {

      public class GamesheetManager {

            public GamesheetManager() { }

            public void ReadAllFiles(string path, Quality quality) {
                  if (_gameSheets.Count != 0) {
                        return;
                  }

                  ExtractResourceFiles(GameFileType.Resource, GDR_Path.BackupResourcesFolder);
                  ExtractResourceFiles(GameFileType.Icon, GDR_Path.BackupResourcesFolder);
            }

            public void ExtractResourceFiles(GameFileType fileType, GDR_Path path) {
                  List<string> fileNames = GetAllFileNames(path, AdvancedConfig.Instance.Quality).ToList();

                  Logger.Write(Logger.Mode.Info, $"Unpacking {fileNames.Count} {fileType.ToString().ToLower()} files from {PathManager.GetPath(GDR_Path.BackupResourcesFolder)}");

                  ProgressState.Instance.CurrentFileType = fileType;
                  ProgressState.Instance.NewFileBatch(fileNames.Count);

                  string backupPath = PathManager.BackupResourcesFolder;

                  for (int i = 0; i < fileNames.Count; i++) {
                        ProgressState.Instance.CurrentFile = fileNames[i];

                        string textFilePath = Path.Combine(backupPath, fileNames[i] + ".plist");
                        string gamesheetFilePath = Path.Combine(backupPath, fileNames[i] + ".png");

                        bool textFileFound = File.Exists(textFilePath);
                        bool gamesheetFileFound = File.Exists(gamesheetFilePath);

                        // If either file is missing
                        if (textFileFound == false || gamesheetFileFound == false) {
                              // Log the missing files
                              if (!textFileFound)
                                    Logger.Write(Logger.Mode.Warn, $"The file \"{textFilePath}\" could not be located in the local backup resources folder");
                              if (!gamesheetFileFound)
                                    Logger.Write(Logger.Mode.Warn, $"The file \"{gamesheetFilePath}\" could not be located in the local backup resources folder");

                              // Remove 1 from the total expected files list
                              ProgressState.Instance.TotalFiles -= 1;

                              // Finally remove the not found file from the list
                              fileNames.RemoveAt(i);
                              i--;

                              continue;
                        }

                        // Otherwise extract them
                        GameSheet newSheet = ExtractGameSheet(textFilePath, gamesheetFilePath, fileType);
                        if (newSheet != null) {
                              _gameSheets.Add(newSheet);
                        }
                        
                        ProgressState.Instance.CompletedFiles++;
                  }
            }

            private GameSheet ExtractGameSheet(string textFilePath, string gamesheetFilePath, GameFileType fileType) {
                  string[] fileData = File.ReadAllLines(textFilePath);
                  List<ResourceSprite> sprites = Plist.BulkDeserialise(fileData);

                  string gameSheetName = Path.GetFileName(textFilePath).RemoveExtension();
                  GameSheet gameSheet = new GameSheet(gameSheetName);
                  gameSheet.FileType = fileType;

                  Bitmap gamesheet = new Bitmap(gamesheetFilePath);
                  if (gamesheet == null) {
                        Logger.Error($"Failed to load gamesheet image from {gamesheetFilePath}");
                        return null;
                  }

                  Rectangle[] rects = sprites.Select(s => s.CroppingRect).ToArray();
                  Bitmap[] cropped = gamesheet.Multicrop(rects, AdvancedConfig.Instance.TextureCropSize);
                  gamesheet.Dispose();

                  for (int i = 0; i < sprites.Count; i++) {
                        sprites[i].Texture = cropped[i];
                  }

                  gameSheet.Sprites = sprites.ToArray();
                  return gameSheet;
            }

            public static IEnumerable<string> GetAllFileNames(GDR_Path source, Quality quality)
                  => GetAllFileNames(PathManager.GetPath(source), quality);

            public static IEnumerable<string> GetAllFileNames(string path, Quality quality) {
                  return Directory.GetFiles(path)
                        .Where(f => Path.GetExtension(f) == ".plist")
                        .FilterFilesByQuality(quality)
                        //.Where(f => File.Exists(f + ".png") && File.Exists(f + ".plist"))
                        .Select(f => Path.GetFileName(f));
            }

            private List<GameSheet> _gameSheets = new List<GameSheet>();
      }
}
