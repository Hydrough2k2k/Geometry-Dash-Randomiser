namespace Geometry_Dash_Randomiser {

      internal static class ReadyStateExtensions {

            internal static string GetMessageString(this ReadyState ready) {
                  if (ready.HasFlag(ReadyState.FolderNotFound)) {
                        return "The given folder doesn't exist";

                  } else if (ready.HasFlag(ReadyState.ResourceFolderNotFound)) {
                        return "The Resources folder in the game directory can't be found";

                  } else if (ready.HasFlag(ReadyState.IconFolderNotFound)) {
                        return "The Icons folder in the game directory can't be found";

                  } else if (ready.HasFlag(ReadyState.ExeNotFound)) {
                        return "The EXE in the game directory can't be found";

                  } else if (ready.HasFlag(ReadyState.NoSettingsEnabled)) {
                        return "There are no randomisation settings enabled";

                  } else {
                        return "The randomisation can begin";
                  }
            }
      }
}
