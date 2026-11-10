namespace Geometry_Dash_Randomiser {

      public interface IToggleableSetting {
            
            bool Enabled { get; set; }

            int TotalSettingsCount { get; }

            int GetEnabledSettingsCount();

            bool AnySettingEnabled();
      }
}
