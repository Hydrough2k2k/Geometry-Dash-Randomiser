using System;

namespace Geometry_Dash_Randomiser.Helpers {

      internal static class Base64 {

            internal static string EncodeAndTrim(byte[] bytes) {
                  return Trim(Encode(bytes));
            }

            internal static string Encode(byte[] bytes) {
                  return Convert.ToBase64String(bytes);
            }

            internal static byte[] Decode(string base64EncodedData) {
                  return Convert.FromBase64String(base64EncodedData);
            }

            internal static string Trim(string B64Data) {
                  B64Data = B64Data.TrimEnd('=').TrimEnd('A');

                  int remainder = B64Data.Length % 4;
                  string padding = string.Empty;

                  if (remainder == 1) {
                        padding = "A==";

                  } else if (remainder == 2) {
                        padding = "==";

                  } else if (remainder == 3) {
                        padding = "=";
                  }

                  return B64Data + padding;
            }
      }
}
