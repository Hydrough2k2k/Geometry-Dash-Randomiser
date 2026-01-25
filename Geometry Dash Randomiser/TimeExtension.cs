using System;
using System.Diagnostics;

namespace Geometry_Dash_Randomiser {

      public static class TimeExtension {

            // Only partially implemented, but expandable
            public enum TimeFormat {
                  none = 0,
                  milliseconds = 1,
                  seconds = 2,
                  minutes = 4,
                  hours = 8,
                  days = 16,

                  HH_MM_SS = hours | minutes | seconds,
                  HH_MM = hours | minutes
            }

            public static string GetElapsedTimeFormatted(this Stopwatch stopwatch, TimeFormat format = TimeFormat.HH_MM_SS, bool removeEmptyLeadingValues = true) {
                  return FormatTimeFromTicks(TimeSpan.FromTicks(stopwatch.ElapsedTicks), format, removeEmptyLeadingValues);
            }

            public static string FormatTimeFromTicks(TimeSpan timeSpan, TimeFormat format, bool removeEmptyLeadingValues) {

                  switch (format) {
                        case TimeFormat.none:
                              throw new ArgumentException("Time format 'none' is not valid for formatting time.", nameof(format));
                        case TimeFormat.milliseconds:
                              return $"{timeSpan.Milliseconds:F3} ms";
                        case TimeFormat.seconds:
                              return $"{timeSpan.Seconds:F3} s";
                        case TimeFormat.minutes:
                              return $"{timeSpan.Minutes:F3} min";
                        case TimeFormat.hours:
                              return $"{timeSpan.Hours:F3} h";
                        case TimeFormat.days:
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
