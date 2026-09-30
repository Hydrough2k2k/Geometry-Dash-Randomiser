using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      internal static class ComboBoxExtensions {

            internal static int GetIndexOfItem(this ComboBox cb, string item) {
                  return cb.Items.IndexOf(item);
            }
      }
}
