using System;
using System.Text;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      internal static class StringExtension {

            const string ASCII = " !\"#$%&\'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

            public static string AlterRandomCharacters(this string str, int probability) {
                  return str.AlterRandomCharacters(null, probability);
            }

            public static string AlterRandomCharacters(this string str, Random random, int probability) {
                  if (random == null)
                        random = new Random(Guid.NewGuid().GetHashCode());

                  StringBuilder sb = new StringBuilder(str);
                  for (int j = 0; j < str.Length; j++) {
                        if (random.Next(probability) == 0) {
                              sb[j] = ASCII[random.Next(ASCII.Length)];
                        }
                  }

                  return sb.ToString();
            }

            public static string RemoveExtension(this string fileName) {
                  int index = fileName.LastIndexOf('.');
                  if (index == -1) return fileName;

                  return fileName.Substring(0, index);
            }

            public static bool EndsWith(this string str, params string[] ends) {
                  if (ends == null) return false;

                  for (int i = 0; i < ends.Length; i++) {
                        if (str.EndsWith(ends))
                              return true;
                  }
                  return false;
            }

            public static string SubstringUntil(this string str, int until) {
                  if (str.Length < until)
                        return string.Empty;

                  return str.Substring(0, str.Length - until);
            }
      }
}
