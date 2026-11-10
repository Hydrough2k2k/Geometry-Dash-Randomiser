using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_Randomiser {

      public class Configuration {

            public string Name {
                  get => _name;
                  set => _name = value ?? string.Empty;
            }

            public string Data {
                  get => _data;
                  set => _data = value ?? string.Empty;
            }

            public Configuration(string name, string data) {
                  this._name = name;
                  this._data = data;
            }

            private string _name = string.Empty;
            private string _data = string.Empty;
      }
}
