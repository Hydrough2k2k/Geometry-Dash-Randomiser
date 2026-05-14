using System;
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

                  HH_MM_SS = Hours | Minutes | Seconds,
                  HH_MM = Hours | Minutes
            }

            public static string GetElapsedTimeFormatted(this Stopwatch stopwatch, TimeFormat format = TimeFormat.HH_MM_SS, bool removeEmptyLeadingValues = true) {
                  return FormatTimeFromTicks(TimeSpan.FromTicks(stopwatch.ElapsedTicks), format, removeEmptyLeadingValues);
            }

            public static string FormatTimeFromTicks(TimeSpan timeSpan, TimeFormat format, bool removeEmptyLeadingValues) {

                  switch (format) {
                        case TimeFormat.None:
                              throw new ArgumentException("Time format 'none' is not valid for formatting time.", nameof(format));
                        case TimeFormat.Milliseconds:
                              return $"{timeSpan.Milliseconds:F3} ms";
                        case TimeFormat.Seconds:
                              return $"{timeSpan.Seconds:F3} s";
                        case TimeFormat.Minutes:
                              return $"{timeSpan.Minutes:F3} min";
                        case TimeFormat.Hours:
                              return $"{timeSpan.Hours:F3} h";
                        case TimeFormat.Days:
                              return $"{timeSpan.Days:F3} d";

                        case TimeFormat.HH_MM_SS:
                              return FormatHH_MM_SS(timeSpan, removeEmptyLeadingValues);
                        default:
                              return $"{timeSpan.Milliseconds:F3} ms"; // Default to milliseconds
                  }
            }

            private static string FormatHH_MM_SS(this TimeSpan timeSpan, bool removeEmptyLeadingValues) {
                  string ret = string.Empty;
                  if (timeSpan.Hours > 0 || removeEmptyLeadingValues == false) {
                        ret += GetHours(timeSpan, removeEmptyLeadingValues) + " ";
                  }
                  if (timeSpan.Minutes > 0 || removeEmptyLeadingValues == false) {
                        ret += GetMinutes(timeSpan, removeEmptyLeadingValues) + " ";
                  }
                  ret += GetSeconds(timeSpan, removeEmptyLeadingValues);
                  return ret;
            }

            private static string GetHours(this TimeSpan timeSpan, bool emptyIfZero) {
                  return (timeSpan.Hours > 0 || emptyIfZero) ? $"{timeSpan.Hours:D1} h" : string.Empty;
            }

            private static string GetMinutes(this TimeSpan timeSpan, bool emptyIfZero) {
                  return (timeSpan.Minutes > 0 || emptyIfZero) ? $"{timeSpan.Minutes:D1} m" : string.Empty;
            }

            private static string GetSeconds(this TimeSpan timeSpan, bool emptyIfZero) {
                  return (timeSpan.Seconds > 0 || emptyIfZero) ? $"{timeSpan.Seconds:D1} s" : string.Empty;
            }
      }
}
