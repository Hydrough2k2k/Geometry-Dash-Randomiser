using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_Randomiser.Helpers {

      internal static class DataStream {

            internal static string Compress(string uncompressedData, CompressionLevel level = CompressionLevel.BestCompression) {
                  string compressedData = Base64.EncodeAndTrim(Zlib.Compress(uncompressedData, level));

                  //      .TrimEnd('=')
                  //      .TrimEnd('A');

                  //int neededEqualsSigns = 4 - (compressedData.Length % 4);
                  //if (neededEqualsSigns < 3) {
                  //      compressedData += new string('=', neededEqualsSigns);
                  //}

                  return compressedData;
            }

            internal static string Decompress(string compressedData) {
                  try {
                        return Zlib.Decompress(Base64.Decode(compressedData));
                  }
                  catch (Exception ex) {
                        Log.Write(Log.Mode.Error, $"Failed to decompress data: {ex.Message}");
                        return string.Empty;
                  }
            }
      }
}
