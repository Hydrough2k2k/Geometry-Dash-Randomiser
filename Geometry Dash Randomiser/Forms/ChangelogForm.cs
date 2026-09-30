using Geometry_Dash_Randomiser.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      public partial class ChangelogForm : ThemedFormBase {

            private enum ChangelogSection {
                  WhatsNew,
                  Bugfixes,
                  KnownBugs,
                  Notes
            }

            // Local data
            readonly private ChangelogData[] _changelogs = Array.Empty<ChangelogData>();
            private int _currentChangelogIndex = 0;

            ChangelogData CurrentChangelog {
                  get {
                        if (_changelogs.Length != 0) {
                              return _changelogs[_currentChangelogIndex];
                        }
                        return ChangelogData.Default;
                  }
            }

            private Control[] arrangedElements;

            public ChangelogForm() {
                  InitializeComponent();

                  arrangedElements = new Control[] {
                        this.whatsNewLabel, this.whatsNewTextBox, this.bugfixesLabel, this.bugfixesTextBox,
                        this.knownBugsLabel, this.knownBugsTextBox, this.notesLabel, this.notesTextBox
                  };

                  StoreOriginalText();

                  ResizeWindowAndElements();

                  _changelogs = LoadChangelogsFromFile();

                  this.nextVersionButton.Enabled = false;

                  if (this._changelogs.Length <= 1) {
                        this.previousVersionButton.Enabled = false;
                  }

                  this.Text = "GDR Changelog";
                  this.originalTitle = this.Text;
            }

            private ChangelogData[] LoadChangelogsFromFile() {
                  if (File.Exists(changelogFileName) == false) {
                        return Array.Empty<ChangelogData>();
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

                  this.notesLabel.Text = "Notes:";
                  this.notesTextBox.Text = string.Join("\n", data.Notes);
            }

            private void ResizeWindowAndElements() {
                  Size size = new Size(windowMinWidth, 0);

                  Size textboxSize = new Size(size.Width - padding * 2, maxTextboxHeight);

                  // Set the size of the text boxes
                  this.whatsNewTextBox.Size = textboxSize;
                  this.bugfixesTextBox.Size = textboxSize;
                  this.knownBugsTextBox.Size = textboxSize;
                  this.notesTextBox.Size = textboxSize;

                  Control latestMovedControl = this.previousVersionButton;

                  // Position the text boxes and their labels
                  for (int i = 0; i < arrangedElements.Length; i++) {
                        ChangelogSection section = (ChangelogSection)(i / 2);
                        bool skip = false;

                        // Check if the current section has any content, if not, skip it
                        switch (section) {
                              case ChangelogSection.WhatsNew:
                                    if (CurrentChangelog.NewStuff.Length == 0)
                                          skip = true;
                                    break;

                              case ChangelogSection.Bugfixes:
                                    if (CurrentChangelog.Bugfixes.Length == 0)
                                          skip = true;
                                    break;

                              case ChangelogSection.KnownBugs:
                                    if (CurrentChangelog.KnownBugs.Length == 0)
                                          skip = true;
                                    break;

                              case ChangelogSection.Notes:
                                    if (CurrentChangelog.Notes.Length == 0)
                                          skip = true;
                                    break;
                        }

                        // Set the visibility based on whether we skip the section or not
                        arrangedElements[i].Visible = !skip;
                        if (skip) {
                              // Set the text box to be hidden as well, and incement the index to skip the next element (the text box) as well
                              arrangedElements[++i].Visible = false;
                              continue;
                        }

                        Point nextLocation;

                        if (i % 2 == 0) {
                              nextLocation = new Point(padding, latestMovedControl.Location.Y + latestMovedControl.Height + padding);

                        } else {
                              nextLocation = new Point(padding, latestMovedControl.Location.Y + latestMovedControl.Height + padding / 2);
                        }

                        arrangedElements[i].Location = nextLocation;

                        latestMovedControl = arrangedElements[i];
                  }

                  // Calculate the new window height
                  size.Height = latestMovedControl.Location.Y + latestMovedControl.Height + 3 * padding;

                  // Set the size of the window based on the calculated values
                  this.Size = new Size(size.Width + padding, size.Height + (int)(padding * 1.5f));

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
                  this._currentChangelogIndex++;
                  if (this._currentChangelogIndex == _changelogs.Length - 1) {
                        this.previousVersionButton.Enabled = false;
                  }

                  this.nextVersionButton.Enabled = true;

                  ResetAllTextToDefault();
                  PopulateTextBoxes(CurrentChangelog);
                  ResizeWindowAndElements();

                  CorruptFormText();
            }

            private void NextVersionButton_Click(object sender, EventArgs e) {
                  this._currentChangelogIndex--;
                  if (this._currentChangelogIndex == 0) {
                        this.nextVersionButton.Enabled = false;
                  }

                  this.previousVersionButton.Enabled = true;

                  ResetAllTextToDefault();
                  PopulateTextBoxes(CurrentChangelog);
                  CorruptFormText();

                  ResizeWindowAndElements();
            }

            public override void On_FormClosing(object sender, FormClosingEventArgs e) {
                  base.On_FormClosing(sender, e);
            }

            public override void On_Activated(object sender, EventArgs e) {
                  SetTheme();

                  ResetAllTextToDefault();
                  PopulateTextBoxes(CurrentChangelog);
                  ResizeWindowAndElements();

                  CorruptFormText();

                  CenterHeaderAndVersionText();
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
