using System.IO;
using System.Linq;
using static Geometry_Dash_Randomiser.GameFileManager;
using static Geometry_Dash_Randomiser.PathManager;

namespace Geometry_Dash_Randomiser {

      public class GamesheetManager {

            public GamesheetManager(GameFileManager creator) {
                  gameFileManager = creator;
            }

            GameFileManager gameFileManager;

            // this is missing a bunch of logic that will be moved from GameFileManager.cs to here, but only later


            public string[] GetAllFileNames(GDR_Path source, Quality quality)
                  => GetAllFileNames(PathManager.GetPath(source), quality);

            public string[] GetAllFileNames(string path, Quality quality) {
                  return Directory.GetFiles(path)
                        .Where(f => Path.GetExtension(f) == ".plist")
                        .FilterFilesByQuality(quality)
                        .Where(f => File.Exists(f + ".png") && File.Exists(f + ".plist"))
                        .Select(f => Path.GetFileName(f))
                        .ToArray();
            }
      }
}
