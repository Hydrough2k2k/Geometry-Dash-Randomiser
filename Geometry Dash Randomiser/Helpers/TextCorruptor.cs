using System;
using System.Text;

namespace Geometry_Dash_Randomiser {

      public class TextCorruptor {

            private double _probability = 0.075f;

            public TextCorruptor() { }

            public int CorruptionLevel { get; set; } = 10;

            public double ProbabilityPercent {
                  get => _probability * 100d;
                  set => _probability = value / 100d;
            }

            public string[] CorruptText(string[] text, int loops) => CorruptText(text, loops, null);

            public string[] CorruptText(string[] text) => CorruptText(text, CorruptionLevel,  null);

            /// <summary>
            /// Returns a copy of the original array with some altered characters
            /// </summary>
            /// <param name="text">The text you want to be altered</param>
            /// <param name="random">The RNG you want to provide</param>
            /// <returns>The altered text</returns>
            public string[] CorruptText(string[] text, int loops, Random random) {
                  string[] ret = new string[text.Length];

                  for (int line = 0; line < text.Length; line++) {
                        ret[line] = CorruptText(text[line], loops, random);
                  }

                  return ret;
            }

            public string CorruptText(string text, int loops) => CorruptText(text, loops, null);
            public string CorruptText(string text) => CorruptText(text, CorruptionLevel, null);

            public string CorruptText(string text, int loops, Random random) {

                  return CorruptText(new StringBuilder(text), loops, random);
            }

            public string CorruptText(StringBuilder sb, int loops) => CorruptText(sb, loops, null);
            public string CorruptText(StringBuilder sb) => CorruptText(sb, CorruptionLevel, null);

            public string CorruptText(StringBuilder sb, int loops, Random random) {
                  CorruptStringBuilder(sb, loops, random);
                  return sb.ToString();
            }

            public void CorruptStringBuilder(StringBuilder sb, int loops, Random random) {
                  if (random == null) {
                        random = new Random(Guid.NewGuid().GetHashCode());
                  }

                  for (int l = 0; l < loops; l++) {

                        for (int c = 0; c < sb.Length; c++) {
                              if (random.NextDouble() <= _probability) {
                                    sb[c] = charSet[random.Next(charSet.Length)];
                              }
                        }
                  }
            }

            const string charSet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz               !\"#$%^&\'*+,-*:;<=>?@_`~";
      }
}
