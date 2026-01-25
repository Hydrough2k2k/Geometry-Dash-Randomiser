using Geometry_Dash_Randomiser.Properties;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Geometry_Dash_Randomiser.Config;

namespace Geometry_Dash_Randomiser {

      public partial class GDR_Form : Form {

            private const string version = "V2.4.0.0";

            private int textCorruptionLevel = 0;
            private const int maxCorruptionLoops = 25;

            private const int UI_UpdateDelay = 25; // milliseconds

            private ImportConfigForm importConfigForm = new ImportConfigForm();
            private ExportConfigForm exportConfigForm = new ExportConfigForm();
            private ChangelogForm changelogForm = new ChangelogForm(version);

            GameFileManager gameFileManager;

            ThemeController themeController = null;

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

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AllocConsole();

            public GDR_Form() {
                  InitializeComponent();

                  #if DEBUG
                  AllocConsole();
                  #endif

                  Config.ReadFile();
                  Config config = Instance;
                  if (config.debugMode) {
                        AllocConsole();
                  }

                  this.gameFileManager = new GameFileManager(this);

                  this.textureQualitySelectorBox.Items.Add(PathManager.highQualityName);
                  this.textureQualitySelectorBox.Items.Add(PathManager.mediumQualityName);
                  this.textureQualitySelectorBox.Items.Add(PathManager.lowQualityName);

                  this.themeController = new ThemeController();

                  if (this.themeController.GetThemeCount() < config.themeID) {
                        Console.WriteLine($"The theme ID [{config.themeID}] is out of range");

                        config.themeID = 0;
                  }

                  this.themeController.activeThemeID = config.themeID;

                  this.applicationThemeSelectorBox.Items.AddRange(this.themeController.GetAllThemeNames());

                  // If the game directory is valid, enable the restore button
                  this.restoreFilesButton.Enabled = gameFileManager.IsGameDirectoryValid();

                  this.versionLabel.Text = version;

                  RefreshUI();

                  this.importConfigForm.ConfigDataChanged += (s, data, e) => {
                        ImportConfigThenRefreshUI(data);
                  };
            }

            private void GDR_Form_Shown(object sender, EventArgs e) {
                  Console.WriteLine($"Total thread count: {Environment.ProcessorCount}");

                  // Get all controls we are interested in changing later for themes all at once to speed up later references
                  labels = GetAll(this, typeof(Label)).Select(c => c as Label).ToArray();
                  checkBoxes = GetAll(this, typeof(CheckBox)).Select(c => c as CheckBox).ToArray();
                  buttons = GetAll(this, typeof(Button)).Select(c => c as Button).ToArray();
                  numericUpDowns = GetAll(this, typeof(NumericUpDown)).Select(c => c as NumericUpDown).ToArray();
                  textBoxes = GetAll(this, typeof(TextBox)).Select(c => c as TextBox).ToArray();
                  domainUpDowns = GetAll(this, typeof(DomainUpDown)).Select(c => c as DomainUpDown).ToArray();
                  richTextBoxes = GetAll(this, typeof(RichTextBox)).Select(c => c as RichTextBox).ToArray();
                  groupBoxes = GetAll(this, typeof(GroupBox)).Select(c => c as GroupBox).ToArray();
                  pictureBoxes = GetAll(this, typeof(PictureBox)).Select(c => c as PictureBox).ToArray();
                  radioButtons = GetAll(this, typeof(RadioButton)).Select(c => c as RadioButton).ToArray();

                  SetTheme();
            }

            private void RefreshUI() {
                  SetAllIconTexturesElements();
                  SetAllGameTexturesElementStates();
                  SetAllFontRandElementStates();
                  CenterHeaderAndVersionText();

                  ReadyState ready = gameFileManager.getReadyState();

                  Config config = Instance;

                  this.gameFolderTextBox.Text = config.gameDirectory;

                  this.seedInputBox.Text = config.seed.ToString();
                  this.seedInputBox.Value = config.seed;

                  this.textureQualitySelectorBox.SelectedIndex = (int)config.quality;
                  this.applicationThemeSelectorBox.SelectedIndex = (int)config.themeID;

                  // Format the sprite size multiplier display's text
                  if (config.maxSpriteMultiplier < 1000) {
                        string fmtString = "F2";
                        if (config.maxSpriteMultiplier > 3 && config.maxSpriteMultiplier <= 50) {
                              fmtString = "F1";
                        } else if (config.maxSpriteMultiplier > 50) {
                              fmtString = "";
                        }

                        this.spriteSizeMultiplierTextbox.Text = config.maxSpriteMultiplier.ToString(fmtString) + "x";
                  } else {
                        this.spriteSizeMultiplierTextbox.Text = "Unlimited";
                  }

                  this.allowDuplicatesCheckbox.Checked = config.allowDuplicates;

                  this.autoOverwriteFilesCheckbox.Checked = config.autoOverwriteFiles;

                  this.readyStatusDisplay.Text = GetReadyStatusDisplayText(ready);

                  SetStartButtonState(ready);
                  SetMissingFoldersOrExeWarningState(ready);
                  SetNoSettingsWarningState(ready);

                  Console.WriteLine("UI Refreshed");
            }

            private void SetStartButtonState(ReadyState ready) {
                  if (ready == ReadyState.Ready) {
                        this.startButton.Enabled = true;
                  } else {
                        this.startButton.Enabled = false;
                  }
            }

            private void SetMissingFoldersOrExeWarningState(ReadyState ready) {

                  if (ready.HasFlag(ReadyState.FolderNotFound) ||
                        ready.HasFlag(ReadyState.ResourceFolderNotFound) ||
                        ready.HasFlag(ReadyState.IconFolderNotFound) ||
                        ready.HasFlag(ReadyState.ExeNotFound)) {

                        this.gameFolderWarningImage.Visible = true;

                        if (ready.HasFlag(ReadyState.FolderNotFound)) {
                              this.gameFolderWarningImage.Image = Resources.Error_half_size;
                        } else {
                              this.gameFolderWarningImage.Image = Resources.Warning_half_size;
                        }

                        string tooltipText = GetReadyStatusDisplayText(ready);
                        this.toolTip.SetToolTip(this.gameFolderWarningImage, tooltipText);

                  } else {
                        this.gameFolderWarningImage.Visible = false;
                  }
            }

