
namespace Geometry_Dash_Randomiser {

      public class IconRandSettings {

            public bool enabled { get; set; } = true;
            public int group { get; set; } = 1;

            public RandomisationSetting cube { get; set; } = new RandomisationSetting(1, true);
            public RandomisationSetting ship { get; set; } = new RandomisationSetting(1, true);
            public RandomisationSetting ball { get; set; } = new RandomisationSetting(1, true);
            public RandomisationSetting ufo { get; set; } = new RandomisationSetting(1, true);
            public RandomisationSetting wave { get; set; } = new RandomisationSetting(1, true);
            public RandomisationSetting robot { get; set; } = new RandomisationSetting(1, true);
            public RandomisationSetting spider { get; set; } = new RandomisationSetting(1, true);
            public RandomisationSetting swing { get; set; } = new RandomisationSetting(1, true);
            public RandomisationSetting jetpack { get; set; } = new RandomisationSetting(1, true);

            public IconRandSettings() { }
      }
}
