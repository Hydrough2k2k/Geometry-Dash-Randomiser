using System;
using System.Collections.Generic;
using System.IO;

namespace Geometry_Dash_Randomiser_Installer.Helpers {

      internal class FileReader {

            private readonly string _path = string.Empty;
            private readonly int _chunkSize = 1024;

            public FileReader(string fileName, int chunkSize) {
                  this._path = fileName;
                  this._chunkSize = chunkSize;
            }

            public FileReader(string fileName) {
                  this._path = fileName;
            }

            public IEnumerable<byte[]> ReadChunk() {
                  var buffer = new byte[_chunkSize];

                  using (var fs = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read)) {

                        using (BinaryReader br = new BinaryReader(fs)) {

                              while (true) {
                                    int bytesRead = br.Read(buffer, 0, buffer.Length);
                                    if (bytesRead == 0) {
                                          yield break;
                                    }
                                    if (bytesRead < buffer.Length) {
                                          Array.Resize(ref buffer, bytesRead);
                                    }
                                    yield return buffer;
                              }
                        }
                  }
            }


      }
}
