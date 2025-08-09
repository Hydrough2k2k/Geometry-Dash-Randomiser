using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Geometry_Dash_Randomiser.GameFilesExtension;
using static Geometry_Dash_Randomiser.GameFileManager;

namespace Geometry_Dash_Randomiser {

      public class FileBlacklist {

            private const string blacklistedFilePath = "blacklisted_files.txt";

            public FileBlacklist() {
                  // If the file doesn't exist, create it with default data
                  if (File.Exists(blacklistedFilePath) == false) {
                        File.WriteAllLines(blacklistedFilePath, blacklist);
                        return;
                  }
                  blacklist = File.ReadAllLines(blacklistedFilePath)
                        .Select(l => l.RemoveExtension().RemoveQualityExtension())
                        .ToArray();
            }

            // Default data
            private readonly string[] blacklist = {
                  "CCControlColourPickerSpriteSheet",
                  "DungeonSheet",
                  "PlayerExplosion_01",
                  "PlayerExplosion_02",
                  "PlayerExplosion_03",
                  "PlayerExplosion_04",
                  "PlayerExplosion_05",
                  "PlayerExplosion_06",
                  "PlayerExplosion_07",
                  "PlayerExplosion_08",
                  "PlayerExplosion_09",
                  "PlayerExplosion_10",
                  "PlayerExplosion_11",
                  "PlayerExplosion_12",
                  "PlayerExplosion_13",
                  "PlayerExplosion_14",
                  "PlayerExplosion_15",
                  "PlayerExplosion_16",
                  "PlayerExplosion_17",
                  "PlayerExplosion_18",
                  "PlayerExplosion_19",
                  "WorldSheet"
            };

            public List<string> FilterBlacklisted(List<string> files) {
                  return FilterBlacklisted(files.ToArray()).ToList();
            }

            public string[] FilterBlacklisted(string[] files) {
                  List<string> ret = new List<string>();

                  for (int i = 0; i < files.Length; i++) {
                        if (isBlacklisted(files[i]) == false)
                              ret.Add(files[i]);
                  }
                  return ret.ToArray();
            }

            public bool isBlacklisted(string fileName) {
                  fileName = fileName.RemoveQualityExtension();

                  for (int i = 0; i < blacklist.Length; i++) {
                        if (fileName == blacklist[i]) {
                              return true;
                        }
                  }
                  return false;
            }
      }
}
