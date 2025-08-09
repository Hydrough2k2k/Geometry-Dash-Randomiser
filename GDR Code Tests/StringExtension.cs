using System.Text.RegularExpressions;

namespace GDR_Code_Tests {

      internal static class StringExtension {

            public static string FilterDigits(this string str) {
                  return Regex.Replace(str, "[^0-9-]+", "", RegexOptions.Compiled);
            }

            public static string RemoveExtension(this string fileName) {
                  int index = fileName.LastIndexOf('.');
                  if (index == -1) return fileName;

                  return fileName.Substring(0, index);
            }

            public static List<int> AllIndexesOf(this string str, string value) {
                  if (String.IsNullOrEmpty(value))
                        throw new ArgumentException("the string to find may not be empty", "value");

                  List<int> indexes = new List<int>();
                  for (int index = 0; ; index += value.Length) {
                        index = str.IndexOf(value, index);
                        if (index == -1)
                              return indexes;
                        indexes.Add(index);
                  }
            }

            public static List<int> AllIndexesOf(this string str, char value) {
                  List<int> indexes = new List<int>();
                  for (int index = 0; ; index++) {
                        index = str.IndexOf(value, index);
                        if (index == -1)
                              return indexes;
                        indexes.Add(index);
                  }
            }
      }
}
