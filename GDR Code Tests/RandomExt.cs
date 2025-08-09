
using System.Drawing;
using System.Runtime.CompilerServices;

namespace GDR_Code_Tests {

      public static class RandomExt {

            public static float NextFloat(float min, float max) {
                  return NextFloat(new Random(Guid.NewGuid().GetHashCode()), min, max);
            }

            public static float NextFloat(this Random random, float min, float max) {
                  return random.NextSingle() * (max - min) + min;
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

            public static void Shuffle<T>(this Random rng, T[] array) {
                  int n = array.Length;
                  while (n > 1) {
                        int k = rng.Next(n--);
                        T temp = array[n];
                        array[n] = array[k];
                        array[k] = temp;
                  }
            }
      }
}
