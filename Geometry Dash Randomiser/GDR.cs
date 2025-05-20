using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using static Geometry_Dash_Randomiser.GameFiles;

namespace Geometry_Dash_Randomiser {

      public partial class GDR_Form : Form {

            static readonly int minGroup = 0;
            static readonly int maxGroup = 10;

            GameFiles GameFiles;

            public GDR_Form() {
                  InitializeComponent();
                  
                  Config.ReadFile();
                  GameFiles = new GameFiles(this);
                  GameFiles.setQuality(Config.quality);

                  qualitySelector.Items.Add(GameFiles.highQualityName);
                  qualitySelector.Items.Add(GameFiles.mediumQualityName);
                  qualitySelector.Items.Add(GameFiles.lowQualityName);

                  this.seedInput.Value = Config.seed;

                  ApplyAllSettings();
            }

            private void GDR_Form_Load(object sender, EventArgs e) { }

            private void ApplyAllSettings() {
                  ReadyState readyState = GameFiles.getReadyState();

                  this.IconTexturesCheckbox.Checked = Config.iconTextures.enabled;
                  this.IconTexturesGroupDisplay.Value = Config.iconTextures.group;
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
                        this.CubeTexturesGroupDisplay.Enabled = Config.iconTextures.cube.enabled;
                        this.ShipTexturesCheckbox.Enabled = true;
                        this.ShipTexturesGroupDisplay.Enabled = Config.iconTextures.ship.enabled;
                        this.BallTexturesCheckbox.Enabled = true;
                        this.BallTexturesGroupDisplay.Enabled = Config.iconTextures.ball.enabled;
                        this.UFO_TexturesCheckbox.Enabled = true;
                        this.UFO_TexturesGroupDisplay.Enabled = Config.iconTextures.ufo.enabled;
                        this.WaveTexturesCheckbox.Enabled = true;
                        this.WaveTexturesGroupDisplay.Enabled = Config.iconTextures.wave.enabled;
                        this.RobotTexturesCheckbox.Enabled = true;
                        this.RobotTexturesGroupDisplay.Enabled = Config.iconTextures.robot.enabled;
                        this.SpiderTexturesCheckbox.Enabled = true;
                        this.SpiderTexturesGroupDisplay.Enabled = Config.iconTextures.spider.enabled;
                        this.SwingTexturesCheckbox.Enabled = true;
                        this.SwingTexturesGroupDisplay.Enabled = Config.iconTextures.swing.enabled;
                        this.JetpackTexturesCheckbox.Enabled = true;
                        this.JetpackTexturesGroupDisplay.Enabled = Config.iconTextures.jetpack.enabled;
                  }

                  this.CubeTexturesCheckbox.Checked = Config.iconTextures.cube.enabled;
                  this.CubeTexturesGroupDisplay.Value = Config.iconTextures.cube.group;
                  this.ShipTexturesCheckbox.Checked = Config.iconTextures.ship.enabled;
                  this.ShipTexturesGroupDisplay.Value = Config.iconTextures.ship.group;
                  this.BallTexturesCheckbox.Checked = Config.iconTextures.ball.enabled;
                  this.BallTexturesGroupDisplay.Value = Config.iconTextures.ball.group;
                  this.UFO_TexturesCheckbox.Checked = Config.iconTextures.ufo.enabled;
                  this.UFO_TexturesGroupDisplay.Value = Config.iconTextures.ufo.group;
                  this.WaveTexturesCheckbox.Checked = Config.iconTextures.wave.enabled;
                  this.WaveTexturesGroupDisplay.Value = Config.iconTextures.wave.group;
                  this.RobotTexturesCheckbox.Checked = Config.iconTextures.robot.enabled;
                  this.RobotTexturesGroupDisplay.Value = Config.iconTextures.robot.group;
                  this.SpiderTexturesCheckbox.Checked = Config.iconTextures.spider.enabled;
                  this.SpiderTexturesGroupDisplay.Value = Config.iconTextures.spider.group;
                  this.SwingTexturesCheckbox.Checked = Config.iconTextures.swing.enabled;
                  this.SwingTexturesGroupDisplay.Value = Config.iconTextures.swing.group;
                  this.JetpackTexturesCheckbox.Checked = Config.iconTextures.jetpack.enabled;
                  this.JetpackTexturesGroupDisplay.Value = Config.iconTextures.jetpack.group;

                  this.MenuTexturesCheckbox.Checked = Config.menuTextures.enabled;
                  this.MenuTexturesGroupDisplay.Value = Config.menuTextures.group;
                  this.MenuTexturesGroupDisplay.Enabled = this.MenuTexturesCheckbox.Checked;

