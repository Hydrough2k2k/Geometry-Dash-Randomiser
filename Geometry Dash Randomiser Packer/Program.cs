namespace Geometry_Dash_Randomiser_Packer {

      internal class Program {

            // Before running, make sure that the "version" in "GDR_Form_Intrinsics.cs" matches the one listed below
            // Make sure to set the folder to the debug or release folder, depending on what makes sense when using this
            static void Main(string[] args) {
                  string GDR_Version = string.Empty;
                  string[] GDR_MainFormFile = File.ReadAllLines("G:\\Coding Stuff\\Projects\\C#\\Winforms Projects\\Geometry-Dash-Randomiser\\Geometry Dash Randomiser\\Forms\\GDR_Form\\GDR_Form_Intrinsics.cs");
                  for (int i = 0; i < GDR_MainFormFile.Length; i++) {
                        if (GDR_MainFormFile[i].Contains("version")) {

                              GDR_Version = GDR_MainFormFile[i]
                                    .Substring(GDR_MainFormFile[i].IndexOf("\"") + 1);

                              GDR_Version = GDR_Version.Substring(0, GDR_Version.IndexOf("\""));
                              break;
                        }
                  }

                  Console.WriteLine("Compiling binary file for GDR Version " + GDR_Version + ".");

                  BinaryPacker bp = new BinaryPacker(
                        "G:\\Coding Stuff\\Projects\\C#\\Winforms Projects\\Geometry-Dash-Randomiser\\Geometry Dash Randomiser\\bin\\Debug",
                        "E:\\Releases\\Geometry Dash Randomiser Releases\\Geometry Dash Randomiser " + GDR_Version + "\\dat.bin"
                  );

                  bp.PackFiles();
            }
      }
}
