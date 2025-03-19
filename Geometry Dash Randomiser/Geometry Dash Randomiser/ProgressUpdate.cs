using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Geometry_Dash_Randomiser.GameFiles;

namespace Geometry_Dash_Randomiser {

      public class ProgressUpdate {

            public ProgressUpdate(string currentFile, int totalPercentComplete, Stage currentStage) {
                  this.currentFile = currentFile;
                  this.totalPercentComplete = totalPercentComplete;
                  this.currentStage = currentStage;
            }

            public string currentFile { get; } = string.Empty;
            public int totalPercentComplete { get; }
            public Stage currentStage { get; }
      }
}
