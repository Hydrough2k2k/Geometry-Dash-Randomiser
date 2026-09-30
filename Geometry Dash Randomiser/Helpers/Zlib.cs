using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_Randomiser.Helpers {

      internal static class Zlib {

            /// <summary>
            /// Compress a string using Zlib
            /// </summary>
            /// <param name="str">The string that will be compressed into the byte array</param>
            /// <returns>The compressed data</returns>
            internal static byte[] Compress(string str, CompressionLevel compressionLevel = CompressionLevel.Level9) {
                  MemoryStream compressedStream = new MemoryStream();
                  ZlibStream zOut;

                  // Compress the JSON string using Zlib
                  zOut = new ZlibStream(compressedStream, CompressionMode.Compress, compressionLevel, true);
                  CopyStream(StringToMemoryStream(str), zOut);
                  zOut.Close();

                  return compressedStream.GetBuffer();
            }

            /// <summary>
            /// Decompress a byte array using Zlib
            /// </summary>
            /// <param name="data">The byte array that will be decompressed into a string</param>
            /// <returns>The decompressed data</returns>
            internal static string Decompress(byte[] data) {
                  try {
                        MemoryStream compressedStream = new MemoryStream(data);

                        // Decompress
                        compressedStream.Seek(0, SeekOrigin.Begin);
                        ZlibStream zOut = new ZlibStream(compressedStream, CompressionMode.Decompress, true);
                        MemoryStream decompressedStream = new MemoryStream();
                        CopyStream(zOut, decompressedStream);
                        zOut.Close();

                        // At this point, decompressedStream contains the decompressed bytes
                        return MemoryStreamToString(decompressedStream);
                  }
                  catch (System.Exception e1) {
                        Console.WriteLine("Exception: " + e1);
                  }

                  return null;
            }

            /// <summary>
            /// Converts a string to a MemoryStream
            /// </summary>
            private static MemoryStream StringToMemoryStream(string s) {
                  byte[] a = Encoding.ASCII.GetBytes(s);
                  return new MemoryStream(a);
            }

            /// <summary>
            /// Converts a MemoryStream to a string. Makes some assumptions about the content of the stream.
            /// </summary>
            /// <param name="mStream"></param>
            /// <returns></returns>
            private static string MemoryStreamToString(MemoryStream mStream) {
                  byte[] ByteArray = mStream.ToArray();
                  return Encoding.ASCII.GetString(ByteArray);
            }

            /// <summary>
            /// Copies over the data from one stream to another in chunks of 1KB
            /// </summary>
            /// <param name="src">The source stream</param>
            /// <param name="dest">The destination stream</param>
            private static void CopyStream(Stream src, Stream dest) {
                  byte[] buffer = new byte[1024];
                  int len;
                  while ((len = src.Read(buffer, 0, buffer.Length)) > 0)
                        dest.Write(buffer, 0, len);
                  dest.Flush();
            }
      }
}