            private void SetNoSettingsWarningState(ReadyState ready) {
                  if (ready.HasFlag(ReadyState.NoSettingsEnabled)) {

                        this.iconWarningImage_1.Visible = true;
                        this.iconWarningImage_2.Visible = true;
                        this.iconWarningImage_3.Visible = true;

                        this.toolTip.SetToolTip(this.iconWarningImage_1, GetReadyStatusDisplayText(ReadyState.NoSettingsEnabled));
                        this.toolTip.SetToolTip(this.iconWarningImage_2, GetReadyStatusDisplayText(ReadyState.NoSettingsEnabled));
                        this.toolTip.SetToolTip(this.iconWarningImage_3, GetReadyStatusDisplayText(ReadyState.NoSettingsEnabled));

                  } else {
                        this.iconWarningImage_1.Visible = false;
                        this.iconWarningImage_2.Visible = false;
                        this.iconWarningImage_3.Visible = false;
                  }
            }

            private string GetReadyStatusDisplayText(ReadyState ready, bool corrupt = true) {
                  string ret = string.Empty;

                  if (ready.HasFlag(ReadyState.FolderNotFound)) {
                        ret = "The given folder doesn't exist";
                  } else if (ready.HasFlag(ReadyState.ResourceFolderNotFound)) {
                        ret = "The Resources folder in the game directory can't be found";
                  } else if (ready.HasFlag(ReadyState.IconFolderNotFound)) {
                        ret = "The Icons folder in the game directory can't be found";
                  } else if (ready.HasFlag(ReadyState.ExeNotFound)) {
                        ret = "The EXE in the game directory can't be found";
                  } else if (ready.HasFlag(ReadyState.NoSettingsEnabled)) {
                        ret = "There are no randomisation settings enabled";
                  } else {
                        ret = "The randomisation can begin";
                  }

                  if (corrupt) {
                        ret = ret.AlterRandomCharactersLooped(this.textCorruptionLevel);
                  }

                  return ret;
            }

            private void SetAllIconTexturesElements() {
                  Config config = Instance;

                  if (config.iconTextures.GetEnabledSettingsCount() == 0) {
                        config.iconTextures.enabled = false;
                  }

                  this.IconTexturesCheckbox.Checked = config.iconTextures.enabled;
                  this.IconTexturesGroupDisplay.Value = config.iconTextures.group;
                  this.IconTexturesGroupDisplay.Enabled = this.IconTexturesCheckbox.Checked;

                  if (this.IconTexturesCheckbox.Checked == false) {
                        this.CubeTexturesCheckbox.Enabled = false;
                        this.CubeTexturesGroupDisplay.Enabled = false;
                        this.ShipTexturesCheckbox.Enabled = false;
                        this.ShipTexturesGroupDisplay.Enabled = false;
                        this.BallTexturesCheckbox.Enabled = false;
                        this.BallTexturesGroupDisplay.Enabled = false;
                        this.UFO_TexturesCheckbox.Enabled = false;
                        this.UFO_TexturesGroupDisplay.Enabled = false;
                        this.WaveTexturesCheckbox.Enabled = false;
                        this.WaveTexturesGroupDisplay.Enabled = false;
                        this.RobotTexturesCheckbox.Enabled = false;
                        this.RobotTexturesGroupDisplay.Enabled = false;
                        this.SpiderTexturesCheckbox.Enabled = false;
                        this.SpiderTexturesGroupDisplay.Enabled = false;
                        this.SwingTexturesCheckbox.Enabled = false;
                        this.SwingTexturesGroupDisplay.Enabled = false;
                        this.JetpackTexturesCheckbox.Enabled = false;
                        this.JetpackTexturesGroupDisplay.Enabled = false;

                  } else {
                        this.CubeTexturesCheckbox.Enabled = true;
                        this.CubeTexturesGroupDisplay.Enabled = config.iconTextures.Cube.enabled;
                        this.ShipTexturesCheckbox.Enabled = true;
                        this.ShipTexturesGroupDisplay.Enabled = config.iconTextures.Ship.enabled;
                        this.BallTexturesCheckbox.Enabled = true;
                        this.BallTexturesGroupDisplay.Enabled = config.iconTextures.Ball.enabled;
                        this.UFO_TexturesCheckbox.Enabled = true;
                        this.UFO_TexturesGroupDisplay.Enabled = config.iconTextures.Ufo.enabled;
                        this.WaveTexturesCheckbox.Enabled = true;
                        this.WaveTexturesGroupDisplay.Enabled = config.iconTextures.Wave.enabled;
                        this.RobotTexturesCheckbox.Enabled = true;
                        this.RobotTexturesGroupDisplay.Enabled = config.iconTextures.Robot.enabled;
                        this.SpiderTexturesCheckbox.Enabled = true;
                        this.SpiderTexturesGroupDisplay.Enabled = config.iconTextures.Spider.enabled;
                        this.SwingTexturesCheckbox.Enabled = true;
                        this.SwingTexturesGroupDisplay.Enabled = config.iconTextures.Swing.enabled;
                        this.JetpackTexturesCheckbox.Enabled = true;
                        this.JetpackTexturesGroupDisplay.Enabled = config.iconTextures.Jetpack.enabled;
                  }
                  this.CubeTexturesCheckbox.Checked = config.iconTextures.Cube.enabled;
                  this.CubeTexturesGroupDisplay.Value = config.iconTextures.Cube.group;
                  this.ShipTexturesCheckbox.Checked = config.iconTextures.Ship.enabled;
                  this.ShipTexturesGroupDisplay.Value = config.iconTextures.Ship.group;
                  this.BallTexturesCheckbox.Checked = config.iconTextures.Ball.enabled;
                  this.BallTexturesGroupDisplay.Value = config.iconTextures.Ball.group;
                  this.UFO_TexturesCheckbox.Checked = config.iconTextures.Ufo.enabled;
                  this.UFO_TexturesGroupDisplay.Value = config.iconTextures.Ufo.group;
                  this.WaveTexturesCheckbox.Checked = config.iconTextures.Wave.enabled;
                  this.WaveTexturesGroupDisplay.Value = config.iconTextures.Wave.group;
                  this.RobotTexturesCheckbox.Checked = config.iconTextures.Robot.enabled;
                  this.RobotTexturesGroupDisplay.Value = config.iconTextures.Robot.group;
                  this.SpiderTexturesCheckbox.Checked = config.iconTextures.Spider.enabled;
                  this.SpiderTexturesGroupDisplay.Value = config.iconTextures.Spider.group;
                  this.SwingTexturesCheckbox.Checked = config.iconTextures.Swing.enabled;
                  this.SwingTexturesGroupDisplay.Value = config.iconTextures.Swing.group;
                  this.JetpackTexturesCheckbox.Checked = config.iconTextures.Jetpack.enabled;
                  this.JetpackTexturesGroupDisplay.Value = config.iconTextures.Jetpack.group;
            }

