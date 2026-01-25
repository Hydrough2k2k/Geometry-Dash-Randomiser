using System;
using System.Text;

namespace Geometry_Dash_Randomiser {

      public class TextCorruptor {

            public int corruptionLoops = 10;
            public int corruptionProbability = 10;

            public TextCorruptor() { }

            public TextCorruptor(int loops, int probability) {
                  corruptionLoops = loops;
                  corruptionProbability = probability;
            }

            public TextCorruptor(int loops) {
                  corruptionLoops = loops;
            }

            public string CorruptText(string text, Random random = null) {
                  if (random == null)
                        random = new Random(Guid.NewGuid().GetHashCode());

                  StringBuilder sb = new StringBuilder(text);

                  for (int l = 0; l < corruptionLoops; l++) {

                        for (int c = 0; c < text.Length; c++) {
                              if (random.Next(corruptionProbability) == 0) {
                                    sb[c] = charSet[random.Next(charSet.Length)];
                              }
                        }
                  }
                  return sb.ToString();
            }

            public string CorruptText(string text) => CorruptText(text, null);

            /// <summary>
            /// Returns a copy of the original array with some altered characters
            /// </summary>
            /// <param name="text">The text you want to be altered</param>
            /// <param name="random">The RNG you want to provide</param>
            /// <returns>The altered text</returns>
            public string[] CorruptText(string[] text, Random random) {
                  string[] ret = new string[text.Length];

                  for (int line = 0; line < text.Length; line++) {
                        ret[line] = CorruptText(text[line], random);
                  }

                  return ret;
            }

            /// <summary>
            /// Returns a copy of the original array with some altered characters
            /// </summary>
            /// <param name="text">The text you want to be altered</param>
            /// <returns>The altered text</returns>
            public string[] CorruptText(string[] text) {
                  string[] ret = new string[text.Length];

                  for (int line = 0; line < text.Length; line++) {
                        ret[line] = CorruptText(text[line]);
                  }

                  return ret;
            }

            const string charSet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ";
      }
}
