using System;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      internal static class TextBoxExtensions {

            internal static bool ParseNumericValue(this TextBox textBox, out Int32 value) {
                  string s = Regex.FilterDigits(textBox.Text);
                  return Int32.TryParse(s, out value);
            }
      }
}
