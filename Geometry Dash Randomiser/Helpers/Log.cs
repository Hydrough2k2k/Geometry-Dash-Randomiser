using System;
using System.IO;
using System.Text;

namespace Geometry_Dash_Randomiser {

      public static class Log {

            // Console Settings
            internal static bool EnableColoursForConsoleWrites { get; set; } = true;
            internal static Mode MinimumLevelForConsole { get; set; } = Mode.Verbose;

            // File Settings
            internal static bool MirrorToFile { get; set; } = true;
            internal static Mode MinimumLevelForFile { get; set; } = Mode.Info;

            public enum Mode {
                  Verbose,
                  Debug,
                  Info,
                  Warn,
                  Error,
                  Fatal
            }

            public static void Write(Mode mode, string message) {
                  if (mode >= MinimumLevelForConsole) {
                        PrintMessageToConsole(mode, message);
                  }

                  if (mode >= MinimumLevelForFile) {
                        PrintMessageToFile(mode, message);
                  }
            }

            private static void PrintMessageToConsole(Mode mode, string message) {
                  if (EnableColoursForConsoleWrites) {
                        PrintColouredModeToConsole(mode);

                  } else {
                        PrintModeToConsole(mode);
                  }

                  Console.WriteLine(message);
            }

            private static void PrintColouredModeToConsole(Mode mode) {
                  Console.ForegroundColor = GetConsoleColour(mode);
                  PrintModeToConsole(mode);
                  Console.ForegroundColor = Default;
            }

            private static void PrintModeToConsole(Mode mode) {
                  Console.Write(GetModeText(mode));
            }

            private static void PrintMessageToFile(Mode mode, string message) {
                  if (MirrorToFile == false) {
                        return;
                  }

                  if (LogFileStream == null) {
                        OpenFileStream();
                  }

                  try {
                        string dataString = GetClockTime(':') + " " + GetModeText(mode) + message + "\n";
                        byte[] data = new UTF8Encoding(true).GetBytes(dataString);
                        LogFileStream.Write(data, 0, data.Length);
                  }
                  catch (Exception ex) {
                        Console.WriteLine(ex);
                  }
            }

            private static void OpenFileStream() {
                  if (Directory.Exists("Logs") == false) {
                        Directory.CreateDirectory("Logs");
                  }

                  LogFileStream = new FileStream(GenerateNewLogFilePath(), mode: FileMode.OpenOrCreate);
            }

            /// <summary>
            /// Converts the mode to an upper case string, surrounds it with square brackets.
            /// The padding is necessary to make all of the logs start on the same character in the line both in the file and the console
            /// </summary>
            /// <param name="logMode">The logging mode</param>
            /// <returns>The formatted log mode</returns>
            private static string GetModeText(Mode logMode) {
                  return ("[" + logMode.ToString().ToUpper() + "]").PadRight(10);
            }

            /// <summary>
            /// Generates a new log file path based on the current date time
            /// </summary>
            /// <returns>The formatted log file path</returns>
            private static string GenerateNewLogFilePath() {
                  DateTime dt = DateTime.Now;
                  return Path.Combine(LogFolderName,
                        "GDR " + dt.Year.ToString("0000") +
                        "-" + dt.Month.ToString("00") +
                        "-" + dt.Day.ToString("00") +
                        " " + GetClockTime('.') + ".log");
            }

            /// <summary>
            /// Get the currect time in the day printed
            /// </summary>
            /// <param name="separator">The character that will separate the hours, minutes and seconds</param>
            private static string GetClockTime(char separator) {
                  DateTime dt = DateTime.Now;
                  return dt.Hour.ToString("00") +
                        separator + dt.Minute.ToString("00") +
                        separator + dt.Second.ToString("00");
            }

            /// <summary>
            /// Gets what colour the mode text should be turned into in the console
            /// </summary>
            /// <param name="logMode">The logging mode</param>
            /// <returns>The colour that should be passed to Console.ForegroundColor</returns>
            private static ConsoleColor GetConsoleColour(Mode logMode) {

                  switch (logMode) {
                        case Mode.Verbose:
                              return VerboseMessageColour;

                        case Mode.Debug:
                              return DebugMessageColour;

                        case Mode.Info:
                              return InfoMessageColour;

                        case Mode.Warn:
                              return WarnMessageColour;

                        case Mode.Error:
                              return ErrorMessageColour;

                        case Mode.Fatal:
                              return FatalMessageColour;

                        default:
                              Log.Write(Mode.Warn, $"Could not find a matching colour for the log mode \"{logMode.ToString()}\"");
                              return ConsoleColor.White;
                  }
            }

            internal static void CloseFileStream() {
                  if (LogFileStream == null)
                        return;

                  LogFileStream.Close();
                  LogFileStream.Dispose();
            }

            // Console print colour settings
            private static ConsoleColor Default { get; set; } = ConsoleColor.White;
            private static ConsoleColor VerboseMessageColour { get; set; } = ConsoleColor.White;
            private static ConsoleColor DebugMessageColour { get; set; } = ConsoleColor.Blue;
            private static ConsoleColor InfoMessageColour { get; set; } = ConsoleColor.Cyan;
            private static ConsoleColor WarnMessageColour { get; set; } = ConsoleColor.Yellow;
            private static ConsoleColor ErrorMessageColour { get; set; } = ConsoleColor.Red;
            private static ConsoleColor FatalMessageColour { get; set; } = ConsoleColor.DarkRed;

            private static FileStream LogFileStream { get; set; } = null;
            private const string LogFolderName = "Logs";

            public static class Config {

                  public static void ChangeColourForLogMode(Mode logMode, ConsoleColor colour) {

                        switch (logMode) {
                              case Mode.Verbose:
                                    VerboseMessageColour = colour;
                                    break;

                              case Mode.Debug:
                                    DebugMessageColour = colour;
                                    break;

                              case Mode.Info:
                                    InfoMessageColour = colour;
                                    break;

                              case Mode.Warn:
                                    WarnMessageColour = colour;
                                    break;

                              case Mode.Error:
                                    ErrorMessageColour = colour;
                                    break;

                              case Mode.Fatal:
                                    FatalMessageColour = colour;
                                    break;

                              default:
                                    Log.Write(Mode.Warn, $"The log mode \"{logMode.ToString()}\" does not have a matching colour, or is not implemented.");
                                    break;
                        }
                  }
            }
      }
}
