using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser_Installer {

      internal static class Program {

            #if DEBUG
                  [DllImport("kernel32.dll", SetLastError = true)]
                  [return: MarshalAs(UnmanagedType.Bool)]
                  public static extern bool AllocConsole();
            #endif

            /// <summary>
            /// The main entry point for the application.
            /// </summary>
            [STAThread]
            static void Main() {
                  #if DEBUG
                        AllocConsole();
                  #endif

                  Application.EnableVisualStyles();
                  Application.SetCompatibleTextRenderingDefault(false);
                  Application.Run(new Installer());
            }
      }
}
