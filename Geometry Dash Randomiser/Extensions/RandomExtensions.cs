using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Geometry_Dash_Randomiser {

      public static class RandomExtensions {

            public static float NextFloat(float min, float max) {
                  return NextFloat(new Random(Guid.NewGuid().GetHashCode()), min, max);
            }

            public static float NextFloat(this Random random, float min, float max) {
                  return (float)random.NextDouble() * (max - min) + min;
            }

            public static float NextFloat(Random random) {
                  double mantissa = (random.NextDouble() * 2.0) - 1.0;
                  // write (-149, x), instead of (-126, x) to also generate subnormal floats (*)
                  // (-126, 128) will generate positive infinity as well
                  double exponent = Math.Pow(2.0, random.Next(-126, 127));
                  return (float)(mantissa * exponent);
            }

            public static int[] GetShuffledIntRange(this Random random, int max) {
                  if (random == null)
                        random = new Random(Guid.NewGuid().GetHashCode());

                  int[] values = Enumerable.Range(0, max).ToArray();
                  random.Shuffle(values);
                  return values;
            }

            public static int[] GetDerangedShuffleInRange(this Random random, int max) {
                  int[] values = GetShuffledIntRange(random, max);

                  int[] matchingIndexes = Array.Empty<int>();
                  do {
                        matchingIndexes = values
                              .Select((value, index) => new { value, index })
                              .Where(pair => pair.value == pair.index)
                              .Select(pair => pair.index)
                              .ToArray();

                        if (matchingIndexes.Length == 1) {
                              int r = random.Next(values.Length);
                              (values[matchingIndexes[0]], values[r]) = (values[r], values[matchingIndexes[0]]);

                        } else if (matchingIndexes.Length == 2) {
                              (values[matchingIndexes[0]], values[matchingIndexes[1]]) = (values[matchingIndexes[1]], values[matchingIndexes[0]]);

                        } else {
                              for (int i = 0; i < matchingIndexes.Length; i++) {
                                    int index1 = matchingIndexes[random.Next(matchingIndexes.Length)];
                                    int index2 = matchingIndexes[random.Next(matchingIndexes.Length)];

                                    (values[index1], values[index2]) = (values[index2], values[index1]);
                              }
                        }

                  } while (matchingIndexes.Length > 0);

                  return values;
            }

            public static void Shuffle<T>(this Random rng, T[] array) {
                  int index = array.Length;
                  while (index > 1) {
                        int swap = rng.Next(index--);
                        (array[index], array[swap]) = (array[swap], array[index]);
                  }
            }

            public static Color GetRandomRGBColor(this Random random) {
                  return Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
            }
      }
}
