using System.Drawing;
using static GDR_Code_Tests.FontManager;

namespace GDR_Code_Tests {

      internal class Start {

            static string outputFolder = "Modified Files";

            static void Main() {

                  FontManager fontManager = new FontManager();

                  string[] fontFileNames = fontManager.GetAllFontFileNames();
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
