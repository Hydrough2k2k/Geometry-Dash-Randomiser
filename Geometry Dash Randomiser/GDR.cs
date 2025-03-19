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

                  qualitySelector.Items.Add(GameFiles.highQualityName);
                  qualitySelector.Items.Add(GameFiles.mediumQualityName);
                  qualitySelector.Items.Add(GameFiles.lowQualityName);

                  ApplyAllSettings();
                  updateElements();
            }

            private void ApplyAllSettings() {
                  this.IconTexturesCheckbox.Checked = Config.iconTextures.enabled;
                  this.IconTexturesGroupDisplay.Value = Config.iconTextures.group;
                  this.IconTexturesGroupDisplay.Enabled = this.IconTexturesCheckbox.Checked;

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

                  this.OrbsAndPadsCheckbox.Checked = Config.orbTextures.enabled;
                  this.OrbsAndPadsGroupDisplay.Value = Config.orbTextures.group;
                  this.OrbsAndPadsGroupDisplay.Enabled = this.OrbsAndPadsCheckbox.Checked;

                  this.ParticleTexturesCheckbox.Checked = Config.particleTextures.enabled;
                  this.ParticleTexturesGroupDisplay.Value = Config.particleTextures.group;
                  this.ParticleTexturesGroupDisplay.Enabled = this.ParticleTexturesCheckbox.Checked;

                  this.EffectsCheckbox.Checked = Config.effectTextures.enabled;
                  this.EffectsGroupDisplay.Value = Config.effectTextures.group;
                  this.EffectsGroupDisplay.Enabled = this.EffectsCheckbox.Checked;

                  this.MiscCheckbox.Checked = Config.miscTextures.enabled;
                  this.MiscGroupDisplay.Value = Config.miscTextures.group;
                  this.MiscGroupDisplay.Enabled = this.MiscCheckbox.Checked;


                  this.GameFolderTextBox.Text = Config.gameDirectory;
                  this.SeedInput.Text = Config.seed.ToString();
                  this.qualitySelector.SelectedIndex = (int)Config.quality;
                  this.textureCachingCheckbox.Checked = Config.caching;

                  readyState ready = GameFiles.getReadyState();
                  updateElements(ready);
            }

            private void IconTexturesSettingsChanged(object sender, EventArgs e) {
                  Config.iconTextures.enabled = this.IconTexturesCheckbox.Checked;
                  Config.iconTextures.group = (int)this.IconTexturesGroupDisplay.Value;
                  ApplyAllSettings();
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

            private void OrbsAndPadsSettingsChanged(object sender, EventArgs e) {
                  Config.orbTextures.enabled = this.OrbsAndPadsCheckbox.Checked;
                  Config.orbTextures.group = (int)this.OrbsAndPadsGroupDisplay.Value;
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

            private void GameFolderTextBox_TextChanged(object sender, EventArgs e) {
                  TextBox textBox = sender as TextBox;
                  Config.gameDirectory = textBox.Text;

                  ApplyAllSettings();
            }

            private void CachingSettingChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.caching = checkBox.Checked;
                  ApplyAllSettings();
            }

            private void SeedValueChanged(object sender, EventArgs e) {
                  NumericUpDown nud = sender as NumericUpDown;
                  Config.seed = (int)nud.Value;
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

            public void SetStartButtonState(bool state) {
                  startButton.Enabled = state;
            }

            private async void startButton_Click(object sender, EventArgs e) {
                  bool ready = GameFiles.isReady();
                  if (ready == false)
                        return;

                  SetUI_ReadonlyState(true);

                  this.startButton.Visible = false;
                  this.infoDisplay.Location = new Point(12, 421);
                  this.infoDisplay.Width = 750;
                  this.allFilesProgressBar.Visible = true;
                  this.fileProgressBar.Visible = true;

                  GameFiles.updateEvent += (eventSender, args) => { this.updateProgress(eventSender, args); };
                  GameFiles.changeDisplayedTextEvent += (eventSender, args) => { this.infoDisplay.Text = args; };
                  GameFiles.updateFileProgressEvent += (eventSender, args) => { this.fileProgressBar.Value = args; };
                  GameFiles.updateTotalProgressEvent += (eventSender, args) => { this.allFilesProgressBar.Value = args; };

                  GameSheet.changeDisplayedTextEvent += (eventSender, args) => { this.infoDisplay.Text = args; };

                  await Task.Run(() => GameFiles.StartRandomising());

                  this.startButton.Visible = true;
                  this.infoDisplay.Location = new Point(12, 450);
                  this.infoDisplay.Width = 611;
                  this.allFilesProgressBar.Visible = false;
                  this.fileProgressBar.Visible = false;

                  GameFiles.updateEvent -= (eventSender, args) => { this.updateProgress(eventSender, args); };
                  GameFiles.changeDisplayedTextEvent -= (eventSender, args) => { this.infoDisplay.Text = args; };
                  GameFiles.updateFileProgressEvent -= (eventSender, args) => { this.fileProgressBar.Value = args; };
                  GameFiles.updateTotalProgressEvent -= (eventSender, args) => { this.allFilesProgressBar.Value = args; };

                  GameSheet.changeDisplayedTextEvent -= (eventSender, args) => { this.infoDisplay.Text = args; };

                  SetUI_ReadonlyState(false);

                  ApplyAllSettings();

                  this.infoDisplay.Text = "Randomisation complete.\n" +
                        " - You can find the new files in the \"Randomised Files\" folder.\n" +
                        " - To reset them copy the files from the \"Unaltered Files\" folder. Have fun!";
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

            private void updateElements(readyState ready = readyState.Unknown) {
                  if (ready == readyState.Unknown)
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
                  this.SeedInput.Value = value;

                  ApplyAllSettings();
            }

            private void SetUI_ReadonlyState(bool readOnly) {
                  bool enabled = !readOnly;

                  this.IconTexturesCheckbox.Enabled = enabled;
                  this.IconTexturesGroupDisplay.Enabled = enabled;
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
                  this.OrbsAndPadsCheckbox.Enabled = enabled;
                  this.OrbsAndPadsGroupDisplay.Enabled = enabled;
                  this.ParticleTexturesCheckbox.Enabled = enabled;
                  this.ParticleTexturesGroupDisplay.Enabled = enabled;
                  this.EffectsCheckbox.Enabled = enabled;
                  this.EffectsGroupDisplay.Enabled = enabled;
                  this.MiscCheckbox.Enabled = enabled;
                  this.MiscGroupDisplay.Enabled = enabled;

                  this.GameFolderTextBox.Enabled = enabled;
                  this.gameFolderSelectorButton.Enabled = enabled;
                  this.SeedInput.Enabled = enabled;
                  this.randomSeedButton.Enabled = enabled;
                  this.qualitySelector.Enabled = enabled;
            }
      }
}
