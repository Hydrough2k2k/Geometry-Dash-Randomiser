using System;

namespace Geometry_Dash_Randomiser {

      public class ConfigUpdateEventArgs : EventArgs {

            public ConfigUpdateEventArgs(string data) {
                  this.configData = data;
            }

            public string configData = string.Empty;
      }
}
