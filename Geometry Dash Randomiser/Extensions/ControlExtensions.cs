using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      public static class ControlExtensions {

            public static void Move_X(this Control control, int offset) {
                  control.Location = new System.Drawing.Point(control.Location.X + offset, control.Location.Y);
            }

            public static void Move_Y(this Control control, int offset) {
                  control.Location = new System.Drawing.Point(control.Location.X, control.Location.Y + offset);
            }
      }
}
