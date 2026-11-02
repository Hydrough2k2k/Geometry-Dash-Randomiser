using RectpackSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using static Geometry_Dash_Randomiser.PathManager;

namespace Geometry_Dash_Randomiser {

      public class GameFileManager {

            public GameFileManager(GDR_Form creator) {
                  GDR = creator;

                  _fileBlacklist = new FileBlacklist();
                  _gamesheetManager = new GamesheetManager();
                  _fontManager = new FontManager();
            }

            private readonly GDR_Form GDR;

            private readonly FileBlacklist _fileBlacklist;

            private readonly GamesheetManager _gamesheetManager;
            private readonly FontManager _fontManager;

            private readonly ProgressState _progressState = ProgressState.Instance;

            // -----------------------------------------------------------------------------------------------

            public ReadyState getReadyState() {
                  ReadyState rs = ReadyState.Ready;

                  if (Directory.Exists(AdvancedConfig.Instance.GameDirectory) == false) {
                        rs |= ReadyState.FolderNotFound;
                  }
                  if (Directory.Exists(PathManager.GameResourcesFolder) == false) {
                        rs |= ReadyState.ResourceFolderNotFound;
                  }
                  if (Directory.Exists(PathManager.GameIconsFolder) == false) {
                        rs |= ReadyState.IconFolderNotFound;
                  }
                  if (File.Exists(Path.Combine(AdvancedConfig.Instance.GameDirectory, "GeometryDash.exe")) == false) {
                        rs |= ReadyState.ExeNotFound;
                  }
                  if (RandomizationConfig.Instance.GetEnabledSettingsCount() == 0) {
                        rs |= ReadyState.NoSettingsEnabled;
                  }

                  return rs;
            }

            public bool IsGameDirectoryValid() {
                  ReadyState gameDirectoryState = getReadyState();
                  if (gameDirectoryState.HasFlag(ReadyState.FolderNotFound) == true) {
                        return false;
                  }
                  return true;
            }

            public void RestoreFiles() {
                  if (IsGameDirectoryValid() == false) {
                        return;
                  }
                  this._progressState.CurrentStage = ApplicationState.Restoring;

                  string[] resourceFiles = Directory.GetFiles(GetPath(GDR_Path.BackupResourcesFolder)).Select(f => Path.GetFileName(f)).ToArray();
                  this._progressState.CurrentFileType = GameFileType.Resource;
                  this._progressState.NewFileBatch(resourceFiles.Length);
                  CopyFiles(GDR_Path.BackupResourcesFolder, GDR_Path.GameResourcesFolder, resourceFiles);

                  string[] iconFiles = Directory.GetFiles(GetPath(GDR_Path.BackupIconsFolder)).Select(f => Path.GetFileName(f)).ToArray();
                  this._progressState.CurrentFileType = GameFileType.Icon;
                  this._progressState.NewFileBatch(iconFiles.Length);
                  CopyFiles(GDR_Path.BackupIconsFolder, GDR_Path.GameIconsFolder, iconFiles);

                  this._progressState.CurrentStage = ApplicationState.Idle;
            }

            public void AutoOverwriteFiles() {
                  if (IsGameDirectoryValid() == false) {
                        return;
                  }

                  this._progressState.CurrentStage = ApplicationState.Copying_Randomised_Files;

                  string[] resourceFiles = Directory.GetFiles(GetPath(GDR_Path.LocalResourcesOutputFolder)).Select(f => Path.GetFileName(f)).ToArray();
                  this._progressState.CurrentFileType = GameFileType.Resource;
                  this._progressState.NewFileBatch(resourceFiles.Length);
                  CopyFiles(GDR_Path.LocalResourcesOutputFolder, GDR_Path.GameResourcesFolder, resourceFiles);

                  string[] iconFiles = Directory.GetFiles(GetPath(GDR_Path.LocalIconsOutputFolder)).Select(f => Path.GetFileName(f)).ToArray();
                  this._progressState.CurrentFileType = GameFileType.Icon;
                  this._progressState.NewFileBatch(iconFiles.Length);
                  CopyFiles(GDR_Path.LocalIconsOutputFolder, GDR_Path.GameIconsFolder, iconFiles);

                  this._progressState.CurrentStage = ApplicationState.Idle;
            }

            void CopyFiles(GDR_Path from, GDR_Path to, string[] files) => CopyFiles(PathManager.GetPath(from), PathManager.GetPath(to), files);

