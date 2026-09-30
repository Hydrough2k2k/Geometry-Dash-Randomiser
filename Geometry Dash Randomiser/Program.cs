using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static Geometry_Dash_Randomiser.Log;

namespace Geometry_Dash_Randomiser {

      internal static class Program {

            [EditorBrowsable(EditorBrowsableState.Never)]
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AllocConsole();

            private static bool consoleAllocated = false;

            /// <summary>
            /// The main entry point for the application.
            /// </summary>
            [STAThread]
            static void Main() {

                  #if DEBUG
                        ForceAllocConsole();
                  #endif

                  // Read config files, allocate the console if required
                  AdvancedConfig.ReadFile();
                  if (AdvancedConfig.Instance.DebugMode) {
                        ForceAllocConsole();
                  }

                  RandomisationConfig.ReadFile();

                  Application.EnableVisualStyles();
                  Application.SetCompatibleTextRenderingDefault(false);
                  GDR_Form mainForm = new GDR_Form();
                  Application.Run(mainForm);

                  // -------------------------------------------------------------------

                  Log.Write(Mode.Info, "Application is exiting...");

                  // Everything below this happens on when the mainForm exits
                  if (mainForm.randomisationThread != null) {
                        // Kill the randomisation thread if it exists
                        mainForm.randomisationThread.Abort();
                  }

                  Log.Write(Mode.Info, "Writing Config files...");

                  // Write the config files
                  RandomisationConfig.Instance.WriteFile();
                  AdvancedConfig.Instance.WriteFile();

                  // Finally close the log file stream to make it accessible to othger applications
                  Log.CloseFileStream();
            }

            public static void ForceAllocConsole() {
                  // Check if the console has been allocated
                  // If it has, don't allocate it. It could lead to bugs I do not feel like debugging
                  if (consoleAllocated)
                        return;

                  AllocConsole();
                  consoleAllocated = true;
            }
      }
}
