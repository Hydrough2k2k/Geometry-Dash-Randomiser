using System;
using System.Diagnostics;

namespace Geometry_Dash_Randomiser {

      public static class TimeExtension {

            // Collector method, might add back a TimeFormat enum to this to make it more customisable
            public static string GetElapsedTimeFormatted(this Stopwatch stopwatch, bool removeEmptyLeadingValues = true) {
                  return FormatTimeFromTicks(TimeSpan.FromTicks(stopwatch.ElapsedTicks), removeEmptyLeadingValues);
            }

            public static string FormatTimeFromTicks(TimeSpan timeSpan, bool removeEmptyLeadingValues) {
                  string ret = string.Empty;
                  if (timeSpan.Days > 0 || removeEmptyLeadingValues == false) {
                        ret += GetDays(timeSpan) + " ";
                  }
                  if (timeSpan.Hours > 0 || removeEmptyLeadingValues == false) {
                        ret += GetHours(timeSpan) + " ";
                  }
                  if (timeSpan.Minutes > 0 || removeEmptyLeadingValues == false) {
                        ret += GetMinutes(timeSpan) + " ";
                  }
                  ret += GetSeconds(timeSpan);
                  return ret;
            }

            private static string GetDays(this TimeSpan timeSpan) {
                  return $"{timeSpan.Days:D1} h";
            }

            private static string GetHours(this TimeSpan timeSpan) {
                  return $"{timeSpan.Hours:D1} h";
            }

            private static string GetMinutes(this TimeSpan timeSpan) {
                  return $"{timeSpan.Minutes:D1} m";
            }

            private static string GetSeconds(this TimeSpan timeSpan, bool includeMillisec = true) {
                  if (includeMillisec) {
                        return $"{timeSpan.Seconds:D1}.{timeSpan.Milliseconds:D3} s";

                  } else {
                        return timeSpan.Seconds > 0 ? $"{timeSpan.Seconds:D1} s" : string.Empty;
                  }
            }
      }
}