            void CopyFiles(string from, string to, string[] files) {
                  for (int i = 0; i < files.Length; i++) {
                        this._progressState.CurrentFile = files[i];

                        // If the file exists in the source folder
                        if (File.Exists(Path.Combine(from, files[i])) == true) {

                              // Delete the file if it exists
                              if (File.Exists(Path.Combine(to, files[i])) == true) {
                                    File.Delete(Path.Combine(to, files[i]));
                              }
                              // Copy the file to the destination folder
                              File.Copy(Path.Combine(from, files[i]), Path.Combine(to, files[i]));
                        }

                        // Race condition, remove if multi-threading is implemented
                        this._progressState.CompletedFiles++;
                  }
            }

            public void StartRandomising(int seed) {
                  Random random = new Random(seed);

                  this._progressState.CurrentStage = ApplicationState.Setting_Up;
                  CreateFolders();

                  this._progressState.CurrentStage = ApplicationState.Backing_Up;
                  BackupOriginalFiles();

                  this._progressState.CurrentStage = ApplicationState.Unpacking;
                  _gamesheetManager.ReadAllFiles(PathManager.BackupResourcesFolder, AdvancedConfig.Instance.Quality);
                  //_fontManager.ReadAllFiles(PathManager.BackupResourcesFolder, AdvancedConfig.Instance.Quality);

                  this._progressState.CurrentStage = ApplicationState.Randomising;
                  /*GameSheet[] randomisedSprites =*/ _gamesheetManager.RandomizeFiles(random);
                  //Font[] randomisedFonts = _fontManager.RandomizeFiles(random);

                  //DebugOptions.WriteGameSheetImagesToDiskByGameSheet(_gamesheetManager.GameSheets.ToArray());

                  this._progressState.CurrentStage = ApplicationState.Repackaging;
                  //_gamesheetManager.WriteFilesToDisk();
                  //_gamesheetManager.WriteFilesToDisk(randomisedSprites);
                  //_fontManager.WriteFilesToDisk(randomisedFonts);

                  //if (AdvancedConfig.Instance.AutoOverwriteFiles == true) {
                  //      AutoOverwriteFiles();
                  //}

                  this._progressState.CurrentStage = ApplicationState.Idle;
                  this._progressState.NewFileBatch(0);
            }

            void CreateFolders() {
                  Directory.CreateDirectory(PathManager.BackupIconsFolder);
                  Directory.CreateDirectory(PathManager.BackupResourcesFolder);
            }

            public void BackupOriginalFiles() {
                  this._progressState.CurrentFileType = GameFileType.Resource;
                  BackupGameFiles(GDR_Path.GameResourcesFolder, GDR_Path.BackupResourcesFolder, GameFileType.Resource);
                  this._progressState.CurrentFileType = GameFileType.Icon;
                  BackupGameFiles(GDR_Path.GameIconsFolder, GDR_Path.BackupIconsFolder, GameFileType.Resource);
                  this._progressState.CurrentFileType = GameFileType.Font;
                  BackupGameFiles(GDR_Path.GameResourcesFolder, GDR_Path.BackupResourcesFolder, GameFileType.Font);
            }

            void BackupGameFiles(GDR_Path source, GDR_Path dest, GameFileType type) {
                  string[] missingFiles;
                  string[] backedUpFiles;
                  string[] fileExtensions;

                  switch (type) {
                        case GameFileType.Resource:
                        case GameFileType.Icon:
                              missingFiles = GamesheetManager.GetAllFileNames(source, AdvancedConfig.Instance.Quality).ToArray();
                              backedUpFiles = GamesheetManager.GetAllFileNames(dest, AdvancedConfig.Instance.Quality).ToArray();
                              fileExtensions = new string[] { ".plist", ".png" };
                              break;

                        case GameFileType.Font:
                              missingFiles = FontManager.GetAllFileNames(source, AdvancedConfig.Instance.Quality).ToArray();
                              backedUpFiles = FontManager.GetAllFileNames(dest, AdvancedConfig.Instance.Quality).ToArray();
                              fileExtensions = new string[] { ".fnt", ".png" };
                              break;
                        default:
                              Logger.Write(Logger.Mode.Error, $"The game file type \"{type}\" does not exist. Cannot proceed with backing up the original files");
                              return;
                  }

                  missingFiles = _fileBlacklist.FilterBlacklisted(missingFiles);
                  Array.Sort(backedUpFiles);

                  this._progressState.NewFileBatch(missingFiles.Length);

                  int copiedFiles = 0;
                  for (int i = 0; i < missingFiles.Length; i++) {
                        this._progressState.CurrentFile = missingFiles[i];

                        int index = Array.BinarySearch(backedUpFiles, missingFiles[i]);
                        // If the file doesn't exist in the backup folder copy it
                        if (index < 0) {
                              string sourcePath = Path.Combine(PathManager.GetPath(source), missingFiles[i]);
                              string destPath = Path.Combine(PathManager.GetPath(dest), missingFiles[i]);

                              // Check if the files exist before copying them just to be sure
                              if (File.Exists(destPath + fileExtensions[0]) == false) {
                                    File.Copy(sourcePath + fileExtensions[0], destPath + fileExtensions[0]);
                                    copiedFiles++;
                              }

                              if (File.Exists(destPath + fileExtensions[1]) == false) {
                                    File.Copy(sourcePath + fileExtensions[1], destPath + fileExtensions[1]);
                                    copiedFiles++;
                              }
                        }
                        this._progressState.CompletedFiles++;
                  }

                  if (copiedFiles != 0) {
                        Logger.Write(Logger.Mode.Info, $"Copied {copiedFiles} files\n\tfrom {PathManager.GetPath(source)}\n\tto {PathManager.GetPath(dest)}");
                  }
            }

