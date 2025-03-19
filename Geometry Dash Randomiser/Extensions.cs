using System.IO;
using System.Text.RegularExpressions;

namespace Geometry_Dash_Randomiser {

      internal static class Extensions {

            // End is inclusive
            public static string[] Trim(this string[] arr, int start, int end) {
                  string[] ret = new string[end - start + 1];

                  if (arr == null || arr.Length == 0 || start > end) return new string[0];

                  for (int i = start; i <= end; i++) {
                        ret[i - start] = arr[i];
                  }
                  return ret;
            }

            public static string[] ReadTextFile(string fileName) {
                  string extension = Path.GetExtension(fileName);
                  return File.ReadAllLines(fileName);
            }

            public static string RemoveExtension(this string fileName) {
                  int index = fileName.LastIndexOf('.');
                  if (index == -1) return fileName;

                  return fileName.Substring(0, index);
            }

            public static string RemoveNonDigits(this string str) {
                  return Regex.Replace(str, "[^0-9,]+", "", RegexOptions.Compiled);
            }

            public static string GetFirstLine(this string[] str) {
                  if (str.Length == 0) return string.Empty;
                  return str[0];
            }
      }
}
