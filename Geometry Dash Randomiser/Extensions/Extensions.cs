using System;

namespace Geometry_Dash_Randomiser {

      internal static class Extensions {

            // End is inclusive
            internal static T[] Trim<T>(this T[] arr, int start, int end) {
                  T[] ret = new T[end - start + 1];

                  if (arr == null || arr.Length == 0 || start > end) return Array.Empty<T>();

                  for (int i = start; i <= end; i++) {
                        ret[i - start] = arr[i];
                  }
                  return ret;
            }
      }
}
