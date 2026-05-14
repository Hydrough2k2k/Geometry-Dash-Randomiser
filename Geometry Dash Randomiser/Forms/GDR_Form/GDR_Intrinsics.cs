using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      // This partial class only stores values, references and other intrinsics. No code logic should be here
      public partial class GDR_Form : Form {

            public const string version = "V2.4.1.0";

            private readonly TextCorruptor textCorruptor = new TextCorruptor {
                  CorruptionLevel = 0,
                  ProbabilityPercent = 5d
            };

            private const int UI_UpdateDelay = 25; // ms

            // Form instances for the various sub forms
            private ImportConfigForm importConfigForm;
            private ExportConfigForm exportConfigForm;
            private ChangelogForm changelogForm;
            private CreditsForm creditsForm;

            readonly GameFileManager gameFileManager;

            readonly ThemeController themeController = new ThemeController();
            Theme CurrentTheme => themeController.Current;

            DateTime lastThemeRefresh = DateTime.MinValue;
            private int themeRefreshCooldown = 825; // ms

            // This is used to prevent the data arrays from being populated multiple times. This will short circuit the method that will populate the arrays
            private bool dataArraysArePopulated = false;

            private CheckBox[] iconCheckBoxes = Array.Empty<CheckBox>();
            private NumericUpDown[] iconGroupDisplays = Array.Empty<NumericUpDown>();

            private Label[] labels = Array.Empty<Label>();
            private CheckBox[] checkBoxes = Array.Empty<CheckBox>();
            private Button[] buttons = Array.Empty<Button>();
            private NumericUpDown[] numericUpDowns = Array.Empty<NumericUpDown>();
            private TextBox[] textBoxes = Array.Empty<TextBox>();
            private DomainUpDown[] domainUpDowns = Array.Empty<DomainUpDown>();
            private RichTextBox[] richTextBoxes = Array.Empty<RichTextBox>();
            private GroupBox[] groupBoxes = Array.Empty<GroupBox>();
            private PictureBox[] pictureBoxes = Array.Empty<PictureBox>();
            private RadioButton[] radioButtons = Array.Empty<RadioButton>();

            private Config config => Config.Instance;

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AllocConsole();
      }
}
