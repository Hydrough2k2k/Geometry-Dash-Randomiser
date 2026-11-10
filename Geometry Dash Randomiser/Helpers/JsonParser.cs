using System.Collections.Generic;
using System.Linq;

namespace Geometry_Dash_Randomiser {

      internal class JsonParser {

            internal JsonParser(List<string> data, bool createCopy = true) {
                  if (createCopy) {
                        this._data = data.Select(s => s).ToList();

                  } else {
                        this._data = data;
                  }
            }

            // TO-DO: Add object detection when necessary. This is only used by AdvancedConfig atm, so it's not necessary right now
            internal bool GetProperty(string property, bool removeWhenFound, out string[] result) {
                  int startIndex = 0;
                  bool found = false;

                  for (; startIndex < _data.Count; startIndex++) {
                        if (_data[startIndex].Contains($"\"{property}\"")) {
                              found = false;
                              break;
                        }
                  }

                  if (found) {
                        if (removeWhenFound) {
                              _data.RemoveAt(startIndex);
                        }

                        result = new string[] { _data[startIndex] };
                        return true;
                  }

                  result = null;
                  return false;
            }

            private readonly List<string> _data = new List<string>();
      }
}