            private void SetAllFontRandElementStates() {
                  FontRandomisationSettings fontRand = Config.Instance.fontRand;

                  this.fontRandEnabledCheckbox.Checked = fontRand.enabled;

                  // Set the controls depending on if font randomisation is enabled
                  this.fontShuffleStylesCheckbox.Enabled = fontRand.enabled;
                  this.fontPerLetterRandomisationButton.Enabled = fontRand.enabled;
                  this.fontPerFontRandomisationButton.Enabled = fontRand.enabled;
                  this.fontRandomiseLettersCheckbox.Enabled = fontRand.enabled;
                  if (fontRand.enabled == false) {
                        return;
                  }

                  this.fontShuffleStylesCheckbox.Checked = fontRand.shuffleFontStyles;

                  this.fontPerLetterRandomisationButton.Enabled = this.fontShuffleStylesCheckbox.Checked;
                  this.fontPerFontRandomisationButton.Enabled = this.fontShuffleStylesCheckbox.Checked;

                  switch (fontRand.shufflingMode) {
                        case FontRandomisationSettings.FontStyleShufflingMode.PerLetter:
                              this.fontPerLetterRandomisationButton.Checked = true;
                              break;
                        case FontRandomisationSettings.FontStyleShufflingMode.PerFont:
                              this.fontPerFontRandomisationButton.Checked = true;
                              break;
                  }

                  this.fontRandomiseLettersCheckbox.Checked = fontRand.randomiseLetters;
            }

            private void CenterHeaderAndVersionText() {
                  int windowWidth = this.Width;
                  int headerWidth = this.headerLabel.PreferredWidth;
                  int versionWidth = this.versionLabel.PreferredWidth;

                  // Center the header label
                  int headerMargin = (windowWidth - headerWidth) / 2;
                  this.headerLabel.Location = new Point(
                        headerMargin,
                        this.headerLabel.Location.Y);

                  // Center the version label to the header
                  this.versionLabel.Location = new Point(
                        headerMargin + headerWidth - 25,
                        this.versionLabel.Location.Y);
            }

            private void SetAllGameTexturesElementStates() {
                  SetMenuTexturesElementStates();
                  SetShopTexturesElementStates();
                  SetEditorTexturesElementStates();
                  SetBlockTexturesElementStates();
                  SetPortalTexturesElementStates();
                  SetOrbsTexturesElementStates();
                  SetPadsTexturesElementStates();
                  SetParticlesTexturesElementStates();
                  SetEffectsTexturesElementStates();
                  SetMiscTexturesElementStates();
            }

            private void SetMenuTexturesElementStates() {
                  this.MenuTexturesCheckbox.Checked = Config.Instance.menuTextures.enabled;
                  this.MenuTexturesGroupDisplay.Value = Config.Instance.menuTextures.group;
                  this.MenuTexturesGroupDisplay.Enabled = this.MenuTexturesCheckbox.Checked;
            }

            private void SetShopTexturesElementStates() {
                  this.ShopTexturesCheckbox.Checked = Config.Instance.shopTextures.enabled;
                  this.ShopTexturesGroupDisplay.Value = Config.Instance.shopTextures.group;
                  this.ShopTexturesGroupDisplay.Enabled = this.ShopTexturesCheckbox.Checked;
            }

            private void SetEditorTexturesElementStates() {
                  this.EditorTexturesCheckbox.Checked = Config.Instance.editorTextures.enabled;
                  this.EditorTexturesGroupDisplay.Value = Config.Instance.editorTextures.group;
                  this.EditorTexturesGroupDisplay.Enabled = this.EditorTexturesCheckbox.Checked;
            }

            private void SetBlockTexturesElementStates() {
                  this.BlockTexturesCheckbox.Checked = Config.Instance.tileTextures.enabled;
                  this.BlockTexturesGroupDisplay.Value = Config.Instance.tileTextures.group;
                  this.BlockTexturesGroupDisplay.Enabled = this.BlockTexturesCheckbox.Checked;
            }

            private void SetPortalTexturesElementStates() {
                  this.PortalTexturesCheckbox.Checked = Config.Instance.portalTextures.enabled;
                  this.PortalTexturesGroupDisplay.Value = Config.Instance.portalTextures.group;
                  this.PortalTexturesGroupDisplay.Enabled = this.PortalTexturesCheckbox.Checked;
            }

            private void SetOrbsTexturesElementStates() {
                  this.OrbsCheckbox.Checked = Config.Instance.orbTextures.enabled;
                  this.OrbsGroupDisplay.Value = Config.Instance.orbTextures.group;
                  this.OrbsGroupDisplay.Enabled = this.OrbsCheckbox.Checked;
            }

            private void SetPadsTexturesElementStates() {
                  this.PadsCheckbox.Checked = Config.Instance.padTextures.enabled;
                  this.PadsGroupDisplay.Value = Config.Instance.padTextures.group;
                  this.PadsGroupDisplay.Enabled = this.PadsCheckbox.Checked;
            }

