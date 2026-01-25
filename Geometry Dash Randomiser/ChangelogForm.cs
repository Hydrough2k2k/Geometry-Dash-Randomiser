using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      public partial class ChangelogForm : Form {

            // Data received from the main Form
            public Theme theme;
            public int textCorruptionLevel = 0;

            // Local data
            private ChangelogData[] changelogs = Array.Empty<ChangelogData>();
            int currentChangelogIndex = 0;

            public ChangelogForm(string version) {
                  InitializeComponent();

                  this.changelogVersionLabel.Text = version;

                  changelogs = LoadChangelogsFromFile();

                  this.nextVersionButton.Enabled = false;
            }

            private void SetTheme() {
                  this.BackColor = theme.backgroundColour.AdjustBrightness(0.60f);

                  this.changelogHeaderLabel.ForeColor = theme.textColour;
                  this.changelogVersionLabel.ForeColor = theme.textColour;

                  this.whatsNewLabel.ForeColor = theme.textColour;
                  this.whatsNewTextBox.BackColor = theme.objectBackColour.AdjustBrightness(0.50f);
                  this.whatsNewTextBox.ForeColor = theme.objectTextColour.AdjustBrightness(0.80f);

                  this.bugfixesLabel.ForeColor = theme.textColour;
                  this.bugfixesTextBox.BackColor = theme.objectBackColour.AdjustBrightness(0.50f);
                  this.bugfixesTextBox.ForeColor = theme.objectTextColour.AdjustBrightness(0.80f);

                  this.knownBugsLabel.ForeColor = theme.textColour;
                  this.knownBugsTextBox.BackColor = theme.objectBackColour.AdjustBrightness(0.50f);
                  this.knownBugsTextBox.ForeColor = theme.objectTextColour.AdjustBrightness(0.80f);

                  this.previousVersionButton.ForeColor = theme.objectTextColour;
                  this.previousVersionButton.BackColor = theme.objectBackColour.AdjustBrightness(0.70f);

                  this.nextVersionButton.ForeColor = theme.objectTextColour;
                  this.nextVersionButton.BackColor = theme.objectBackColour.AdjustBrightness(0.70f);
            }

            private void _FormClosing(object sender, FormClosingEventArgs e) {
                  e.Cancel = true;
                  this._Deactivate(sender, e as EventArgs);
            }

            private void _Activated(object sender, EventArgs e) {
                  SetTheme();

                  PopulateTextBoxes(changelogs[currentChangelogIndex]);
                  CorruptFormText(textCorruptionLevel);
                  ResizeWindowAndElements();

                  this.Text = "GDR Changelog";
            }

            private void _Deactivate(object sender, EventArgs e) {
                  this.Hide();
            }

            private ChangelogData[] LoadChangelogsFromFile() {
                  if (File.Exists(changelogFileName) == false) {
                        Console.WriteLine($"Could not find file {changelogFileName}, the changelogs could not be loaded");
                        return new ChangelogData[] { ChangelogData.Default };
                  }
                  
                  List<ChangelogData> changelogList = new List<ChangelogData>();

                  string[] fileData = File.ReadAllLines(changelogFileName);

                  List<string> croppedData = new List<string>();
                  for (int i = 0; i < fileData.Length; i++) {
                        croppedData.Add(fileData[i]);

                        if (fileData[i].Contains("----------")) {
                              changelogList.Add(
                                    ChangelogData.ConvertFromData(croppedData)
                              );

                              croppedData.Clear();
                        }
                  }

                  changelogList.Add(
                        ChangelogData.ConvertFromData(croppedData)
                  );

                  return changelogList.ToArray();
            }

            private void CorruptFormText(int corruptionLevel) {
                  if (textCorruptionLevel == 0)
                        return;

                  this.previousVersionButton.Text = this.previousVersionButton.Text.AlterRandomCharactersLooped(corruptionLevel);
                  this.nextVersionButton.Text = this.nextVersionButton.Text.AlterRandomCharactersLooped(corruptionLevel);

                  this.changelogHeaderLabel.Text = this.changelogHeaderLabel.Text.AlterRandomCharactersLooped(corruptionLevel);
                  this.changelogVersionLabel.Text = this.changelogVersionLabel.Text.AlterRandomCharactersLooped(corruptionLevel);

                  this.whatsNewLabel.Text = this.whatsNewLabel.Text.AlterRandomCharactersLooped(corruptionLevel);
                  this.whatsNewTextBox.Text = this.whatsNewTextBox.Text.AlterRandomCharactersLooped(corruptionLevel);

                  this.bugfixesLabel.Text = this.bugfixesLabel.Text.AlterRandomCharactersLooped(corruptionLevel);
                  this.bugfixesTextBox.Text = this.bugfixesTextBox.Text.AlterRandomCharactersLooped(corruptionLevel);

                  this.knownBugsLabel.Text = this.knownBugsLabel.Text.AlterRandomCharactersLooped(corruptionLevel);
                  this.knownBugsTextBox.Text = this.knownBugsTextBox.Text.AlterRandomCharactersLooped(corruptionLevel);
            }

            private void PopulateTextBoxes(ChangelogData data) {
                  this.changelogHeaderLabel.Text = "Changelog for";
                  this.changelogVersionLabel.Text = data.Version;

                  this.previousVersionButton.Text = "Previous Version";
                  this.nextVersionButton.Text = "Next Version";

                  this.whatsNewLabel.Text = "What's New?";
                  this.whatsNewTextBox.Text = string.Join("\n", data.NewStuff);

                  this.bugfixesLabel.Text = "Bugfixes:";
                  this.bugfixesTextBox.Text = string.Join("\n", data.Bugfixes);

                  this.knownBugsLabel.Text = "Known Bugs:";
                  this.knownBugsTextBox.Text = string.Join("\n", data.KnownBugs);
            }

            private void ResizeWindowAndElements() {
                  Size size = new Size(windowMinWidth, 0);

                  // Set the size of the text boxes
                  this.whatsNewTextBox.Size = new Size(size.Width - padding * 2, maxTextboxHeight);
                  this.bugfixesTextBox.Size = new Size(size.Width - padding * 2, maxTextboxHeight);
                  this.knownBugsTextBox.Size = new Size(size.Width - padding * 2, maxTextboxHeight);

                  // Position the text boxes and their labels
                  this.whatsNewLabel.Location = new Point(padding, this.previousVersionButton.Location.Y + this.previousVersionButton.Height + padding);
                  this.whatsNewTextBox.Location = new Point(padding, this.whatsNewLabel.Location.Y + this.whatsNewLabel.Height + padding / 2);

                  this.bugfixesLabel.Location = new Point(padding, this.whatsNewTextBox.Location.Y + this.whatsNewTextBox.Height + padding);
                  this.bugfixesTextBox.Location = new Point(padding, this.bugfixesLabel.Location.Y + this.bugfixesLabel.Height + padding / 2);

                  this.knownBugsLabel.Location = new Point(padding, this.bugfixesTextBox.Location.Y + this.bugfixesTextBox.Height + padding);
                  this.knownBugsTextBox.Location = new Point(padding, this.knownBugsLabel.Location.Y + this.knownBugsLabel.Height + padding / 2);

                  // Calculate the new window height
                  size.Height = this.knownBugsTextBox.Location.Y + this.knownBugsTextBox.Height + 3 * padding;

                  // Set the size of the window based on the calculated values
                  this.Size = new Size(size.Width + padding, size.Height + padding);

                  // Move the buttons to the correct location
                  this.previousVersionButton.Location = new Point(padding, padding);
                  this.nextVersionButton.Location = new Point(this.Width - this.nextVersionButton.Width - 2 * padding, padding);

                  CenterHeaderAndVersionText();
            }

            private void CenterHeaderAndVersionText() {
                  int windowWidth = this.Width;
                  int textWidth = this.changelogHeaderLabel.PreferredWidth + this.changelogVersionLabel.PreferredWidth;
                  int headerMargin = ((windowWidth - textWidth) / 2);

                  // Position the header label
                  this.changelogHeaderLabel.Location = new Point(
                        headerMargin,
                        this.changelogHeaderLabel.Location.Y);

                  // Position the version label next to the header
                  this.changelogVersionLabel.Location = new Point(
                        this.changelogHeaderLabel.Location.X + this.changelogHeaderLabel.Width - 12,
                        this.changelogVersionLabel.Location.Y);
            }

            private void previousVersionButton_Click(object sender, EventArgs e) {
                  this.currentChangelogIndex++;
                  if (this.currentChangelogIndex == changelogs.Length - 1) {
                        this.previousVersionButton.Enabled = false;
                  }

                  this.nextVersionButton.Enabled = true;

                  PopulateTextBoxes(changelogs[currentChangelogIndex]);
                  CorruptFormText(textCorruptionLevel);
            }

            private void nextVersionButton_Click(object sender, EventArgs e) {
                  this.currentChangelogIndex--;
                  if (this.currentChangelogIndex == 0) {
                        this.nextVersionButton.Enabled = false;
                  }

                  this.previousVersionButton.Enabled = true;

                  PopulateTextBoxes(changelogs[currentChangelogIndex]);
                  CorruptFormText(textCorruptionLevel);
            }

            const string changelogFileName = "Changelog.txt";

            const int padding = 12;

            const int maxTextboxHeight = 105;
            const int minTextboxHeight = 25;
            const int baseTextboxHeight = 5;
            const int lineHeight = 20;

            const int windowMinWidth = 750;
      }
}
