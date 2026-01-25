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
            public float percentComplete {
                  get {
                        if (totalFiles <= 0) return 0f;
                        return (float) completedFiles / totalFiles * 100f;
                  }
            }

            public string GetProgressString() {
                  switch (currentStage) {
                        case ApplicationState.Idle:
                        case ApplicationState.Setting_Up:
                        case ApplicationState.Randomising:
                        case ApplicationState.Finishing_Up:
                              return currentStage.ToString().Replace('_', ' ');
                        case ApplicationState.Backing_Up:
                        case ApplicationState.Unpacking:
                        case ApplicationState.Repackaging:
                        case ApplicationState.Restoring:
                             return currentStage.ToString().Replace('_', ' ') + " " + currentFileType.ToString().Replace('_', ' ').ToLower() + " file: " + currentFile;
                        default:
                              return string.Empty;
                  }
            }

            public void NewFileBatch(int filesCount) {
                  totalFiles = filesCount;
                  completedFiles = 0;
            }
      }
}
