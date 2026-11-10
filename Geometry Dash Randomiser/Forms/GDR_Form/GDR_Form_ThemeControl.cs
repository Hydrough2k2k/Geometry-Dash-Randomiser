using Geometry_Dash_Randomiser.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      public partial class GDR_Form : Form {

            private void SetTheme() {
                  if (this.themeController.Current.Name == ThemeController.RandomThemeName) {
                        // Generate an entirely random theme because why not
                        Theme randomTheme = Theme.CreateRandom();
                        this.themeController.Current.CopyColoursFrom(randomTheme);
                        SetTheme(randomTheme);

                        Logger.Write(Logger.Mode.Verbose,
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

                        Logger.Write(Logger.Mode.Verbose,
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
      }
}