            private void SetParticlesTexturesElementStates() {
                  this.ParticleTexturesCheckbox.Checked = Config.Instance.particleTextures.enabled;
                  this.ParticleTexturesGroupDisplay.Value = Config.Instance.particleTextures.group;
                  this.ParticleTexturesGroupDisplay.Enabled = this.ParticleTexturesCheckbox.Checked;
            }

            private void SetEffectsTexturesElementStates() {
                  this.EffectsCheckbox.Checked = Config.Instance.effectTextures.enabled;
                  this.EffectsGroupDisplay.Value = Config.Instance.effectTextures.group;
                  this.EffectsGroupDisplay.Enabled = this.EffectsCheckbox.Checked;
            }

            private void SetMiscTexturesElementStates() {
                  this.MiscCheckbox.Checked = Config.Instance.miscTextures.enabled;
                  this.MiscGroupDisplay.Value = Config.Instance.miscTextures.group;
                  this.MiscGroupDisplay.Enabled = this.MiscCheckbox.Checked;
            }

            private void SaveConfigFileAfterDelay(int seconds = 0) {
                  Task.Delay(new TimeSpan(0, 0, seconds)).ContinueWith(o => { Config.Instance.WriteFile(); });
            }

            /// <summary>
            /// Sets the enabled state of every element that directly affects the randomisation
            /// </summary>
            private void SetUI_EnabledState(bool enabled) {
                  this.IconTexturesCheckbox.Enabled = enabled;
                  this.IconTexturesGroupDisplay.Enabled = enabled;
                  this.CubeTexturesCheckbox.Enabled = enabled;
                  this.CubeTexturesGroupDisplay.Enabled = enabled;
                  this.ShipTexturesCheckbox.Enabled = enabled;
                  this.ShipTexturesGroupDisplay.Enabled = enabled;
                  this.BallTexturesCheckbox.Enabled = enabled;
                  this.BallTexturesGroupDisplay.Enabled = enabled;
                  this.UFO_TexturesCheckbox.Enabled = enabled;
                  this.UFO_TexturesGroupDisplay.Enabled = enabled;
                  this.WaveTexturesCheckbox.Enabled = enabled;
                  this.WaveTexturesGroupDisplay.Enabled = enabled;
                  this.RobotTexturesCheckbox.Enabled = enabled;
                  this.RobotTexturesGroupDisplay.Enabled = enabled;
                  this.SpiderTexturesCheckbox.Enabled = enabled;
                  this.SpiderTexturesGroupDisplay.Enabled = enabled;
                  this.SwingTexturesCheckbox.Enabled = enabled;
                  this.SwingTexturesGroupDisplay.Enabled = enabled;
                  this.JetpackTexturesCheckbox.Enabled = enabled;
                  this.JetpackTexturesGroupDisplay.Enabled = enabled;
                  
                  this.MenuTexturesCheckbox.Enabled = enabled;
                  this.MenuTexturesGroupDisplay.Enabled = enabled;
                  this.ShopTexturesCheckbox.Enabled = enabled;
                  this.ShopTexturesGroupDisplay.Enabled = enabled;
                  this.EditorTexturesCheckbox.Enabled = enabled;
                  this.EditorTexturesGroupDisplay.Enabled = enabled;
                  this.BlockTexturesCheckbox.Enabled = enabled;
                  this.BlockTexturesGroupDisplay.Enabled = enabled;
                  this.PortalTexturesCheckbox.Enabled = enabled;
                  this.PortalTexturesGroupDisplay.Enabled = enabled;
                  this.OrbsCheckbox.Enabled = enabled;
                  this.OrbsGroupDisplay.Enabled = enabled;
                  this.PadsCheckbox.Enabled = enabled;
                  this.PadsGroupDisplay.Enabled = enabled;
                  this.ParticleTexturesCheckbox.Enabled = enabled;
                  this.ParticleTexturesGroupDisplay.Enabled = enabled;
                  this.EffectsCheckbox.Enabled = enabled;
                  this.EffectsGroupDisplay.Enabled = enabled;
                  this.MiscCheckbox.Enabled = enabled;
                  this.MiscGroupDisplay.Enabled = enabled;

                  this.gameFolderTextBox.Enabled = enabled;
                  this.gameFolderSelectorButton.Enabled = enabled;
                  this.seedInputBox.Enabled = enabled;
                  this.randomSeedButton.Enabled = enabled;
                  this.textureQualitySelectorBox.Enabled = enabled;

                  this.fontRandEnabledCheckbox.Enabled = enabled;
                  this.fontShuffleStylesCheckbox.Enabled = enabled;
                  this.fontPerFontRandomisationButton.Enabled = enabled;
                  this.fontPerLetterRandomisationButton.Enabled = enabled;
                  this.fontRandomiseLettersCheckbox.Enabled = enabled;

                  this.spriteSizeMultiplierTrackbar.Enabled = enabled;
                  this.spriteSizeMultiplierTextbox.Enabled = enabled;
                  this.allowDuplicatesCheckbox.Enabled = enabled;

                  this.startButton.Enabled = enabled;
                  this.restoreFilesButton.Enabled = enabled;

                  this.autoOverwriteFilesCheckbox.Enabled = enabled;
            }

            private void Logo_Click(object sender, EventArgs e) {

                  const string caption = "About this app";
                  string[] message = {
                        "You found a sneaky button :)\n",

                        "This app (and it's many horrible past variants) were born from passion.",
                        "This version of GDR is compatible with Geometry Dash version 2.207\n",

                        "Credits:",
                        " - Developer: Hydrough",
                        " - Logo created by: Hydrough",
                        " - RectpackSharp library made by ThomasMiz:",
                        "       https://github.com/ThomasMiz/RectpackSharp",
                        " - Wisteria theme made by my friend, Maya\n",

                        "Contact me:",
                        " - Discord: hydrough_7165",
                        " - GitHub: https://github.com/Hydrough2k2k\n",

                        "Special thanks to my friends for testing this and giving me ideas!",
                  };

                  MessageBoxButtons buttons = MessageBoxButtons.OK;
                  MessageBox.Show(string.Join("\n", message), caption, buttons);
            }