            //void RandomiseData(int seed) {
            //      Logger.Write(Logger.Mode.Info, $"Randomising game data using the seed {seed}");

            //      Randomiser randomiser = new Randomiser(this, seed);
            //      List<Sprite> randomisedSprites = randomiser.RandomiseData();

            //      // Get all distint source files from all the sprites
            //      string[] gameSheetFiles = randomisedSprites
            //            .Select(s => s.SourceFile)
            //            .Distinct()
            //            .ToArray();

            //      this.progressState.CurrentStage = ApplicationState.Repackaging;
            //      this.progressState.CurrentFileType = GameFileType.Resource;
            //      this.progressState.NewFileBatch(gameSheetFiles.Length);

            //      // Default the output folders to the game folders
            //      string iconsOutputFolder = PathManager.LocalIconsOutputFolder;
            //      string resourcesOutputFolder = PathManager.LocalResourcesOutputFolder;

            //      Directory.CreateDirectory(iconsOutputFolder);
            //      Directory.CreateDirectory(resourcesOutputFolder);

            //      for (int i = 0; i < gameSheetFiles.Length; i++) {
            //            this.progressState.CurrentFile = gameSheetFiles[i];

            //            // Get all sprites that go into this file
            //            Sprite[] sprites = randomisedSprites
            //                  .Where(s => s.SourceFile == gameSheetFiles[i])
            //                  .ToArray();

            //            PackingRectangle[] rects = new PackingRectangle[sprites.Length];

            //            // Populate rects array with sprite data
            //            for (int j = 0; j < sprites.Length; j++) {
            //                  // Add 1 pixel on every side of all sprites to not make them flow into each other
            //                  rects[j] = new PackingRectangle(0, 0, (uint)sprites[j].CropRect.Width + 2, (uint)sprites[j].CropRect.Height + 2, j);
            //            }

            //            // Get new rectangles for how to rearrange the sprites
            //            getPackingRects(ref rects, out PackingRectangle bounds);

            //            for (int j = 0; j < sprites.Length; j++) {
            //                  // If the texture is offset by 2 pixel on either side at least (1 + 1)
            //                  if (sprites[j].TextureRect.X != sprites[j].CropRect.X || sprites[j].TextureRect.Y != sprites[j].CropRect.Y) {
            //                        sprites[j].TextureRect = new Rectangle((int)rects[j].X + 2, (int)rects[j].Y + 2, (int)rects[j].Width, (int)rects[j].Height);

            //                  } else {
            //                        // Otherwise add just 1 pixel to account for sprites flowing into each other
            //                        sprites[j].TextureRect = new Rectangle((int)rects[j].X + 1, (int)rects[j].Y + 1, (int)rects[j].Width, (int)rects[j].Height);
            //                  }  
            //            }

            //            // Assemble new gamesheet
            //            Bitmap finalGameSheet = GameSheetAssembler.Assemble(sprites, rects, bounds);

            //            // Compile the new plist file
            //            string[] plistFile = Plist.Serialise(sprites, gameSheetFiles[i], new Size(finalGameSheet.Width, finalGameSheet.Height));

            //            // Determine if the gamesheet contains icons to determine where the new files need to be saved
            //            bool isIconsFile = sprites.Any(s => s.ResourceType == ResourceType.Icon);
            //            string outputFolder = isIconsFile ? iconsOutputFolder : resourcesOutputFolder;

            //            File.WriteAllLines(Path.Combine(outputFolder, gameSheetFiles[i] + ".plist"), plistFile);
            //            finalGameSheet.Save(Path.Combine(outputFolder, gameSheetFiles[i] + ".png"));
            //            // Get rid of the bitmap once it is saved
            //            finalGameSheet.Dispose();

            //            this.progressState.CompletedFiles++;
            //      }
            //}

            //void getPackingRects(ref PackingRectangle[] rects, out PackingRectangle bounds) {
            //      RectanglePacker.Pack(rects, out bounds, PackingHints.TryByArea, 1, 2);
            //      Array.Sort(rects, (x, y) => x.Id.CompareTo(y.Id));
            //}
      }
}
