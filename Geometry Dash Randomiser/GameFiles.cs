using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Drawing;
using System.Text.Json;
using RectpackSharp;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Geometry_Dash_Randomiser {

      public class GameFiles {

            public enum Quality { High, Medium, Low }

            public enum Stage { BackingUp, Reading, Unpacking, Caching, Randomising, Repackaging }

            public GameFiles(GDR_Form creator) {
                  GDR = creator;

                  setQuality(Config.quality);
            }

            private GDR_Form GDR = null;

            public List<Sprite> spriteList = new List<Sprite>();

            static readonly string cachedFilesFolderName = "Cached Game Files";
            static readonly string resourcesFolderName = "Resources";
            static readonly string iconsFolderName = "icons";
            static readonly string sfxFolderName = "sfx";

            static readonly string randomisedFilesFolderName = "Randomised Files";
            static readonly string unalteredFilesFolderName = "Unaltered Files";

            public const string lowQualityName = "Low Quality";
            public const string mediumQualityName = "Medium Quality";
            public const string highQualityName = "High Quality";

            // ----------------------------------------------------------

            static readonly string gameFilesFolder = Config.gameDirectory;
            static readonly string gameResourcesFolder = Path.Combine(gameFilesFolder, resourcesFolderName);
            static readonly string gameIconsFolder = Path.Combine(gameResourcesFolder, iconsFolderName);
            static readonly string gameSfxFolder = Path.Combine(gameResourcesFolder, sfxFolderName);

            // ----------------------------------------------------------

            static readonly string localCachedFilesFolder = cachedFilesFolderName;
            static readonly string localResourcesFolder = Path.Combine(localCachedFilesFolder, resourcesFolderName);
            static readonly string localIconsFolder = Path.Combine(localCachedFilesFolder, iconsFolderName);
            static readonly string localSfxFolder = Path.Combine(localCachedFilesFolder, sfxFolderName);

            static readonly string localIconsOutputFolder = Path.Combine(randomisedFilesFolderName, iconsFolderName);
            static readonly string localResourcesOutputFolder = Path.Combine(randomisedFilesFolderName, resourcesFolderName);

            static readonly string localUnalteredIconsFolder = Path.Combine(unalteredFilesFolderName, iconsFolderName);
            static readonly string localUnalteredResourcesFolder = Path.Combine(unalteredFilesFolderName, resourcesFolderName);

            // ----------------------------------------------------------

            static readonly string localCachedTexturesJson = "cachedTextures.json";

            // ----------------------------------------------------------

            Stopwatch mainUpdateTimer = Stopwatch.StartNew();
            Stopwatch secondaryUpdateTimer = Stopwatch.StartNew();
            /// <summary>
            /// Minimum time that needs to elapse in millisec between prints
            /// </summary>
            int mainPrintDelta = 33;
            int secondaryPrintDelta = 33;

            /// <summary>
            /// Name of the folder where the cached rescources will go
            /// </summary>
            string currentCachedQualityFolder;

            public void setQuality(Quality quality) {
                  switch (quality) {
                        case Quality.Low:
                              currentCachedQualityFolder = Path.Combine(localCachedFilesFolder, lowQualityName);
                              break;
                        case Quality.Medium:
                              currentCachedQualityFolder = Path.Combine(localCachedFilesFolder, mediumQualityName);
                              break;
                        case Quality.High:
                              currentCachedQualityFolder = Path.Combine(localCachedFilesFolder, highQualityName);
                              break;
                        default:
                              break;
                  }
            }

            // ----------------------------------------------------------

            public static readonly string[] fileBlacklist = {
                  "CCControlColourPickerSpriteSheet",
                  "DungeonSheet",
                  "PlayerExplosion"
            };

            public enum ReadyState {
                  Unknown,
                  Ready,
                  ReadyCacheIsMissing,
                  GameFolderNotFound,
                  GameResourceFolderNotFound,
                  GameIconFolderNotFound,
                  ExeNotFound,
                  NoSettingsEnabled,
                  OutputFolderNotFound,
                  OutputFolderIsCreatable
            };

            public static readonly string[] readyStateStrings = {
                  "The application is not ready due to an unknown error",
                  "The randomisation is ready to start",
                  "The randomisation is ready to start\n - Data may need to be cached first, it might take a few minutes",
                  "The given folder doesn't exist",
                  "The \"Recources\" folder could not be found in the given game path",
                  "The \"Recources\\icons\" folder could not be found in the given game path",
                  "The executable \"GeometryDash.exe\" could not be found in the given path",
                  "No settings are enabled for randomisation. Enable at least one setting to start it",
                  "The given output folder doesn't exist",
                  "The given output folder doesn't exist, but can be created"
            };

            // ----------------------------------------------------------

            // Declare the delegate (if using non-generic pattern).
            public delegate void ProgressUpdateEvent(object sender, ProgressUpdate e);

            // Declare the event.
            public event ProgressUpdateEvent updateEvent;

            public delegate void UpdateFileProgress(object sender, int percent);
            public event UpdateFileProgress updateFileProgressEvent;

            public delegate void UpdateTotalProgress(object sender, int percent);
            public event UpdateTotalProgress updateTotalProgressEvent;

            public delegate void ChangeDisplayedText(object sender, string s);
            public event ChangeDisplayedText changeDisplayedTextEvent;

            public bool isReady() {
                  ReadyState ready = getRandomisationReadyState();

                  return isReady(ready);
            }

            public bool isReady(ReadyState ready) {
                  if (ready != ReadyState.Ready &&
                        ready != ReadyState.ReadyCacheIsMissing &&
                        ready != ReadyState.OutputFolderIsCreatable) {

                        return false;
                  }
                  return true;
            }

            public ReadyState getReadyState() {
                  ReadyState ready = getRandomisationReadyState();
                  return ready;
            }

            ReadyState getRandomisationReadyState() {

                  if (Directory.Exists(Config.gameDirectory) == false)
                        return ReadyState.GameFolderNotFound;

                  ReadyState readyState = getGameDirectoryStatus();

                  if (readyState != ReadyState.Ready)
                        return readyState;

                  //if (Config.caching == true && verifyCacheIntegrity() == false)
                  //      return readyState.ReadyCacheIsMissing;

                  if (Config.GetEnabledSettingsCount() == 0)
                        return ReadyState.NoSettingsEnabled;

                  switch (Config.GetOutputDirectoryStatus()) {
                        case Config.OutputFolder.Unknown:
                              return ReadyState.Unknown;
                        case Config.OutputFolder.Default:
                              break;
                        case Config.OutputFolder.Overwritten:
                              break;
                        case Config.OutputFolder.Invalid:
                              return ReadyState.OutputFolderNotFound;
                        case Config.OutputFolder.Creatable:
                              return ReadyState.OutputFolderIsCreatable;
                  }
                  return ReadyState.Ready;
            }

            public ReadyState getGameDirectoryStatus() {

                  if (File.Exists(Path.Combine(Config.gameDirectory, "GeometryDash.exe")) == false)
                        return ReadyState.ExeNotFound;

                  if (Directory.Exists(gameFilesFolder) == false)
                        return ReadyState.GameResourceFolderNotFound;

                  if (Directory.Exists(gameResourcesFolder) == false)
                        return ReadyState.GameResourceFolderNotFound;

                  if (Directory.Exists(gameIconsFolder) == false)
                        return ReadyState.GameIconFolderNotFound;

                  return ReadyState.Ready;
            }

            // Very simple right now, will be more complex when new features require complexity
            bool verifyCacheIntegrity() {
                  return File.Exists(Path.Combine(currentCachedQualityFolder, localCachedTexturesJson));
            }

            public void StartRandomising(int seed) {

                  mainUpdateTimer.Restart();
                  secondaryUpdateTimer.Restart();

                  backupUnalteredFiles();

                  if (spriteList.Count == 0) {
                        extractGameFiles();
                  }

                  // Caching is disabled for the time being
                  //if (Config.caching == true && verifyCacheIntegrity() == false) {
                        //CacheGameFiles();
                  //}

                  changeDisplayedTextEvent?.Invoke(this, "Randomising data...");
                  randomiseData(seed);
            }

            void randomiseData(int seed) {
                  Randomiser random = new Randomiser(this, seed);
                  List<Sprite> randomisedSprites = random.RandomiseData();

                  string[] gameSheetFiles = randomisedSprites
                        .Select(s => s.sourceFile)
                        .Distinct()
                        .ToArray();

                  string outputPath;

                  if (Config.outputDirectory == string.Empty) {
                        outputPath = randomisedFilesFolderName;

                  } else {
                        outputPath = Config.outputDirectory;
                  }

                  Directory.CreateDirectory(Path.Combine(outputPath, iconsFolderName));
                  Directory.CreateDirectory(Path.Combine(outputPath, resourcesFolderName));

                  for (int i = 0; i < gameSheetFiles.Length; i++) {
                        if (mainUpdateTimer.ElapsedMilliseconds > mainPrintDelta) {
                              int ProgressPercent = (int)Math.Ceiling((float)i / gameSheetFiles.Length * 100);
                              updateEvent?.Invoke(this, new ProgressUpdate(gameSheetFiles[i], ProgressPercent, Stage.Repackaging));

                              mainUpdateTimer.Restart();
                        }

                        Sprite[] sprites = randomisedSprites
                              .Where(s => s.sourceFile == gameSheetFiles[i])
                              .ToArray();

                        updateFileProgressEvent?.Invoke(this, 0);

                        PackingRectangle bounds;
                        PackingRectangle[] rects = getPackingRects(ref sprites, out bounds);

                        Bitmap finalGameSheet = GameSheet.Assemble(sprites, rects, bounds);
                        updateFileProgressEvent?.Invoke(this, 50);

                        string[] plistFile = Plist.Serialise(sprites, rects, gameSheetFiles[i], new Size(finalGameSheet.Width, finalGameSheet.Height));

                        bool isIconsFile = sprites.Any(s => s.type == Sprite.Type.Icon);
                        string outputFolder = isIconsFile ? iconsFolderName : resourcesFolderName;
                        outputFolder = Path.Combine(outputPath, outputFolder);

                        finalGameSheet.Save(Path.Combine(outputFolder, gameSheetFiles[i] + ".png"));
                        File.WriteAllLines(Path.Combine(outputFolder, gameSheetFiles[i] + ".plist"), plistFile);

                        updateFileProgressEvent?.Invoke(this, 100);
                  }
            }

            PackingRectangle[] getPackingRects(ref Sprite[] sprites, out PackingRectangle bounds) {

                  PackingRectangle[] rects = new PackingRectangle[sprites.Length];

                  for (int j = 0; j < sprites.Length; j++) {
                        rects[j] = new PackingRectangle(0, 0, (uint)sprites[j].textureRect.Width, (uint)sprites[j].textureRect.Height, j);
                  }
                  RectanglePacker.Pack(rects, out bounds, PackingHints.TryByArea, 1, 2);
                  Array.Sort(rects, (x, y) => x.Id.CompareTo(y.Id));

                  return rects;
            }

            void backupUnalteredFiles() {
                  int matches = 0;
                  string[] iconsFileNames = getAllIconsGameFilesFromGameDir();
                  
                  // Check if the backup folder contains the aount of files we would expect
                  // This will be modified later to check the names, but it's good enough for now
                  if (Directory.Exists(localUnalteredIconsFolder) == true) {
                        int foundFiles = Directory.GetFiles(localUnalteredIconsFolder).Length;
                        // Multiply by 2 to accounr for both the .png and the .plist files too
                        int expectedFiles = iconsFileNames.Length * 2;

                        if (foundFiles == expectedFiles)
                              matches++;
                  }

                  string[] resourcesFileNames = getAllResourceGameFilesFromGameDir();

                  if (Directory.Exists(localUnalteredResourcesFolder) == true) {
                        int foundFiles = Directory.GetFiles(localUnalteredResourcesFolder).Length;
                        // Multiply by 2 to accounr for both the .png and the .plist files too
                        int expectedFiles = resourcesFileNames.Length * 2;

                        if (foundFiles == expectedFiles)
                              matches++;
                  }

                  if (matches == 2)
                        return;

                  Directory.CreateDirectory(localUnalteredIconsFolder);
                  Directory.CreateDirectory(localUnalteredResourcesFolder);

                  for (int i = 0; i < iconsFileNames.Length; i++) {
                        string fileName = Path.GetFileName(iconsFileNames[i]);
                        string source = Path.Combine(gameIconsFolder, fileName);
                        string dest = Path.Combine(localUnalteredIconsFolder, fileName);

                        if (mainUpdateTimer.ElapsedMilliseconds > mainPrintDelta) {
                              int ProgressPercent = (int)Math.Ceiling((float)i / iconsFileNames.Length * 100);
                              updateEvent?.Invoke(this, new ProgressUpdate(fileName, ProgressPercent, Stage.BackingUp));

                              mainUpdateTimer.Restart();
                        }

                        if (File.Exists(dest + ".plist") == false)
                              File.Copy(source + ".plist", dest + ".plist");

                        if (File.Exists(dest + ".png") == false)
                              File.Copy(source + ".png", dest + ".png");
                  }

                  for (int i = 0; i < resourcesFileNames.Length; i++) {
                        string fileName = Path.GetFileName(resourcesFileNames[i]);
                        string source = Path.Combine(gameResourcesFolder, fileName);
                        string dest = Path.Combine(localUnalteredResourcesFolder, fileName);

                        if (mainUpdateTimer.ElapsedMilliseconds > mainPrintDelta) {
                              int ProgressPercent = (int)Math.Ceiling((float)i / iconsFileNames.Length * 100);
                              updateEvent?.Invoke(this, new ProgressUpdate(fileName, ProgressPercent, Stage.BackingUp));

                              mainUpdateTimer.Restart();
                        }

                        if (File.Exists(dest + ".plist") == false)
                              File.Copy(source + ".plist", dest + ".plist");

                        if (File.Exists(dest + ".png") == false)
                              File.Copy(source + ".png", dest + ".png");
                  }
            }

            void extractGameFiles() {
                  string[] files = getAllGameFiles();

                  for (int i = 0; i < files.Length; i++) {
                        if (mainUpdateTimer.ElapsedMilliseconds > mainPrintDelta) {
                              int ProgressPercent = (int)Math.Ceiling((float)i / files.Length * 100);
                              updateEvent?.Invoke(this, new ProgressUpdate(Path.GetFileName(files[i]), ProgressPercent, Stage.Unpacking));

                              mainUpdateTimer.Restart();
                        }

                        spriteList.AddRange(getAllSpritesFromGameFile(files[i]));
                  }
                  updateFileProgressEvent?.Invoke(this, 0);
            }

            string[] getAllGameFilesFromGameDir() {
                  List<string> files = new List<string>();

                  files.AddRange(getAllIconsGameFilesFromGameDir());
                  files.AddRange(getAllResourceGameFilesFromGameDir());

                  files.Sort();
                  return files.ToArray();
            }

            string[] getAllGameFiles() {
                  List<string> files = new List<string>();

                  files.AddRange(getAllIconsGameFiles());
                  files.AddRange(getAllResourceGameFiles());

                  files.Sort();
                  return files.ToArray();
            }

            string[] getAllIconsGameFiles() {
                  return Directory.GetFiles(localUnalteredIconsFolder).filterByQuality(Config.quality).blacklistFilter();
            }

            string[] getAllResourceGameFiles() {
                  return Directory.GetFiles(localUnalteredResourcesFolder).filterByQuality(Config.quality).blacklistFilter();
            }

            string[] getAllIconsGameFilesFromGameDir() {
                  return Directory.GetFiles(gameIconsFolder).filterByQuality(Config.quality).blacklistFilter();
            }

            string[] getAllResourceGameFilesFromGameDir() {
                  return Directory.GetFiles(gameResourcesFolder).filterByQuality(Config.quality).blacklistFilter();
            }

            public void CacheGameFiles() {
                  if (Config.caching == false)
                        return;

                  Directory.CreateDirectory(Path.Combine(currentCachedQualityFolder, resourcesFolderName));
                  Directory.CreateDirectory(Path.Combine(currentCachedQualityFolder, iconsFolderName));
                  Directory.CreateDirectory(Path.Combine(currentCachedQualityFolder, sfxFolderName));

                  updateEvent?.Invoke(this, new ProgressUpdate("", 0, Stage.Caching));
                  changeDisplayedTextEvent?.Invoke(this, "Caching files...");

                  // Create all folders for all icon gamesheets
                  string[] iconFolders = spriteList
                        .Where(s => s.type == Sprite.Type.Icon)
                        .Select(s => s.sourceFile)
                        .Distinct()
                        .ToArray();

                  string[] resourceFolders = spriteList
                        .Where(s => s.type != Sprite.Type.Icon)
                        .Select(s => s.sourceFile)
                        .Distinct()
                        .ToArray();

                  changeDisplayedTextEvent?.Invoke(this, "Creating folders for cached files...");

                  for (int i = 0; i < iconFolders.Length; i++)
                        Directory.CreateDirectory(Path.Combine(currentCachedQualityFolder, iconsFolderName, iconFolders[i]));

                  for (int i = 0; i < resourceFolders.Length; i++)
                        Directory.CreateDirectory(Path.Combine(currentCachedQualityFolder, resourcesFolderName, resourceFolders[i]));

                  changeDisplayedTextEvent?.Invoke(this, "Backing up files...");

                  for (int i = 0; i < spriteList.Count; i++) {

                        int progressPercent = (int)Math.Ceiling((float)i / spriteList.Count * 100);
                        updateTotalProgressEvent?.Invoke(this, progressPercent);

                        string fileName = string.Empty;
                        if (spriteList[i].type == Sprite.Type.Icon) {
                              fileName = Path.Combine(currentCachedQualityFolder, iconsFolderName, spriteList[i].sourceFile, spriteList[i].spriteName);
                        } else {
                              fileName = Path.Combine(currentCachedQualityFolder, resourcesFolderName, spriteList[i].sourceFile, spriteList[i].spriteName);
                        }
                        spriteList[i].texture.Save(fileName);
                  }

                  JsonSerializerOptions options = new JsonSerializerOptions();
                  options.WriteIndented = true;
                  string outStream = JsonSerializer.Serialize(spriteList, options);

                  File.WriteAllText(Path.Combine(currentCachedQualityFolder, localCachedTexturesJson), outStream);
            }

            public List<Sprite> getAllSpritesOfType(Sprite.Type type) {
                  return spriteList.Where(s => s.type == type).ToList();
            }

            List<Sprite> getAllSpritesFromGameFile(string path) {
                  string textFile = path + ".plist";
                  string imageFile = path + ".png";
                  
                  if (!File.Exists(textFile) || !File.Exists(imageFile)) {
                        return new List<Sprite>();
                  }

                  string[] data = Extensions.ReadTextFile(textFile);
                  Bitmap gamesheet = new Bitmap(imageFile);

                  List<Sprite> sprites = Plist.BulkDeserialise(data);

                  int subdivideSize = 500;

                  // If the gamesheet is over a certain size, slice it to make the unpacking faster
                  if (gamesheet.Width < (subdivideSize * 1.5f) || gamesheet.Height < (subdivideSize * 1.5f)) {
                        extractSprites(ref sprites, path, gamesheet);

                  } else {
                        extractSprites(ref sprites, path, gamesheet.Subdivide(subdivideSize), subdivideSize);
                  }

                  for (int i = 0; i < sprites.Count; i++) {
                        sprites[i].AssignType();
                  }

                  return sprites;
            }

            List<Sprite> extractSprites(ref List<Sprite> sprites, string path, Bitmap gamesheet) {

                  for (int i = 0; i < sprites.Count; i++) {

                        if (secondaryUpdateTimer.ElapsedMilliseconds > secondaryPrintDelta) {
                              int progressPercent = (int)Math.Ceiling((float)i / sprites.Count * 100);
                              updateFileProgressEvent?.Invoke(this, progressPercent);

                              secondaryUpdateTimer.Restart();
                        }

                        sprites[i].sourceFile = Path.GetFileName(path).RemoveExtension();
                        sprites[i].texture = gamesheet.cropImage(sprites[i].textureRect);
                  }
                  return sprites;
            }

            List<Sprite> extractSprites(ref List<Sprite> sprites, string path, Bitmap[,] subsheets, int subdivideSize) {

                  int horizontalSlices = subsheets.GetLength(0);
                  int verticalSlices = subsheets.GetLength(1);

                  for (int i = 0; i < sprites.Count; i++) {
                        if (secondaryUpdateTimer.ElapsedMilliseconds > secondaryPrintDelta) {
                              int progressPercent = (int)Math.Ceiling((float)i / sprites.Count * 100);
                              updateFileProgressEvent?.Invoke(this, progressPercent);

                              secondaryUpdateTimer.Restart();
                        }

                        sprites[i].sourceFile = Path.GetFileName(path).RemoveExtension();

                        int column = sprites[i].textureRect.X / subdivideSize;
                        int row = sprites[i].textureRect.Y / subdivideSize;

                        // Modulo is important to make sure the coordinates are inside of the subsheet
                        Point point = new Point(sprites[i].textureRect.X % subdivideSize, sprites[i].textureRect.Y % subdivideSize);
                        Size size = new Size(sprites[i].textureRect.Width, sprites[i].textureRect.Height);

                        Rectangle cropRect = new Rectangle(point, size);

                        Bitmap texture = new Bitmap(sprites[i].spriteSize.X, sprites[i].spriteSize.Y);

                        if (point.X + size.Width <= subdivideSize && point.Y + size.Height <= subdivideSize) {
                              // Crop the image from a subsheet, since it's entirely within it
                              sprites[i].texture = subsheets[column, row].cropImage(cropRect); // Crashes sometimes, debugging necessary

                        } else {
                              // Cropping from the subsheets. Way faster than copying from the big gamesheet
                              sprites[i].texture = getSpriteFromMultipleSubsheets(sprites[i].textureRect, subsheets, subdivideSize);
                        }
                  }
                  updateFileProgressEvent?.Invoke(this, 100);
                  return sprites;
            }

            Bitmap getSpriteFromMultipleSubsheets(Rectangle cropRect, Bitmap[,] subsheets, int subdivideSize) {
                  int imageStartX = cropRect.X;
                  int imageEndX = imageStartX + cropRect.Width;

                  int imageStartY = cropRect.Y;
                  int imageEndY = imageStartY + cropRect.Height;

                  int startingColumn = imageStartX / subdivideSize;
                  int startingRow = imageStartY / subdivideSize;

                  int columns = (imageEndX - 1) / subdivideSize - (imageStartX / subdivideSize) + 1;
                  int rows = (imageEndY - 1) / subdivideSize - (imageStartY / subdivideSize) + 1;

                  // Coordinates for cropping the data from the subsheets
                  int[] X_coords = Enumerable.Repeat(0, columns).ToArray();
                  int[] Y_coords = Enumerable.Repeat(0, rows).ToArray();

                  // Set the first crop's parameters
                  X_coords[0] = imageStartX % subdivideSize;

                  // Set the last crop's parameters
                  Y_coords[0] = imageStartY % subdivideSize;

                  // Make the default values equal to "subdivideSize" 
                  int[] columnWidths = Enumerable.Repeat(subdivideSize, columns).ToArray();
                  int[] rowHeights = Enumerable.Repeat(subdivideSize, rows).ToArray();

                  if (columnWidths.Length > 1) {
                        columnWidths[0] = subdivideSize - (imageStartX % subdivideSize);
                        columnWidths[columns - 1] = imageEndX % subdivideSize;

                        // Set the width to the size of the subsheet to avoid making it 0
                        if (columnWidths[columns - 1] == 0)
                              columnWidths[columns - 1] = subdivideSize;

                  } else {
                        columnWidths[0] = cropRect.Width;
                  }

                  if (rowHeights.Length > 1) {
                        rowHeights[0] = subdivideSize - (imageStartY % subdivideSize);
                        rowHeights[rows - 1] = imageEndY % subdivideSize;

                        // Set the height to the size of the subsheet to avoid making it 0
                        if (rowHeights[rows - 1] == 0)
                              rowHeights[rows - 1] = subdivideSize;

                  } else {
                        rowHeights[0] = cropRect.Height;
                  }

                  Bitmap texture = new Bitmap(cropRect.Width, cropRect.Height);

                  // X and Y offsets are for keeping track of where the image should be printed onto the compiled texture
                  int Y_offset = 0;
                  for (int y = startingRow; y < startingRow + rows; y++) {

                        int X_offset = 0;
                        for (int x = startingColumn; x < startingColumn + columns; x++) {
                              Point point = new Point(X_coords[x - startingColumn], Y_coords[y - startingRow]);
                              Size size = new Size(columnWidths[x - startingColumn], rowHeights[y - startingRow]);

                              Rectangle crop = new Rectangle(point, size);
                              Bitmap fragment = subsheets[x, y].cropImage(crop);
                              texture.CopyTo(fragment, X_offset, Y_offset);

                              X_offset += columnWidths[x - startingColumn];
                        }
                        Y_offset += rowHeights[y - startingRow];
                  }

                  return texture;
            }
      }
}
