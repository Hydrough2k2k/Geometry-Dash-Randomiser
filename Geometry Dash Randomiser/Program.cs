using System;
using System.Windows.Forms;
<<<<<<< Updated upstream
=======
using static Geometry_Dash_Randomiser.Logger;
>>>>>>> Stashed changes

namespace Geometry_Dash_Randomiser {

      internal static class Program {

            /// <summary>
            /// The main entry point for the application.
            /// </summary>
            [STAThread]
            static void Main() {
                  Application.EnableVisualStyles();
                  Application.SetCompatibleTextRenderingDefault(false);
                  Application.Run(new GDR_Form());

<<<<<<< Updated upstream
                  Config.Instance.WriteFile();
                  Log.CloseFileStream();
=======
                  // -------------------------------------------------------------------

                  Logger.Write(Mode.Info, "Application is exiting...");

                  // Everything below this happens on when the mainForm exits
                  if (mainForm.randomisationThread != null) {
                        // Kill the randomisation thread if it exists
                        mainForm.randomisationThread.Abort();
                  }

                  Logger.Write(Mode.Info, "Writing Config files...");

                  // Write the config files
                  RandomisationConfig.Instance.WriteFile();
                  AdvancedConfig.Instance.WriteFile();

                  // Finally close the log file stream to make it accessible to othger applications
                  Logger.CloseFileStream();
>>>>>>> Stashed changes
            }
      }
}
