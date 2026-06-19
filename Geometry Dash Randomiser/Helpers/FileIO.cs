using System.IO;
using System.Threading;
using static Geometry_Dash_Randomiser.PathManager;

namespace Geometry_Dash_Randomiser {

      public static class FileIO {

            public static void ReplaceFile(string fileName, string from, string to, bool overwrite = true) {
                  bool fileExists = File.Exists(Path.Combine(from, fileName));
                  if (overwrite == true && fileExists == true) {
                        File.Delete(Path.Combine(from, fileName));

                  } else if (fileExists == false) {
                        File.Copy(Path.Combine(from, fileName), Path.Combine(to, fileName));
                  }
            }

            /// <summary>
            /// Tries to open a file, with a user defined number of attempt and Sleep delay between attempts.
            /// </summary>
            /// <param name="filePath">The full file path to be opened</param>
            /// <param name="fileMode">Required file mode enum value(see MSDN documentation)</param>
            /// <param name="fileAccess">Required file access enum value(see MSDN documentation)</param>
            /// <param name="fileShare">Required file share enum value(see MSDN documentation)</param>
            /// <param name="maximumAttempts">The total number of attempts to make (multiply by attemptWaitMS for the maximum time the function with Try opening the file)</param>
            /// <param name="attemptWaitMS">The delay in Milliseconds between each attempt.</param>
            /// <returns>A valid FileStream object for the opened file, or null if the File could not be opened after the required attempts</returns>
            public static FileStream TryOpen(string filePath, out bool success,
                  FileMode fileMode, FileAccess fileAccess, FileShare fileShare,
                  int maximumAttempts = 10, int attemptWaitMS = 1) {

                  FileStream fs = null;
                  int attempts = 0;

                  // Loop allow multiple attempts
                  while (true) {
                        try {
                              fs = File.Open(filePath, fileMode, fileAccess, fileShare);

                              //If we get here, the File.Open succeeded, so break out of the loop and return the FileStream
                              success = true;
                              break;
                        }
                        catch (IOException) {
                              // IOException is thrown if the file is in use by another process
                              attempts++;
                              if (attempts > maximumAttempts) {
                                    // Too many attempts,cannot Open File, break and return null 
                                    success = false;
                                    break;
                              } else {
                                    Thread.Sleep(attemptWaitMS);
                              }
                        }
                  }
                  // Return the filestream, check success before trying to read it
                  return fs;
            }
      }
}
