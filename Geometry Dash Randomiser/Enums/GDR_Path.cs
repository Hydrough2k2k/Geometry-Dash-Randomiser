using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_Randomiser {

      public enum GDR_Path {
            /// <summary> Resources folder where the game is installed </summary>
            GameResourcesFolder,

            /// <summary> Icons folder where the game is installed </summary>
            GameIconsFolder,

            /// <summary> Resources folder in the application's folder. This stores the unaltered files </summary>
            BackupResourcesFolder,

            /// <summary> Icons folder in the application's folder. This stores the unaltered files </summary>
            BackupIconsFolder,

            /// <summary> This is where the randomised Resources files will go by default </summary>
            LocalResourcesOutputFolder,

            /// <summary> This is where the randomised Icons files will go by default </summary>
            LocalIconsOutputFolder
      }
}
