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

                  fileBlacklist = new FileBlacklist();
                  gamesheetManager = new GamesheetManager(this);
                  fontManager = new FontManager(this);
            }

            private readonly GDR_Form GDR;

            private readonly FileBlacklist fileBlacklist;
            private readonly GamesheetManager gamesheetManager;
            private readonly FontManager fontManager;

            public List<Sprite> spriteList = new List<Sprite>();

            public ProgressState progressState = new ProgressState(0, 0, string.Empty);

            // -----------------------------------------------------------------------------------------------

            public ReadyState getReadyState() {
                  ReadyState rs = ReadyState.Ready;

                  if (Directory.Exists(Config.Instance.gameDirectory) == false) {
                        rs |= ReadyState.FolderNotFound;
                  }
                  if (Directory.Exists(PathManager.gameResourcesFolder) == false) {
                        rs |= ReadyState.ResourceFolderNotFound;
                  }
                  if (Directory.Exists(PathManager.gameIconsFolder) == false) {
                        rs |= ReadyState.IconFolderNotFound;
                  }
                  if (File.Exists(Path.Combine(Config.Instance.gameDirectory, "GeometryDash.exe")) == false) {
                        rs |= ReadyState.ExeNotFound;
                  }
                  if (Config.Instance.GetEnabledSettingsCount() == 0) {
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
                  this.progressState.currentStage = ApplicationState.Restoring;

                  string[] resourceFiles = Directory.GetFiles(GetPath(GDR_Path.BackupResourcesFolder)).Select(f => Path.GetFileName(f)).ToArray();
                  this.progressState.currentFileType = GameFileType.Resource;
                  this.progressState.NewFileBatch(resourceFiles.Length);
                  CopyFiles(GDR_Path.BackupResourcesFolder, GDR_Path.GameResourcesFolder, resourceFiles);

                  string[] iconFiles = Directory.GetFiles(GetPath(GDR_Path.BackupIconsFolder)).Select(f => Path.GetFileName(f)).ToArray();
                  this.progressState.currentFileType = GameFileType.Icon;
                  this.progressState.NewFileBatch(iconFiles.Length);
                  CopyFiles(GDR_Path.BackupIconsFolder, GDR_Path.GameIconsFolder, iconFiles);

                  this.progressState.currentStage = ApplicationState.Idle;
            }

            public void AutoOverwriteFiles() {
                  if (IsGameDirectoryValid() == false) {
                        return;
                  }
                  this.progressState.currentStage = ApplicationState.Copying_Randomised_Files;

                  string[] resourceFiles = Directory.GetFiles(GetPath(GDR_Path.LocalResourcesOutputFolder)).Select(f => Path.GetFileName(f)).ToArray();
                  this.progressState.currentFileType = GameFileType.Resource;
                  this.progressState.NewFileBatch(resourceFiles.Length);
                  CopyFiles(GDR_Path.LocalResourcesOutputFolder, GDR_Path.GameResourcesFolder, resourceFiles);

                  string[] iconFiles = Directory.GetFiles(GetPath(GDR_Path.LocalIconsOutputFolder)).Select(f => Path.GetFileName(f)).ToArray();
                  this.progressState.currentFileType = GameFileType.Icon;
                  this.progressState.NewFileBatch(iconFiles.Length);
                  CopyFiles(GDR_Path.LocalIconsOutputFolder, GDR_Path.GameIconsFolder, iconFiles);

                  this.progressState.currentStage = ApplicationState.Idle;
            }

            void CopyAllFiles(GDR_Path from, GDR_Path to) => CopyAllFiles(PathManager.GetPath(from), PathManager.GetPath(to));

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

            void CopyFiles(GDR_Path from, GDR_Path to, string[] files) => CopyFiles(PathManager.GetPath(from), PathManager.GetPath(to), files);

            void CopyFiles(string from, string to, string[] files) {
                  for (int i = 0; i < files.Length; i++) {
                        this.progressState.currentFile = files[i];

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

            public void StartRandomising(int seed) {
                  this.progressState.currentStage = ApplicationState.Setting_Up;
                  CreateFolders();

                  this.progressState.currentStage = ApplicationState.Backing_Up;
                  backupOriginalFiles();

                  this.progressState.currentStage = ApplicationState.Unpacking;

                  if (spriteList.Count == 0) {
                        this.spriteList.AddRange(ExtractGameFiles());
                  }

                  if (fontManager.fontCount == 0)
                        fontManager.ReadAllFontFiles(PathManager.backupResourcesFolder, Config.Instance.quality);

                  this.progressState.currentStage = ApplicationState.Randomising;
                  Font[] randomisedFonts = fontManager.RandomiseFiles(fontManager.GetRandomisationMode(), seed);
                  RandomiseData(seed);

                  this.progressState.currentStage = ApplicationState.Repackaging;
                  this.progressState.currentFileType = GameFileType.Font;

                  fontManager.WriteFontsToDisk(randomisedFonts);

                  this.progressState.currentStage = ApplicationState.Copying_Randomised_Files;

                  if (Config.Instance.autoOverwriteFiles == true) {
                        AutoOverwriteFiles();
                  }

                  this.progressState.currentStage = ApplicationState.Idle;

                  this.progressState.NewFileBatch(0);
            }

            void CreateFolders() {
                  Directory.CreateDirectory(PathManager.backupIconsFolder);
                  Directory.CreateDirectory(PathManager.backupResourcesFolder);
            }

            public void backupOriginalFiles() {
                  this.progressState.currentFileType = GameFileType.Resource;
                  BackupGameFiles(GDR_Path.GameResourcesFolder, GDR_Path.BackupResourcesFolder, GameFileType.Resource);
                  this.progressState.currentFileType = GameFileType.Icon;
                  BackupGameFiles(GDR_Path.GameIconsFolder, GDR_Path.BackupIconsFolder, GameFileType.Resource);
                  this.progressState.currentFileType = GameFileType.Font;
                  BackupGameFiles(GDR_Path.GameResourcesFolder, GDR_Path.BackupResourcesFolder, GameFileType.Font);
            }

            string[] GetAllFiles(GDR_Path path, string[] extensions) {
                  return Directory.GetFiles(PathManager.GetPath(path)).Where(f => f.EndsWith(extensions) == true).ToArray();
            }

            void BackupGameFiles(GDR_Path source, GDR_Path dest, GameFileType type) {
                  string[] missingFiles;
                  string[] backedUpFiles;
                  string[] fileExtensions;

                  switch (type) {
                        case GameFileType.Resource:
                        case GameFileType.Icon:
                              missingFiles = gamesheetManager.GetAllFileNames(source, Config.Instance.quality);
                              backedUpFiles = gamesheetManager.GetAllFileNames(dest, Config.Instance.quality);
                              fileExtensions = new string[] { ".plist", ".png" };
                              break;

                        case GameFileType.Font:
                              missingFiles = fontManager.GetAllFileNames(source, Config.Instance.quality);
                              backedUpFiles = fontManager.GetAllFileNames(dest, Config.Instance.quality);
                              fileExtensions = new string[] { ".fnt", ".png" };
                              break;
                        default:
                              return;
                  }

                  missingFiles = fileBlacklist.FilterBlacklisted(missingFiles);
                  Array.Sort(backedUpFiles);

                  this.progressState.NewFileBatch(missingFiles.Length);

                  int copiedFiles = 0;
                  for (int i = 0; i < missingFiles.Length; i++) {
                        this.progressState.currentFile = missingFiles[i];

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
                        this.progressState.completedFiles++;
                  }

                  if (copiedFiles != 0) {
                        Console.WriteLine($"Copied {copiedFiles} files\n\tfrom {PathManager.GetPath(source)}\n\tto {PathManager.GetPath(dest)}");
                  }
            }

            public List<Sprite> ExtractGameFiles() {
                  List<Sprite> extractedSprites = new List<Sprite>();

                  extractedSprites.AddRange(ExtractResourceFiles(GameFileType.Resource, GDR_Path.BackupResourcesFolder));
                  extractedSprites.AddRange(ExtractResourceFiles(GameFileType.Icon, GDR_Path.BackupIconsFolder));

                  return extractedSprites;
            }

            public List<Sprite> ExtractResourceFiles(GameFileType type, GDR_Path path) {
                  List<Sprite> extractedSprites = new List<Sprite>();

                  this.progressState.currentFileType = type;

                  string[] files = gamesheetManager.GetAllFileNames(path, Config.Instance.quality)
                        .Select(f => Path.Combine(PathManager.GetPath(path), f)).ToArray();

                  Console.WriteLine($"Unpacking {files.Length} {type.ToString().ToLower()} files from {PathManager.GetPath(path)}");

                  this.progressState.NewFileBatch(files.Length);

                  for (int i = 0; i < files.Length; i++) {
                        this.progressState.currentFile = Path.GetFileName(files[i]);
                        extractedSprites.AddRange(getAllSpritesFromGameFile(files[i]));
                        this.progressState.completedFiles++;
                  }

                  return extractedSprites;
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

                  string[] data = File.ReadAllLines(textFile);
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
                  Console.WriteLine($"Randomising game data using the seed {seed}");

                  Randomiser randomiser = new Randomiser(this, seed);
                  List<Sprite> randomisedSprites = randomiser.RandomiseData();

                  // Get all distint source files from all the sprites
                  string[] gameSheetFiles = randomisedSprites
                        .Select(s => s.sourceFile)
                        .Distinct()
                        .ToArray();

                  this.progressState.currentStage = ApplicationState.Repackaging;
                  this.progressState.currentFileType = GameFileType.Resource;
                  this.progressState.NewFileBatch(gameSheetFiles.Length);

                  // Default the output folders to the game folders
                  string iconsOutputFolder = PathManager.localIconsOutputFolder;
                  string resourcesOutputFolder = PathManager.localResourcesOutputFolder;

                  Directory.CreateDirectory(iconsOutputFolder);
                  Directory.CreateDirectory(resourcesOutputFolder);

                  for (int i = 0; i < gameSheetFiles.Length; i++) {
                        this.progressState.currentFile = gameSheetFiles[i];

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

                        this.progressState.completedFiles++;
                  }
            }

            void getPackingRects(ref PackingRectangle[] rects, out PackingRectangle bounds) {
                  RectanglePacker.Pack(rects, out bounds, PackingHints.TryByArea, 1, 2);
                  Array.Sort(rects, (x, y) => x.Id.CompareTo(y.Id));
            }
      }
}
