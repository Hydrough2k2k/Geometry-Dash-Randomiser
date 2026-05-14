using Geometry_Dash_Randomiser.Properties;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Geometry_Dash_Randomiser.Config;

namespace Geometry_Dash_Randomiser {

      public partial class GDR_Form : Form {

            public GDR_Form() {
                  InitializeComponent();

#if DEBUG
                  AllocConsole();
                  Control_AllocConsole();
#endif

                  Config.ReadFile();

                  if (config.debugMode) {
                        AllocConsole();
                        Control_AllocConsole();
                  }

                  this.gameFileManager = new GameFileManager(this);

                  RefreshThemes(animate: false);

                  // If the game directory is valid, enable the restore button
                  this.restoreFilesButton.Enabled = gameFileManager.IsGameDirectoryValid();
            }

            private void GDR_Form_Shown(object sender, EventArgs e) {
                  Debug.WriteLine($"Total thread count: {Environment.ProcessorCount}");

                  this.versionLabel.Text = version;

                  InitialiseControlContainers();

                  PopulateQualityDropdown();

                  SetTheme();

                  RefreshUI();
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
                  SetNoSettingsWarningState(ready);

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

                        this.gameFolderWarningImage.Visible = true;

                        if (ready.HasFlag(ReadyState.FolderNotFound)) {
                              this.gameFolderWarningImage.Image = Resources.Error;
                        } else {
                              this.gameFolderWarningImage.Image = Resources.Warning;
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
                  string ret;

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
                  SetCheckboxAndGroupDisplayPairStates(BlockTexturesCheckbox, BlockTexturesGroupDisplay, config.tileTextures);
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

                  this.importConfigButton.Enabled = enabled;
            }

            private void ChangeProgressDisplayState(bool enabled) {
                  // Disable the UI elements that are not needed during the randomisation
                  this.statusDisplay.Visible = !enabled;

                  // Enable the progress display elements
                  this.elapsedTimeDisplay.Visible = enabled;
                  this.randomisingProgressBar.Visible = enabled;
                  this.randomisingProgressDisplay.Visible = enabled;
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
                        this.textCorruptor.CorruptStringBuilder(statusText);

                        // Make sure we don't corrupt the seed value. It may be funny, but... well, it is kinda funny, actually
                        statusText.Append(seed.ToString("N0"));
                  }

                  statusText.Append(this.textCorruptor.CorruptText(". You can close GDR"));

                  this.statusDisplay.Text = statusText.ToString();

                  // If the game directory is valid, enable the restore button
                  this.restoreFilesButton.Enabled = gameFileManager.IsGameDirectoryValid();
            }

            private void UpdateProgressStateObjects(string newDisplayPrint) {

                  this.randomisingProgressDisplay.Text = this.textCorruptor.CorruptText(newDisplayPrint);
                  this.randomisingProgressBar.Value = (int)Math.Round(gameFileManager.progressState.percentComplete);
            }

            private void UpdateProgressElapsedTime(string newTimePrint) {

                  this.elapsedTimeDisplay.Text = this.textCorruptor.CorruptText(newTimePrint);
                  // Right-align the text so it will be near the Randomise button
                  this.elapsedTimeDisplay.Location = new Point(735 - this.elapsedTimeDisplay.PreferredWidth, this.elapsedTimeDisplay.Location.Y);
            }

            private void SeedValueChanged(object sender, EventArgs e) {
                  NumericUpDown nud = sender as NumericUpDown;
                  config.seed = (int)nud.Value;
                  RefreshUI();
            }

