using System;

namespace Geometry_Dash_Randomiser {

      public static class DateTimeExtensions {

            public static TimeSpan GetElapsedTime(this DateTime dt) {
                  return DateTime.UtcNow - dt;
            }
      }
}
