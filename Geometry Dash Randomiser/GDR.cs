using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Runtime.InteropServices;
using System.Threading;
using static Geometry_Dash_Randomiser.GameFileManager;
using System.IO;

namespace Geometry_Dash_Randomiser {

      public partial class GDR_Form : Form {

            private const string version = "V2.3.0";

            private int textCorruptionLevel = 0;

            PathManager pathManager;
            GameFileManager gameFileManager;

            ThemeController themeController = new ThemeController();

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
            static extern bool AllocConsole();

            public GDR_Form() {
                  InitializeComponent();

                  #if DEBUG
                        AllocConsole();
                  #endif

                  Config.ReadFile();
                  this.gameFileManager = new GameFileManager(this);
                  this.pathManager = gameFileManager.pathManager;
                  this.pathManager.SetQuality(Config.quality);

                  this.textureQualitySelectorBox.Items.Add(PathManager.highQualityName);
                  this.textureQualitySelectorBox.Items.Add(PathManager.mediumQualityName);
                  this.textureQualitySelectorBox.Items.Add(PathManager.lowQualityName);

                  if (this.themeController.GetThemeCount() < Config.themeID) {
                        Config.themeID = 0;
                  }

                  this.themeController.activeThemeID = Config.themeID;

                  this.applicationThemeSelectorBox.Items.AddRange(this.themeController.GetAllThemeNames());

                  ReadyState readyState = ApplyAllSettings();

                  this.versionLabel.Text = version;
            }

            private void GDR_Form_Shown(object sender, EventArgs e) {
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

            private void GDR_Form_Load(object sender, EventArgs e) { }

            private ReadyState ApplyAllSettings() {
                  ReadyState ready = gameFileManager.getReadyState();

                  SetAllIconTexturesElements();
                  SetAllMenuTexturesElements();
                  SetAllFontRandElements();

                  this.gameFolderTextBox.Text = Config.gameDirectory;

                  this.seedInputBox.Text = Config.seed.ToString();
                  this.seedInputBox.Value = Config.seed;

                  this.textureQualitySelectorBox.SelectedIndex = (int)Config.quality;
                  this.applicationThemeSelectorBox.SelectedIndex = (int)Config.themeID;

                  if (Config.maxSpriteMultiplier < 1000) {
                        this.spriteSizeMultiplierTextbox.Text = Config.maxSpriteMultiplier.ToString() + "x";
                  } else {
                        this.spriteSizeMultiplierTextbox.Text = "Unlimited";
                  }

                  this.allowDuplicatesCheckbox.Checked = Config.allowDuplicates;

                  this.ReadyStatusDisplay.Text = GetReadyStatusDisplayText(ready);
                  this.startButton.Enabled = ready.HasFlag(ReadyState.Ready);

                  return ready;
            }

            private string GetReadyStatusDisplayText(ReadyState ready) {
                  if (ready.HasFlag(ReadyState.Ready)) {
                        return "The randomisation can begin";

                  } else if (ready.HasFlag(ReadyState.GameFolderNotFound)) {
                        return "The game folder doesn't exist, or the exe can not be found";

                  } else if (ready.HasFlag(ReadyState.NoSettingsEnabled)) {
                        return "No settings are enabled for randomisation. Enable at least one setting";
                  }
                  Console.WriteLine("No valid state is displayable to the user");
                  return string.Empty;
            }

            private void SetAllIconTexturesElements() {
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
            }

            private void SetAllMenuTexturesElements() {
                  this.MenuTexturesCheckbox.Checked = Config.menuTextures.enabled;
                  this.MenuTexturesGroupDisplay.Value = Config.menuTextures.group;
                  this.MenuTexturesGroupDisplay.Enabled = this.MenuTexturesCheckbox.Checked;

                  this.ShopTexturesCheckbox.Checked = Config.shopTextures.enabled;
                  this.ShopTexturesGroupDisplay.Value = Config.shopTextures.group;
                  this.ShopTexturesGroupDisplay.Enabled = this.ShopTexturesCheckbox.Checked;

                  this.EditorTexturesCheckbox.Checked = Config.editorTextures.enabled;
                  this.EditorTexturesGroupDisplay.Value = Config.editorTextures.group;
                  this.EditorTexturesGroupDisplay.Enabled = this.EditorTexturesCheckbox.Checked;

                  this.BlockTexturesCheckbox.Checked = Config.tileTextures.enabled;
                  this.BlockTexturesGroupDisplay.Value = Config.tileTextures.group;
                  this.BlockTexturesGroupDisplay.Enabled = this.BlockTexturesCheckbox.Checked;

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
            }

            private void SetAllFontRandElements() {
                  FontRandomisationSettings fontRand = Config.fontRand;

                  this.fontRandEnabledCheckbox.Checked = fontRand.enabled;

                  // If the randomisation is disabled, deactivate all related controls
                  if (fontRand.enabled == false) {
                        this.fontShuffleStylesCheckbox.Enabled = false;
                        this.fontPerLetterRandomisationButton.Enabled = false;
                        this.fontPerFontRandomisationButton.Enabled = false;
                        this.fontRandomiseLettersCheckbox.Enabled = false;
                        return;

                  } else {
                        this.fontShuffleStylesCheckbox.Enabled = true;
                        this.fontPerLetterRandomisationButton.Enabled = true;
                        this.fontPerFontRandomisationButton.Enabled = true;
                        this.fontRandomiseLettersCheckbox.Enabled = true;
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

            private void SaveConfigFileAfterDelay(int seconds = 3) {
                  Task.Delay(new TimeSpan(0, 0, seconds)).ContinueWith(o => { Config.WriteFile(); });
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
                  SaveConfigFileAfterDelay();
            }

            private void CubeTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.iconTextures.cube.enabled = checkBox.Checked;
                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
            }

            private void ShipTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.iconTextures.ship.enabled = checkBox.Checked;
                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
            }

            private void BallTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.iconTextures.ball.enabled = checkBox.Checked;
                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
            }