            private void ChangeProgressDisplayState(bool enabled) {
                  // Disable the UI elements that are not needed during the randomisation
                  this.readyStatusDisplay.Visible = !enabled;

                  // Enable the progress display elements
                  this.elapsedTimeDisplay.Visible = enabled;
                  this.randomisingProgressBar.Visible = enabled;
                  this.randomisingProgressDisplay.Visible = enabled;
            }

            private async void restoreFilesButton_Click(object sender, EventArgs e) {
                  const string caption = "Restore Game Files";
                  string[] message = {
                        "This will restore all of the game's files to their defaults.",
                        "Are you sure you want to do this?"
                  };

                  // Ask if the user wants to restore the files
                  MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                  DialogResult result = MessageBox.Show(string.Join("\n", message), caption, buttons);

                  // Only continue, if the user pressed yes
                  if (result != DialogResult.Yes) {
                        return;
                  }

                  ChangeProgressDisplayState(enabled: true);

                  Stopwatch stopwatch = new Stopwatch();
                  Thread restoreThread = new Thread(() => {
                        gameFileManager.RestoreFiles();
                  });
                  restoreThread.Start();

                  string lastDisplayPrint = string.Empty;
                  string lastTimePrint = string.Empty;

                  while (restoreThread.IsAlive == true) {
                        string newDisplayPrint = gameFileManager.progressState.GetProgressString();
                        if (newDisplayPrint != lastDisplayPrint) {
                              lastDisplayPrint = newDisplayPrint;
                              UpdateProgressStateObjects(newDisplayPrint);
                        }

                        string newTimePrint = stopwatch.GetElapsedTimeFormatted(TimeExtension.TimeFormat.HH_MM_SS, true);
                        if (newTimePrint != lastTimePrint) {
                              lastTimePrint = newTimePrint;
                              UpdateProgressElapsedTime(newTimePrint);
                        }

                        // Update the without locking the application's UI
                        await Task.Run(() => {
                              Thread.Sleep(UI_UpdateDelay);
                        });
                  }

                  restoreThread.Join();

                  this.readyStatusDisplay.Text = "File Restoration Complete";

                  ChangeProgressDisplayState(enabled: false);
            }

            private async void startButton_Click(object sender, EventArgs e) {
                  bool ready = gameFileManager.getReadyState() == ReadyState.Ready;
                  if (ready == false)
                        return;

                  SetUI_EnabledState(false);

                  ChangeProgressDisplayState(enabled: true);

                  bool randomSeed = false;
                  // Create a new random seed if the input value is 0
                  int seed = Config.Instance.seed;
                  if (seed == 0) {
                        seed = Guid.NewGuid().GetHashCode();
                        randomSeed = true;
                  }

                  Stopwatch stopwatch = new Stopwatch();
                  stopwatch.Start();
                  Thread randomisationThread = new Thread(() => {
                        gameFileManager.StartRandomising(seed);
                  });
                  randomisationThread.Start();

                  string lastDisplayPrint = string.Empty;
                  string lastTimePrint = string.Empty;

                  while (randomisationThread.IsAlive == true) {
                        string newDisplayPrint = gameFileManager.progressState.GetProgressString();
                        if (newDisplayPrint != lastDisplayPrint) {
                              lastDisplayPrint = newDisplayPrint;
                              UpdateProgressStateObjects(newDisplayPrint);
                        }

                        string newTimePrint = stopwatch.GetElapsedTimeFormatted(TimeExtension.TimeFormat.HH_MM_SS, true);
                        if (newTimePrint != lastTimePrint) {
                              lastTimePrint = newTimePrint;
                              UpdateProgressElapsedTime(newTimePrint);
                        }

                        // Update the without locking the application's UI
                        await Task.Run(() => {
                              Thread.Sleep(UI_UpdateDelay);
                        });
                  }

                  randomisationThread.Join();

                  SetUI_EnabledState(true);

                  RefreshUI();

                  ChangeProgressDisplayState(enabled: false);

                  StringBuilder statusText = new StringBuilder("Randomisation complete in " +
                        stopwatch.GetElapsedTimeFormatted(TimeExtension.TimeFormat.HH_MM_SS, true));

                  if (randomSeed) {
                        statusText.Append(". The seed was " + seed.ToString("N0"));
                  }

                  statusText.Append(". You can close GDR");

                  int corruptionLoops = Math.Min(this.textCorruptionLevel, maxCorruptionLoops);
                  this.readyStatusDisplay.Text = statusText.ToString().AlterRandomCharactersLooped(corruptionLoops);

                  // If the game directory is valid, enable the restore button
                  this.restoreFilesButton.Enabled = gameFileManager.IsGameDirectoryValid();
            }

            private void UpdateProgressStateObjects(string newDisplayPrint) {
                  // Limit the max Corruption loops to make it not lag too much
                  int corruptionLoops = Math.Min(this.textCorruptionLevel, maxCorruptionLoops);

                  this.randomisingProgressDisplay.Text = newDisplayPrint.AlterRandomCharactersLooped(corruptionLoops);
                  this.randomisingProgressBar.Value = (int)Math.Round(gameFileManager.progressState.percentComplete);
            }

            private void UpdateProgressElapsedTime(string newTimePrint) {
                  // Limit the max Corruption loops to make it not lag too much
                  int curruptionLoops = Math.Min(this.textCorruptionLevel, maxCorruptionLoops);

                  this.elapsedTimeDisplay.Text = newTimePrint.AlterRandomCharactersLooped(curruptionLoops);
                  // Right-align the text so it will be near the Randomise button
                  this.elapsedTimeDisplay.Location = new Point(735 - this.elapsedTimeDisplay.PreferredWidth, this.elapsedTimeDisplay.Location.Y);
            }

            private void SeedValueChanged(object sender, EventArgs e) {
                  NumericUpDown nud = sender as NumericUpDown;
                  Config.Instance.seed = (int)nud.Value;
                  RefreshUI();
            }