                  this.ShopTexturesCheckbox.Checked = Config.shopTextures.enabled;
                  this.ShopTexturesGroupDisplay.Value = Config.shopTextures.group;
                  this.ShopTexturesGroupDisplay.Enabled = this.ShopTexturesCheckbox.Checked;

                  this.EditorTexturesCheckbox.Checked = Config.editorTextures.enabled;
                  this.EditorTexturesGroupDisplay.Value = Config.editorTextures.group;
                  this.EditorTexturesGroupDisplay.Enabled = this.EditorTexturesCheckbox.Checked;

                  this.BlocksCheckbox.Checked = Config.tileTextures.enabled;
                  this.BlocksGroupDisplay.Value = Config.tileTextures.group;
                  this.BlocksGroupDisplay.Enabled = this.BlocksCheckbox.Checked;

                  this.PortalTexturesCheckbox.Checked = Config.portalTextures.enabled;
                  this.PortalTexturesGroupDisplay.Value = Config.portalTextures.group;
                  this.PortalTexturesGroupDisplay.Enabled = this.PortalTexturesCheckbox.Checked;

                  this.OrbsCheckbox.Checked = Config.orbTextures.enabled;
                  this.OrbsGroupDisplay.Value = Config.orbTextures.group;
                  this.OrbsGroupDisplay.Enabled = this.OrbsCheckbox.Checked;

                  this.PadsCheckbox.Checked = Config.padTextures.enabled;
                  this.PadsGroupDisplay.Value = Config.padTextures.group;
                  this.PadsGroupDisplay.Enabled = this.PadsCheckbox.Checked;

                  this.ParticleTexturesCheckbox.Checked = Config.particleTextures.enabled;
                  this.ParticleTexturesGroupDisplay.Value = Config.particleTextures.group;
                  this.ParticleTexturesGroupDisplay.Enabled = this.ParticleTexturesCheckbox.Checked;

                  this.EffectsCheckbox.Checked = Config.effectTextures.enabled;
                  this.EffectsGroupDisplay.Value = Config.effectTextures.group;
                  this.EffectsGroupDisplay.Enabled = this.EffectsCheckbox.Checked;

                  this.MiscCheckbox.Checked = Config.miscTextures.enabled;
                  this.MiscGroupDisplay.Value = Config.miscTextures.group;
                  this.MiscGroupDisplay.Enabled = this.MiscCheckbox.Checked;


                  this.gameFolderTextBox.Text = Config.gameDirectory;
                  this.outputFolderTextBox.Text = Config.outputDirectory;

                  this.seedInput.Text = Config.seed.ToString();

                  this.qualitySelector.SelectedIndex = (int)Config.quality;
                  this.textureCachingCheckbox.Checked = Config.caching;

                  updateElements(readyState);
            }

            private void IconTexturesSettingsChanged(object sender, EventArgs e) {
                  bool enabled = this.IconTexturesCheckbox.Checked;

                  Config.iconTextures.enabled = enabled;

                  Config.iconTextures.cube.enabled = enabled;
                  Config.iconTextures.ship.enabled = enabled;
                  Config.iconTextures.ball.enabled = enabled;
                  Config.iconTextures.ufo.enabled = enabled;
                  Config.iconTextures.wave.enabled = enabled;
                  Config.iconTextures.robot.enabled = enabled;
                  Config.iconTextures.spider.enabled = enabled;
                  Config.iconTextures.swing.enabled = enabled;
                  Config.iconTextures.jetpack.enabled = enabled;
                  ApplyAllSettings();
            }

