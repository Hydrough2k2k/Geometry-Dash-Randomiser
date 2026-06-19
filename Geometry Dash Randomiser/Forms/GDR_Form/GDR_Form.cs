using Geometry_Dash_Randomiser.Forms;
using Geometry_Dash_Randomiser.Properties;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      public partial class GDR_Form : Form {

            public GDR_Form() {
                  InitializeComponent();

#if DEBUG
                  AllocConsole();
#endif

                  WriteSystemAnalysisInfoToConsole();

                  Config.ReadFile();

                  if (config.debugMode) {
                        AllocConsole();
                  }

                  this.gameFileManager = new GameFileManager(this);
                  this.themeController = new ThemeController();

                  RefreshThemes(animate: false, setTheme: false);
            }

            private void GDR_Form_Shown(object sender, EventArgs e) {
                  SetSpriteSizeMultiplierSliderAndTextBox();

                  // If the game directory is valid, enable the restore button
                  this.restoreFilesButton.Enabled = gameFileManager.IsGameDirectoryValid();

                  this.versionLabel.Text = version;

                  InitialiseControlContainers();

                  PopulateQualityDropdown();

                  SetTheme();

                  RefreshUI();

                  isFormInitialised = true;
            }

            private void WriteSystemAnalysisInfoToConsole() {
                  Log.Write(Log.Mode.Verbose, $"Total thread count: {Environment.ProcessorCount}");
                  Log.Write(Log.Mode.Verbose, $"Is 64bit Process: {Environment.Is64BitProcess}");
                  Log.Write(Log.Mode.Verbose, $"Is 64bit OS: {Environment.Is64BitOperatingSystem}");
                  Log.Write(Log.Mode.Verbose, $"OS Version: {Environment.OSVersion}");
                  Log.Write(Log.Mode.Verbose, $"System Page Size: {Environment.SystemPageSize}");
                  Log.Write(Log.Mode.Verbose, $"CLR Version: {Environment.Version}\n");
            }

            private void RefreshUI(bool overwriteStatusDisplayText = true, bool allowStatusDisplayTextCorruption = true) {
                  SetAllIconTexturesElements();
                  SetAllGameTexturesElementStates();
                  SetAllFontRandElementStates();

                  ReadyState ready = gameFileManager.getReadyState();

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

                  SetStartButtonState(ready);
                  SetMissingFoldersOrExeWarningState(ready);
                  SetWarningIconStates(ready);

                  PadInfoIcons();

                  if (overwriteStatusDisplayText) {
                        this.statusDisplay.Text = GetReadyStatusDisplayText(ready);
                  }

                  if (allowStatusDisplayTextCorruption) {
                        this.statusDisplay.Text = this.textCorruptor.CorruptText(this.statusDisplay.Text);
                  }
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

                        this.gameFolderWarningIcon.Visible = true;

                        if (ready.HasFlag(ReadyState.FolderNotFound)) {
                              this.gameFolderWarningIcon.Image = Resources.Error;
                        } else {
                              this.gameFolderWarningIcon.Image = Resources.Warning;
                        }

                        string tooltipText = GetReadyStatusDisplayText(ready);
                        this.toolTip.SetToolTip(this.gameFolderWarningIcon, tooltipText);

                  } else {
                        this.gameFolderWarningIcon.Visible = false;
                  }
            }

            private void SetWarningIconStates(ReadyState ready) {
                  bool newState = ready.HasFlag(ReadyState.NoSettingsEnabled) ? true : false;

                  this.iconTextureWarningIcon.Visible = newState;
                  this.gameTextureWarningIcon.Visible = newState;
                  this.fontRandWarningIcon.Visible = newState;

                  this.toolTip.SetToolTip(this.iconTextureWarningIcon, GetReadyStatusDisplayText(ReadyState.NoSettingsEnabled));
                  this.toolTip.SetToolTip(this.gameTextureWarningIcon, GetReadyStatusDisplayText(ReadyState.NoSettingsEnabled));
                  this.toolTip.SetToolTip(this.fontRandWarningIcon, GetReadyStatusDisplayText(ReadyState.NoSettingsEnabled));

                  this.seedInfoIcon.Visible = config.seed == 0;
            }

            private void PadInfoIcons() {
                  RightAlignImage(iconTextureTypeLabel, iconTextureWarningIcon);
                  RightAlignImage(gameTextureTypeLabel, gameTextureWarningIcon);
                  RightAlignImage(fontRandEnabledCheckbox, fontRandWarningIcon);

                  RightAlignImage(randomisationSeedLabel, seedInfoIcon);

                  RightAlignImage(gameFolderLabel, gameFolderWarningIcon);
            }

            private void RightAlignImage(Label label, PictureBox pb) {
                  pb.Location = new Point(label.Location.X + label.PreferredWidth, pb.Location.Y);
            }

            private void RightAlignImage(CheckBox checkBox, PictureBox pb) {
                  pb.Location = new Point(checkBox.Location.X + checkBox.Width, pb.Location.Y);
            }

            private string GetReadyStatusDisplayText(ReadyState ready, bool corrupt = true) {
                  string ret = ready.GetMessageString();

                  if (corrupt) {
                        ret = this.textCorruptor.CorruptText(ret);
                  }

                  return ret;
            }

            private void SetAllIconTexturesElements() {
                  if (config.iconTextures.GetEnabledSettingsCount() == 0) {
                        config.iconTextures.enabled = false;
                  }

                  SetCheckboxAndGroupDisplayPairStates(IconTexturesCheckbox, IconTexturesGroupDisplay, config.iconTextures);

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

                  SetCheckboxAndGroupDisplayPairStates(CubeTexturesCheckbox, CubeTexturesGroupDisplay, config.iconTextures.Cube);
                  SetCheckboxAndGroupDisplayPairStates(ShipTexturesCheckbox, ShipTexturesGroupDisplay, config.iconTextures.Ship);
                  SetCheckboxAndGroupDisplayPairStates(BallTexturesCheckbox, BallTexturesGroupDisplay, config.iconTextures.Ball);
                  SetCheckboxAndGroupDisplayPairStates(UFO_TexturesCheckbox, UFO_TexturesGroupDisplay, config.iconTextures.Ufo);
                  SetCheckboxAndGroupDisplayPairStates(WaveTexturesCheckbox, WaveTexturesGroupDisplay, config.iconTextures.Wave);
                  SetCheckboxAndGroupDisplayPairStates(RobotTexturesCheckbox, RobotTexturesGroupDisplay, config.iconTextures.Robot);
                  SetCheckboxAndGroupDisplayPairStates(SpiderTexturesCheckbox, SpiderTexturesGroupDisplay, config.iconTextures.Spider);
                  SetCheckboxAndGroupDisplayPairStates(SwingTexturesCheckbox, SwingTexturesGroupDisplay, config.iconTextures.Swing);
                  SetCheckboxAndGroupDisplayPairStates(JetpackTexturesCheckbox, JetpackTexturesGroupDisplay, config.iconTextures.Jetpack);
            }

            private void SetAllFontRandElementStates() {
                  FontRandomisationSettings fontRand = config.fontRand;

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

            private void SetAllGameTexturesElementStates() {
                  SetCheckboxAndGroupDisplayPairStates(MenuTexturesCheckbox, MenuTexturesGroupDisplay, config.menuTextures);
                  SetCheckboxAndGroupDisplayPairStates(ShopTexturesCheckbox, ShopTexturesGroupDisplay, config.shopTextures);
                  SetCheckboxAndGroupDisplayPairStates(EditorTexturesCheckbox, EditorTexturesGroupDisplay, config.editorTextures);
                  SetCheckboxAndGroupDisplayPairStates(BlockTexturesCheckbox, TileTexturesGroupDisplay, config.tileTextures);
                  SetCheckboxAndGroupDisplayPairStates(PortalTexturesCheckbox, PortalTexturesGroupDisplay, config.portalTextures);
                  SetCheckboxAndGroupDisplayPairStates(OrbsCheckbox, OrbsGroupDisplay, config.orbTextures);
                  SetCheckboxAndGroupDisplayPairStates(PadsCheckbox, PadsGroupDisplay, config.padTextures);
                  SetCheckboxAndGroupDisplayPairStates(ParticleTexturesCheckbox, ParticleTexturesGroupDisplay, config.particleTextures);
                  SetCheckboxAndGroupDisplayPairStates(EffectsCheckbox, EffectsGroupDisplay, config.effectTextures);
                  SetCheckboxAndGroupDisplayPairStates(MiscCheckbox, MiscGroupDisplay, config.miscTextures);
            }

            private void SetCheckboxAndGroupDisplayPairStates(CheckBox checkbox, NumericUpDown groupDisplay, RandomisationSetting setting) {
                  checkbox.Checked = setting.enabled;
                  groupDisplay.Value = setting.group;
                  groupDisplay.Enabled = setting.enabled;
            }

            private void SaveConfigFileAfterDelay(int seconds = 0) {
                  Task.Delay(new TimeSpan(0, 0, seconds)).ContinueWith(o => { config.WriteFile(); });
            }

            #region Randomization and File Restoration

            private async void StartButton_Click(object sender, EventArgs e) {
                  bool ready = gameFileManager.getReadyState() == ReadyState.Ready;
                  if (ready == false)
                        return;

                  SetUI_EnabledState(false);

                  ChangeProgressDisplayState(enabled: true);

                  bool randomSeed = false;
                  // Create a new random seed if the input value is 0
                  int seed = config.seed;
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

                  RefreshUI(false, true);

                  ChangeProgressDisplayState(enabled: false);

                  // Build the status display's string in multiple parts
                  StringBuilder statusText = new StringBuilder("Randomisation complete in " +
                        stopwatch.GetElapsedTimeFormatted(TimeExtension.TimeFormat.HH_MM_SS, true));

                  if (randomSeed) {
                        statusText.Append(". The seed was ");
                        this.textCorruptor.CorruptText(statusText);

                        // Make sure we don't corrupt the seed value. It may be funny, but... well, it is kinda funny, actually
                        statusText.Append(seed.ToString("N0"));
                  }

                  statusText.Append(this.textCorruptor.CorruptText(". You can close GDR"));

                  this.statusDisplay.Text = statusText.ToString();

                  // If the game directory is valid, enable the restore button
                  this.restoreFilesButton.Enabled = gameFileManager.IsGameDirectoryValid();
            }

            private async void RestoreFilesButton_Click(object sender, EventArgs e) {
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

                  this.statusDisplay.Text = "File Restoration Complete";

                  ChangeProgressDisplayState(enabled: false);
            }

            #endregion

            /// <summary>
            /// Sets the enabled state of every element that directly affects the randomisation
            /// </summary>
            private void SetUI_EnabledState(bool enabled) {
                  for (int i = 0; i < controlsToggledDuringRandomisation.Count; i++) {
                        controlsToggledDuringRandomisation[i].Enabled = enabled;
                  }

                  this.importConfigButton.Enabled = enabled;
                  this.restoreFilesButton.Enabled = enabled;
                  this.startButton.Enabled = enabled;
            }

            private void ChangeProgressDisplayState(bool enabled) {
                  // Disable the UI elements that are not needed during the randomisation
                  this.statusDisplay.Visible = !enabled;

                  // Enable the progress display elements
                  this.elapsedTimeDisplay.Visible = enabled;
                  this.randomisingProgressBar.Visible = enabled;
                  this.randomisingProgressDisplay.Visible = enabled;
            }

            private void UpdateProgressStateObjects(string newDisplayPrint) {

                  this.randomisingProgressDisplay.Text = this.textCorruptor.CorruptText(newDisplayPrint);
                  this.randomisingProgressBar.Value = (int)gameFileManager.progressState.PercentComplete;
            }

            private void UpdateProgressElapsedTime(string newTimePrint) {

                  this.elapsedTimeDisplay.Text = this.textCorruptor.CorruptText(newTimePrint);
                  // Right-align the text so it will be near the Randomise button
                  this.elapsedTimeDisplay.Location = new Point(735 - this.elapsedTimeDisplay.PreferredWidth, this.elapsedTimeDisplay.Location.Y);
            }

            private void SetTheme() {
                  if (this.themeController.Current.Name == ThemeController.RandomThemeName) {
                        // Generate an entirely random theme because why not
                        Theme randomTheme = Theme.CreateRandom();
                        this.themeController.Current.CopyColoursFrom(randomTheme);
                        SetTheme(randomTheme);

                        Log.Write(Log.Mode.Info,
                              $"New random theme:\n\t" +
                              $"Background:  {randomTheme.BackgroundColour}\n\t" +
                              $"Text:        {randomTheme.TextColour}\n\t" +
                              $"Object Back: {randomTheme.ObjectBackColour}\n\t" +
                              $"Object Text: {randomTheme.ObjectTextColour}\n\t" +
                              $"Beam Colour: {randomTheme.BeamColour}"
                        );

                  } else if (this.themeController.Current.Name == ThemeController.SystemThemeName) {
                        Theme systemTheme = new Theme(ThemeController.SystemThemeName);

                        Color systemColour = ColorExtensions.Convert(GetSysColor(26));

                        systemTheme.BackgroundColour = systemColour.AdjustBrightness(0.7f);
                        systemTheme.TextColour = systemColour.AdjustBrightness(1.5f);
                        systemTheme.ObjectBackColour = systemColour.AdjustBrightness(0.5f);
                        systemTheme.ObjectTextColour = systemColour.AdjustBrightness(1.2f);
                        systemTheme.BeamColour = systemColour.AdjustBrightness(1.0f);

                        this.themeController.Current.CopyColoursFrom(systemTheme);
                        SetTheme(systemTheme);

                        Log.Write(Log.Mode.Info,
                              $"New system theme:\n\t" +
                              $"Background:  {systemTheme.BackgroundColour}\n\t" +
                              $"Text:        {systemTheme.TextColour}\n\t" +
                              $"Object Back: {systemTheme.ObjectBackColour}\n\t" +
                              $"Object Text: {systemTheme.ObjectTextColour}\n\t" +
                              $"Beam Colour: {systemTheme.BeamColour}"
                        );

                  } else {
                        SetTheme(this.themeController.Current);
                  }
            }

            private void SetTheme(Theme theme) {
                  SetFormColours(theme.BackgroundColour);

                  Color activeColour = theme.TextColour;
                  SetTextColours(activeColour);
                  SetCheckboxColours(activeColour);
                  SetRadioButtonColours(activeColour);

                  SetMenuElementColours(theme.ObjectBackColour, theme.ObjectTextColour);
                  ChangeGroupBoxColours(theme.TextColour);

                  UpdateImageThemes(theme);
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
            }

            private void ChangeGroupBoxColours(Color colour) {
                  for (int i = 0; i < groupBoxes.Length; i++) {
                        groupBoxes[i].ForeColor = colour;
                  }
            }

            private void UpdateImageThemes(Theme theme) {
                  UpdateIconTheme(Resources.RefreshImage, theme,
                        new PictureBox[] { refreshThemesButton });

                  UpdateIconTheme(Resources.SettingsImage, theme,
                        new PictureBox[] { themesSettingsButton });

                  UpdateIconTheme(Resources.ConnectorBeamWhite, theme,
                        new PictureBox[] { iconsConnectorBeam, shuffleFontsConnectorBeam });
            }

            private void UpdateIconTheme(Bitmap baseImage, Theme theme, PictureBox[] boxes) {
                  Bitmap recolouredImage = ((Bitmap)baseImage.Clone())
                        .BlackAndWhiteRecolour(theme.BackgroundColour, theme.BeamColour);

                  for (int i = 0; i < boxes.Length; i++) {
                        boxes[i].Image = recolouredImage;
                  }
            }

            private void GDR_HeaderLabel_Click(object sender, EventArgs e) {
                  textCorruptor.CorruptionLevel++;

                  // Permanently alter the text of every element that stores text
                  for (int i = 0; i < this.labels.Length; i++) {
                        this.labels[i].Text = this.textCorruptor.CorruptText(this.labels[i].Text, 1);
                  }

                  for (int i = 0; i < this.checkBoxes.Length; i++) {
                        this.checkBoxes[i].Text = this.textCorruptor.CorruptText(this.checkBoxes[i].Text, 1);
                  }

                  for (int i = 0; i < this.groupBoxes.Length; i++) {
                        this.groupBoxes[i].Text = this.textCorruptor.CorruptText(this.groupBoxes[i].Text, 1);
                  }

                  for (int i = 0; i < this.buttons.Length; i++) {
                        this.buttons[i].Text = this.textCorruptor.CorruptText(this.buttons[i].Text, 1);
                  }

                  for (int i = 0; i < this.radioButtons.Length; i++) {
                        this.radioButtons[i].Text = this.textCorruptor.CorruptText(this.radioButtons[i].Text, 1);
                  }

                  PadInfoIcons();
            }

            private void ChangeConfigData(object sender, RandomisationSetting setting, Control toggledControl) {
                  ChangeConfigData(sender, setting);

                  toggledControl.Enabled = setting.enabled;
            }

            private void ChangeConfigData(object sender, RandomisationSetting setting) {
                  if (sender is CheckBox) {
                        ChangeConfigData(sender as CheckBox, setting);

                  } else if (sender is NumericUpDown) {
                        ChangeConfigData(sender as NumericUpDown, setting);

                  } else {
                        Log.Write(Log.Mode.Warn, $"Sender is not a supported type for changing properties of {setting}");
                  }
            }

            private void ChangeConfigData(CheckBox checkBox, RandomisationSetting setting) {
                  setting.enabled = checkBox.Checked;
            }

            private void ChangeConfigData(NumericUpDown numUpDown, RandomisationSetting setting) {
                  setting.group = (int)numUpDown.Value;
            }

            #region Icon Texture Settings Changed Events

            private void IconTexturesSettingsChanged(object sender, EventArgs e) {
                  bool state = this.IconTexturesCheckbox.Checked;

                  config.iconTextures.enabled = state;

                  if (isFormInitialised == false) {
                        return;
                  }

                  config.iconTextures.Cube.enabled = state;
                  config.iconTextures.Ship.enabled = state;
                  config.iconTextures.Ball.enabled = state;
                  config.iconTextures.Ufo.enabled = state;
                  config.iconTextures.Wave.enabled = state;
                  config.iconTextures.Robot.enabled = state;
                  config.iconTextures.Spider.enabled = state;
                  config.iconTextures.Swing.enabled = state;
                  config.iconTextures.Jetpack.enabled = state;

                  RefreshUI();
            }

            private void IconTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  if (numericUpDown.Value > 100) {
                        numericUpDown.Value = 100;
                  }

                  config.iconTextures.group = (int)numericUpDown.Value;

                  if (isFormInitialised == false) {
                        return;
                  }

                  CubeTexturesSettingsChanged(sender, e);
                  ShipTexturesSettingsChanged(sender, e);
                  BallTexturesSettingsChanged(sender, e);
                  UFO_TexturesSettingsChanged(sender, e);
                  WaveTexturesSettingsChanged(sender, e);
                  RobotTexturesSettingsChanged(sender, e);
                  SpiderTexturesSettingsChanged(sender, e);
                  SwingTexturesSettingsChanged(sender, e);
                  JetpackTexturesSettingsChanged(sender, e);

                  RefreshUI();
            }

            private void CubeTexturesSettingsChanged(object sender, EventArgs e) => ChangeConfigData(sender, config.iconTextures.Cube);

            private void ShipTexturesSettingsChanged(object sender, EventArgs e) => ChangeConfigData(sender, config.iconTextures.Ship);

            private void BallTexturesSettingsChanged(object sender, EventArgs e) => ChangeConfigData(sender, config.iconTextures.Ball);

            private void UFO_TexturesSettingsChanged(object sender, EventArgs e) => ChangeConfigData(sender, config.iconTextures.Ufo);

            private void WaveTexturesSettingsChanged(object sender, EventArgs e) => ChangeConfigData(sender, config.iconTextures.Wave);

            private void RobotTexturesSettingsChanged(object sender, EventArgs e) => ChangeConfigData(sender, config.iconTextures.Robot);

            private void SpiderTexturesSettingsChanged(object sender, EventArgs e) => ChangeConfigData(sender, config.iconTextures.Spider);

            private void SwingTexturesSettingsChanged(object sender, EventArgs e) => ChangeConfigData(sender, config.iconTextures.Swing);

            private void JetpackTexturesSettingsChanged(object sender, EventArgs e) => ChangeConfigData(sender, config.iconTextures.Jetpack);

            #endregion

            #region Game Texture Settings Changed Events

            private void MenuTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.menuTextures, MenuTexturesGroupDisplay);

            private void ShopTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.shopTextures, ShopTexturesGroupDisplay);

            private void EditorTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.editorTextures, EditorTexturesGroupDisplay);

            private void TilesTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.tileTextures, TileTexturesGroupDisplay);

            private void PortalTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.portalTextures, PortalTexturesGroupDisplay);

            private void OrbsTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.orbTextures, OrbsGroupDisplay);

            private void PadsTexturesSettingChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.padTextures, PadsGroupDisplay);

            private void ParticleTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.particleTextures, ParticleTexturesGroupDisplay);

            private void EffectsTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.effectTextures, EffectsGroupDisplay);

            private void MiscTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.miscTextures, MiscGroupDisplay);

            #endregion

            #region Font Randomisation Settings Controls

            private void FontRandEnabledCheckbox_Click(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  config.fontRand.enabled = checkBox.Checked;
                  RefreshUI();
            }

            private void FontShuffleStylesCheckbox_Click(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  config.fontRand.shuffleFontStyles = checkBox.Checked;
                  RefreshUI();
            }

            private void FontPerFontRandomisationButton_Click(object sender, EventArgs e) {
                  config.fontRand.shufflingMode = FontRandomisationSettings.FontStyleShufflingMode.PerFont;
                  RefreshUI();
            }

            private void FontPerLetterRandomisationButton_Click(object sender, EventArgs e) {
                  config.fontRand.shufflingMode = FontRandomisationSettings.FontStyleShufflingMode.PerLetter;
                  RefreshUI();
            }

            private void FontRandomiseLettersCheckbox_Click(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  config.fontRand.randomiseLetters = checkBox.Checked;
                  RefreshUI();
            }

            #endregion

            #region Form Opening Methods

            private void Logo_Click(object sender, EventArgs e) {
                  if (creditsForm == null) {
                        creditsForm = new CreditsForm();
                  }

                  creditsForm.theme = this.CurrentTheme;
                  creditsForm.textCorruptor = this.textCorruptor;
                  creditsForm.Show();

                  return;
            }

            private void ShowImportConfigForm(object sender, EventArgs e) {
                  if (importConfigForm == null) {
                        importConfigForm = new ImportConfigForm();

                        this.importConfigForm.ConfigDataChanged += (s, data, ev) => {
                              ImportConfigThenRefreshUI(data);
                        };
                  }

                  importConfigForm.textCorruptor = this.textCorruptor;
                  importConfigForm.theme = this.CurrentTheme;
                  importConfigForm.Show();
            }

            private void ShowExportConfigForm(object sender, EventArgs e) {
                  if (exportConfigForm == null) {
                        exportConfigForm = new ExportConfigForm();
                  }

                  exportConfigForm.textCorruptor = this.textCorruptor;
                  exportConfigForm.theme = this.CurrentTheme;
                  exportConfigForm.Show();
            }

            private void ChangelogButton_Click(object sender, EventArgs e) {
                  if (changelogForm == null) {
                        changelogForm = new ChangelogForm();
                  }

                  changelogForm.theme = this.CurrentTheme;
                  changelogForm.textCorruptor = this.textCorruptor;
                  changelogForm.Show();
            }

            private void ThemeConfigButton_Click(object sender, EventArgs e) {
                  if (themeConfigForm == null) {
                        themeConfigForm = new ThemeConfigForm();
                  }

                  themeConfigForm.theme = this.CurrentTheme;
                  themeConfigForm.textCorruptor = this.textCorruptor;
                  themeConfigForm.Show();
            }

            #endregion

            private void ImportConfigThenRefreshUI(string configData) {
                  bool success = config.ImportConfigData(configData);

                  if (success == true) {
                        this.statusDisplay.Text = "Config Imported Successfully";
                  } else {
                        this.statusDisplay.Text = "Failed to Import Config.";
                  }

                  RefreshUI(false);
            }

            private IEnumerable<Control> GetAll(Control control, Type type) {
                  var controls = control.Controls.Cast<Control>();

                  return controls
                        .SelectMany(ctrl => GetAll(ctrl, type))
                        .Concat(controls)
                        .Where(c => c.GetType() == type);
            }

            private IEnumerable<Control> GetAll(Control[] controls, Type[] types) {
                  List<Control> ret = new List<Control>();

                  for (int i = 0; i < controls.Length; i++) {
                        ret.AddRange(GetAll(controls[i], types));
                  }

                  return ret;
            }

            private IEnumerable<Control> GetAll(Control control, Type[] types) {
                  Control[] controls = control.Controls.Cast<Control>().ToArray();
                  List<Control> ret = new List<Control>();

                  for (int i = 0; i < controls.Length; i++) {
                        for (int type = 0; type < types.Length; type++) {

                              if (controls[i].GetType() == types[type]) {
                                    ret.Add(controls[i]);
                                    break;
                              }
                        }
                  }

                  return ret;
            }

            #region Randomisation Settings Control Methods

            private void SpriteSizeMultiplierTrackbar_Scroll(object sender, EventArgs e) {
                  TrackBar trackBar = sender as TrackBar;

                  config.maxSpriteMultiplier = GetSliderMultiplierForSpriteSize(trackBar.Value);

                  RefreshUI();
            }

            private float GetSliderMultiplierForSpriteSize(int sliderValue) {
                  if (sliderValue <= 50) {
                        return (float)sliderValue / 100 + 1f; // Values between 1.01 and 1.5

                  } else if (sliderValue <= 75) {
                        return (float)(sliderValue - 50) / 50 + 1.5f; // Values between 1.5 and 2

                  } else if (sliderValue <= 100) {
                        return (float)(sliderValue - 75) / 25 + 2f; // Values between 2 and 3

                  } else if (sliderValue <= 170) {
                        return (float)(sliderValue - 100) / 10 + 3f; // Values between 3 and 10

                  } else if (sliderValue <= 220) {
                        return (float)(sliderValue - 170) / 5 + 10f; // Values between 10 and 20

                  } else if (sliderValue <= 280) {
                        return (float)(sliderValue - 220) / 2 + 20f; // Values between 20 and 50

                  } else if (sliderValue <= 330) {
                        return (float)(sliderValue - 280) / 1 + 50f; // Values between 50 and 100

                  } else {
                        return 1000f;
                  }
            }

            private void SetSpriteSizeMultiplierSliderAndTextBox() {
                  float[] multipliers = Enumerable.Range(1, this.spriteSizeMultiplierTrackbar.Maximum + 1)
                        .Select(v => GetSliderMultiplierForSpriteSize(v))
                        .ToArray();

                  int index = Array.BinarySearch(multipliers, config.maxSpriteMultiplier);
                  if (index >= 0) {
                        this.spriteSizeMultiplierTrackbar.Value = index;
                  }
            }

            private void AllowDuplicatesCheckbox_Click(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  config.allowDuplicates = checkBox.Checked;

                  RefreshUI();
            }

            private void SeedValueChanged(object sender, EventArgs e) {
                  NumericUpDown nud = sender as NumericUpDown;
                  config.seed = (int)nud.Value;
                  RefreshUI();
            }

            private void RandomSeedButton_Click(object sender, EventArgs e) {
                  Random random = new Random(Guid.NewGuid().GetHashCode());
                  int value = random.Next(int.MinValue, int.MaxValue);
                  this.seedInputBox.Value = value;
                  config.seed = value;

                  RefreshUI();
            }

            #endregion

            #region Application Settings Control Methods

            private void SetGameFolder(object sender, EventArgs e) {
                  string folder = GetFolderViaExplorer(config.gameDirectory, true);
                  if (folder != string.Empty) {
                        config.gameDirectory = folder;
                  }

                  RefreshUI();
            }

            private void GameFolderTextBox_TextChanged(object sender, EventArgs e) {
                  TextBox textBox = sender as TextBox;
                  config.gameDirectory = textBox.Text;
                  RefreshUI();
            }

            private string GetFolderViaExplorer(string InitialDirectory, bool IsFolderPicker) {
                  CommonOpenFileDialog dialog = new CommonOpenFileDialog {
                        InitialDirectory = InitialDirectory,
                        IsFolderPicker = IsFolderPicker
                  };
                  if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                        return dialog.FileName;
                  }
                  return string.Empty;
            }

            private void AutoOverwriteFilesCheckbox_Click(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  config.autoOverwriteFiles = checkBox.Checked;
            }

            private void ChangeTextureQuality(object sender, EventArgs e) {
                  DomainUpDown domainUpDown = sender as DomainUpDown;
                  if (domainUpDown == null) {
                        return;
                  }

                  config.quality = (Quality)domainUpDown.SelectedIndex;
            }

            private void ChangeApplicationTheme(object sender, EventArgs e) {
                  if (this.lastThemeRefresh.GetElapsedTime().TotalMilliseconds < themeRefreshCooldown) {
                        return;
                  }

                  DomainUpDown domainUpDown = sender as DomainUpDown;
                  int newThemeID = domainUpDown.SelectedIndex;

                  this.themeController.ActiveThemeID = newThemeID;
                  config.themeID = newThemeID;

                  Log.Write(Log.Mode.Verbose, $"Switching to theme ID {newThemeID}: {themeController.Current.Name}");

                  SetTheme();
            }

            private void RefreshThemesButton_Click(object sender, EventArgs e) {
                  this.statusDisplay.Text = this.textCorruptor.CorruptText("Refreshing themes...");

                  RefreshThemes();

                  this.statusDisplay.Text = $"Themes Refreshed. Found {this.themeController.ThemeCount} themes after refreshing.";
            }

            private void RefreshThemes(bool animate = true, bool setTheme = true) {
                  if (this.lastThemeRefresh.GetElapsedTime().TotalMilliseconds < themeRefreshCooldown) {
                        return;
                  }
                  this.lastThemeRefresh = DateTime.UtcNow;

                  this.themeController.GetAllThemesFromFile();

                  if (this.themeController.GetThemeCount() <= config.themeID) {
                        Log.Write(Log.Mode.Warn, $"The theme ID [{config.themeID}] is out of range");

                        config.themeID = 0;
                  }

                  this.themeController.ActiveThemeID = config.themeID;

                  this.applicationThemeSelectorBox.Items.Clear();
                  this.applicationThemeSelectorBox.Items.AddRange(this.themeController.GetAllThemeNames());

                  if (setTheme) {
                        SetTheme();
                  }

                  if (animate) {
                        AnimateImageRotation(
                              this.refreshThemesButton,
                              Resources.RefreshImage.BlackAndWhiteRecolour(
                                    themeController.Current.BackgroundColour,
                                    themeController.Current.BeamColour
                                    ),
                              540
                        );
                  }
            }

            #endregion

            #region Container Initialiser Methods

            private void PopulateQualityDropdown() {
                  this.textureQualitySelectorBox.Items.Clear();

                  this.textureQualitySelectorBox.Items.Add(PathManager.highQualityName);
                  this.textureQualitySelectorBox.Items.Add(PathManager.mediumQualityName);
                  this.textureQualitySelectorBox.Items.Add(PathManager.lowQualityName);
            }

            /// <summary>
            /// This populates all Lists with Controls and other similar items to help simplify code.<br/>
            /// This should only be called once
            /// </summary>
            internal void InitialiseControlContainers() {

                  // Check if this method has already run. If so, short circuit.
                  if (dataArraysArePopulated == true) {
                        return;
                  }

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

                  controlsToggledDuringRandomisation.AddRange(
                        GetAll(
                              new Control[] {
                                    iconTextureContainer,
                                    gameTextureContainer,
                                    fontRandomisationSettingsContainer,
                                    randomisationSettingsContainer,
                                    applicationSettingsContainer
                              },
                              new Type[] {
                                    typeof(NumericUpDown),
                                    typeof(DomainUpDown),
                                    typeof(RadioButton),
                                    typeof(CheckBox),
                                    typeof(TrackBar),
                                    typeof(TextBox),
                                    typeof(Button)
                              }
                        )
                  );

                  // Finally, signal that the arrays are populated
                  dataArraysArePopulated = true;
            }

            #endregion

            #region Animations And Related Methods

            private void AnimateImageRotation(PictureBox pb, Bitmap image, float rotationSpeed, Action onAnimationComplete = null, int updateInterval = 25) {
                  float elapsed = 0f;

                  System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Interval = updateInterval };

                  timer.Tick += (s, e) => {
                        elapsed += updateInterval;
                        float angle = elapsed / 1000 * rotationSpeed;

                        if (angle >= 360) {
                              timer.Stop();
                              onAnimationComplete?.Invoke();
                              pb.Image = image;

                        } else {
                              pb.Image = image.RotateImage(angle);
                        }
                  };

                  timer.Start();
            }

            #endregion
      }
}