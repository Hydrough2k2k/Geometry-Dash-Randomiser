using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using static Geometry_Dash_Randomiser.PathManager;

namespace Geometry_Dash_Randomiser {

      public class GamesheetManager {

            public GamesheetManager() { }

            public void ReadAllFiles(string path, Quality quality) {
                  if (_gameSheets.Count != 0) {
                        return;
                  }

                  ExtractResourceFiles(GameFileType.Resource, GDR_Path.BackupResourcesFolder);
                  ExtractResourceFiles(GameFileType.Icon, GDR_Path.BackupIconsFolder);
            }

            public void ExtractResourceFiles(GameFileType fileType, GDR_Path path) {
                  List<string> fileNames = GetAllFileNames(path, AdvancedConfig.Instance.Quality).ToList();

                  Logger.Write(Logger.Mode.Info, $"Unpacking {fileNames.Count} {fileType.ToString().ToLower()} files from {PathManager.GetPath(GDR_Path.BackupResourcesFolder)}");

                  ProgressState.Instance.CurrentFileType = fileType;
                  ProgressState.Instance.NewFileBatch(fileNames.Count);

                  string backupPath = PathManager.GetPath(path);

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
                  string gameSheetName = Path.GetFileName(textFilePath).RemoveExtension();
                  Bitmap gameSheetTexture = new Bitmap(gamesheetFilePath);
                  return GameSheet.Deserialize(gameSheetName, fileType, fileData, gameSheetTexture);
            }

            public void WriteFilesToDisk() => WriteFilesToDisk(_gameSheets.ToArray());

            public static void WriteFilesToDisk(GameSheet[] gameSheets) {
                  WriteFilesToDisk(gameSheets.Where(g => g.FileType == GameFileType.Resource).ToArray(), PathManager.LocalResourcesOutputFolder, GameFileType.Resource);
                  WriteFilesToDisk(gameSheets.Where(g => g.FileType == GameFileType.Icon).ToArray(), PathManager.LocalIconsOutputFolder, GameFileType.Icon);
            }

            private static void WriteFilesToDisk(GameSheet[] gameSheets, string path, GameFileType type) {
                  Directory.CreateDirectory(path);

                  ProgressState.Instance.NewFileBatch(gameSheets.Length);
                  ProgressState.Instance.CurrentFileType = type;

                  for (int i = 0; i < gameSheets.Length; i++) {
                        string gameSheetName = gameSheets[i].FileName;

                        ProgressState.Instance.CurrentFile = gameSheetName;

                        string textFileName = Path.Combine(path, gameSheetName + ".plist");
                        string gamesheetFileName = Path.Combine(path, gameSheetName + ".png");

                        File.WriteAllLines(textFileName, gameSheets[i].Serialize());

                        Bitmap gameSheetImage = gameSheets[i].AssembleGameSheet();

                        //for (int y = 0; y < gameSheetImage.Height; y++) {
                        //      for (int x = 0; x < gameSheetImage.Width; x++) {
                        //            Color ogColor = gameSheets[i].Resource.GetPixel(x, y);
                        //            Color newColor = gameSheetImage.GetPixel(x, y);

                        //            if (ogColor.A != newColor.A || ogColor.R != newColor.R || ogColor.G != newColor.G || ogColor.B != newColor.B) {
                        //                  gameSheetImage.SetPixel(x, y, Color.Red);
                        //            }
                        //      }
                        //}

                        gameSheetImage.Save(gamesheetFileName);

                        ProgressState.Instance.CompletedFiles++;
                  }
            }

            public static IEnumerable<string> GetAllFileNames(GDR_Path source, Quality quality)
                  => GetAllFileNames(PathManager.GetPath(source), quality);

            public static IEnumerable<string> GetAllFileNames(string path, Quality quality) {
                  return Directory.GetFiles(path)
                        .Where(f => Path.GetExtension(f) == ".plist")
                        .FilterFilesByQuality(quality)
                        .Select(f => Path.GetFileName(f));
            }

            private List<GameSheet> _gameSheets = new List<GameSheet>();
      }
}
