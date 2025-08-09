using System.Drawing;
using static Geometry_Dash_Randomiser.FontManager;
using static Geometry_Dash_Randomiser.Font;

namespace Geometry_Dash_Randomiser {

      public class Program {

            public static string sourceFiles = "C:\\Users\\Hydrough\\Desktop\\GD Files backup";
            public static string outputFolder = "Modified Files";

            public static void Main(string[] args) {

                  FontManager fontManager = new FontManager();

                  string[] fontFileNames = fontManager.GetAllFontFileNames(sourceFiles);
                  fontManager.ReadFontFiles(fontFileNames);
                  RandomisationMode randomisationMode = RandomisationMode.ShuffleEverything | RandomisationMode.ShuffleFontStyles;
                  Font[] randomisedFonts = fontManager.RandomiseFiles(randomisationMode);

                  Directory.CreateDirectory(outputFolder);

                  for (int i = 0; i < randomisedFonts.Length; i++) {
                        string textFileName = Path.Combine(outputFolder, fontManager.fontFileNames[i] + ".fnt");
                        string gamesheetFileName = Path.Combine(outputFolder, fontManager.fontFileNames[i] + ".png");

                        File.WriteAllText(textFileName, randomisedFonts[i].Serialise());
                        Bitmap gamesheet = randomisedFonts[i].AssembleGamesheet();
                        gamesheet.Save(gamesheetFileName);

                        gamesheet.Dispose();
                  }
            }
      }
}
