using System;
using System.Linq;

namespace Geometry_Dash_Randomiser_Installer.Enums {

      [Flags]
      internal enum ReadyState {
            Ready =                  0x0000_0000,
            PathIsNullOrEmpty =      0x0000_0001,
            InvalidDestinationPath = 0x0000_0002,
            NotWritableFolder =      0x0000_0004,
            FolderNotEmpty =         0x0000_0008,
            MissingDataFile =        0x0000_0010,
      }

      internal static class ReadyStateExtensions {

            /// <summary>
            /// Gets whether the ready state will block the installation process' start<br/>
            /// Some ready states are not blocking, but will still show a warning to the user<br/>
            /// This method will return true for all ready states that will block the installation process from starting.
            /// </summary>
            /// <param name="state">The passed state</param>
            /// <returns>If the state contains any blocking or critical states</returns>
            internal static bool IsBlocking(this ReadyState state) {
                  return GetBlockingFlag(state) != ReadyState.Ready;
            }

            internal static ReadyState GetBlockingFlag(this ReadyState state) {
                  if (state.HasFlag(ReadyState.PathIsNullOrEmpty))
                        return ReadyState.PathIsNullOrEmpty;

                  if (state.HasFlag(ReadyState.InvalidDestinationPath))
                        return ReadyState.InvalidDestinationPath;

                  if (state.HasFlag(ReadyState.NotWritableFolder))
                        return ReadyState.NotWritableFolder;

                  if (state.HasFlag(ReadyState.MissingDataFile))
                        return ReadyState.MissingDataFile;

                  return ReadyState.Ready;
            }

            internal static bool IsWarning(this ReadyState state) {
                  return GetWarningFlag(state) != ReadyState.Ready;
            }

            internal static ReadyState GetWarningFlag(this ReadyState state) {
                  if (state.HasFlag(ReadyState.FolderNotEmpty))
                        return ReadyState.FolderNotEmpty;

                  return ReadyState.Ready;
            }

            internal static bool IsReady(this ReadyState state) {
                  return state == ReadyState.Ready;
            }

            internal static string GetStateMessageString(this ReadyState state) {
                  if (state == ReadyState.Ready)
                        return "Installation is ready";

                  if (state.HasFlag(ReadyState.PathIsNullOrEmpty))
                        return "The given path is null or empty";

                  if (state.HasFlag(ReadyState.InvalidDestinationPath))
                        return "The given folder does not exist";

                  if (state.HasFlag(ReadyState.NotWritableFolder))
                        return "The given folder cannot be written to";

                  if (state.HasFlag(ReadyState.FolderNotEmpty))
                        return "The given folder is not empty. Installation can begin";

                  if (state.HasFlag(ReadyState.MissingDataFile))
                        return "The data file {InstallerCore.DataFileName} is missing";

                  return "Unrecognised error";
            }
      }
}
