using System.Collections.Generic;
using System.Linq;
using static Geometry_Dash_Randomiser.GameFileManager;

namespace Geometry_Dash_Randomiser {

      public static class GameFilesExtension {

            public static string[] FilterFileExtensions(this string[] files, params string[] extensions) {
                  List<string> ret = new List<string>();
                  for (int i = 0; i < files.Count(); i++) {
                        if (files[i].EndsWith(extensions))
                              ret.Add(files[i]);
                  }
                  return ret.ToArray();
            }

            public static string[] FilterFilesByQuality(this IEnumerable<string> files, Quality quality) {
                  files = files.Select(f => f.RemoveExtension());
                  if (quality == Quality.Low) {
                        return files
                              .Where(f => f.EndsWith(GetQualityExtension(Quality.Medium)) == false
                                       && f.EndsWith(GetQualityExtension(Quality.High)) == false)
                              .ToArray();
                  }
                  return files
                        .Where(f => f.EndsWith(GetQualityExtension(quality)) == true)
                        .ToArray();
            }

            public static string GetQualityExtension(Quality quality) {
                  switch (quality) {
                        case Quality.Medium:
                              return "-hd";
                        case Quality.High:
                              return "-uhd";
                        default:
                              return string.Empty;
                  }
            }

            public static List<string> RemoveQualityExtension(this List<string> str) {
                  return RemoveQualityExtension(str.ToArray()).ToList();
            }

            public static string[] RemoveQualityExtension(this string[] str) {
                  for (int i = 0; i < str.Length; i++) {
                        str[i] = str[i].RemoveQualityExtension();
                  }
                  return str;
            }

            public static string RemoveQualityExtension(this string str) {
                  if (str.EndsWith(GetQualityExtension(Quality.High))) {
                        return str.Substring(0, str.Length - GetQualityExtension(Quality.High).Length);

                  } else if (str.EndsWith(GetQualityExtension(Quality.Medium))) {
                        return str.Substring(0, str.Length - GetQualityExtension(Quality.Medium).Length);
                  }
                  return str;
            }
      }
}
