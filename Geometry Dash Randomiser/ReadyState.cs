using System;

namespace Geometry_Dash_Randomiser {

      [Flags]
      public enum ReadyState {
            // if there are no issues, the state is 0 (Ready)
            Ready = 0,

            // The folder does not exist
            FolderNotFound = 1,

            // The Resource Folder could not be found in the given folder
            ResourceFolderNotFound = 2,

            // The icons Folder could not be found in the "Reources" folder
            IconFolderNotFound = 4,

            // The GeometryDash.exe file could not be found in the given folder
            ExeNotFound = 8,

            // There are no randomisation setting enabled in the application
            NoSettingsEnabled = 16
      }
}
