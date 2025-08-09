using static Geometry_Dash_Randomiser.GameFileManager;

namespace Geometry_Dash_Randomiser {

      public class ProgressState {

            public ProgressState(int totalFiles, int currentFileID, string currentFile) {
                  this.totalFiles = totalFiles;
                  this.currentFileID = currentFileID;
                  this.currentFile = currentFile;
            }

            public ApplicationState currentStage { get; set; } = ApplicationState.Idle;
            public GameFileType currentFileType { get; set; } = GameFileType.None;
            public int totalFiles { get; set; }
            public int currentFileID { get; set; }
            public string currentFile { get; set; }
            public float percentComplete => currentFileID / totalFiles * 100;
      }
}