            private void SetTheme() {
                  if (this.themeController.current.name != "Random Theme") {
                        SetTheme(this.themeController.current);

                  } else {
                        Random random = new Random(Guid.NewGuid().GetHashCode());

                        // Generate an entirely random theme because why not
                        Theme randomTheme = new Theme(
                              name: "Random Theme",
                              backgroundColour: random.GetRandomRGBColor(),
                              textColour: random.GetRandomRGBColor(),
                              objectBackColour: random.GetRandomRGBColor(),
                              objectTextColour: random.GetRandomRGBColor(),
                              beamColour: random.GetRandomRGBColor()
                        );

                        SetTheme(randomTheme);

                        Console.WriteLine($"New random theme:\n" +
                              $"Background:  {randomTheme.backgroundColour}\n" +
                              $"Text:        {randomTheme.textColour}\n" +
                              $"Object Back: {randomTheme.objectBackColour}\n" +
                              $"Object Text: {randomTheme.objectTextColour}\n" +
                              $"Beam Colour: {randomTheme.beamColour}\n"
                        );
                  }
            }

            private void SetTheme(Theme theme) {
                  SetFormColours(theme.backgroundColour);

                  Color activeColour = theme.textColour;
                  SetTextColours(activeColour);
                  SetCheckboxColours(activeColour);
                  SetRadioButtonColours(activeColour);

                  SetMenuElementColours(theme.objectBackColour, theme.objectTextColour);

                  UpdateImageTheme(theme);

                  RefreshUI();
            }

            private void SetFormColours(Color back) {
                  this.BackColor = back;
            }

            private void SetTextColours(Color activeColour) {
                  for (int i = 0; i < labels.Length; i++) {
                        if (labels[i].Name.Contains("NoCol") == false) {
                              labels[i].ForeColor = activeColour;
                        }
                  }
            }

            private void SetCheckboxColours(Color textColour) {
                  for (int i = 0; i < checkBoxes.Length; i++) {
                        checkBoxes[i].ForeColor = textColour;
                  }
            }

            private void SetRadioButtonColours(Color textColour) {
                  for (int i = 0; i < radioButtons.Length; i++) {
                        radioButtons[i].ForeColor = textColour;
                  }
            }

            private void SetMenuElementColours(Color back, Color fore) {
                  for (int i = 0; i < buttons.Length; i++) {
                        buttons[i].BackColor = back;
                        buttons[i].ForeColor = fore;
                  }

                  for (int i = 0; i < numericUpDowns.Length; i++) {
                        numericUpDowns[i].BackColor = back;
                        numericUpDowns[i].ForeColor = fore;
                  }

                  for (int i = 0; i < textBoxes.Length; i++) {
                        textBoxes[i].BackColor = back;
                        textBoxes[i].ForeColor = fore;
                  }

                  for (int i = 0; i < domainUpDowns.Length; i++) {
                        domainUpDowns[i].BackColor = back;
                        domainUpDowns[i].ForeColor = fore;
                  }

                  for (int i = 0; i < richTextBoxes.Length; i++) {
                        richTextBoxes[i].BackColor = back;
                        richTextBoxes[i].ForeColor = fore;
                  }

                  for (int i = 0; i < groupBoxes.Length; i++) {
                        groupBoxes[i].ForeColor = fore;
                  }
            }

            private void UpdateImageTheme(Theme theme) {
                  Color beamColour = theme.beamColour;
                  Bitmap connectorBeam = (Bitmap)Properties.Resources.ConnectorBeamWhite.Clone();

                  // Get every pixel and change it to the new colour if it's white. Not efficient, but what can you do, I'm lazy
                  for (int y = 0; y < connectorBeam.Height; y++) {
                        for (int x = 0; x < connectorBeam.Width; x++) {
                              Color pixel = connectorBeam.GetPixel(x, y);
                              if (pixel == Color.FromArgb(255, 255, 255)) {
                                    connectorBeam.SetPixel(x, y, beamColour);
                              }
                        }
                  }

                  for (int i = 0; i < this.pictureBoxes.Length; i++) {
                        if (pictureBoxes[i].Name.EndsWith("ConnectorBeam")) {
                              pictureBoxes[i].Image = (Bitmap)connectorBeam.Clone();
                        }
                  }
            }

            private void applicationThemeSelectorBox_Click(object sender, EventArgs e) {
                  DomainUpDown domainUpDown = sender as DomainUpDown;
                  int oldThemeID = this.themeController.activeThemeID;
                  int newThemeID = domainUpDown.SelectedIndex;

                  Console.WriteLine($"Switching to theme ID {newThemeID}");

                  this.themeController.activeThemeID = newThemeID;
                  Config.Instance.themeID = newThemeID;

                  SetTheme();
            }

            public IEnumerable<Control> GetAll(Control control, Type type) {
                  var controls = control.Controls.Cast<Control>();

                  return controls
                        .SelectMany(ctrl => GetAll(ctrl, type))
                        .Concat(controls)
                        .Where(c => c.GetType() == type);
            }

            private void GDR_HeaderLabel_Click(object sender, EventArgs e) {
                  Random random = new Random(Guid.NewGuid().GetHashCode());
                  
                  // Permanently alter the text of every element that stores text
                  for (int i = 0; i < this.labels.Length; i++) {
                        this.labels[i].Text = this.labels[i].Text.AlterRandomCharacters();
                  }

                  for (int i = 0; i < this.checkBoxes.Length; i++) {
                        this.checkBoxes[i].Text = this.checkBoxes[i].Text.AlterRandomCharacters();
                  }

                  for (int i = 0; i < this.groupBoxes.Length; i++) {
                        this.groupBoxes[i].Text = this.groupBoxes[i].Text.AlterRandomCharacters();
                  }

                  for (int i = 0; i < this.buttons.Length; i++) {
                        this.buttons[i].Text = this.buttons[i].Text.AlterRandomCharacters();
                  }

                  for (int i = 0; i < this.radioButtons.Length; i++) {
                        this.radioButtons[i].Text = this.radioButtons[i].Text.AlterRandomCharacters();
                  }

                  textCorruptionLevel++;

                  RefreshUI();
            }

