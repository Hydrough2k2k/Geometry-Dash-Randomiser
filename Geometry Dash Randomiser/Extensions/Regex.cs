using System.Text.RegularExpressions;

namespace Geometry_Dash_Randomiser {

      internal static class Regex {

            internal static string FilterDigits(string s) {
                  return System.Text.RegularExpressions.Regex.Replace(s, "[^0-9]+", "", RegexOptions.Compiled);
            }
      }
}
