using System.Drawing;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      public static class ControlExtensions {

            public static void Move_X(this Control control, int offset) {
                  control.Location = new Point(control.Location.X + offset, control.Location.Y);
            }

            public static void Move_Y(this Control control, int offset) {
                  control.Location = new Point(control.Location.X, control.Location.Y + offset);
            }

            public static void RightAlign(this Label left, Control right, int X_offset = 0) {
                  right.Location = new Point(left.Location.X + left.PreferredWidth + X_offset, right.Location.Y);
            }

            public static void RightAlign(this CheckBox left, Control right, int X_offset = -4) {
                  right.Location = new Point(left.Location.X + left.Width + X_offset, right.Location.Y);
            }

            public static void RightAlign(this ComboBox left, Control right, int X_offset = 5) {
                  right.Location = new Point(left.Location.X + left.Width + X_offset, right.Location.Y);
            }

            public static void LeftAlign(this Control right, Label left, int X_offset = 0) {
                  left.Location = new Point(right.Left - left.PreferredWidth, left.Location.Y);
            }
      }
}
