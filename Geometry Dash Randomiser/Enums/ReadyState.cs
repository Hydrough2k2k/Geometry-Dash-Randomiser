using System;

namespace Geometry_Dash_Randomiser {

      [Flags]
      public enum ReadyState {
            /// <summary>
            /// if there are no issues, the state is 0 (Ready)
            /// </summary>
            Ready = 0,

            /// <summary>
            /// The folder does not exist
            /// </summary>
            FolderNotFound = 1,

            /// The Resource Folder could not be found in the given folder
            ResourceFolderNotFound = 2,

            /// <summary>
            /// The icons Folder could not be found in the "Reources" folder
            /// </summary>
            IconFolderNotFound = 4,

            /// <summary>
            /// The GeometryDash.exe file could not be found in the given folder
            /// </summary>
            ExeNotFound = 8,

            /// <summary>
            /// There is at least one folder or file missing from the given path. Randomisation cannot start until this is resolved
            /// </summary>
            GameFolderPartiallyNotFound = FolderNotFound | ResourceFolderNotFound | IconFolderNotFound | ExeNotFound,

            /// <summary>
            /// There are no randomisation setting enabled in the application
            /// </summary>
            NoSettingsEnabled = 16
      }
}
