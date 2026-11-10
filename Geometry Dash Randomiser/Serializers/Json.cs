using System.Text.Json;

namespace Geometry_Dash_Randomiser {

      public static class Json {

            public static string Serialise(object obj) {
                  return JsonSerializer.Serialize(obj);
            }
      }
}
