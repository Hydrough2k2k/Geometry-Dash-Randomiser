using System;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Geometry_Dash_Randomiser {

      public class TextCorruptor {

            private double _probability = 0.10f;

            public TextCorruptor() { }

            public int CorruptionLevel { get; set; } = 10;

            public double ProbabilityPercent {
                  get => _probability * 100d;
                  set => _probability = value / 100d;
            }

            //public uint MaxCorruptionLoops { get; set; }

            public string[] CorruptText(string[] text) => CorruptText(text, null);

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

            public string CorruptText(string text) => CorruptText(text, null);

            public string CorruptText(string text, Random random) {

                  return CorruptText(new StringBuilder(text), random);
            }

            public string CorruptText(StringBuilder sb) => CorruptText(sb, null);

            public string CorruptText(StringBuilder sb, Random random) {
                  CorruptStringBuilder(sb, random);
                  return sb.ToString();
            }

            public void CorruptStringBuilder(StringBuilder sb) => CorruptStringBuilder(sb, null);

            public void CorruptStringBuilder(StringBuilder sb, Random random) {
                  if (random == null) {
                        random = new Random(Guid.NewGuid().GetHashCode());
                  }

                  for (int l = 0; l < CorruptionLevel; l++) {

                        for (int c = 0; c < sb.Length; c++) {
                              if (random.NextDouble() <= _probability) {
                                    sb[c] = charSet[random.Next(charSet.Length)];
                              }
                        }
                  }
            }

            const string charSet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz                         !\"#$%^&\'*+,-*:;<=>?@_`~";
      }
}
