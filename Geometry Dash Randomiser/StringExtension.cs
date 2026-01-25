using System;
using System.Text;

namespace Geometry_Dash_Randomiser {

      internal static class StringExtension {

            const string ASCII = numbersAndLetters + symbols;
            const string numbers = "0123456789";
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            const string symbols = "!\"#$%&\'()*+,-./:;<=>?@[\\]^_`{|}~";
            const string numbersAndLetters = numbers + letters + " ";

            public static string AlterRandomCharactersLooped(this string str, int loops) {
                  for (int i = 0; i < loops; i++) {
                        str = str.AlterRandomCharacters(10);
                  }
                  return str;
            }

            public static string AlterRandomCharacters(this string str, int probability = 10) {
                  return str.AlterRandomCharacters(null, probability);
            }

            public static string AlterRandomCharacters(this string str, Random random, int probability) {
                  if (random == null)
                        random = new Random(Guid.NewGuid().GetHashCode());

                  StringBuilder sb = new StringBuilder(str);
                  for (int j = 0; j < str.Length; j++) {
                        if (random.Next(probability) == 0) {
                              sb[j] = numbersAndLetters[random.Next(numbersAndLetters.Length)];
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
