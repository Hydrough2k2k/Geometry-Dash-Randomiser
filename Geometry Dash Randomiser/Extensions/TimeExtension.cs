using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Geometry_Dash_Randomiser {

      public static class TimeExtension {

            // Only partially implemented, but expandable
            public enum TimeFormat {
                  None = 0,
                  Milliseconds = 1,
                  Seconds = 2,
                  Minutes = 4,
                  Hours = 8,
                  Days = 16,

                  DD_HH_MM_SS_MS = Days | Hours | Minutes | Seconds | Milliseconds,
                  HH_MM_SS_MS = Hours | Minutes | Seconds | Milliseconds,
                  HH_MM_SS = Hours | Minutes | Seconds,
                  HH_MM = Hours | Minutes
            }

            public static string GetElapsedTimeFormatted(this Stopwatch stopwatch, TimeFormat format, bool removeEmptyLeadingValues = true) {
                  return FormatTimeFromTicks(TimeSpan.FromTicks(stopwatch.ElapsedTicks), format, removeEmptyLeadingValues);
            }

            public static string FormatTimeFromTicks(TimeSpan ts, TimeFormat format, bool removeEmptyLeadingValues) {
                  List<string> ret = new List<string>();

                  if (format.HasFlag(TimeFormat.Days)) {
                        ret.Add(GetDays(ts));
                  }

                  if (format.HasFlag(TimeFormat.Hours)) {
                        ret.Add(GetHours(ts));
                  }

                  if (format.HasFlag(TimeFormat.Minutes)) {
                        ret.Add(GetMinutes(ts));
                  }

                  if (format.HasFlag(TimeFormat.Seconds)) {
                        ret.Add(GetSeconds(ts, format.HasFlag(TimeFormat.Milliseconds)));
                  }

                  return string.Join(" ", ret).Trim();
            }

            private static string GetDays(TimeSpan ts) {
                  return ts.Days > 0 ? $"{ts.Days:D1} d" : string.Empty;
            }

            private static string GetHours(TimeSpan ts) {
                  return ts.Hours > 0 ? $"{ts.Hours:D1} h" : string.Empty;
            }

            private static string GetMinutes(TimeSpan ts) {
                  return ts.Minutes > 0 ? $"{ts.Minutes:D1} m" : string.Empty;
            }

            private static string GetSeconds(TimeSpan ts, bool includeMillisec) {
                  if (includeMillisec) {
                        return $"{ts.Seconds:D1}.{ts.Milliseconds:D3} s";

                  } else {
                        return $"{ts.Seconds:D1} s";
                  }
            }
      }
}
