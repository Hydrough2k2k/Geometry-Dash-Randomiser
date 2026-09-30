using System.Text.Json.Serialization;

namespace Geometry_Dash_Randomiser {

      public interface IToggleableSetting {
            
            bool Enabled { get; set; }

            [JsonIgnore]
            int TotalSettingsCount { get; }

            int GetEnabledSettingsCount();

            bool AnySettingEnabled();
      }
}
