using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_Randomiser {

      public static class MathExt {

             public static int Clamp(int value, int min, int max) {
                   if (value < min) return min;
                   if (value > max) return max;
                   return value;
             }
      }
}
