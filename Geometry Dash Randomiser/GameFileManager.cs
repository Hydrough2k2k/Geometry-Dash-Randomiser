using RectpackSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Serialization;
using static Geometry_Dash_Randomiser.PathManager;

namespace Geometry_Dash_Randomiser {

      public class GameFileManager {

            public enum Quality { High, Medium, Low }

            public enum ApplicationState { Idle, Setting_Up, Backing_Up, Unpacking, Randomising, Repackaging, Finishing_Up, Restoring, Complete }

            public enum GameFileType { None, Resource, Icon, Font }

            public GameFileManager(GDR_Form creator) {
                  GDR = creator;

                  fileBlacklist = new FileBlacklist();
                  pathManager = new PathManager(this);
                  gamesheetManager = new GamesheetManager(this, this.pathManager);
                  fontManager = new FontManager(this, this.pathManager);

                  pathManager.SetQuality(Config.quality);
            }

            private readonly GDR_Form GDR;

            private readonly FileBlacklist fileBlacklist;
            public readonly PathManager pathManager;
            private readonly GamesheetManager gamesheetManager;
            private readonly FontManager fontManager;

            public List<Sprite> spriteList = new List<Sprite>();

            // -----------------------------------------------------------------------------------------------

            public ProgressState progressState = new ProgressState(0, 0, string.Empty);

            // -----------------------------------------------------------------------------------------------

            public enum ReadyState {
                  Ready = 1,
                  GameFolderNotFound = 2,
                  NoSettingsEnabled = 4
            }

            // ----------------------------------------------------------

            public ReadyState getReadyState() {
                  ReadyState readyState = getGameDirectoryStatus();

                  if (Config.GetEnabledSettingsCount() == 0)
                        readyState |= ReadyState.NoSettingsEnabled;

                  // See if there are any critical errors. If there are none, add Ready to the readystate
                  if (readyState.HasFlag(ReadyState.GameFolderNotFound) == false &&
                        readyState.HasFlag(ReadyState.NoSettingsEnabled) == false) {

                        readyState |= ReadyState.Ready;
                  }
                  return readyState;
            }

            ReadyState getGameDirectoryStatus() {
                  if (Directory.Exists(Config.gameDirectory) == false) {
                        return ReadyState.GameFolderNotFound;
                  } else if (File.Exists(Path.Combine(Config.gameDirectory, "GeometryDash.exe")) == false) {
                        return ReadyState.GameFolderNotFound;
                  } else if(Directory.Exists(Config.gameDirectory) == false) {
                        return ReadyState.GameFolderNotFound;
                  } else if (Directory.Exists(pathManager.gameResourcesFolder) == false) {
                        return ReadyState.GameFolderNotFound;
                  } else if (Directory.Exists(pathManager.gameResourcesFolder) == false) {
                        return ReadyState.GameFolderNotFound;
                  }
                  return new ReadyState();
            }

            public bool IsGameDirectoryValid() {
                  ReadyState gameDirectoryState = getGameDirectoryStatus();
                  if (gameDirectoryState.HasFlag(ReadyState.GameFolderNotFound) == true) {
                        return false;
                  }
                  return true;
            }

            public void RestoreFiles() {
                  if (IsGameDirectoryValid() == false) {
                        return;
                  }

                  string[] resourceFiles = Directory.GetFiles(pathManager.GetPath(GDR_Path.BackupResourcesFolder)).Select(f => Path.GetFileName(f)).ToArray();
                  string[] iconFiles = Directory.GetFiles(pathManager.GetPath(GDR_Path.BackupIconsFolder)).Select(f => Path.GetFileName(f)).ToArray();

                  int totalFilesToBeRestored = resourceFiles.Length + iconFiles.Length;
                  this.progressState.totalFiles = totalFilesToBeRestored;

                  CopyFiles(GDR_Path.BackupResourcesFolder, GDR_Path.GameResourcesFolder, resourceFiles);
                  CopyFiles(GDR_Path.BackupIconsFolder, GDR_Path.GameIconsFolder, iconFiles);
            }

            void CopyAllFiles(GDR_Path from, GDR_Path to) => CopyAllFiles(pathManager.GetPath(from), pathManager.GetPath(to));

            void CopyAllFiles(string from, string to) {
                  string[] files = Directory.GetFiles(from).Select(f => Path.GetFileName(f)).ToArray();

                  for (int i = 0; i < files.Length; i++) {
                        // Delete the file if it exists
                        if (File.Exists(Path.Combine(to, files[i])) == true) {
                              File.Delete(Path.Combine(to, files[i]));
                        }
                        File.Copy(Path.Combine(from, files[i]), Path.Combine(to, files[i]));
                  }
            }

            void CopyFiles(GDR_Path from, GDR_Path to, string[] files) => CopyFiles(pathManager.GetPath(from), pathManager.GetPath(to), files);

            void CopyFiles(string from, string to, string[] files) {
                  for (int i = 0; i < files.Length; i++) {
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
                        this.progressState.completedFiles++;
                  }
            }

