using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Geometry_Dash_Randomiser {

      public class GameSheetManager {

            public GameSheetManager() { }

            public void ReadAllFiles(string path, Quality quality) {
                  if (_fileQuality == AdvancedConfig.Instance.Quality) {
                        ClearGameSheets();
                  }

                  if (_gameSheets.Count != 0) {
                        return;
                  }

                  ExtractResourceFiles(GameFileType.Resource, GDR_Path.BackupResourcesFolder);
                  ExtractResourceFiles(GameFileType.Icon, GDR_Path.BackupIconsFolder);

                  _fileQuality = AdvancedConfig.Instance.Quality;
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

            public GameSheet[] RandomizeFiles(Random random) {
                  GameSheet[] clonedGameSheets = CloneAllGameSheets();

                  // Get a list of randomization settings, and filter for ones that are enabled
                  RandomizationSetting[] randSettings = RandomizationConfig.Instance.GetAllRandSettings().Where(r => r.Enabled).ToArray();

                  int[] groups = RandomizationConfig.Instance.GetDistinctGroups().ToArray();
                  Array.Sort(groups);

                  for (int g = 0; g < groups.Length; g++) {
                        // Skip group 0, as it is a special case
                        if (groups[g] == 0)
                              continue;

                        // Get every sprite we want to shuffle
                        List<ResourceSprite> sprites = GetAllSpritesByGroup(clonedGameSheets, randSettings, groups[g], random);
                        ResourceType[] distinctTypes = sprites.Select(s => s.Type).Distinct().ToArray();

                        Logger.Debug($"Shuffling {sprites.Count} textures in group {groups[g]}. Types: {string.Join(", ", distinctTypes)}");

                        ShuffleSprites(sprites, random);
                  }

                  if (groups[0] == 0) {
                        // Get every sprite we want to shuffle
                        RandomizationSetting[] filteredSettings = randSettings.Where(r => r.Group == 0).ToArray();

                        // Shuffle all settings with a group of 0 separately
                        for (int i = 0; i < filteredSettings.Length; i++) {
                              List<ResourceSprite> sprites = GetSpritesFromRandSetting(clonedGameSheets, filteredSettings[i], random);
                              ResourceType[] distinctTypes = sprites.Select(s => s.Type).Distinct().ToArray();

                              Logger.Debug($"Shuffling {sprites.Count} textures in group {0}. Types: {string.Join(", ", distinctTypes)}");

                              ShuffleSprites(sprites, random);
                        }
                  }

                  clonedGameSheets.ToList().ForEach(g => g.Repack());

                  return clonedGameSheets;
            }

            private List<ResourceSprite> GetAllSpritesByGroup(GameSheet[] gameSheets, RandomizationSetting[] randSettings, int group, Random random) {
                  // Get every randSetting that has the group we are looking for
                  RandomizationSetting[] filteredSettings = randSettings.Where(r => r.Group == group).ToArray();

                  // Get every sprite we want to shuffle
                  return GetSpritesFromRandSettings(gameSheets, filteredSettings, random);
            }

            private List<ResourceSprite> GetSpritesFromRandSettings(GameSheet[] gameSheets, RandomizationSetting[] randSettings, Random random) {
                  List<ResourceSprite> sprites = new List<ResourceSprite>();

                  for (int r = 0; r < randSettings.Length; r++) {
                        sprites.AddRange(GetSpritesFromRandSetting(gameSheets, randSettings[r], random));
                  }

                  return sprites;
            }

            private List<ResourceSprite> GetSpritesFromRandSetting(GameSheet[] gameSheets, RandomizationSetting randSettings, Random random) {
                  return GetSpritesByType(gameSheets, randSettings.AssociatedType, randSettings.PercentageRandomised, random);
            }

            private List<ResourceSprite> GetSpritesByType(GameSheet[] gameSheets, ResourceType type, int probability, Random random) {
                  List<ResourceSprite> sprites = new List<ResourceSprite>();
                  
                  for (int i = 0; i < gameSheets.Length; i++) {
                        sprites.AddRange(gameSheets[i].GetSpritesByType(type, probability, random));
                  }

                  return sprites;
            }

            private void ShuffleSprites(List<ResourceSprite> sprites, Random random) {
                  int[] newSpriteOrder = GetNewSpriteOrderByArea(sprites.Select(s => (float)s.Area).ToArray(), random);

                  ResourceSprite[] spriteCopies = sprites.Select(s => s.DeepCopy()).ToArray();

                  for (int i = 0; i < sprites.Count; i++) {
                        int copyIndex = newSpriteOrder[i];
                        sprites[i].CopyPropertiesFrom(spriteCopies[copyIndex]);
                  }
            }

            // This needs a rework, but later
            int[] GetNewSpriteOrderByArea(float[] spriteAreas, Random random) {
                  bool[] shuffled = new bool[spriteAreas.Length];
                  int[] newSpriteOrder = new int[spriteAreas.Length];

                  for (int i = 0; i < spriteAreas.Length; i++) {
                        float maxArea = spriteAreas[i] * RandomizationConfig.Instance.MaxSpriteMultiplier;
                        float minArea = spriteAreas[i] * (1 / RandomizationConfig.Instance.MaxSpriteMultiplier);

                        int[] candidateSprites = spriteAreas
                              .Select((area, index) => new { index, area })
                              .Where(sprite => sprite.area >= minArea && sprite.area <= maxArea && shuffled[sprite.index] == false)
                              .Select(sprite => sprite.index)
                              .ToArray();

                        int newSpriteIndex = i;

                        if (candidateSprites.Length != 0) {
                              int randomInt = random.Next(candidateSprites.Length);
                              newSpriteIndex = candidateSprites[randomInt];
                        }

                        newSpriteOrder[i] = newSpriteIndex;

                        if (RandomizationConfig.Instance.AllowDuplicates == false)
                              shuffled[newSpriteIndex] = true;
                  }
                  return newSpriteOrder;
            }

            public void RepackGameSheets() {
                  _gameSheets.ToList().ForEach(g => g.Repack());
            }

            public static void WriteFilesToDisk(GameSheet[] gameSheets) {
                  WriteResourceFilesToDisk(gameSheets);
                  WriteIconFilesToDisk(gameSheets);
            }

            private static void WriteResourceFilesToDisk(GameSheet[] gameSheets) {
                  // Get the output path based on if AutoOverwriteFiles is enabled
                  // If it's enabled, the new files will go directly to the game's folder, otherwise the local randomized files folder
                  string outputPath = PathManager.LocalResourcesOutputFolder;
                  if (AdvancedConfig.Instance.AutoOverwriteFiles) {
                        outputPath = PathManager.GameResourcesFolder;
                  }

                  WriteFilesToDisk(gameSheets
                        .Where(g => g.FileType == GameFileType.Resource).ToArray(),
                        outputPath, GameFileType.Resource);
            }

            private static void WriteIconFilesToDisk(GameSheet[] gameSheets) {
                  // Get the output path based on if AutoOverwriteFiles is enabled
                  // If it's enabled, the new files will go directly to the game's folder, otherwise the local randomized files folder
                  string outputPath = PathManager.LocalIconsOutputFolder;
                  if (AdvancedConfig.Instance.AutoOverwriteFiles) {
                        outputPath = PathManager.GameIconsFolder;
                  }

                  WriteFilesToDisk(gameSheets
                        .Where(g => g.FileType == GameFileType.Icon).ToArray(),
                        outputPath, GameFileType.Icon);
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

                        gameSheetImage.Save(gamesheetFileName);

                        ProgressState.Instance.CompletedFiles++;
                  }
            }

            private void ClearGameSheets() {
                  for (int i = 0; i < _gameSheets.Count; i++) {
                        _gameSheets[i].Dispose();
                  }
                  _gameSheets = new List<GameSheet>();
            }

            private GameSheet[] CloneAllGameSheets() {
                  return _gameSheets.Select(g => g.DeepCopy()).ToArray();
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
            private Quality _fileQuality = Quality.High;
      }
}
