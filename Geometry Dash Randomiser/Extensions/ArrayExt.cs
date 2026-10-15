using System;
using System.Collections.Generic;
using System.Linq;

namespace Geometry_Dash_Randomiser {

      internal static class ArrayExt {

            internal static List<int> GetIndexesFrom(int[] source, int[] find) {
                  List<int> ret = new List<int>();

                  for (int i = 0; i < find.Length; i++) {
                        int index = Array.IndexOf(source, find[i]);
                        if (index != -1) {
                              ret.Add(index);
                        }
                  }

                  return ret;
            }

            //internal static int[] DerangementShuffle(int[] source, Random random) {
            //      int[] derangementIndexes = random.GetDerangedShuffleInRange(source.Length);



            //      int[] shuffled = new int[source.Length];



            //}

            internal static int[] RandomSelect(int[] source, Random random, int percentage) {
                  // If the percentage is 100 or more, return a copy of the array. If the copy is altered, the original reference array won't be altered
                  if (percentage >= 100)
                        return source.Select(v => v).ToArray();

                  if (percentage <= 0)
                        return Array.Empty<int>();

                  return source.Where(v => random.Next(1, 100) >= percentage).ToArray();
            }
      }
}
