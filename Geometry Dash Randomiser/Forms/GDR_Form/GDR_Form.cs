using Geometry_Dash_Randomiser.Properties;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Geometry_Dash_Randomiser.Logger;

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
                  SetSliderValueForSpriteSize();

                  // If the game directory is valid, enable the restore button
                  this.restoreFilesButton.Enabled = gameFileManager.IsGameDirectoryValid();

                  this.versionLabel.Text = version;

                  InitialiseControlContainers();

                  PopulateQualityDropdown();

                  SetTheme();

                  RefreshUI();

                  Thread cleanerThread = new Thread(() => {
                        Logger.CleanUpLogs();
                  });
                  cleanerThread.Start();

                  autoRefreshLinkedControls = true;
            }

            private void WriteSystemAnalysisInfoToConsole() {
                  Logger.Write(Logger.Mode.Verbose, $"Total thread count: {Environment.ProcessorCount}");
                  Logger.Write(Logger.Mode.Verbose, $"Is 64bit Process: {Environment.Is64BitProcess}");
                  Logger.Write(Logger.Mode.Verbose, $"Is 64bit OS: {Environment.Is64BitOperatingSystem}");
                  Logger.Write(Logger.Mode.Verbose, $"OS Version: {Environment.OSVersion}");
                  Logger.Write(Logger.Mode.Verbose, $"System Page Size: {Environment.SystemPageSize}");
                  Logger.Write(Logger.Mode.Verbose, $"CLR Version: {Environment.Version}");
            }

            #region Randomization

            private async void StartRandomzation(object sender, EventArgs e) {
                  bool ready = gameFileManager.getReadyState() == ReadyState.Ready;
                  if (ready == false)
                        return;

                  SetUI_EnabledState(false);

                  ChangeProgressDisplayState(enabled: true);

                  bool randomSeed = false;
                  // Create a new random seed if the input value is 0
                  int seed = config.Seed;
                  if (seed == 0) {
                        seed = System.Math.Abs(Guid.NewGuid().GetHashCode());
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
                        string newDisplayPrint = ProgressState.Instance.GetProgressString();
                        if (newDisplayPrint != lastDisplayPrint) {
                              lastDisplayPrint = newDisplayPrint;
                              UpdateProgressStateObjects(newDisplayPrint);
                        }

                        string newTimePrint = stopwatch.GetElapsedTimeFormatted();
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

                  stopwatch.Stop();
                  // Build the status display's string in multiple parts
                  StringBuilder statusText = new StringBuilder("Randomisation complete in " +
                        stopwatch.GetElapsedTimeFormatted());

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

            private async void RestoreFiles(object sender, EventArgs e) {
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
                        string newDisplayPrint = ProgressState.Instance.GetProgressString();
                        if (newDisplayPrint != lastDisplayPrint) {
                              lastDisplayPrint = newDisplayPrint;
                              UpdateProgressStateObjects(newDisplayPrint);
                        }

                        string newTimePrint = stopwatch.GetElapsedTimeFormatted();
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
                  stopwatch.Stop();

                  this.statusDisplay.Text = $"File Restoration Complete in {stopwatch.GetElapsedTimeFormatted()}";

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

            #region Config Data Updater Methods

            private void ChangeConfigData(object sender, RandomizationSetting setting, params Control[] toggledControls) {
                  ChangeConfigData(sender, setting);

                  if (toggledControls == null)
                        return;

                  toggledControls.ToList().ForEach(c => c.Enabled = setting.Enabled);
            }

            private void ChangeConfigData(object sender, RandomizationSetting setting, Control toggledControl) {
                  ChangeConfigData(sender, setting);

                  toggledControl.Enabled = setting.Enabled;
            }

            private void ChangeConfigData(object sender, RandomizationSetting setting) {
                  if (sender is CheckBox checkBox) {
                        ChangeConfigData(checkBox, setting);
                        ValidateReadyState();

                  } else if (sender is NumericUpDown numericUpDown) {
                        ChangeConfigData(numericUpDown, setting);

                  } else if (sender is TextBox textBox) {
                        ChangeConfigData(textBox, setting);

                  } else {
                        Logger.Write(Logger.Mode.Warn, $"Sender is not a supported type for changing properties of {setting}");
                  }

                  UpdateAllImageThemes();
            }

            private void ChangeConfigData(CheckBox checkBox, RandomizationSetting setting) {
                  setting.Enabled = checkBox.Checked;
            }

            private void ChangeConfigData(NumericUpDown numUpDown, RandomizationSetting setting) {
                  setting.Group = (int)numUpDown.Value;
            }

            private void ChangeConfigData(TextBox textBox, RandomizationSetting setting) {
                  bool success = textBox.ParseNumericValue(out Int32 result);
                  int newValue = 0;

                  if (success) {
                        newValue = result;
                  }

                  setting.PercentageRandomised = newValue;
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

                  for (int i = 0; i < this.comboBoxes.Length; i++) {
                        for (int c = 0; c < this.comboBoxes[i].Items.Count; c++) {
                              this.comboBoxes[i].Items[c] = this.textCorruptor.CorruptText(this.comboBoxes[i].Items[c].ToString(), 1);
                        }
                  }

                  RepositionControlPairs();
            }

            #endregion

            #region Icon Texture Settings Changed Events

            private void IconTexturesSettingsChanged(object sender, EventArgs e) {
                  bool newState = (sender as CheckBox).Checked;

                  if (autoRefreshLinkedControls == false) {
                        return;
                  }

                  config.IconTextures.Enabled = newState;
                  
                  RefreshUI();
            }

            private void IconTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;

                  if (autoRefreshLinkedControls == false) {
                        return;
                  }

                  config.IconTextures.Group = (int)numericUpDown.Value;

                  SetAllIconTexturesElementStates();
                  ValidateReadyState();
            }

            private void IconTexturesPercentageChanged(object sender, EventArgs e) {
                  TextBox textBox = sender as TextBox;
                  bool success = textBox.ParseNumericValue(out Int32 result);

                  if (autoRefreshLinkedControls == false) {
                        return;
                  }

                  config.IconTextures.PercentageRandomised = result;

                  SetAllIconTexturesElementStates();
                  ValidateReadyState();
            }

            private void CubeTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.IconTextures.Cube, CubeTexturesGroupDisplay_Gr);

            private void ShipTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.IconTextures.Ship, ShipTexturesGroupDisplay_Gr);

            private void BallTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.IconTextures.Ball, BallTexturesGroupDisplay_Gr);

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
                  => ChangeConfigData(sender, config.GameTextureConfig.Menu, MenuTexturesGroupDisplay);

            private void ShopTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.GameTextureConfig.Shop, ShopTexturesGroupDisplay);

            private void EditorTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.GameTextureConfig.Editor, EditorTexturesGroupDisplay);

            private void TilesTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.GameTextureConfig.Tile, TileTexturesGroupDisplay);

            private void PortalTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.GameTextureConfig.Portal, PortalTexturesGroupDisplay);

            private void OrbsTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.GameTextureConfig.Orb, OrbTexturesGroupDisplay);

            private void PadsTexturesSettingChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.GameTextureConfig.Pad, PadTexturesGroupDisplay);

            private void ParticleTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.GameTextureConfig.Particle, ParticleTexturesGroupDisplay);

            private void EffectsTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.GameTextureConfig.Effect, EffectTexturesGroupDisplay);

            private void MiscTexturesSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.GameTextureConfig.Misc, MiscTexturesGroupDisplay);

            #endregion

            #region Font Randomisation Settings Controls

            private void FontRandEnabledCheckbox_Click(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  if (checkBox == null) {
                        Logger.Write(Mode.Error, "Checkbox returned null in the \"FontRandEnabledCheckbox_Click\" method");
                        return;
                  }

                  config.FontRand.Enabled = checkBox.Checked;

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
                  config.FontRand.ShuffleSettings.SetMode(comboBox.SelectedIndex);

                  Logger.Write(Logger.Mode.Debug, $"New shuffling mode is {config.FontRand.ShuffleSettings.Mode}");
                  RefreshUI();
            }

            private void RandomiseCharactersSettingsChanged(object sender, EventArgs e) {
                  bool newState = (sender as CheckBox).Checked;

                  // If the form is not initialised, skip the rest of the changes
                  // Otherwise some of the configuration data will be overwritten when the form is first initialised
                  if (autoRefreshLinkedControls == false) {
                        return;
                  }

                  config.FontRand.CharacterRandSettings.Enabled = newState;

                  RefreshUI();
            }

            private void LettersRandomisationSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.FontRand.CharacterRandSettings.Letter, characterLetterRandPercentageBox, characterLetterRandGroupDisplay);

            private void NumbersRandomisationSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.FontRand.CharacterRandSettings.Number, characterNumberRandPercentageBox, characterNumberRandGroupDisplay);

            private void SymbolsRandomisationSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.FontRand.CharacterRandSettings.Symbol, characterSymbolRandPercentageBox, characterSymbolRandGroupDisplay);

            private void SpecialsRandomisationSettingsChanged(object sender, EventArgs e)
                  => ChangeConfigData(sender, config.FontRand.CharacterRandSettings.Special, characterSpecialRandPercentageBox, characterSpecialRandGroupDisplay);

            private void LetterSpacingSettingsChanged(object sender, EventArgs e) {
                  bool newState = (sender as CheckBox).Checked;
                  config.FontRand.LetterSpacingSettings.Enabled = newState;

                  RefreshUI();
            }

            private void LetterSpacingModeSettingsChanged(object sender, EventArgs e) {
                  ComboBox comboBox = sender as ComboBox;
                  config.FontRand.LetterSpacingSettings.SetMode(comboBox.SelectedIndex);

                  Logger.Write(Logger.Mode.Debug, $"New letter spacing mode is {config.FontRand.LetterSpacingSettings.Mode}");
                  RefreshUI();
            }

            private void X_OffsetMinInput_TextChanged(object sender, EventArgs e)
                  => ChangeLetterSpacingValue(sender, 0);

            private void X_OffsetMaxInput_TextChanged(object sender, EventArgs e)
                  => ChangeLetterSpacingValue(sender, 1);

            private void KerningMinInput_TextChanged(object sender, EventArgs e)
                  => ChangeLetterSpacingValue(sender, 2);

            private void KerningMaxInput_TextChanged(object sender, EventArgs e)
                  => ChangeLetterSpacingValue(sender, 3);

            private void ChangeLetterSpacingValue(object sender, int settingID) {
                  TextBox textBox = sender as TextBox;
                  if (textBox == null) {
                        Logger.Write(Mode.Error, $"The sender returned null when cast to a TextBox. ID: {settingID}");
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
                        return RandomizationConfig.UnlimitedSpriteMultiplier;
                  }
            }

            private void SetSliderValueForSpriteSize() {
                  if (config.MaxSpriteMultiplier == RandomizationConfig.UnlimitedSpriteMultiplier) {
                        this.spriteSizeMultiplierTrackbar.Value = this.spriteSizeMultiplierTrackbar.Maximum;
                        return;
                  }

                  int minIndex = 1;
                  int maxIndex = this.spriteSizeMultiplierTrackbar.Maximum;
                  float closestValue = float.PositiveInfinity;
                  int closestIndex = 0;

                  // My shoddy implementation of a binary search
                  do {
                        int searchIndex = (maxIndex - minIndex) / 2 + minIndex;
                        float multiplier = GetSliderMultiplierForSpriteSize(searchIndex);
                        float delta = Math.Abs(multiplier - config.MaxSpriteMultiplier);

                        if (closestValue > delta) {
                              closestValue = delta;
                              closestIndex = searchIndex;
                        }

                        if (multiplier < config.MaxSpriteMultiplier) {
                              minIndex = searchIndex + 1;

                        } else if (multiplier >= config.MaxSpriteMultiplier) {
                              maxIndex = searchIndex;

                        } else {
                              break;
                        }

                  } while (minIndex < maxIndex);

                  this.spriteSizeMultiplierTrackbar.Value = closestIndex;
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

                  Logger.Write(Logger.Mode.Verbose, $"Switching to theme ID {newThemeID}: {themeController.Current.Name}");

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
                        Logger.Write(Logger.Mode.Warn, $"The theme ID [{advancedConfig.ThemeID}] is out of range");

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

                  controlsToggledDuringRandomisation.Remove(applicationThemeSelectorBox);

                  // Finally, signal that the arrays are populated
                  dataArraysArePopulated = true;
            }

            #endregion

            #region Validations

            /// <summary>
            /// Makes sure the displayed percentage value is accurate to what is in the back end.<br/>
            /// This should be always called as a "Validated" event
            /// </summary>
            /// <param name="sender">The object to update</param>
            private void ValidatePercentageTextBox(object sender, EventArgs e) {
                  if (sender is TextBox textbox) {
                        bool success = Int32.TryParse(Regex.FilterDigits(textbox.Text), out Int32 result);
                        if (success) {
                              result = Math.Clamp(result, 0, 100);
                        }

                        textbox.Text = result + "%";

                  } else {
                        Logger.Warn("Cannot append percentage to non-TextBox controls");
                  }
            }

            #endregion
      }
}