            private void IconTexturesSettingsChanged(object sender, EventArgs e) {
                  bool enabled = this.IconTexturesCheckbox.Checked;

                  Config config = Instance;

                  config.iconTextures.enabled = enabled;

                  config.iconTextures.Cube.enabled = enabled;
                  config.iconTextures.Ship.enabled = enabled;
                  config.iconTextures.Ball.enabled = enabled;
                  config.iconTextures.Ufo.enabled = enabled;
                  config.iconTextures.Wave.enabled = enabled;
                  config.iconTextures.Robot.enabled = enabled;
                  config.iconTextures.Spider.enabled = enabled;
                  config.iconTextures.Swing.enabled = enabled;
                  config.iconTextures.Jetpack.enabled = enabled;

                  RefreshUI();
            }

            private void IconTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  Config.Instance.iconTextures.group = (int)numericUpDown.Value;
                  CubeTexturesGroupChanged(sender, null);
                  ShipTexturesGroupChanged(sender, null);
                  BallTexturesGroupChanged(sender, null);
                  UFO_TexturesGroupChanged(sender, null);
                  WaveTexturesGroupChanged(sender, null);
                  RobotTexturesGroupChanged(sender, null);
                  SpiderTexturesGroupChanged(sender, null);
                  SwingTexturesGroupChanged(sender, null);
                  JetpackTexturesGroupChanged(sender, null);

                  RefreshUI();
            }

