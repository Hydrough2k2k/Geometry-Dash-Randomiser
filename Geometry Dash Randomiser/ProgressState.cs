using static Geometry_Dash_Randomiser.GameFileManager;

namespace Geometry_Dash_Randomiser {

      public class ProgressState {

            public ProgressState(int totalFiles, int completedFiles, string currentFile) {
                  this.totalFiles = totalFiles;
                  this.completedFiles = completedFiles;
                  this.currentFile = currentFile;
            }

            public ApplicationState currentStage { get; set; } = ApplicationState.Idle;
            public GameFileType currentFileType { get; set; } = GameFileType.None;
            public int totalFiles { get; set; }
            public int completedFiles { get; set; }
            public string currentFile { get; set; }
            public float percentComplete => completedFiles / totalFiles * 100;

            public string GetProgressString() {
                  switch (currentStage) {
                        case ApplicationState.Idle:
                        case ApplicationState.Setting_Up:
                        case ApplicationState.Backing_Up:
                        case ApplicationState.Randomising:
                        case ApplicationState.Finishing_Up:
                        case ApplicationState.Restoring:
                        case ApplicationState.Complete:
                        case ApplicationState.Unpacking:
                        case ApplicationState.Repackaging:
                              return currentStage.ToString().Replace('_', ' ');
                             //return currentStage.ToString().Replace('_', ' ') + " " + currentFile;
                        default:
                              return string.Empty;
                  }
            }
      }
}
