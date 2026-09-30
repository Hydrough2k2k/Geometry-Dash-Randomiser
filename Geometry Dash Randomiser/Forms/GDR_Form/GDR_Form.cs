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
using static Geometry_Dash_Randomiser.Log;

namespace Geometry_Dash_Randomiser {

      public partial class GDR_Form : Form {

            public GDR_Form() {
                  InitializeComponent();

                  WriteSystemAnalysisInfoToConsole();

                  this.gameFileManager = new GameFileManager(this);
                  this.themeController = new ThemeController();

                  RefreshThemes(animate: false, setTheme: false);
            }

            private void FormShown(object sender, EventArgs e) {
                  SetSpriteSizeMultiplierSliderAndTextBox();

                  // If the game directory is valid, enable the restore button
                  this.restoreFilesButton.Enabled = gameFileManager.IsGameDirectoryValid();

                  this.versionLabel.Text = version;

                  InitialiseControlContainers();

                  PopulateQualityDropdown();

                  SetTheme();

                  RefreshUI();

                  Thread cleanerThread = new Thread(() => {
                        Log.CleanUpLogs();
                  });
                  cleanerThread.Start();

                  autoRefreshLinkedControls = true;
            }

            private void WriteSystemAnalysisInfoToConsole() {
                  Log.Write(Log.Mode.Verbose, $"Total thread count: {Environment.ProcessorCount}");
                  Log.Write(Log.Mode.Verbose, $"Is 64bit Process: {Environment.Is64BitProcess}");
                  Log.Write(Log.Mode.Verbose, $"Is 64bit OS: {Environment.Is64BitOperatingSystem}");
                  Log.Write(Log.Mode.Verbose, $"OS Version: {Environment.OSVersion}");
                  Log.Write(Log.Mode.Verbose, $"System Page Size: {Environment.SystemPageSize}");
                  Log.Write(Log.Mode.Verbose, $"CLR Version: {Environment.Version}");
            }

            #region UI Control

            private void RefreshUI(bool overwriteStatusDisplayText = true) {
                  SetAllIconTexturesElementStates();
                  SetAllGameTexturesElementStates();
                  SetAllFontRandElementStates();

                  UpdateRandomisationControlStates();
                  UpdateApplicationSettingsControlStates();

                  ValidateReadyState();

                  RepositionControlPairs();

                  UpdateAllImageThemes();

                  this.statusDisplay.Text = this.textCorruptor.CorruptText(this.statusDisplay.Text);
            }

            private void ValidateReadyState(bool overwriteStatusDisplayText = true) {
                  ToggleAllLinkedControls();

                  ReadyState ready = gameFileManager.getReadyState();
                  SetStartButtonState(ready);
                  SetMissingFoldersOrExeWarningState(ready);
                  SetWarningIconStates(ready);

                  if (overwriteStatusDisplayText) {
                        this.statusDisplay.Text = GetReadyStatusDisplayText(ready);
                  }
            }

            private void ToggleAllLinkedControls() {
                  ToggleIconSettingsLinkedControls();
                  ToggleFontSettingsLinkedControls();
                  ToggleFontLetterRandSettingsLinkedControls();
            }

            private void ToggleIconSettingsLinkedControls() {
                  ToggleLinkedControls(config.IconTextures, IconTexturesCheckbox, IconTexturesGroupDisplay);
            }

            private void ToggleFontSettingsLinkedControls() {
                  ToggleLinkedControls(config.FontRand, fontRandEnabledCheckbox);
            }

            private void ToggleFontLetterRandSettingsLinkedControls() {
                  ToggleLinkedControls(config.FontRand.CharacterRandSettings, randomiseCharactersCheckBox);
            }