            private void CubeTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.Instance.iconTextures.Cube.enabled = checkBox.Checked;
                  RefreshUI();
            }

            private void CubeTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  Config.Instance.iconTextures.Cube.group = (int)numericUpDown.Value;
                  RefreshUI();
            }

            private void ShipTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.Instance.iconTextures.Ship.enabled = checkBox.Checked;
                  RefreshUI();
            }

            private void ShipTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  Config.Instance.iconTextures.Ship.group = (int)numericUpDown.Value;
                  RefreshUI();
            }

            private void BallTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.Instance.iconTextures.Ball.enabled = checkBox.Checked;
                  RefreshUI();
            }

            private void BallTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  Config.Instance.iconTextures.Ball.group = (int)numericUpDown.Value;
                  RefreshUI();
            }

            private void UFO_TexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.Instance.iconTextures.Ufo.enabled = checkBox.Checked;
                  RefreshUI();
            }

            private void UFO_TexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  Config.Instance.iconTextures.Ufo.group = (int)numericUpDown.Value;
                  RefreshUI();
            }

            private void WaveTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.Instance.iconTextures.Wave.enabled = checkBox.Checked;
                  RefreshUI();
            }

            private void WaveTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  Config.Instance.iconTextures.Wave.group = (int)numericUpDown.Value;
                  RefreshUI();
            }

            private void RobotTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.Instance.iconTextures.Robot.enabled = checkBox.Checked;
                  RefreshUI();
            }

            private void RobotTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  Config.Instance.iconTextures.Robot.group = (int)numericUpDown.Value;
                  RefreshUI();
            }

            private void SpiderTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.Instance.iconTextures.Spider.enabled = checkBox.Checked;
                  RefreshUI();
            }

            private void SpiderTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  Config.Instance.iconTextures.Spider.group = (int)numericUpDown.Value;
                  RefreshUI();
            }

            private void SwingTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.Instance.iconTextures.Swing.enabled = checkBox.Checked;
                  RefreshUI();
            }

            private void SwingTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  Config.Instance.iconTextures.Swing.group = (int)numericUpDown.Value;
                  RefreshUI();
            }

            private void JetpackTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.Instance.iconTextures.Jetpack.enabled = checkBox.Checked;
                  RefreshUI();
            }

            private void JetpackTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  Config.Instance.iconTextures.Jetpack.group = (int)numericUpDown.Value;
                  RefreshUI();
            }

            private void MenuTexturesSettingsChanged(object sender, EventArgs e) {
                  Config.Instance.menuTextures.enabled = this.MenuTexturesCheckbox.Checked;
                  Config.Instance.menuTextures.group = (int)this.MenuTexturesGroupDisplay.Value;
                  RefreshUI();
            }

            private void ShopTexturesSettingsChanged(object sender, EventArgs e) {
                  Config.Instance.shopTextures.enabled = this.ShopTexturesCheckbox.Checked;
                  Config.Instance.shopTextures.group = (int)this.ShopTexturesGroupDisplay.Value;
                  RefreshUI();
            }

            private void EditorTexturesSettingsChanged(object sender, EventArgs e) {
                  Config.Instance.editorTextures.enabled = this.EditorTexturesCheckbox.Checked;
                  Config.Instance.editorTextures.group = (int)this.EditorTexturesGroupDisplay.Value;
                  RefreshUI();
            }

            private void TilesTexturesSettingsChanged(object sender, EventArgs e) {
                  Config.Instance.tileTextures.enabled = this.BlockTexturesCheckbox.Checked;
                  Config.Instance.tileTextures.group = (int)this.BlockTexturesGroupDisplay.Value;
                  RefreshUI();
            }

            private void PortalTexturesSettingsChanged(object sender, EventArgs e) {
                  Config.Instance.portalTextures.enabled = this.PortalTexturesCheckbox.Checked;
                  Config.Instance.portalTextures.group = (int)this.PortalTexturesGroupDisplay.Value;
                  RefreshUI();
            }

            private void OrbsSettingsChanged(object sender, EventArgs e) {
                  Config.Instance.orbTextures.enabled = this.OrbsCheckbox.Checked;
                  Config.Instance.orbTextures.group = (int)this.OrbsGroupDisplay.Value;
                  RefreshUI();
            }

            private void PadsSettingChanged(object sender, EventArgs e) {
                  Config.Instance.padTextures.enabled = this.PadsCheckbox.Checked;
                  Config.Instance.padTextures.group = (int)this.PadsGroupDisplay.Value;
                  RefreshUI();
            }

            private void ParticleTexturesSettingsChanged(object sender, EventArgs e) {
                  Config.Instance.particleTextures.enabled = this.ParticleTexturesCheckbox.Checked;
                  Config.Instance.particleTextures.group = (int)this.ParticleTexturesGroupDisplay.Value;
                  RefreshUI();
            }

            private void EffectsSettingsChanged(object sender, EventArgs e) {
                  Config.Instance.effectTextures.enabled = this.EffectsCheckbox.Checked;
                  Config.Instance.effectTextures.group = (int)this.EffectsGroupDisplay.Value;
                  RefreshUI();
            }

            private void MiscSettingsChanged(object sender, EventArgs e) {
                  Config.Instance.miscTextures.enabled = this.MiscCheckbox.Checked;
                  Config.Instance.miscTextures.group = (int)this.MiscGroupDisplay.Value;
                  RefreshUI();
            }

            private void fontRandEnabledCheckbox_Click(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.Instance.fontRand.enabled = checkBox.Checked;
                  RefreshUI();
            }

            private void fontShuffleStylesCheckbox_Click(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.Instance.fontRand.shuffleFontStyles = checkBox.Checked;
                  RefreshUI();
            }

            private void fontPerFontRandomisationButton_Click(object sender, EventArgs e) {
                  Config.Instance.fontRand.shufflingMode = FontRandomisationSettings.FontStyleShufflingMode.PerFont;
                  RefreshUI();
            }

            private void fontPerLetterRandomisationButton_Click(object sender, EventArgs e) {
                  Config.Instance.fontRand.shufflingMode = FontRandomisationSettings.FontStyleShufflingMode.PerLetter;
                  RefreshUI();
            }

            private void fontRandomiseLettersCheckbox_Click(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.Instance.fontRand.randomiseLetters = checkBox.Checked;
                  RefreshUI();
            }

            private void SetGameFolder(object sender, EventArgs e) {
                  string folder = GetFolderViaExplorer(Config.Instance.gameDirectory, true);
                  if (folder != string.Empty) {
                        Config.Instance.gameDirectory = folder;
                  }

                  RefreshUI();
            }

            private void GameFolderTextBox_TextChanged(object sender, EventArgs e) {
                  TextBox textBox = sender as TextBox;
                  Config.Instance.gameDirectory = textBox.Text;
                  RefreshUI();
            }

            private string GetFolderViaExplorer(string InitialDirectory, bool IsFolderPicker) {
                  CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                  dialog.InitialDirectory = InitialDirectory;
                  dialog.IsFolderPicker = IsFolderPicker;
                  if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                        return dialog.FileName;
                  }
                  return string.Empty;
            }

            private void autoOverwriteFilesCheckbox_Click(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.Instance.autoOverwriteFiles = checkBox.Checked;
            }

            private void qualityOptionChanged(object sender, EventArgs e) {
                  DomainUpDown qualityDropdown = sender as DomainUpDown;
                  switch (qualityDropdown.Text) {
                        case PathManager.lowQualityName:
                              Config.Instance.quality = Quality.Low;
                              break;
                        case PathManager.mediumQualityName:
                              Config.Instance.quality = Quality.Medium;
                              break;
                        case PathManager.highQualityName:
                              Config.Instance.quality = Quality.High;
                              break;
                        default:
                              break;
                  }

                  RefreshUI();
            }

            private void randomSeedButton_Click(object sender, EventArgs e) {
                  Random random = new Random(Guid.NewGuid().GetHashCode());
                  int value = random.Next(int.MinValue, int.MaxValue);
                  this.seedInputBox.Value = value;
                  Config.Instance.seed = value;

                  RefreshUI();
            }

            private void spriteSizeMultiplierTrackbar_Scroll(object sender, EventArgs e) {
                  TrackBar trackBar = sender as TrackBar;

                  float newMultiplier = 0;
                  if (trackBar.Value <= 50) {
                        newMultiplier = (float)trackBar.Value / 100 + 1f; // Values between 1.01 and 1.5

                  } else if (trackBar.Value <= 75) {
                        newMultiplier = (float)(trackBar.Value - 50) / 50 + 1.5f; // Values between 1.5 and 2

                  } else if (trackBar.Value <= 100) {
                        newMultiplier = (float)(trackBar.Value - 75) / 25 + 2f; // Values between 2 and 3

                  } else if (trackBar.Value <= 170) {
                        newMultiplier = (float)(trackBar.Value - 100) / 10 + 3f; // Values between 3 and 10

                  } else if (trackBar.Value <= 220) {
                        newMultiplier = (float)(trackBar.Value - 170) / 5 + 10f; // Values between 10 and 20

                  } else if (trackBar.Value <= 280) {
                        newMultiplier = (float)(trackBar.Value - 220) / 2 + 20f; // Values between 20 and 50

                  } else if (trackBar.Value <= 330) {
                        newMultiplier = (float)(trackBar.Value - 280) / 1 + 50f; // Values between 50 and 100

                  } else {
                        newMultiplier = 1000f;
                  }

                  Config.Instance.maxSpriteMultiplier = newMultiplier;

                  RefreshUI();
            }

            private void allowDuplicatesCheckbox_Click(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.Instance.allowDuplicates = checkBox.Checked;

                  RefreshUI();
            }

            private void showImportConfigForm(object sender, EventArgs e) {

                  if (importConfigForm == null) {
                        importConfigForm = new ImportConfigForm();
                  }
                  
                  importConfigForm.theme = this.themeController.current;
                  importConfigForm.Show();
            }

            private void showExportConfigForm(object sender, EventArgs e) {
                  
                  if (exportConfigForm == null) {
                        exportConfigForm = new ExportConfigForm();
                  }

                  exportConfigForm.theme = this.themeController.current;
                  exportConfigForm.Show();
            }

            private void ImportConfigThenRefreshUI(string configData) {
                  bool success = Config.Instance.ImportConfigData(configData);

                  RefreshUI();

                  if (success == true) {
                        this.readyStatusDisplay.Text = "Config Imported Successfully";
                  } else {
                        this.readyStatusDisplay.Text = "Failed to Import Config";
                  }
            }

            private void ChangelogButton_Click(object sender, EventArgs e) {

                  if (changelogForm == null) {
                        changelogForm = new ChangelogForm(version);
                  }

                  changelogForm.theme = this.themeController.current;
                  changelogForm.textCorruptionLevel = this.textCorruptionLevel;
                  changelogForm.Show();
            }
      }
}