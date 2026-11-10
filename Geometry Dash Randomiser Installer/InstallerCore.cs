using Geometry_Dash_Randomiser_Installer.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_Randomiser_Installer {

      internal static class InstallerCore {

            internal enum InstallFailReason {
                  None = 0,
                  PathIsNullOrEmpty = 1,
                  InstallationHasAlreadyStarted = 2,
                  BlockingReadyState = 3,
            }

            private static string _installationPath;

            private static int _totalFileCount = 0;
            private static int _finishedFilesCount = 0;
            private const string _dataFileName = "dat.bin";

            private static bool _hasStarted = false;

            internal static int TotalFileCount => _totalFileCount;
            internal static int FinishedFilesCount => _finishedFilesCount;
            internal static bool HasStarted => _hasStarted;
            internal static string DataFileName => _dataFileName;

            internal static ReadyState GetReadyState(string destinationPath) {
                  ReadyState readyState = ReadyState.Ready;

                  // C# does not know how to make a funtional "IsNullOrEmpty" method
                  if (destinationPath == null || destinationPath == string.Empty) {
                        return ReadyState.PathIsNullOrEmpty;
                  }

                  // Check if the directory exists
                  if (!Directory.Exists(destinationPath)) {

                        // If it doesn't go out by 1 folder and see if that one exists
                        if (!Directory.Exists(Path.GetDirectoryName(destinationPath))) {
                              readyState |= ReadyState.InvalidDestinationPath;
                        }

                  } else {
                        // If it does exist, checkl if it writable
                        if (!IsFolderWritable(destinationPath)) {
                              readyState |= ReadyState.NotWritableFolder;
                        }

                        // Also check if it empty
                        if (!IsFolderEmpty(destinationPath)) {
                              readyState |= ReadyState.FolderNotEmpty;
                        }
                  }

                  // Search for the required file(s)
                  if (!File.Exists(_dataFileName)) {
                        readyState |= ReadyState.MissingDataFile;
                  }

                  return readyState;
            }

            private static bool IsFolderEmpty(string path) {
                  if (string.IsNullOrEmpty(path)) {
                        throw new ArgumentException("Path cannot be null or empty.", nameof(path));
                  }

                  if (!Directory.Exists(path)) {
                        throw new DirectoryNotFoundException($"The directory '{path}' does not exist.");
                  }

                  IEnumerable<string> items = Directory.EnumerateFileSystemEntries(path);
                  using (IEnumerator<string> en = items.GetEnumerator()) {
                        return !en.MoveNext();
                  }
            }

            private static bool IsFolderWritable(string path) {
                  if (Directory.Exists(path) == false) {
                        path = Path.GetDirectoryName(path);

                        if (Directory.Exists(path) == false) {
                              return false;
                        }
                  }

                  try {
                        string testFilePath = Path.Combine(path, Path.GetRandomFileName());
                        using (FileStream fs = File.Create(testFilePath, 1, FileOptions.DeleteOnClose)) { }
                        return true;
                  }
                  catch (UnauthorizedAccessException) {
                        return false;
                  }
            }

            internal static bool Install(string path, bool startMenuShortcut, bool desktopShortcut, out InstallFailReason failReason) {
                  failReason = InstallFailReason.None;

                  if (!string.IsNullOrEmpty(path)) {
                        failReason |= InstallFailReason.PathIsNullOrEmpty;
                        return false;
                  }

                  if (_hasStarted) {
                        failReason |= InstallFailReason.InstallationHasAlreadyStarted;
                        return false;
                  }
                  _hasStarted = true;

                  ReadyState readyState = GetReadyState(path);
                  if (readyState.IsBlocking()) {
                        failReason |= InstallFailReason.BlockingReadyState;
                        _hasStarted = false;
                        return false;
                  }

                  _installationPath = path;
                  //CreateDirectory(path);



                  return true;
            }

            private static void CreateDirectory(string path) {
                  if (Directory.Exists(path) == false) {
                        Directory.CreateDirectory(path);
                  }
            }
      }
}
