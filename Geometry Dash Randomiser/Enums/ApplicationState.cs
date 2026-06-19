namespace Geometry_Dash_Randomiser {

      public enum ApplicationState {

            /// <summary>
            /// The application is not doing any work actively, and is waiting for user input
            /// </summary>
            Idle,

            /// <summary>
            /// The application is doing pre-flight checks, creates the necessary folders, etc. before starting the randomisation process
            /// </summary>
            Setting_Up,

            /// <summary>
            /// Copies all of the files that need to be randomised from the Geometry Dash installation folder to the local backup folder
            /// </summary>
            Backing_Up,

            /// <summary>
            /// Grabs all of the .plist, .fnt and .png files, deserialise the first 2, and slices the .png files according to the data stored in the data files
            /// </summary>
            Unpacking,

            /// <summary>
            /// Shuffles all of the sprites around according to the rules the user set
            /// </summary>
            Randomising,

            /// <summary>
            /// Takes all of the shuffled sprites and re-assembles them into .plist, .fnt and .png files, and writes them to a local folder
            /// </summary>
            Repackaging,

            /// <summary>
            /// If the "Auto-Copy Game Files" setting is enabled, the files just written to the local folder are all copied to the Geometry Dash installation folder
            /// </summary>
            Copying_Randomised_Files,

            /// <summary>
            /// Does a few miscellaneous steps when all files have been randomised and conditionally copied
            /// </summary>
            Finishing_Up,

            /// <summary>
            /// Copies all of the unaltered files from the local backup folder to the game's installation folder to make the game normal again
            /// </summary>
            Restoring
      }
}
