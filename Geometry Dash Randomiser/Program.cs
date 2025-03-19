using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

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

                  Config.WriteFile();
            }
      }
}
