using Geometry_Dash_Randomiser.Forms;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      public partial class GDR_Form : Form {

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AllocConsole();

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern int GetSysColor(int nIndex);

<<<<<<< Updated upstream
            public const string version = "V2.4.2.0";
=======
            public const string version = "V2.5.0.0";
>>>>>>> Stashed changes

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
            private ThemeConfigForm themeConfigForm;

            readonly GameFileManager gameFileManager;

            readonly ThemeController themeController;
            Theme CurrentTheme => themeController.Current;

            DateTime lastThemeRefresh = DateTime.MinValue;
            private int themeRefreshCooldown = 825; // ms

            DateTime lastThemeSwitch = DateTime.MinValue;
            private int themeSwitchCooldown = 500; // ms

            // This is used to prevent the data arrays from being populated multiple times. This will short circuit the method that will populate the arrays
            private bool dataArraysArePopulated = false;

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

            private List<Control> controlsToggledDuringRandomisation = new List<Control>();

            private bool isFormInitialised = false;

<<<<<<< Updated upstream
            private Config config => Config.Instance;
=======
            // ---------------------------------------------------------------------------------

            // Recoloured or redrawn images for the themes and other image based icons
            // These are populated on theme refreshes
            private Bitmap activeStandardBeam = null;
            private Bitmap inactiveStandardBeam = null;

            private Bitmap activeFontBeam = null;
            private Bitmap inactiveFontBeam = null;

            private Bitmap refreshIcon = null;
            private Bitmap settingsIcon = null;

            // ---------------------------------------------------------------------------------

            private RandomisationConfig config => RandomisationConfig.Instance;
            private AdvancedConfig advancedConfig => AdvancedConfig.Instance;
>>>>>>> Stashed changes
      }
}