            private void UFO_TexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.iconTextures.ufo.enabled = checkBox.Checked;
                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
            }

            private void WaveTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.iconTextures.wave.enabled = checkBox.Checked;
                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
            }

            private void RobotTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.iconTextures.robot.enabled = checkBox.Checked;
                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
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
                  SaveConfigFileAfterDelay();
            }

            private void JetpackTexturesEnabledChanged(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  Config.iconTextures.jetpack.enabled = checkBox.Checked;
                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
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
                  SaveConfigFileAfterDelay();
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
                  SaveConfigFileAfterDelay();
            }

            private void ShopTexturesSettingsChanged(object sender, EventArgs e) {
                  Config.shopTextures.enabled = this.ShopTexturesCheckbox.Checked;
                  Config.shopTextures.group = (int)this.ShopTexturesGroupDisplay.Value;
                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
            }

            private void EditorTexturesSettingsChanged(object sender, EventArgs e) {
                  Config.editorTextures.enabled = this.EditorTexturesCheckbox.Checked;
                  Config.editorTextures.group = (int)this.EditorTexturesGroupDisplay.Value;
                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
            }

            private void TilesTexturesSettingsChanged(object sender, EventArgs e) {
                  Config.tileTextures.enabled = this.BlockTexturesCheckbox.Checked;
                  Config.tileTextures.group = (int)this.BlockTexturesGroupDisplay.Value;
                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
            }

            private void PortalTexturesSettingsChanged(object sender, EventArgs e) {
                  Config.portalTextures.enabled = this.PortalTexturesCheckbox.Checked;
                  Config.portalTextures.group = (int)this.PortalTexturesGroupDisplay.Value;
                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
            }

            private void OrbsSettingsChanged(object sender, EventArgs e) {
                  Config.orbTextures.enabled = this.OrbsCheckbox.Checked;
                  Config.orbTextures.group = (int)this.OrbsGroupDisplay.Value;
                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
            }

            private void PadsSettingChanged(object sender, EventArgs e) {
                  Config.padTextures.enabled = this.PadsCheckbox.Checked;
                  Config.padTextures.group = (int)this.PadsGroupDisplay.Value;
                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
            }

            private void ParticleTexturesSettingsChanged(object sender, EventArgs e) {
                  Config.particleTextures.enabled = this.ParticleTexturesCheckbox.Checked;
                  Config.particleTextures.group = (int)this.ParticleTexturesGroupDisplay.Value;
                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
            }

            private void EffectsSettingsChanged(object sender, EventArgs e) {
                  Config.effectTextures.enabled = this.EffectsCheckbox.Checked;
                  Config.effectTextures.group = (int)this.EffectsGroupDisplay.Value;
                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
            }

            private void MiscSettingsChanged(object sender, EventArgs e) {
                  Config.miscTextures.enabled = this.MiscCheckbox.Checked;
                  Config.miscTextures.group = (int)this.MiscGroupDisplay.Value;
                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
            }

            private void fontRandEnabledCheckbox_CheckedChanged(object sender, EventArgs e) {
                  Config.fontRand.enabled = (sender as CheckBox).Checked;
                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
            }

            private void fontShuffleStylesCheckbox_CheckedChanged(object sender, EventArgs e) {
                  Config.fontRand.shuffleFontStyles = (sender as CheckBox).Checked;
                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
            }

            private void fontRandomiseLettersCheckbox_CheckedChanged(object sender, EventArgs e) {
                  Config.fontRand.randomiseLetters = (sender as CheckBox).Checked;
                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
            }

            private void fontPerFontRandomisationButton_CheckedChanged(object sender, EventArgs e) {
                  Config.fontRand.shufflingMode = FontRandomisationSettings.FontStyleShufflingMode.PerFont;
            }

            private void fontPerLetterRandomisationButton_CheckedChanged(object sender, EventArgs e) {
                  Config.fontRand.shufflingMode = FontRandomisationSettings.FontStyleShufflingMode.PerLetter;
            }

            private void SetGameFolder(object sender, EventArgs e) {
                  string folder = GetFolderViaExplorer(Config.gameDirectory, true);
                  if (folder != string.Empty)
                        Config.gameDirectory = folder;
                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
            }

            private void GameFolderTextBox_TextChanged(object sender, EventArgs e) {
                  TextBox textBox = sender as TextBox;
                  Config.gameDirectory = textBox.Text;

                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
            }

            private void CachingSettingChanged(object sender, EventArgs e) {

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

            private void qualityOptionChanged(object sender, EventArgs e) {
                  DomainUpDown qualityDropdown = sender as DomainUpDown;

                  switch (qualityDropdown.Text) {
                        case PathManager.lowQualityName:
                              pathManager.SetQuality(Quality.Low);
                              Config.quality = Quality.Low;
                              break;
                        case PathManager.mediumQualityName:
                              pathManager.SetQuality(Quality.Medium);
                              Config.quality = Quality.Medium;
                              break;
                        case PathManager.highQualityName:
                              pathManager.SetQuality(Quality.High);
                              Config.quality = Quality.High;
                              break;
                        default:
                              break;
                  }

                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
            }

            private void randomSeedButton_Click(object sender, EventArgs e) {
                  Random random = new Random(Guid.NewGuid().GetHashCode());
                  int value = random.Next(int.MinValue, int.MaxValue);
                  this.seedInputBox.Value = value;
                  Config.seed = value;

                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
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

                  Config.maxSpriteMultiplier = newMultiplier;

                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
            }

            private void allowDuplicatesCheckbox_Click(object sender, EventArgs e) {
                  CheckBox trackBar = sender as CheckBox;

                  Config.allowDuplicates = trackBar.Checked;

                  ApplyAllSettings();
                  SaveConfigFileAfterDelay();
            }

            // MORE THINGS NEED TO BE ADDED
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

                  this.spriteSizeMultiplierTrackbar.Enabled = enabled;
                  this.spriteSizeMultiplierTextbox.Enabled = enabled;
                  this.allowDuplicatesCheckbox.Enabled = enabled;
            }

            private void ChangelogButton_Click(object sender, EventArgs e) {

                  const string caption = "Changelog v2.3.0";
                  string[] message = new string[] {
                        "What's new?",
                        " - Major visual overhaul, it should be easier to navigate this app now",
                        " - Text Randomisation with a lot of settings with more to come later",
                        " - Restore files button to quickly return the game to it's normal (boring) state",
                        " - File for \"Blacklisted files\". Every file added to this will be ignored when randomising files",
                        " - Some new and reworked themes. Random theme also exists for some reason\n",

                        "Changes:",
                        " - Removed the custom output directory. The backup files will always be in the application's folder\n",

                        "Bugfixes:",
                        " - Fixed crashes caused by config file writing and reading running into errors",
                        " - Every value on the Texture Size Multiplier slider is available now\n"
                  };

                  MessageBoxButtons buttons = MessageBoxButtons.OK;
                  MessageBox.Show(string.Join("\n", message), caption, buttons);
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
                  MessageBox.Show(string.Join("\n", message), caption, buttons);
            }

            private void Logo_Click(object sender, EventArgs e) {

                  const string caption = "About this app";
                  string[] message = {
                        "You found a sneaky button :)\n",

                        "This app (and it's many horrible past variants) were born from passion.",
                        "This version of GDR is compatible with Geometry Dash version 2.207\n",

                        "Developer: Hydrough",
                        "Logo created by: Hydrough",
                        "RectpackSharp library made by ThomasMiz:",
                        " - https://github.com/ThomasMiz/RectpackSharp",
                        "Wisteria theme made by my friend, Maya\n",

                        "Contact me:",
                        " - Discord: hydrough_7165",
                        " - GitHub: https://github.com/Hydrough2k2k\n",

                        "Special thanks to my friends for testing this and giving me ideas!",
                  };

                  MessageBoxButtons buttons = MessageBoxButtons.OK;
                  MessageBox.Show(string.Join("\n", message), caption, buttons);
            }

            private void restoreFilesButton_Click(object sender, EventArgs e) {
                  this.startButton.Enabled = false;
                  this.restoreFilesButton.Enabled = false;
                  this.ReadyStatusDisplay.Visible = false;
                  this.RandomisingProgressBar.Visible = true;
                  this.RandomisingProgressDisplay.Visible = true;

                  gameFileManager.RestoreFiles();

                  this.startButton.Enabled = true;
                  this.restoreFilesButton.Enabled = false;
                  this.ReadyStatusDisplay.Visible = true;
                  this.RandomisingProgressBar.Visible = false;
                  this.RandomisingProgressDisplay.Visible = false;
            }

            private async void startButton_Click(object sender, EventArgs e) {
                  bool ready = gameFileManager.getReadyState().HasFlag(ReadyState.Ready);
                  if (ready == false)
                        return;

                  SetUI_EnabledState(false);

                  this.startButton.Enabled = false;
                  this.restoreFilesButton.Enabled = false;
                  this.ReadyStatusDisplay.Visible = false;
                  this.RandomisingProgressBar.Visible = true;
                  this.RandomisingProgressDisplay.Visible = true;

                  // Create a new random seed if the input value is 0
                  int seed = Config.seed;
                  if (seed == 0) {
                        seed = Guid.NewGuid().GetHashCode();
                  }

                  Thread randomisationThread = new Thread(() => {
                        gameFileManager.StartRandomising(seed);
                  });
                  randomisationThread.Start();

                  string lastPrint = string.Empty;

                  while (randomisationThread.IsAlive == true) {
                        string newPrint = gameFileManager.progressState.GetProgressString();

                        if (newPrint != lastPrint) {
                              lastPrint = newPrint;

                              this.RandomisingProgressDisplay.Text = newPrint;
                        }
                        await Task.Run(() => {
                              Thread.Sleep(25);
                        });
                  }

                  SetUI_EnabledState(true);

                  ApplyAllSettings();

                  this.ReadyStatusDisplay.Visible = true;
                  this.RandomisingProgressBar.Visible = false;
                  this.RandomisingProgressDisplay.Visible = false;

                  this.ReadyStatusDisplay.Text = "Randomisation complete";

                  if (Config.seed != seed) {
                        this.ReadyStatusDisplay.Text += ". The used seed was " + seed.ToString("N0");
                  }

                  // If the game directory is valid, enable the restore button
                  this.restoreFilesButton.Enabled = gameFileManager.IsGameDirectoryValid();
            }

            private void SeedValueChanged(object sender, EventArgs e) {
                  NumericUpDown nud = sender as NumericUpDown;
                  Config.seed = (int)nud.Value;
                  ApplyAllSettings();
            }

            private void SetTheme() {
                  if (this.themeController.current.name == "Random Theme") {
                        Random random = new Random(Guid.NewGuid().GetHashCode());

                        // Help me, what is this
                        SetTheme(new Theme(
                              name: "Random Theme",
                              formBackColour: Color.FromArgb(random.Next(255), random.Next(255), random.Next(255)),
                              defaultTextColour: Color.FromArgb(random.Next(255), random.Next(255), random.Next(255)),
                              menuElementBackColour: Color.FromArgb(random.Next(255), random.Next(255), random.Next(255)),
                              menuElementForeColour: Color.FromArgb(random.Next(255), random.Next(255), random.Next(255)),
                              beamColour: Color.FromArgb(random.Next(255), random.Next(255), random.Next(255))
                        ));
                  } else {
                        SetTheme(this.themeController.current);
                  }
            }

            private void SetTheme(Theme theme) {
                  SetFormColours(theme.formBackColour);

                  Color TextColor = theme.defaultTextColour;
                  SetTextColours(TextColor);
                  SetCheckboxColours(TextColor);
                  SetRadioButtonColours(TextColor);

                  SetMenuElementColours(theme.menuElementBackColour, theme.menuElementForeColour);

                  UpdateImageTheme(theme);
            }

            private void SetFormColours(Color back) {
                  this.BackColor = back;
            }

            private void SetTextColours(Color color) {
                  for (int i = 0; i < labels.Length; i++) {
                        if (labels[i].Name.Contains("NoCol") == false) {
                              labels[i].ForeColor = color;
                        }
                  }
            }

            private void SetCheckboxColours(Color fore) {
                  for (int i = 0; i < checkBoxes.Length; i++) {
                        checkBoxes[i].ForeColor = fore;
                  }
            }

            private void SetRadioButtonColours(Color fore) {
                  for (int i = 0; i < radioButtons.Length; i++) {
                        radioButtons[i].ForeColor = fore;
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
                  this.themeController.activeThemeID = newThemeID;

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
                  
                  int charAlteringProbability = 10;

                  // Random chance to alter the text of most elements on screen
                  if (random.Next(3) > 0) {
                        for (int i = 0; i < this.labels.Length; i++)
                              this.labels[i].Text = this.labels[i].Text.AlterRandomCharacters(charAlteringProbability);

                        for (int i = 0; i < this.checkBoxes.Length; i++)
                              this.checkBoxes[i].Text = this.checkBoxes[i].Text.AlterRandomCharacters(charAlteringProbability);

                        for (int i = 0; i < this.groupBoxes.Length; i++)
                              this.groupBoxes[i].Text = this.groupBoxes[i].Text.AlterRandomCharacters(charAlteringProbability);

                        for (int i = 0; i < this.buttons.Length; i++)
                              this.buttons[i].Text = this.buttons[i].Text.AlterRandomCharacters(charAlteringProbability);

                        for (int i = 0; i < this.radioButtons.Length; i++)
                              this.radioButtons[i].Text = this.radioButtons[i].Text.AlterRandomCharacters(charAlteringProbability);

                        textCorruptionLevel++;
                  }
            }

            private void SetStatusTextColour(Color fore) {
                  SetColours(this.ReadyStatusDisplay, fore, Color.FromArgb(0, 0, 0, 0));
            }

            private void SetColours(Label label, Color2 colours) {
                  SetColours(label, colours.color1, colours.color2);
            }

            private void SetColours(Label label, Color fore, Color back) {
                  label.ForeColor = fore;
                  label.BackColor = back;
            }
      }
}
