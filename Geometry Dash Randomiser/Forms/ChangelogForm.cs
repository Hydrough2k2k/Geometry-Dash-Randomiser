using Geometry_Dash_Randomiser.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      public partial class ChangelogForm : ThemedFormBase {

            // Local data
            readonly private ChangelogData[] changelogs = Array.Empty<ChangelogData>();
            int currentChangelogIndex = 0;

            ChangelogData CurrentChangelog => changelogs[currentChangelogIndex];

            public ChangelogForm() {
                  InitializeComponent();

                  StoreOriginalText();

                  ResizeWindowAndElements();

                  changelogs = LoadChangelogsFromFile();

                  this.nextVersionButton.Enabled = false;

                  if (this.changelogs.Length <= 1) {
                        this.previousVersionButton.Enabled = false;
                  }

                  this.Text = "GDR Changelog";
                  this.originalTitle = this.Text;
            }

            private ChangelogData[] LoadChangelogsFromFile() {
                  if (File.Exists(changelogFileName) == false) {
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

                  Size textboxSize = new Size(size.Width - padding * 2, maxTextboxHeight);

                  // Set the size of the text boxes
                  this.whatsNewTextBox.Size = textboxSize;
                  this.bugfixesTextBox.Size = textboxSize;
                  this.knownBugsTextBox.Size = textboxSize;

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

            private void PreviousVersionButton_Click(object sender, EventArgs e) {
                  this.currentChangelogIndex++;
                  if (this.currentChangelogIndex == changelogs.Length - 1) {
                        this.previousVersionButton.Enabled = false;
                  }

                  this.nextVersionButton.Enabled = true;

                  ResetAllTextToDefault();
                  PopulateTextBoxes(changelogs[currentChangelogIndex]);
                  CorruptFormText();
            }

            private void NextVersionButton_Click(object sender, EventArgs e) {
                  this.currentChangelogIndex--;
                  if (this.currentChangelogIndex == 0) {
                        this.nextVersionButton.Enabled = false;
                  }

                  this.previousVersionButton.Enabled = true;

                  ResetAllTextToDefault();
                  PopulateTextBoxes(changelogs[currentChangelogIndex]);
                  CorruptFormText();
            }

            public override void On_FormClosing(object sender, FormClosingEventArgs e) {
                  base.On_FormClosing(sender, e);
            }

            public override void On_Activated(object sender, EventArgs e) {
                  SetTheme();

                  ResetAllTextToDefault();
                  PopulateTextBoxes(CurrentChangelog);
                  CorruptFormText();
            }

            public override void On_Deactivate(object sender, EventArgs e) {
                  base.On_Deactivate(sender, e);
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
