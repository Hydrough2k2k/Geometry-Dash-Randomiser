using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_Randomiser_Packer {

      internal class PackedFile {

            internal string filePath = string.Empty;
            internal byte[] data = Array.Empty<byte>();
            internal int fileOffset = 0;

            internal PackedFile(string filePath, byte[] data) {
                  this.filePath = filePath;
                  this.data = data;
            }

            internal int FilePathLength => filePath.Length;
            internal int DataLength => data.Length;
      }
}