            //[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
            public void StartRandomising(int seed) {
                  this.progressState.currentStage = ApplicationState.Setting_Up;
                  CreateFolders();

                  this.progressState.currentStage = ApplicationState.Backing_Up;
                  backupOriginalFiles();

                  this.progressState.currentStage = ApplicationState.Unpacking;
                  ExtractFiles();

                  this.progressState.currentStage = ApplicationState.Randomising;
                  RandomiseData(seed);
                  Font[] randomisedFonts = fontManager.RandomiseFiles(fontManager.GetRandomisationMode(), seed);
                  fontManager.WriteFontsToDisk(pathManager.localResourcesOutputFolder, randomisedFonts);

                  this.progressState.currentStage = ApplicationState.Idle;
            }

            void CreateFolders() {
                  Directory.CreateDirectory(pathManager.backupIconsFolder);
                  Directory.CreateDirectory(pathManager.backupResourcesFolder);
            }

            public void backupOriginalFiles() {
                  BackupGameFiles(GDR_Path.GameResourcesFolder, GDR_Path.BackupResourcesFolder, GameFileType.Resource);
                  BackupGameFiles(GDR_Path.GameIconsFolder, GDR_Path.BackupIconsFolder, GameFileType.Resource);
                  BackupGameFiles(GDR_Path.GameResourcesFolder, GDR_Path.BackupResourcesFolder, GameFileType.Font);
            }

            string[] GetAllFiles(GDR_Path path, string[] extensions) {
                  return Directory.GetFiles(pathManager.GetPath(path)).Where(f => f.EndsWith(extensions) == true).ToArray();
            }

            void BackupGameFiles(GDR_Path source, GDR_Path dest, GameFileType type) {
                  string[] missingFiles;
                  string[] backedUpFiles;
                  string[] fileExtensions;

                  switch (type) {
                        case GameFileType.Resource:
                        case GameFileType.Icon:
                              missingFiles = gamesheetManager.GetAllFileNames(source, Config.quality);
                              backedUpFiles = gamesheetManager.GetAllFileNames(dest, Config.quality);
                              fileExtensions = new string[] { ".plist", ".png" };
                              break;

                        case GameFileType.Font:
                              missingFiles = fontManager.GetAllFileNames(source, Config.quality);
                              backedUpFiles = fontManager.GetAllFileNames(dest, Config.quality);
                              fileExtensions = new string[] { ".fnt", ".png" };
                              break;
                        default:
                              return;
                  }

                  missingFiles = fileBlacklist.FilterBlacklisted(missingFiles);
                  Array.Sort(backedUpFiles);

                  for (int i = 0; i < missingFiles.Length; i++) {
                        int index = Array.BinarySearch(backedUpFiles, missingFiles[i]);
                        // If the file doesn't exist in the backup folder copy it
                        if (index < 0) {
                              string sourcePath = Path.Combine(pathManager.GetPath(source), missingFiles[i]);
                              string destPath = Path.Combine(pathManager.GetPath(dest), missingFiles[i]);

                              // Check if the files exist before copying them just to be sure
                              if (File.Exists(destPath + fileExtensions[0]) == false) {
                                    File.Copy(sourcePath + fileExtensions[0], destPath + fileExtensions[0]);
                              }

                              if (File.Exists(destPath + fileExtensions[1]) == false) {
                                    File.Copy(sourcePath + fileExtensions[1], destPath + fileExtensions[1]);
                              }
                        }
                  }
            }

            void ExtractFiles() {
                  if (spriteList.Count == 0)
                        extractGameFiles();

                  if (fontManager.fontCount == 0)
                        fontManager.ReadAllFontFiles(pathManager.backupResourcesFolder, Config.quality);
            }

            void extractGameFiles() {
                  string[] files = gamesheetManager.GetAllFileNames(GDR_Path.BackupResourcesFolder, Config.quality)
                        .Select(f => Path.Combine(pathManager.GetPath(GDR_Path.BackupResourcesFolder), f)).ToArray();

                  for (int i = 0; i < files.Length; i++) {
                        Console.WriteLine(files[i]);
                        spriteList.AddRange(getAllSpritesFromGameFile(files[i]));
                  }

                  files = gamesheetManager.GetAllFileNames(GDR_Path.BackupIconsFolder, Config.quality)
                        .Select(f => Path.Combine(pathManager.GetPath(GDR_Path.BackupIconsFolder), f)).ToArray();

                  for (int i = 0; i < files.Length; i++) {
                        Console.WriteLine(files[i]);
                        spriteList.AddRange(getAllSpritesFromGameFile(files[i]));
                  }
            }

            public List<Sprite> getAllSpritesOfType(Sprite.ResourceType type) {
                  return spriteList.Where(s => s.type == type).ToList();
            }

            public List<Sprite> getAllSpritesOfType(Sprite.IconType iconType) {
                  return spriteList.Where(s => s.iconType == iconType).ToList();
            }

