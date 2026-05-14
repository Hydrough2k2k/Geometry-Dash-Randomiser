using System;

namespace Geometry_Dash_Randomiser {

      internal static class StringExtension {

            public static string RemoveExtension(this string fileName) {
                  int index = fileName.LastIndexOf('.');
                  if (index == -1) return fileName;

                  return fileName.Substring(0, index);
            }

            public static bool EndsWith(this string str, params string[] ends) {
                  if (ends == null) return false;

                  for (int i = 0; i < ends.Length; i++) {
                        if (str.EndsWith(ends[i]))
                              return true;
                  }
                  return false;
            }

            public static string SubstringUntil(this string str, int index) {
                  if (str.Length < index)
                        return string.Empty;

                  return str.Substring(0, str.Length - index);
            }

            public static bool ContainsAny(this string str, string[] contains) {
                  if (contains == null)
                        throw new Exception("Array cannot be null");

                  for (int i = 0; i < contains.Length; i++) {
                        if (str.Contains(contains[i])) {
                              return true;
                        }
                  }
                  return false;
            }
      }
}
