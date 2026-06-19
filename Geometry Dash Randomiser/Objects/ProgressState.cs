namespace Geometry_Dash_Randomiser {

      public class ProgressState {

            public ProgressState(int totalFiles, int completedFiles, string currentFile) {
                  this.TotalFiles = totalFiles;
                  this.CompletedFiles = completedFiles;
                  this.CurrentFile = currentFile;
            }

            public ApplicationState CurrentStage { get; set; } = ApplicationState.Idle;

            public GameFileType CurrentFileType { get; set; } = GameFileType.None;

            public int TotalFiles { get; set; }

            public int CompletedFiles { get; set; }

            public string CurrentFile { get; set; }

            public float PercentComplete {
                  get {
                        if (TotalFiles <= 0)
                              return 0f;

                        if (CompletedFiles == TotalFiles)
                              return 100f;

                        return (float) CompletedFiles / TotalFiles * 100f;
                  }
            }

            public string GetProgressString() {
                  switch (CurrentStage) {
                        case ApplicationState.Idle:
                        case ApplicationState.Setting_Up:
                        case ApplicationState.Randomising:
                        case ApplicationState.Copying_Randomised_Files:
                        case ApplicationState.Finishing_Up:
                              return CurrentStage.ToString().Replace('_', ' ');
                        case ApplicationState.Backing_Up:
                        case ApplicationState.Unpacking:
                        case ApplicationState.Repackaging:
                        case ApplicationState.Restoring:
                             return CurrentStage.ToString().Replace('_', ' ') + " " + CurrentFileType.ToString().Replace('_', ' ').ToLower() + " file: " + CurrentFile;
                        default:
                              return string.Empty;
                  }
            }

            public void NewFileBatch(int filesCount) {
                  TotalFiles = filesCount;
                  CompletedFiles = 0;
            }
      }
}
