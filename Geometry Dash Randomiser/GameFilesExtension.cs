using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Geometry_Dash_Randomiser.GameFiles;

namespace Geometry_Dash_Randomiser {

      internal static class GameFilesExtension {

            public static string[] filterByQuality(this string[] files, Quality quality) {

                  string qualityExtension = string.Empty;

                  switch (quality) {
                        case Quality.Medium:
                              qualityExtension = "-hd";
                              break;

                        case Quality.High:
                              qualityExtension = "-uhd";
                              break;

                        default:
                              break;
                  }

                  if (quality == Quality.Low) {
                        return files
                              .Where(f => Path.GetExtension(f) == ".plist")
                              .Select(f => f.RemoveExtension())
                              .Where(f => f.EndsWith("-hd") == false && f.EndsWith("-uhd") == false)
                              .Where(f => File.Exists(f + ".png") && File.Exists(f + ".plist"))
                              .ToArray();
                  }
                  return files
                        .Where(f => Path.GetExtension(f) == ".plist")
                        .Select(f => f.RemoveExtension())
                        .Where(f => f.EndsWith(qualityExtension) == true)
                        .Where(f => File.Exists(f + ".png") && File.Exists(f + ".plist"))
                        .ToArray();
            }

            public static string[] blacklistFilter(this string[] files) {
                  List<string> ret = new List<string>();

                  for (int i = 0; i < files.Length; i++) {
                        bool found = false;
                        for (int j = 0; j < fileBlacklist.Length; j++) {
                              if (Path.GetFileName(files[i]).StartsWith(fileBlacklist[j])) {
                                    found = true;
                                    break;
                              }
                        }
                        if (found == false) {
                              ret.Add(files[i]);
                        }
                  }
                  return ret.ToArray();
            }
      }
}