            private void SetTheme() {
                  if (this.themeController.Current.Name != "Random Theme") {
                        SetTheme(this.themeController.Current);

                  } else {
                        // Generate an entirely random theme because why not
                        Theme randomTheme = Theme.CreateRandom();
                        this.themeController.Current.CopyColoursFrom(randomTheme);

                        SetTheme(randomTheme);

                        Console.WriteLine($"New random theme:\n" +
                              $"Background:  {randomTheme.BackgroundColour}\n" +
                              $"Text:        {randomTheme.TextColour}\n" +
                              $"Object Back: {randomTheme.ObjectBackColour}\n" +
                              $"Object Text: {randomTheme.ObjectTextColour}\n" +
                              $"Beam Colour: {randomTheme.BeamColour}\n"
                        );
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

                  UpdateImageTheme(theme);

                  UpdateImageThemes();
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

            private void UpdateImageTheme(Theme theme) {
                  Color beamColour = theme.BeamColour;
                  Bitmap connectorBeam = (Bitmap)Resources.ConnectorBeamWhite.Clone();

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

            private void UpdateImageThemes() {
                  // Update theme for the refresh button
                  Bitmap newRefreshButton = (Bitmap)Resources.RefreshImage.Clone();
                  refreshThemesButton.Image = newRefreshButton.BlackAndWhiteRecolour(themeController.Current.BackgroundColour, themeController.Current.BeamColour); ;
            }

            private void ApplicationThemeSelectorBox_Click(object sender, EventArgs e) {
                  if (this.lastThemeRefresh.GetElapsedTime().TotalMilliseconds < themeRefreshCooldown) {
                        return;
                  }

                  DomainUpDown domainUpDown = sender as DomainUpDown;
                  int newThemeID = domainUpDown.SelectedIndex;

                  this.themeController.ActiveThemeID = newThemeID;
                  config.themeID = newThemeID;

                  Console.WriteLine($"Switching to theme ID {newThemeID}: {themeController.Current.Name}");

                  SetTheme();
            }

            private void GDR_HeaderLabel_Click(object sender, EventArgs e) {
                  textCorruptor.CorruptionLevel++;

                  // Permanently alter the text of every element that stores text
                  for (int i = 0; i < this.labels.Length; i++) {
                        this.labels[i].Text = this.textCorruptor.CorruptText(this.labels[i].Text);
                  }

                  for (int i = 0; i < this.checkBoxes.Length; i++) {
                        this.checkBoxes[i].Text = this.textCorruptor.CorruptText(this.checkBoxes[i].Text);
                  }

                  for (int i = 0; i < this.groupBoxes.Length; i++) {
                        this.groupBoxes[i].Text = this.textCorruptor.CorruptText(this.groupBoxes[i].Text);
                  }

                  for (int i = 0; i < this.buttons.Length; i++) {
                        this.buttons[i].Text = this.textCorruptor.CorruptText(this.buttons[i].Text);
                  }

                  for (int i = 0; i < this.radioButtons.Length; i++) {
                        this.radioButtons[i].Text = this.textCorruptor.CorruptText(this.radioButtons[i].Text);
                  }

                  RefreshUI();
            }

            private void ChangeConfigData(object sender, RandomisationSetting setting) => ChangeConfigData(sender, setting, refreshUI: true);

            private void ChangeConfigData(object sender, RandomisationSetting setting, bool refreshUI) {
                  if (sender is CheckBox) {
                        ChangeConfigDataFromCheckbox(sender as CheckBox, setting);

                  } else if (sender is NumericUpDown) {
                        ChangeConfigDataFromNumUpDown(sender as NumericUpDown, setting);

                  } else {
                        throw new Exception($"Sender is not a supported type for changing properties of {setting}");
                  }

                  if (refreshUI) {
                        RefreshUI();
                  }
            }

            private void ChangeConfigDataFromCheckbox(CheckBox checkBox, RandomisationSetting setting) {
                  setting.enabled = checkBox.Checked;
            }

            private void ChangeConfigDataFromNumUpDown(NumericUpDown numUpDown, RandomisationSetting setting) {
                  setting.group = (int)numUpDown.Value;
            }

            private void IconTexturesSettingsChanged(object sender, EventArgs e) {
                  bool enabled = this.IconTexturesCheckbox.Checked;

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
                  config.iconTextures.group = (int)numericUpDown.Value;
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

            private void CubeTexturesSettingsChanged(object sender, EventArgs e) => ChangeConfigData(sender, config.iconTextures.Cube);

            private void CubeTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  config.iconTextures.Cube.enabled = checkBox.Checked;
                  RefreshUI();
            }

            private void CubeTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  config.iconTextures.Cube.group = (int)numericUpDown.Value;
                  RefreshUI();
            }

            private void ShipTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  config.iconTextures.Ship.enabled = checkBox.Checked;
                  RefreshUI();
            }

            private void ShipTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  config.iconTextures.Ship.group = (int)numericUpDown.Value;
                  RefreshUI();
            }