            List<Sprite> getAllSpritesFromGameFile(string path) {
                  string textFile = path + ".plist";
                  string imageFile = path + ".png";
                  
                  if (!File.Exists(textFile) || !File.Exists(imageFile)) {
                        return new List<Sprite>();
                  }

                  string[] data = Extensions.ReadTextFile(textFile);
                  List<Sprite> sprites = Plist.BulkDeserialise(data);

                  string fileName = Path.GetFileName(path).RemoveExtension();
                  
                  for (int i = 0; i < sprites.Count; i++) {
                        sprites[i].sourceFile = fileName;
                        sprites[i].AssignType();

                        sprites[i].cropRect = sprites[i].textureRect;
                  }

                  if (fileName.StartsWith("GJ_GameSheet03") == false &&
                        fileName.StartsWith("PixelSheet_01") == false) {
                        
                        for (int i = 0; i < sprites.Count; i++) {
                              sprites[i].cropRect = new Rectangle(
                                    sprites[i].textureRect.X - 1, sprites[i].textureRect.Y - 1,
                                    sprites[i].textureRect.Width + 2, sprites[i].textureRect.Height + 2);
                        }
                  }

                  Rectangle[] rects = sprites.Select(s => s.cropRect).ToArray();
                  int sliceSize = 512;
                  Bitmap gamesheet = new Bitmap(imageFile);
                  Bitmap[] cropped = gamesheet.Multicrop(rects, sliceSize);

                  for (int i = 0; i < cropped.Length; i++) {
                        sprites[i].texture = cropped[i];
                  }

                  // Free memory
                  gamesheet.Dispose();

                  return sprites;
            }

            void RandomiseData(int seed) {
                  Randomiser randomiser = new Randomiser(this, seed);
                  List<Sprite> randomisedSprites = randomiser.RandomiseData();

                  // Get all distint source files from all the sprites
                  string[] gameSheetFiles = randomisedSprites
                        .Select(s => s.sourceFile)
                        .Distinct()
                        .ToArray();

                  string iconsOutputFolder = pathManager.localIconsOutputFolder;
                  string resourcesOutputFolder = pathManager.localResourcesOutputFolder;

                  Directory.CreateDirectory(iconsOutputFolder);
                  Directory.CreateDirectory(resourcesOutputFolder);

                  for (int i = 0; i < gameSheetFiles.Length; i++) {
                        // Get all sprites that go into this file
                        Sprite[] sprites = randomisedSprites
                              .Where(s => s.sourceFile == gameSheetFiles[i])
                              .ToArray();

                        PackingRectangle[] rects = new PackingRectangle[sprites.Length];

                        // Populate rects array with sprite data
                        for (int j = 0; j < sprites.Length; j++) {
                              // Add 1 pixel on every side of all sprites to not make them flow into each other
                              rects[j] = new PackingRectangle(0, 0, (uint)sprites[j].cropRect.Width + 2, (uint)sprites[j].cropRect.Height + 2, j);
                        }

                        // Get new rectangles for how to rearrange the sprites
                        getPackingRects(ref rects, out PackingRectangle bounds);

                        for (int j = 0; j < sprites.Length; j++) {
                              // If the texture is offset by 2 pixel on either side at least (1 + 1)
                              if (sprites[j].textureRect.X != sprites[j].cropRect.X || sprites[j].textureRect.Y != sprites[j].cropRect.Y) {
                                    sprites[j].textureRect = new Rectangle((int)rects[j].X + 2, (int)rects[j].Y + 2, (int)rects[j].Width, (int)rects[j].Height);

                              } else {
                                    // Otherwise add just 1 pixel to account for sprites flowing into each other
                                    sprites[j].textureRect = new Rectangle((int)rects[j].X + 1, (int)rects[j].Y + 1, (int)rects[j].Width, (int)rects[j].Height);
                              }  
                        }

                        // Assemble new gamesheet
                        Bitmap finalGameSheet = GameSheet.Assemble(sprites, rects, bounds);

                        // Compile the new plist file
                        string[] plistFile = Plist.Serialise(sprites, gameSheetFiles[i], new Size(finalGameSheet.Width, finalGameSheet.Height));

                        // Determine if the gamesheet contains icons to determine where the new files need to be saved
                        bool isIconsFile = sprites.Any(s => s.type == Sprite.ResourceType.Icon);
                        string outputFolder = isIconsFile ? iconsOutputFolder : resourcesOutputFolder;

                        File.WriteAllLines(Path.Combine(outputFolder, gameSheetFiles[i] + ".plist"), plistFile);
                        finalGameSheet.Save(Path.Combine(outputFolder, gameSheetFiles[i] + ".png"));
                        // Get rid of the bitmap once it is saved
                        finalGameSheet.Dispose();
                  }
            }

            void getPackingRects(ref PackingRectangle[] rects, out PackingRectangle bounds) {
                  RectanglePacker.Pack(rects, out bounds, PackingHints.TryByArea, 1, 2);
                  Array.Sort(rects, (x, y) => x.Id.CompareTo(y.Id));
            }
      }
}
