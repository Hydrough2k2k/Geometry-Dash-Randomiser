using Geometry_Dash_Randomiser.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;
using static Geometry_Dash_Randomiser.Logger;

namespace Geometry_Dash_Randomiser {

      public partial class GDR_Form : Form {

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
                        Logger.Write(Mode.Error, $"The \"randomisationSetting\" was passed as null to \"ToggleLinkedControls\"");
                        return;
                  }

                  if (parentControl == null) {
                        Logger.Write(Mode.Error, $"The \"parentControl\" was passed as null to \"ToggleLinkedControls\"");
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
                  iconTextureTypeLabel.RightAlign(iconTextureWarningIcon);

                  // Game Textures
                  gameTextureTypeLabel.RightAlign(gameTextureWarningIcon);

                  // Font Settings
                  fontRandEnabledCheckbox.RightAlign(fontRandWarningIcon);
                  fontStyleRandModeLabel.RightAlign(fontStyleModeSelector);
                  randomiseCharactersCheckBox.RightAlign(fontRandomiseCharactersInfoIcon);
                  letterSpacingModeLabel.RightAlign(letterSpacingModeSelector);
                  letterSpacingModeSelector.RightAlign(letterSpacingModeInfoIcon);
                  letterSpacingLevelLabel.RightAlign(letterSpacingLevelInputBox);

                  // Randomisation Settings
                  randomisationSeedLabel.RightAlign(seedInfoIcon);

                  // Application Settings
                  gameFolderLabel.RightAlign(gameFolderWarningIcon);
            }

            private string GetReadyStatusDisplayText(ReadyState ready, bool corrupt = true) {
                  string ret = ready.GetMessageString();

                  if (corrupt) {
                        ret = this.textCorruptor.CorruptText(ret);
                  }

                  return ret;
            }

            private void UpdateRandomisationControlsStates(CheckBox checkBox, TextBox textBox, NumericUpDown groupDisplay, RandomizationSetting setting) {
                  checkBox.Checked = setting.Enabled;
                  textBox.Enabled = setting.Enabled;
                  textBox.Text = setting.PercentageRandomised + "%";
                  groupDisplay.Enabled = setting.Enabled;
                  groupDisplay.Value = setting.Group;
            }

            private void SetAllIconTexturesElementStates() {
                  ToggleAllLinkedControls();

                  // I know all of these are horribly long, but to keep the names distinct, I kinda have to do this :/
                  UpdateRandomisationControlsStates(IconTexturesCheckbox, IconTexturesPercentageBox, IconTexturesGroupDisplay, (RandomizationSetting)config.IconTextures);

                  UpdateRandomisationControlsStates(CubeTexturesCheckbox, CubeTexturesPercentageBox, CubeTexturesGroupDisplay_Gr, config.IconTextures.Cube);
                  UpdateRandomisationControlsStates(ShipTexturesCheckbox, ShipTexturesPercentageBox, ShipTexturesGroupDisplay_Gr, config.IconTextures.Ship);
                  UpdateRandomisationControlsStates(BallTexturesCheckbox, BallTexturesPercentageBox, BallTexturesGroupDisplay_Gr, config.IconTextures.Ball);
                  UpdateRandomisationControlsStates(UFO_TexturesCheckbox, UFO_TexturesPercentageBox, UFO_TexturesGroupDisplay, config.IconTextures.Ufo);
                  UpdateRandomisationControlsStates(WaveTexturesCheckbox, WaveTexturesPercentageBox, WaveTexturesGroupDisplay, config.IconTextures.Wave);
                  UpdateRandomisationControlsStates(RobotTexturesCheckbox, RobotTexturesPercentageBox, RobotTexturesGroupDisplay, config.IconTextures.Robot);
                  UpdateRandomisationControlsStates(SpiderTexturesCheckbox, SpiderTexturesPercentageBox, SpiderTexturesGroupDisplay, config.IconTextures.Spider);
                  UpdateRandomisationControlsStates(SwingTexturesCheckbox, SwingTexturesPercentageBox, SwingTexturesGroupDisplay, config.IconTextures.Swing);
                  UpdateRandomisationControlsStates(JetpackTexturesCheckbox, JetpackTexturesPercentageBox, JetpackTexturesGroupDisplay, config.IconTextures.Jetpack);
            }

            private void SetAllGameTexturesElementStates() {
                  // I know all of these are horribly long, but to keep the names distinct, I kinda have to do this :/
                  UpdateRandomisationControlsStates(MenuTexturesCheckbox, MenuTexturesPercentageBox, MenuTexturesGroupDisplay, config.MenuTextures);
                  UpdateRandomisationControlsStates(ShopTexturesCheckbox, ShopTexturesPercentageBox, ShopTexturesGroupDisplay, config.ShopTextures);
                  UpdateRandomisationControlsStates(EditorTexturesCheckbox, EditorTexturesPercentageBox, EditorTexturesGroupDisplay, config.EditorTextures);
                  UpdateRandomisationControlsStates(BlockTexturesCheckbox, TileTexturesPercentageBox, TileTexturesGroupDisplay, config.TileTextures);
                  UpdateRandomisationControlsStates(PortalTexturesCheckbox, PortalTexturesPercentageBox, PortalTexturesGroupDisplay, config.PortalTextures);
                  UpdateRandomisationControlsStates(OrbsTexturesCheckbox, OrbTexturesPercentageBox, OrbTexturesGroupDisplay, config.OrbTextures);
                  UpdateRandomisationControlsStates(PadsTexturesCheckbox, PadTexturesPercentageBox, PadTexturesGroupDisplay, config.PadTextures);
                  UpdateRandomisationControlsStates(ParticleTexturesCheckbox, ParticleTexturesPercentageBox, ParticleTexturesGroupDisplay, config.ParticleTextures);
                  UpdateRandomisationControlsStates(EffectTexturesCheckbox, EffectTexturesPercentageBox, EffectTexturesGroupDisplay, config.EffectTextures);
                  UpdateRandomisationControlsStates(MiscTexturesCheckbox, MiscTexturesPercentageBox, MiscTexturesGroupDisplay, config.MiscTextures);
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

                  UpdateRandomisationControlsStates(characterLetterRandCheckBox, characterLetterRandPercentageBox, characterLetterRandGroupDisplay, CRS.Letter);
                  UpdateRandomisationControlsStates(characterNumberRandCheckBox, characterNumberRandPercentageBox, characterNumberRandGroupDisplay, CRS.Number);
                  UpdateRandomisationControlsStates(characterSymbolRandCheckBox, characterSymbolRandPercentageBox, characterSymbolRandGroupDisplay, CRS.Symbol);
                  UpdateRandomisationControlsStates(characterSpecialRandCheckBox, characterSpecialRandPercentageBox, characterSpecialRandGroupDisplay, CRS.Special);
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

                  SetSliderValueForSpriteSize();

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
                  this.randomisingProgressBar.Value = (int)ProgressState.Instance.PercentComplete;
            }

            private void UpdateProgressElapsedTime(string newTimePrint) {

                  this.elapsedTimeDisplay.Text = this.textCorruptor.CorruptText(newTimePrint);
                  // Right-align the text so it will be near the Randomise button
                  this.elapsedTimeDisplay.Location = new Point(803 - this.elapsedTimeDisplay.PreferredWidth, this.elapsedTimeDisplay.Location.Y);
            }

      }
}
