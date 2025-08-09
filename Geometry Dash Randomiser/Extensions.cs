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

            public static string[] ReadTextFile(string fileName) {
                  string extension = Path.GetExtension(fileName);
                  return File.ReadAllLines(fileName);
            }

            public static string RemoveNonDigits(this string str) {
                  return Regex.Replace(str, "[^0-9,]+", "", RegexOptions.Compiled);
            }

            public static string FilterDigits(this string str) {
                  return Regex.Replace(str, "[^0-9]+", "", RegexOptions.Compiled);
            }

            public static string GetUtcDateTime() {
                  DateTime dt = DateTime.UtcNow;
                  return dt.Year + "/" + dt.Month + "/" + dt.Day + " " + dt.Hour + ":" + dt.Minute + ":" + dt.Second + "." + dt.Millisecond;
            }

            public static float Area(this Size size) {
                  return size.Width * size.Height;
            }

            public static int Clamp(this int val, int min, int max) {
                  if (val < min) val = min;
                  else if (val > max) val = max;
                  return val;
            }

            public static uint Clamp(this uint val, uint min, uint max) {
                  if (val < min) val = min;
                  else if (val > max) val = max;
                  return val;
            }

            public static float Clamp(this float val, float min, float max) {
                  if (val < min) val = min;
                  else if (val > max) val = max;
                  return val;
            }

            public static double Clamp(this double val, double min, double max) {
                  if (val < min) val = min;
                  else if (val > max) val = max;
                  return val;
            }
      }
}