            private void ToggleLinkedControls(IToggleableSetting setting, CheckBox parentControl, Control childControl = null) {
                  if (setting == null) {
                        Log.Write(Mode.Error, $"The \"randomisationSetting\" was passed as null to \"ToggleLinkedControls\"");
                        return;
                  }

                  if (parentControl == null) {
                        Log.Write(Mode.Error, $"The \"parentControl\" was passed as null to \"ToggleLinkedControls\"");
                        return;
                  }

                  int enabledSettings = setting.GetEnabledSettingsCount();
                  bool controlsNewState = false;

                  if (enabledSettings != 0) {
                        controlsNewState = true;

                        if (enabledSettings == setting.TotalSettingsCount) {
                              parentControl.CheckState = CheckState.Checked;

                        } else {
                              parentControl.CheckState = CheckState.Indeterminate;
                        }
                  }

                  parentControl.Checked = controlsNewState;

                  if (childControl != null) {
                        childControl.Enabled = controlsNewState;
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

                  this.letterSpacingModeInfoIcon.Visible = (config.FontRand.LetterSpacingSettings.Mode == LetterSpacingSettings.SpacingMode.Advanced);

                  this.seedInfoIcon.Visible = config.Seed == 0;
            }

            private void RepositionControlPairs() {
                  // Icon textures
                  RightAlignControl(iconTextureTypeLabel, iconTextureWarningIcon);

                  // Game Textures
                  RightAlignControl(gameTextureTypeLabel, gameTextureWarningIcon);

                  // Font Settings
                  RightAlignControl(fontRandEnabledCheckbox, fontRandWarningIcon);
                  RightAlignControl(fontStyleRandModeLabel, fontStyleModeSelector);
                  RightAlignControl(randomiseCharactersCheckBox, fontRandomiseCharactersInfoIcon);
                  RightAlignControl(letterSpacingModeLabel, letterSpacingModeSelector);
                  RightAlignControl(letterSpacingModeSelector, letterSpacingModeInfoIcon);
                  RightAlignControl(letterSpacingLevelLabel, letterSpacingLevelInputBox);

                  // Randomisation Settings
                  RightAlignControl(randomisationSeedLabel, seedInfoIcon);

                  // Application Settings
                  RightAlignControl(gameFolderLabel, gameFolderWarningIcon);
            }

            private void RightAlignControl(Label label, Control control, int X_offset = 0) {
                  control.Location = new Point(label.Location.X + label.PreferredWidth + X_offset, control.Location.Y);
            }

            private void RightAlignControl(CheckBox checkBox, Control control, int X_offset = -4) {
                  control.Location = new Point(checkBox.Location.X + checkBox.Width + X_offset, control.Location.Y);
            }

            private void RightAlignControl(ComboBox left, Control right, int X_offset = 5) {
                  right.Location = new Point(left.Location.X + left.Width + X_offset, right.Location.Y);
            }

            private string GetReadyStatusDisplayText(ReadyState ready, bool corrupt = true) {
                  string ret = ready.GetMessageString();

                  if (corrupt) {
                        ret = this.textCorruptor.CorruptText(ret);
                  }

                  return ret;
            }

            private void ChangeControlStates(CheckBox checkBox, NumericUpDown groupDisplay, RandomisationSetting setting) {
                  checkBox.Checked = setting.Enabled;
                  groupDisplay.Enabled = setting.Enabled;
                  groupDisplay.Value = setting.Group;
            }

            private void SetAllIconTexturesElementStates() {
                  ToggleAllLinkedControls();

                  ChangeControlStates(IconTexturesCheckbox, IconTexturesGroupDisplay, config.IconTextures.ToRandSetting());

                  ChangeControlStates(CubeTexturesCheckbox, CubeTexturesGroupDisplay, config.IconTextures.Cube);
                  ChangeControlStates(ShipTexturesCheckbox, ShipTexturesGroupDisplay, config.IconTextures.Ship);
                  ChangeControlStates(BallTexturesCheckbox, BallTexturesGroupDisplay, config.IconTextures.Ball);
                  ChangeControlStates(UFO_TexturesCheckbox, UFO_TexturesGroupDisplay, config.IconTextures.Ufo);
                  ChangeControlStates(WaveTexturesCheckbox, WaveTexturesGroupDisplay, config.IconTextures.Wave);
                  ChangeControlStates(RobotTexturesCheckbox, RobotTexturesGroupDisplay, config.IconTextures.Robot);
                  ChangeControlStates(SpiderTexturesCheckbox, SpiderTexturesGroupDisplay, config.IconTextures.Spider);
                  ChangeControlStates(SwingTexturesCheckbox, SwingTexturesGroupDisplay, config.IconTextures.Swing);
                  ChangeControlStates(JetpackTexturesCheckbox, JetpackTexturesGroupDisplay, config.IconTextures.Jetpack);
            }

            private void SetAllGameTexturesElementStates() {
                  ChangeControlStates(MenuTexturesCheckbox, MenuTexturesGroupDisplay, config.MenuTextures);
                  ChangeControlStates(ShopTexturesCheckbox, ShopTexturesGroupDisplay, config.ShopTextures);
                  ChangeControlStates(EditorTexturesCheckbox, EditorTexturesGroupDisplay, config.EditorTextures);
                  ChangeControlStates(BlockTexturesCheckbox, TileTexturesGroupDisplay, config.TileTextures);
                  ChangeControlStates(PortalTexturesCheckbox, PortalTexturesGroupDisplay, config.PortalTextures);
                  ChangeControlStates(OrbsTexturesCheckbox, OrbsGroupDisplay, config.OrbTextures);
                  ChangeControlStates(PadsTexturesCheckbox, PadsGroupDisplay, config.PadTextures);
                  ChangeControlStates(ParticleTexturesCheckbox, ParticleTexturesGroupDisplay, config.ParticleTextures);
                  ChangeControlStates(EffectsCheckbox, EffectsGroupDisplay, config.EffectTextures);
                  ChangeControlStates(MiscCheckbox, MiscGroupDisplay, config.MiscTextures);
            }

            #region Font Rand UI Controls

            private void SetAllFontRandElementStates() {
                  this.fontRandEnabledCheckbox.Checked = config.FontRand.AnySettingEnabled();

                  FontStylesControlToggle();
                  FontCharacterRandomisationControlToggle();
                  FontLetterSpacingControlToggle();
            }

            private void FontStylesControlToggle() {
                  FontStyleShuffleSettings fontStyle = config.FontRand.ShuffleSettings;

                  this.fontShuffleStylesCheckbox.Checked = fontStyle.Enabled;
                  this.fontStyleModeSelector.Enabled = fontStyle.Enabled;
                  this.fontStyleModeSelector.SelectedIndex = (int)fontStyle.Mode;
            }

            private void FontCharacterRandomisationControlToggle() {
                  FontRandomisationSettings fontRand = config.FontRand;
                  CharacterRandomisationSettings CRS = fontRand.CharacterRandSettings;

                  this.randomiseCharactersCheckBox.Checked = CRS.Enabled;

                  this.characterLetterRandCheckBox.Enabled = CRS.Enabled;
                  ChangeControlStates(characterLetterRandCheckBox, characterLetterRandGroupDisplay, CRS.Letter);

                  this.characterNumberRandCheckBox.Enabled = CRS.Enabled;
                  ChangeControlStates(characterNumberRandCheckBox, characterNumberRandGroupDisplay, CRS.Number);

                  this.characterSymbolRandCheckBox.Enabled = CRS.Enabled;
                  ChangeControlStates(characterSymbolRandCheckBox, characterSymbolRandGroupDisplay, CRS.Symbol);
            }

            private void FontLetterSpacingControlToggle() {
                  LetterSpacingSettings LSS = config.FontRand.LetterSpacingSettings;

                  this.randomLetterSpacingCheckBox.Checked = LSS.Enabled;

                  this.letterSpacingModeSelector.Enabled = LSS.Enabled;
                  this.letterSpacingModeSelector.SelectedIndex = (int)LSS.Mode;

                  bool advancedMode = LSS.Mode == LetterSpacingSettings.SpacingMode.Advanced;

                  // Set all advanced mode control states
                  this.kerningLabel.Visible = advancedMode;
                  this.x_OffsetLabel.Visible = advancedMode;
                  this.fontAdvancedModeSeparatorConnectorBeam.Visible = advancedMode;

                  this.kerningMinLabel.Visible = advancedMode;
                  this.kerningMinInput.Visible = advancedMode;
                  this.kerningMaxLabel.Visible = advancedMode;
                  this.kerningMaxInput.Visible = advancedMode;

                  this.x_OffsetMinLabel.Visible = advancedMode;
                  this.x_OffsetMinInput.Visible = advancedMode;
                  this.x_OffsetMaxLabel.Visible = advancedMode;
                  this.x_OffsetMaxInput.Visible = advancedMode;

                  // Set all simple mode control states
                  this.letterSpacingLevelLabel.Visible = !advancedMode;
                  this.letterSpacingLevelInputBox.Visible = !advancedMode;

                  if (LSS.Mode == LetterSpacingSettings.SpacingMode.Simple) {
                        // Make the beam smaller
                        this.letterSpacingConnectorBeam.Size = new Size(17, 52);
                        this.letterSpacingLevelInputBox.Enabled = LSS.Enabled;

                  } else {
                        // Make the beam full size to connect 2 settings to it visually
                        this.letterSpacingConnectorBeam.Size = new Size(17, 82);

                        // Set the Kerning input boxes
                        this.kerningMinInput.Enabled = LSS.Enabled;
                        this.kerningMinInput.Text = LSS.Kerning.Min.ToString();
                        this.kerningMaxInput.Enabled = LSS.Enabled;
                        this.kerningMaxInput.Text = LSS.Kerning.Max.ToString();

                        // Set the X Offset input boxes
                        this.x_OffsetMinInput.Text = LSS.X_Offset.Min.ToString();
                        this.x_OffsetMinInput.Enabled = LSS.Enabled;
                        this.x_OffsetMaxInput.Text = LSS.X_Offset.Max.ToString();
                        this.x_OffsetMaxInput.Enabled = LSS.Enabled;
                  }
            }

            #endregion

            private void UpdateRandomisationControlStates() {
                  this.seedInputBox.Text = config.Seed.ToString();
                  this.seedInputBox.Value = config.Seed;

                  // Format the sprite size multiplier display's text
                  if (config.MaxSpriteMultiplier < 1000) {
                        string fmtString = "F2";
                        if (config.MaxSpriteMultiplier > 3 && config.MaxSpriteMultiplier <= 50) {
                              fmtString = "F1";
                        } else if (config.MaxSpriteMultiplier > 50) {
                              fmtString = "";
                        }

                        this.spriteSizeMultiplierTextbox.Text = config.MaxSpriteMultiplier.ToString(fmtString) + "x";
                  } else {
                        this.spriteSizeMultiplierTextbox.Text = "Unlimited";
                  }

                  this.allowDuplicatesCheckbox.Checked = config.AllowDuplicates;
            }

            private void UpdateApplicationSettingsControlStates() {
                  this.gameFolderTextBox.Text = advancedConfig.GameDirectory;

                  this.textureQualitySelectorBox.SelectedIndex = (int)advancedConfig.Quality;
                  this.applicationThemeSelectorBox.SelectedIndex = (int)advancedConfig.ThemeID;

                  this.autoOverwriteFilesCheckbox.Checked = advancedConfig.AutoOverwriteFiles;
            }

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

            #endregion

            #region Randomization

            private async void StartButton_Click(object sender, EventArgs e) {
                  bool ready = gameFileManager.getReadyState() == ReadyState.Ready;
                  if (ready == false)
                        return;

                  SetUI_EnabledState(false);

                  ChangeProgressDisplayState(enabled: true);

                  bool randomSeed = false;
                  // Create a new random seed if the input value is 0
                  int seed = config.Seed;
                  if (seed == 0) {
                        seed = Math.Abs(Guid.NewGuid().GetHashCode());
                        randomSeed = true;
                  }

                  Stopwatch stopwatch = new Stopwatch();
                  stopwatch.Start();
                  this.randomisationThread = new Thread(() => {
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

                  RefreshUI(false);

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

            #endregion

            #region File Restoration

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

            #region Config Importing

            private void ImportConfigThenRefreshUI(string configData) {

                  autoRefreshLinkedControls = false;
                  bool success = config.ImportConfigData(configData);
                  RefreshUI();
                  autoRefreshLinkedControls = true;

                  if (success == true) {
                        this.statusDisplay.Text = "Config Imported Successfully";

                  } else {
                        this.statusDisplay.Text = "Failed to Import Config.";
                  }
            }

            #endregion

            #region Theme Control Region

            private void SetTheme() {
                  if (this.themeController.Current.Name == ThemeController.RandomThemeName) {
                        // Generate an entirely random theme because why not
                        Theme randomTheme = Theme.CreateRandom();
                        this.themeController.Current.CopyColoursFrom(randomTheme);
                        SetTheme(randomTheme);

                        Log.Write(Log.Mode.Verbose,
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

                        Log.Write(Log.Mode.Verbose,
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

                  GenerateBeamImages(theme);
                  UpdateAllImageThemes();
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

                  for (int i = 0; i < comboBoxes.Length; i++) {
                        comboBoxes[i].BackColor = back;
                        comboBoxes[i].ForeColor = fore;
                  }
            }

            private void ChangeGroupBoxColours(Color colour) {
                  for (int i = 0; i < groupBoxes.Length; i++) {
                        groupBoxes[i].ForeColor = colour;
                  }
            }

            private void GenerateBeamImages(Theme theme) {
                  // Free the old images from RAM
                  ClearOldResourceImages();

                  // Uhm... yeah, this is a line of code that I am not proud of. It works, but it is a bit of a mess. I will try to clean it up later
                  Color inactiveColourFront = theme.BackgroundColour.Add(theme.BackgroundColour.GetDelta(theme.BeamColour).AdjustBrightness(0.5f));
                  Color inactiveColourBack = theme.BackgroundColour;

                  Console.WriteLine(theme.BackgroundColour.ToString());
                  Console.WriteLine(theme.BeamColour.ToString());
                  Console.WriteLine(inactiveColourFront.ToString());

                  this.activeStandardBeam = Resources.ConnectorBeamWhite.RepaintImage(theme);
                  this.inactiveStandardBeam = Resources.ConnectorBeamWhite.RepaintImage(inactiveColourBack, inactiveColourFront);

                  this.activeFontBeam = Resources.FontConnectorBeam.RepaintImage(theme);
                  this.inactiveFontBeam = Resources.FontConnectorBeam.RepaintImage(inactiveColourBack, inactiveColourFront);

                  this.refreshIcon = Resources.RefreshImage.RepaintImage(theme.BackgroundColour, theme.BeamColour);
                  this.settingsIcon = Resources.SettingsImage.RepaintImage(theme.BackgroundColour, theme.BeamColour);
            }

            private void ClearOldResourceImages() {
                  if (this.activeStandardBeam != null)
                        this.activeStandardBeam.Dispose();
                  if (this.inactiveStandardBeam != null)
                        this.inactiveStandardBeam.Dispose();

                  if (this.activeFontBeam != null)
                        this.activeFontBeam.Dispose();
                  if (this.inactiveFontBeam != null)
                        this.inactiveFontBeam.Dispose();

                  if (this.refreshIcon != null)
                        this.refreshIcon.Dispose();
                  if (this.settingsIcon != null)
                        this.settingsIcon.Dispose();
            }

            private void UpdateAllImageThemes() {
                  UpdateIconThemes(this.refreshIcon,
                        new PictureBox[] { refreshThemesButton });

                  UpdateIconThemes(this.settingsIcon,
                        new PictureBox[] { themesSettingsButton });

                  this.fontRandMainConnectorBeam.Image = config.FontRand.Enabled ?
                        this.activeFontBeam : this.inactiveFontBeam;

                  this.iconsConnectorBeam.Image = config.IconTextures.Enabled ?
                        this.activeStandardBeam : this.inactiveStandardBeam;

                  this.shuffleFontsConnectorBeam.Image = config.FontRand.ShuffleSettings.Enabled ?
                        this.activeStandardBeam : this.inactiveStandardBeam;

                  this.fontRandomiseLettersConnectorBeam.Image = config.FontRand.CharacterRandSettings.Enabled ?
                        this.activeStandardBeam : this.inactiveStandardBeam;

                  this.letterSpacingConnectorBeam.Image = config.FontRand.LetterSpacingSettings.Enabled ?
                        this.activeStandardBeam : this.inactiveStandardBeam;

                  this.fontAdvancedModeSeparatorConnectorBeam.Image = config.FontRand.LetterSpacingSettings.Enabled ?
                        this.activeStandardBeam : this.inactiveStandardBeam;
            }

            private void UpdateIconThemes(Bitmap newImage, PictureBox[] boxes) {
                  for (int i = 0; i < boxes.Length; i++) {
                        boxes[i].Image = newImage;
                  }
            }

            #endregion

            #region Config Data Updater Methods

            private void ChangeConfigData(object sender, RandomisationSetting setting, params Control[] toggledControls) {
                  ChangeConfigData(sender, setting);

                  if (toggledControls == null)
                        return;

                  toggledControls.ToList().ForEach(c => c.Enabled = setting.Enabled);
            }

            private void ChangeConfigData(object sender, RandomisationSetting setting, Control toggledControl) {
                  ChangeConfigData(sender, setting);

                  toggledControl.Enabled = setting.Enabled;
            }

            private void ChangeConfigData(object sender, RandomisationSetting setting) {
                  if (sender is CheckBox checkBox) {
                        ChangeConfigData(checkBox, setting);
                        ValidateReadyState();

                  } else if (sender is NumericUpDown numericUpDown) {
                        ChangeConfigData(numericUpDown, setting);

                  } else {
                        Log.Write(Log.Mode.Warn, $"Sender is not a supported type for changing properties of {setting}");
                  }

                  UpdateAllImageThemes();
            }

            private void ChangeConfigData(CheckBox checkBox, RandomisationSetting setting) {
                  setting.Enabled = checkBox.Checked;
            }

            private void ChangeConfigData(NumericUpDown numUpDown, RandomisationSetting setting) {
                  setting.Group = (int)numUpDown.Value;
            }

            #endregion

            #region Miscellaneous Events

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

                  RepositionControlPairs();
            }

            #endregion

            #region Icon Texture Settings Changed Events

            private void IconTexturesSettingsChanged(object sender, EventArgs e) {
                  bool newState = (sender as CheckBox).Checked;

                  // If the form is not initialised, skip the rest of the changes
                  // Otherwise some of the configuration data will be overwritten when the form is first initialised
                  if (autoRefreshLinkedControls == false) {
                        return;
                  }

                  config.IconTextures.SetAll(newState);
                  
                  RefreshUI();
            }

            private void IconTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  if (numericUpDown.Value > 100) {
                        numericUpDown.Value = 100;
                  }

                  config.IconTextures.Group = (int)numericUpDown.Value;

                  // If the form is not initialised, skip the rest of the changes
                  // Otherwise some of the configuration data will be overwritten when the form is first initialised
                  if (autoRefreshLinkedControls == false) {
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

                  SetAllIconTexturesElementStates();
                  ValidateReadyState();
            }

            private void CubeTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.IconTextures.Cube, CubeTexturesGroupDisplay);

            private void ShipTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.IconTextures.Ship, ShipTexturesGroupDisplay);

            private void BallTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.IconTextures.Ball, BallTexturesGroupDisplay);

            private void UFO_TexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.IconTextures.Ufo, UFO_TexturesGroupDisplay);

            private void WaveTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.IconTextures.Wave, WaveTexturesGroupDisplay);

            private void RobotTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.IconTextures.Robot, RobotTexturesGroupDisplay);