            private void BallTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  config.iconTextures.Ball.enabled = checkBox.Checked;
                  RefreshUI();
            }

            private void BallTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  config.iconTextures.Ball.group = (int)numericUpDown.Value;
                  RefreshUI();
            }

            private void UFO_TexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  config.iconTextures.Ufo.enabled = checkBox.Checked;
                  RefreshUI();
            }

            private void UFO_TexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  config.iconTextures.Ufo.group = (int)numericUpDown.Value;
                  RefreshUI();
            }

            private void WaveTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  config.iconTextures.Wave.enabled = checkBox.Checked;
                  RefreshUI();
            }

            private void WaveTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  config.iconTextures.Wave.group = (int)numericUpDown.Value;
                  RefreshUI();
            }

            private void RobotTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  config.iconTextures.Robot.enabled = checkBox.Checked;
                  RefreshUI();
            }

            private void RobotTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  config.iconTextures.Robot.group = (int)numericUpDown.Value;
                  RefreshUI();
            }

            private void SpiderTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  config.iconTextures.Spider.enabled = checkBox.Checked;
                  RefreshUI();
            }

            private void SpiderTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  config.iconTextures.Spider.group = (int)numericUpDown.Value;
                  RefreshUI();
            }

            private void SwingTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  config.iconTextures.Swing.enabled = checkBox.Checked;
                  RefreshUI();
            }

            private void SwingTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  config.iconTextures.Swing.group = (int)numericUpDown.Value;
                  RefreshUI();
            }

            private void JetpackTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  config.iconTextures.Jetpack.enabled = checkBox.Checked;
                  RefreshUI();
            }

            private void JetpackTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  config.iconTextures.Jetpack.group = (int)numericUpDown.Value;
                  RefreshUI();
            }

            private void MenuTexturesSettingsChanged(object sender, EventArgs e) {
                  config.menuTextures.enabled = this.MenuTexturesCheckbox.Checked;
                  config.menuTextures.group = (int)this.MenuTexturesGroupDisplay.Value;
                  RefreshUI();
            }

            private void ShopTexturesSettingsChanged(object sender, EventArgs e) {
                  config.shopTextures.enabled = this.ShopTexturesCheckbox.Checked;
                  config.shopTextures.group = (int)this.ShopTexturesGroupDisplay.Value;
                  RefreshUI();
            }

            private void EditorTexturesSettingsChanged(object sender, EventArgs e) {
                  config.editorTextures.enabled = this.EditorTexturesCheckbox.Checked;
                  config.editorTextures.group = (int)this.EditorTexturesGroupDisplay.Value;
                  RefreshUI();
            }

            private void TilesTexturesSettingsChanged(object sender, EventArgs e) {
                  config.tileTextures.enabled = this.BlockTexturesCheckbox.Checked;
                  config.tileTextures.group = (int)this.BlockTexturesGroupDisplay.Value;
                  RefreshUI();
            }

            private void PortalTexturesSettingsChanged(object sender, EventArgs e) {
                  config.portalTextures.enabled = this.PortalTexturesCheckbox.Checked;
                  config.portalTextures.group = (int)this.PortalTexturesGroupDisplay.Value;
                  RefreshUI();
            }

            private void OrbsSettingsChanged(object sender, EventArgs e) {
                  config.orbTextures.enabled = this.OrbsCheckbox.Checked;
                  config.orbTextures.group = (int)this.OrbsGroupDisplay.Value;
                  RefreshUI();
            }

            private void PadsSettingChanged(object sender, EventArgs e) {
                  config.padTextures.enabled = this.PadsCheckbox.Checked;
                  config.padTextures.group = (int)this.PadsGroupDisplay.Value;
                  RefreshUI();
            }

            private void ParticleTexturesSettingsChanged(object sender, EventArgs e) {
                  config.particleTextures.enabled = this.ParticleTexturesCheckbox.Checked;
                  config.particleTextures.group = (int)this.ParticleTexturesGroupDisplay.Value;
                  RefreshUI();
            }

            private void EffectsSettingsChanged(object sender, EventArgs e) {
                  config.effectTextures.enabled = this.EffectsCheckbox.Checked;
                  config.effectTextures.group = (int)this.EffectsGroupDisplay.Value;
                  RefreshUI();
            }

            private void MiscSettingsChanged(object sender, EventArgs e) {
                  config.miscTextures.enabled = this.MiscCheckbox.Checked;
                  config.miscTextures.group = (int)this.MiscGroupDisplay.Value;
                  RefreshUI();
            }

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

            private void QualityOptionChanged(object sender, EventArgs e) {
                  DomainUpDown qualityDropdown = sender as DomainUpDown;
                  switch (qualityDropdown.Text) {
                        case PathManager.lowQualityName:
                              config.quality = Quality.Low;
                              break;
                        case PathManager.mediumQualityName:
                              config.quality = Quality.Medium;
                              break;
                        case PathManager.highQualityName:
                              config.quality = Quality.High;
                              break;
                        default:
                              break;
                  }

                  RefreshUI();
            }

            private void RandomSeedButton_Click(object sender, EventArgs e) {
                  Random random = new Random(Guid.NewGuid().GetHashCode());
                  int value = random.Next(int.MinValue, int.MaxValue);
                  this.seedInputBox.Value = value;
                  config.seed = value;

                  RefreshUI();
            }

            private void SpriteSizeMultiplierTrackbar_Scroll(object sender, EventArgs e) {
                  TrackBar trackBar = sender as TrackBar;

                  float newMultiplier;
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

                  config.maxSpriteMultiplier = newMultiplier;

                  RefreshUI();
            }

            private void AllowDuplicatesCheckbox_Click(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  config.allowDuplicates = checkBox.Checked;

                  RefreshUI();
            }

            #region Form Opening Methods

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

            private void Logo_Click(object sender, EventArgs e) {
                  if (creditsForm == null) {
                        creditsForm = new CreditsForm();
                  }

                  creditsForm.theme = this.CurrentTheme;
                  creditsForm.textCorruptor = this.textCorruptor;
                  creditsForm.Show();

                  return;
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

            private void RefreshThemesButton_Click(object sender, EventArgs e) {
                  this.statusDisplay.Text = this.textCorruptor.CorruptText("Refreshing themes...");

                  RefreshThemes();

                  this.statusDisplay.Text = $"Themes Refreshed. Found {this.themeController.ThemeCount} themes after refreshing.";

                  RefreshUI(false);
            }

            private void RefreshThemes(bool animate = true) {
                  if (this.lastThemeRefresh.GetElapsedTime().TotalMilliseconds < themeRefreshCooldown) {
                        return;
                  }
                  this.lastThemeRefresh = DateTime.UtcNow;

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

                  this.themeController.GetAllThemesFromFile();

                  if (this.themeController.GetThemeCount() < config.themeID) {
                        Console.WriteLine($"The theme ID [{config.themeID}] is out of range");

                        config.themeID = 0;
                  }

                  this.themeController.ActiveThemeID = config.themeID;

                  this.applicationThemeSelectorBox.Items.Clear();
                  this.applicationThemeSelectorBox.Items.AddRange(this.themeController.GetAllThemeNames());
            }

            private void PopulateQualityDropdown() {
                  this.textureQualitySelectorBox.Items.Clear();

                  this.textureQualitySelectorBox.Items.Add(PathManager.highQualityName);
                  this.textureQualitySelectorBox.Items.Add(PathManager.mediumQualityName);
                  this.textureQualitySelectorBox.Items.Add(PathManager.lowQualityName);
            }
      }
}