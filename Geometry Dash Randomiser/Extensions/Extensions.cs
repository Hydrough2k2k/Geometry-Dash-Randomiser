using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace Geometry_Dash_Randomiser {

      internal static class Extensions {

            // End is inclusive
            public static T[] Trim<T>(this T[] arr, int start, int end) {
                  T[] ret = new T[end - start + 1];

                  if (arr == null || arr.Length == 0 || start > end) return new T[0];

                  for (int i = start; i <= end; i++) {
                        ret[i - start] = arr[i];
                  }
                  return ret;
            }

            public static string FilterDigits(this string str) {
                  return Regex.Replace(str, "[^0-9]+", "", RegexOptions.Compiled);
            }
      }
}