            private void SpiderTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.IconTextures.Spider, SpiderTexturesGroupDisplay);

            private void SwingTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.IconTextures.Swing, SwingTexturesGroupDisplay);

            private void JetpackTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.IconTextures.Jetpack, JetpackTexturesGroupDisplay);

            #endregion

            #region Game Texture Settings Changed Events

            private void MenuTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.MenuTextures, MenuTexturesGroupDisplay);

            private void ShopTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.ShopTextures, ShopTexturesGroupDisplay);

            private void EditorTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.EditorTextures, EditorTexturesGroupDisplay);

            private void TilesTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.TileTextures, TileTexturesGroupDisplay);

            private void PortalTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.PortalTextures, PortalTexturesGroupDisplay);

            private void OrbsTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.OrbTextures, OrbsGroupDisplay);

            private void PadsTexturesSettingChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.PadTextures, PadsGroupDisplay);

            private void ParticleTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.ParticleTextures, ParticleTexturesGroupDisplay);

            private void EffectsTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.EffectTextures, EffectsGroupDisplay);

            private void MiscTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.MiscTextures, MiscGroupDisplay);

            #endregion

            #region Font Randomisation Settings Controls

            private void FontRandEnabledCheckbox_Click(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  if (checkBox == null) {
                        Log.Write(Mode.Error, "Checkbox returned null in the \"FontRandEnabledCheckbox_Click\" method");
                        return;
                  }

                  config.FontRand.SetAll(checkBox.Checked);

                  // If the form is not initialised, skip the rest of the changes
                  // Otherwise some of the configuration data will be overwritten when the form is first initialised
                  if (autoRefreshLinkedControls == false) {
                        return;
                  }

                  RefreshUI();
            }

            private void FontShuffleStylesCheckbox_Click(object sender, EventArgs e) {
                  bool newState = (sender as CheckBox).Checked;
                  config.FontRand.ShuffleSettings.Enabled = newState;

                  RefreshUI();
            }

            private void FontShuffleStylesMode_Change(object sender, EventArgs e) {
                  ComboBox comboBox = sender as ComboBox;
                  config.FontRand.ShuffleSettings.SetMode(comboBox.SelectedItem.ToString());

                  Log.Write(Log.Mode.Debug, $"New shuffling mode is {config.FontRand.ShuffleSettings.Mode}");
                  RefreshUI();
            }

            private void RandomiseCharactersSettingsChanged(object sender, EventArgs e) {
                  bool newState = (sender as CheckBox).Checked;
                  config.FontRand.CharacterRandSettings.Enabled = newState;

                  // If the form is not initialised, skip the rest of the changes
                  // Otherwise some of the configuration data will be overwritten when the form is first initialised
                  if (autoRefreshLinkedControls == false) {
                        return;
                  }

                  config.FontRand.CharacterRandSettings.SetAll(newState);

                  RefreshUI();
            }

            private void LettersRandomisationSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.FontRand.CharacterRandSettings.Letter, characterLetterRandGroupDisplay);

            private void NumbersRandomisationSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.FontRand.CharacterRandSettings.Number, characterNumberRandGroupDisplay);

            private void SymbolsRandomisationSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.FontRand.CharacterRandSettings.Symbol, characterSymbolRandGroupDisplay);

            private void LetterSpacingSettingsChanged(object sender, EventArgs e) {
                  bool newState = (sender as CheckBox).Checked;
                  config.FontRand.LetterSpacingSettings.Enabled = newState;

                  RefreshUI();
            }

            private void LetterSpacingModeSettingsChanged(object sender, EventArgs e) {
                  ComboBox comboBox = sender as ComboBox;
                  config.FontRand.LetterSpacingSettings.SetMode(comboBox.SelectedItem.ToString());

                  Log.Write(Log.Mode.Debug, $"New letter spacing mode is {config.FontRand.LetterSpacingSettings.Mode}");
                  RefreshUI();
            }

            private void KerningMinInput_TextChanged(object sender, EventArgs e)
                  => ChangeLetterSpacingValue(sender, 0);

            private void KerningMaxInput_TextChanged(object sender, EventArgs e)
                  => ChangeLetterSpacingValue(sender, 1);

            private void X_OffsetMinInput_TextChanged(object sender, EventArgs e)
                  => ChangeLetterSpacingValue(sender, 2);

            private void X_OffsetMaxInput_TextChanged(object sender, EventArgs e)
                  => ChangeLetterSpacingValue(sender, 3);

            private void ChangeLetterSpacingValue(object sender, int settingID) {
                  TextBox textBox = sender as TextBox;
                  if (textBox == null) {
                        Log.Write(Mode.Error, $"The sender returned null when cast to a TextBox. ID: {settingID}");
                        return;
                  }

                  config.FontRand.LetterSpacingSettings.SetRangeValue(textBox.Text, settingID);

                  RefreshUI();
            }

            private void letterSpacingLevelInputBox_ValueChanged(object sender, EventArgs e) {
                  NumericUpDown NUD = sender as NumericUpDown;
                  config.FontRand.LetterSpacingSettings.Level = (int)NUD.Value;
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

            #region Control List Getters 

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

            #endregion

            #region Randomisation Settings Control Methods

            private void SpriteSizeMultiplierTrackbar_Scroll(object sender, EventArgs e) {
                  TrackBar trackBar = sender as TrackBar;

                  config.MaxSpriteMultiplier = GetSliderMultiplierForSpriteSize(trackBar.Value);

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
                  // Get all multipliers that can be set for the texture size slider
                  float[] multipliers = Enumerable.Range(1, this.spriteSizeMultiplierTrackbar.Maximum + 1)
                        .Select(v => GetSliderMultiplierForSpriteSize(v))
                        .ToArray();

                  // Find the index of the current multiplier in the array
                  int index = Array.BinarySearch(multipliers, config.MaxSpriteMultiplier);
                  if (index >= 0) {
                        // If found, add 1 to it, and set the slider to that value
                        // The offset by 1 is to account for the starting value of 1 for the slider
                        this.spriteSizeMultiplierTrackbar.Value = index + 1;
                  }
            }

            private void AllowDuplicatesCheckbox_Click(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  config.AllowDuplicates = checkBox.Checked;

                  RefreshUI();
            }

            private void SeedValueChanged(object sender, EventArgs e) {
                  NumericUpDown nud = sender as NumericUpDown;
                  config.Seed = (int)nud.Value;
                  RefreshUI();
            }

            private void RandomSeedButton_Click(object sender, EventArgs e) {
                  Random random = new Random(Guid.NewGuid().GetHashCode());
                  int value = random.Next(int.MinValue, int.MaxValue);
                  this.seedInputBox.Value = value;
                  config.Seed = value;

                  RefreshUI();
            }

            #endregion

            #region Application Settings Control Methods

            private void SetGameFolder(object sender, EventArgs e) {
                  string folder = GetFolderViaExplorer(advancedConfig.GameDirectory, true);
                  if (folder != string.Empty) {
                        advancedConfig.GameDirectory = folder;
                  }

                  RefreshUI();
            }

            private void GameFolderTextBox_TextChanged(object sender, EventArgs e) {
                  TextBox textBox = sender as TextBox;
                  advancedConfig.GameDirectory = textBox.Text;
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
                  advancedConfig.AutoOverwriteFiles = checkBox.Checked;
            }

            private void ChangeTextureQuality(object sender, EventArgs e) {
                  DomainUpDown domainUpDown = sender as DomainUpDown;
                  if (domainUpDown == null) {
                        return;
                  }

                  advancedConfig.Quality = (Quality)domainUpDown.SelectedIndex;
            }

            private void ChangeApplicationTheme(object sender, EventArgs e) {
                  if (this.lastThemeRefresh.GetElapsedTime().TotalMilliseconds < themeRefreshCooldown) {
                        return;
                  }

                  DomainUpDown domainUpDown = sender as DomainUpDown;
                  int newThemeID = domainUpDown.SelectedIndex;

                  this.themeController.ActiveThemeID = newThemeID;
                  advancedConfig.ThemeID = newThemeID;

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

                  if (this.themeController.GetThemeCount() <= advancedConfig.ThemeID) {
                        Log.Write(Log.Mode.Warn, $"The theme ID [{advancedConfig.ThemeID}] is out of range");

                        advancedConfig.ThemeID = 0;
                  }

                  this.themeController.ActiveThemeID = advancedConfig.ThemeID;

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
                  comboBoxes = GetAll(this, typeof(ComboBox)).Select(c => c as ComboBox).ToArray();
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
                                    typeof(ComboBox),
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

            #region Animations

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