            private void CubeTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.iconTextures.cube.enabled = checkBox.Checked;
                  ApplyAllSettings();
            }

            private void ShipTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.iconTextures.ship.enabled = checkBox.Checked;
                  ApplyAllSettings();
            }

            private void BallTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.iconTextures.ball.enabled = checkBox.Checked;
                  ApplyAllSettings();
            }

            private void UFO_TexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.iconTextures.ufo.enabled = checkBox.Checked;
                  ApplyAllSettings();
            }

            private void WaveTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.iconTextures.wave.enabled = checkBox.Checked;
                  ApplyAllSettings();
            }

            private void RobotTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.iconTextures.robot.enabled = checkBox.Checked;
                  ApplyAllSettings();
            }

            private void SpiderTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.iconTextures.spider.enabled = checkBox.Checked;
                  ApplyAllSettings();
            }

            private void SwingTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.iconTextures.swing.enabled = checkBox.Checked;
                  ApplyAllSettings();
            }

            private void JetpackTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.iconTextures.jetpack.enabled = checkBox.Checked;
                  ApplyAllSettings();
            }

            private void IconTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  Config.iconTextures.group = (int)numericUpDown.Value;
                  CubeTexturesGroupChanged(sender, null);
                  ShipTexturesGroupChanged(sender, null);
                  BallTexturesGroupChanged(sender, null);
                  UFO_TexturesGroupChanged(sender, null);
                  WaveTexturesGroupChanged(sender, null);
                  RobotTexturesGroupChanged(sender, null);
                  SpiderTexturesGroupChanged(sender, null);
                  SwingTexturesGroupChanged(sender, null);
                  JetpackTexturesGroupChanged(sender, null);

                  ApplyAllSettings();
            }

            private void CubeTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  Config.iconTextures.cube.group = (int)numericUpDown.Value;
            }

            private void ShipTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  Config.iconTextures.ship.group = (int)numericUpDown.Value;
            }

            private void BallTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  Config.iconTextures.ball.group = (int)numericUpDown.Value;
            }

            private void UFO_TexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  Config.iconTextures.ufo.group = (int)numericUpDown.Value;
            }

            private void WaveTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  Config.iconTextures.wave.group = (int)numericUpDown.Value;
            }

            private void RobotTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  Config.iconTextures.robot.group = (int)numericUpDown.Value;
            }

            private void SpiderTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  Config.iconTextures.spider.group = (int)numericUpDown.Value;
            }

            private void SwingTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  Config.iconTextures.swing.group = (int)numericUpDown.Value;
            }

            private void JetpackTexturesGroupChanged(object sender, EventArgs e) {
                  NumericUpDown numericUpDown = sender as NumericUpDown;
                  Config.iconTextures.jetpack.group = (int)numericUpDown.Value;
            }

            private void MenuTexturesSettingsChanged(object sender, EventArgs e) {
                  Config.menuTextures.enabled = this.MenuTexturesCheckbox.Checked;
                  Config.menuTextures.group = (int)this.MenuTexturesGroupDisplay.Value;
                  ApplyAllSettings();
            }

            private void ShopTexturesSettingsChanged(object sender, EventArgs e) {
                  Config.shopTextures.enabled = this.ShopTexturesCheckbox.Checked;
                  Config.shopTextures.group = (int)this.ShopTexturesGroupDisplay.Value;
                  ApplyAllSettings();
            }

            private void EditorTexturesSettingsChanged(object sender, EventArgs e) {
                  Config.editorTextures.enabled = this.EditorTexturesCheckbox.Checked;
                  Config.editorTextures.group = (int)this.EditorTexturesGroupDisplay.Value;
                  ApplyAllSettings();
            }

            private void TilesTexturesSettingsChanged(object sender, EventArgs e) {
                  Config.tileTextures.enabled = this.BlocksCheckbox.Checked;
                  Config.tileTextures.group = (int)this.BlocksGroupDisplay.Value;
                  ApplyAllSettings();
            }

            private void PortalTexturesSettingsChanged(object sender, EventArgs e) {
                  Config.portalTextures.enabled = this.PortalTexturesCheckbox.Checked;
                  Config.portalTextures.group = (int)this.PortalTexturesGroupDisplay.Value;
                  ApplyAllSettings();
            }

            private void OrbsSettingsChanged(object sender, EventArgs e) {
                  Config.orbTextures.enabled = this.OrbsCheckbox.Checked;
                  Config.orbTextures.group = (int)this.OrbsGroupDisplay.Value;
                  ApplyAllSettings();
            }

            private void PadsSettingChanged(object sender, EventArgs e) {
                  Config.padTextures.enabled = this.PadsCheckbox.Checked;
                  Config.padTextures.group = (int)this.PadsGroupDisplay.Value;
                  ApplyAllSettings();
            }

            private void ParticleTexturesSettingsChanged(object sender, EventArgs e) {
                  Config.particleTextures.enabled = this.ParticleTexturesCheckbox.Checked;
                  Config.particleTextures.group = (int)this.ParticleTexturesGroupDisplay.Value;
                  ApplyAllSettings();
            }

            private void EffectsSettingsChanged(object sender, EventArgs e) {
                  Config.effectTextures.enabled = this.EffectsCheckbox.Checked;
                  Config.effectTextures.group = (int)this.EffectsGroupDisplay.Value;
                  ApplyAllSettings();
            }

            private void MiscSettingsChanged(object sender, EventArgs e) {
                  Config.miscTextures.enabled = this.MiscCheckbox.Checked;
                  Config.miscTextures.group = (int)this.MiscGroupDisplay.Value;
                  ApplyAllSettings();
            }

            private void SetGameFolder(object sender, EventArgs e) {
                  string folder = GetFolderViaExplorer(Config.gameDirectory, true);
                  if (folder != string.Empty)
                        Config.gameDirectory = folder;
                  ApplyAllSettings();
            }

            private void SetOutputFolder(object sender, EventArgs e) {
                  string folder = GetFolderViaExplorer(Config.outputDirectory, true);
                  if (folder != string.Empty)
                        Config.outputDirectory = folder;
                  ApplyAllSettings();
            }

            private void GameFolderTextBox_TextChanged(object sender, EventArgs e) {
                  TextBox textBox = sender as TextBox;
                  Config.gameDirectory = textBox.Text;

                  ApplyAllSettings();
            }

            private void outputFolderTextBox_TextChanged(object sender, EventArgs e) {
                  TextBox textBox = sender as TextBox;
                  Config.outputDirectory = textBox.Text;

                  ApplyAllSettings();
            }

            private void CachingSettingChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.caching = checkBox.Checked;
                  ApplyAllSettings();
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

            private void updateProgress(object sender, ProgressUpdate update) {
                  if (update.currentFile != string.Empty) {

                        string newText = string.Empty;
                        switch (update.currentStage) {
                              case Stage.BackingUp:
                                    newText = "Backing Up ";
                                    break;
                              case Stage.Unpacking:
                                    newText = "Unpacking ";
                                    break;
                              case Stage.Caching:
                                    newText = "Caching ";
                                    break;
                              case Stage.Randomising:
                                    newText = "Randomising ";
                                    break;
                              case Stage.Repackaging:
                                    newText = "Repackaging ";
                                    break;
                        }
                        newText += update.currentFile;
                        this.infoDisplay.Text = newText;
                  }

                  if (update.totalPercentComplete != -1) {
                        this.allFilesProgressBar.Value = update.totalPercentComplete;
                  }
            }

            private void updateElements(ReadyState ready = ReadyState.Unknown) {
                  if (ready == ReadyState.Unknown)
                        ready = GameFiles.getReadyState();

                  this.infoDisplay.Text = readyStateStrings[(int)ready];
                  this.startButton.Enabled = GameFiles.isReady(ready);
            }

            private void qualityOptionChanged(object sender, EventArgs e) {
                  DomainUpDown qualityDropdown = sender as DomainUpDown;

                  switch (qualityDropdown.Text) {
                        case GameFiles.lowQualityName:
                              GameFiles.setQuality(Quality.Low);
                              Config.quality = Quality.Low;
                              break;
                        case GameFiles.mediumQualityName:
                              GameFiles.setQuality(Quality.Medium);
                              Config.quality = Quality.Medium;
                              break;
                        case GameFiles.highQualityName:
                              GameFiles.setQuality(Quality.High);
                              Config.quality = Quality.High;
                              break;
                        default:
                              break;
                  }

                  ApplyAllSettings();
            }

            private void randomSeedButton_Click(object sender, EventArgs e) {
                  Random random = new Random(Guid.NewGuid().GetHashCode());
                  int value = random.Next(int.MinValue, int.MaxValue);
                  this.seedInput.Value = value;
                  Config.seed = value;

                  ApplyAllSettings();
            }

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
                  this.BlocksCheckbox.Enabled = enabled;
                  this.BlocksGroupDisplay.Enabled = enabled;
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
                  this.outputFolderTextBox.Enabled = enabled;
                  this.outputFolderSelectorButton.Enabled = enabled;
                  this.seedInput.Enabled = enabled;
                  this.randomSeedButton.Enabled = enabled;
                  this.qualitySelector.Enabled = enabled;
            }

            private void ChangelogButton_Click(object sender, EventArgs e) {

                  const string caption = "Changelog v2.1.2";
                  string[] message = new string[] {
                        "What's new?",
                        " - Added a Changelog button, woohoo!",
                        " - Added a top sneaky button. Where could it be?\n",

                        "Bugfixes:",
                        " - Fixed most of the glitched and cut off textures when remaking gamesheets",
                        " - Lowered memory usage of program from 1.5GB to under 1GB after randomisation"
                  };

                  MessageBoxButtons buttons = MessageBoxButtons.OK;
                  DialogResult result;

                  // Displays the MessageBox.
                  result = MessageBox.Show(string.Join("\n", message), caption, buttons);
                  if (result == System.Windows.Forms.DialogResult.Yes) {
                        // Closes the parent form.
                        this.Close();
                  }
            }

            private void groupInfoHelpButton_Click(object sender, EventArgs e) {

                  const string caption = "Texture Group Help";
                  string[] message = {
                        "You can add texture groups to randomisation groups via the number boxes.",
                        "If you have more than 1 texture group in a group, their textures will be mixed together.\n",
                        "For example: If you add both Menu and Editor groups to group 1, then the editor and the menu elements will be randomised together.",
                        "Some elements from the menu will appear in the editor and vica-versa.\n",
                        "If you add a texture group to group 0, they will not be mixed together, but rather each group will be shuffled separately.\n",
                        "If you add everything to group 1, the game will become very chaotic :)"
                  };

                  MessageBoxButtons buttons = MessageBoxButtons.OK;
                  DialogResult result;

                  // Displays the MessageBox.
                  result = MessageBox.Show(string.Join("\n", message), caption, buttons);
                  if (result == System.Windows.Forms.DialogResult.Yes) {
                        // Closes the parent form.
                        this.Close();
                  }
            }

            private void Logo_Click(object sender, EventArgs e) {

                  const string caption = "About this app";
                  string[] message = {
                        "You found a sneaky button :)\n",

                        "This app (and it's many horrible past variants) were born from passion.",
                        "This version of GDR is compatible with Geometry Dash version 2.207\n",

                        "Developer: Hydrough",
                        "Logo created by: Hydrough",
                        "Contact me:",
                        " - Discord: hydrough_7165",
                        " - GitHub: https://github.com/Hydrough2k2k\n",

                        "Special thanks to Danny for helping keep my sanity in tact while working on this project!"
                  };

                  MessageBoxButtons buttons = MessageBoxButtons.OK;
                  DialogResult result;

                  // Displays the MessageBox.
                  result = MessageBox.Show(string.Join("\n", message), caption, buttons);
                  if (result == System.Windows.Forms.DialogResult.Yes) {
                        // Closes the parent form.
                        this.Close();
                  }
            }

            private async void startButton_Click(object sender, EventArgs e) {
                  bool ready = GameFiles.isReady();
                  if (ready == false)
                        return;

                  SetUI_EnabledState(false);

                  this.startButton.Visible = false;
                  this.infoDisplay.Location = new Point(12, 421);
                  this.infoDisplay.Width = 984;
                  this.allFilesProgressBar.Visible = true;
                  this.fileProgressBar.Visible = true;

                  GameFiles.updateEvent += (eventSender, args) => { this.updateProgress(eventSender, args); };
                  GameFiles.changeDisplayedTextEvent += (eventSender, args) => { this.infoDisplay.Text = args; };
                  GameFiles.updateFileProgressEvent += (eventSender, args) => { this.fileProgressBar.Value = args; };
                  GameFiles.updateTotalProgressEvent += (eventSender, args) => { this.allFilesProgressBar.Value = args; };

                  // Create a new random seed if the input value is 0
                  int seed = Config.seed;
                  if (seed == 0) {
                        seed = Guid.NewGuid().GetHashCode();
                  }

                  await Task.Run(() => GameFiles.StartRandomising(seed));

                  this.startButton.Visible = true;
                  this.infoDisplay.Location = new Point(12, 450);
                  this.infoDisplay.Width = 846;
                  this.allFilesProgressBar.Visible = false;
                  this.fileProgressBar.Visible = false;

                  GameFiles.updateEvent -= (eventSender, args) => { this.updateProgress(eventSender, args); };
                  GameFiles.changeDisplayedTextEvent -= (eventSender, args) => { this.infoDisplay.Text = args; };
                  GameFiles.updateFileProgressEvent -= (eventSender, args) => { this.fileProgressBar.Value = args; };
                  GameFiles.updateTotalProgressEvent -= (eventSender, args) => { this.allFilesProgressBar.Value = args; };

                  SetUI_EnabledState(true);

                  ApplyAllSettings();

                  this.infoDisplay.Text = "Randomisation complete.\n";
                  switch (Config.GetOutputDirectoryStatus()) {
                        case Config.OutputFolder.Default:
                              this.infoDisplay.Text += " - You can find the new files in the \"Randomised Files\" folder.";
                              break;
                        case Config.OutputFolder.Overwritten:
                              this.infoDisplay.Text += " - You can find the new files in the given output folder.";
                              break;
                  }
                  if (Config.seed != seed)
                        this.infoDisplay.Text += " The used seed was " + seed.ToString("N0");

                  this.infoDisplay.Text += "\n - To reset them copy the files from the \"Unaltered Files\" folder. Have fun!";
            }

            private void SeedValueChanged(object sender, EventArgs e) {
                  NumericUpDown nud = sender as NumericUpDown;
                  Config.seed = (int)nud.Value;
                  ApplyAllSettings();
            }
      }
}
