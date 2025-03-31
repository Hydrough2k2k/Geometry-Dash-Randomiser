
namespace Geometry_Dash_Randomiser {

      internal class IconRandSettings {

            public bool enabled { get; set; } = true;
            public int group { get; set; } = 1;

            public RandSetting cube { get; set; } = new RandSetting(1, true);
            public RandSetting ship { get; set; } = new RandSetting(1, true);
            public RandSetting ball { get; set; } = new RandSetting(1, true);
            public RandSetting ufo { get; set; } = new RandSetting(1, true);
            public RandSetting wave { get; set; } = new RandSetting(1, true);
            public RandSetting robot { get; set; } = new RandSetting(1, true);
            public RandSetting spider { get; set; } = new RandSetting(1, true);
            public RandSetting swing { get; set; } = new RandSetting(1, true);
            public RandSetting jetpack { get; set; } = new RandSetting(1, true);

            public IconRandSettings() { }
      }
}
