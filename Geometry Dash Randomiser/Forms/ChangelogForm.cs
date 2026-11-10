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
                  Changes,
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
                        this.whatsNewLabel, this.whatsNewTextBox,
                        this.changesLabel, this.changesTextBox,
                        this.bugfixesLabel, this.bugfixesTextBox,
                        this.knownBugsLabel, this.knownBugsTextBox,
                        this.notesLabel, this.notesTextBox
                  };

                  StoreOriginalText();

                  _changelogs = LoadChangelogsFromFile();

                  ResizeWindowAndElements();

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
                  this.changelogHeaderLabel.Text = "Changelog";
                  this.changelogVersionLabel.Text = data.Version;

                  this.ReleaseDateLabel.Text = data.ReleaseDate;

                  this.previousVersionButton.Text = "Previous Version";
                  this.nextVersionButton.Text = "Next Version";

                  LengthMeasurerLabel_Hidden.Text = data.Bugfixes[0];

                  this.whatsNewLabel.Text = "What's New?";
                  this.whatsNewTextBox.Text = string.Join("\n", data.NewStuff);

                  this.changesLabel.Text = "Changes:";
                  this.changesTextBox.Text = string.Join("\n", data.Changes);

                  this.bugfixesLabel.Text = "Bugfixes:";
                  this.bugfixesTextBox.Text = string.Join("\n", data.Bugfixes);

                  this.knownBugsLabel.Text = "Known Bugs:";
                  this.knownBugsTextBox.Text = string.Join("\n", data.KnownBugs);

                  this.notesLabel.Text = "Notes:";
                  this.notesTextBox.Text = string.Join("\n", data.Notes);
            }

            private void ResizeWindowAndElements() {
                  Size size = new Size(_windowWidth, 0);

                  Size textboxSize = DefaultTextBoxSize();

                  this.whatsNewTextBox.Size = GetTextBoxHeightFromContents(this.whatsNewTextBox);
                  this.changesTextBox.Size = GetTextBoxHeightFromContents(this.changesTextBox);
                  this.bugfixesTextBox.Size = GetTextBoxHeightFromContents(this.bugfixesTextBox);
                  this.knownBugsTextBox.Size = GetTextBoxHeightFromContents(this.knownBugsTextBox);
                  this.notesTextBox.Size = GetTextBoxHeightFromContents(this.notesTextBox);

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

                              case ChangelogSection.Changes:
                                    if (CurrentChangelog.Changes.Length == 0)
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
                              // Label relocation logic
                              nextLocation = new Point(_padding, latestMovedControl.Location.Y + latestMovedControl.Height + _padding);

                        } else {
                              // Textbox relocation logic
                              nextLocation = new Point(_padding, latestMovedControl.Location.Y + latestMovedControl.Height + _padding / 2);
                        }

                        arrangedElements[i].Location = nextLocation;

                        latestMovedControl = arrangedElements[i];
                  }

                  // Calculate the new window height
                  size.Height = latestMovedControl.Location.Y + latestMovedControl.Height + 3 * _padding;

                  // Set the size of the window based on the calculated values
                  this.Size = new Size(size.Width + _padding, size.Height + _padding);

                  // Move the buttons to the correct location
                  this.previousVersionButton.Location = new Point(_padding, _padding);
                  this.nextVersionButton.Location = new Point(this.Width - this.nextVersionButton.Width - 2 * _padding, _padding);

                  CenterHeaderAndVersionText();
            }

            private Size GetTextBoxHeightFromContents(RichTextBox tb) {
                  if (tb == null || tb.Text == string.Empty)
                        return DefaultTextBoxSize();

                  int preferredHeight = _baseTextboxHeight;
                  string[] lines = tb.Text.Split('\n');

                  for (int i = 0; i < lines.Length; i++) {
                        preferredHeight += GetTotalLineRequirements(lines[i]) * _lineHeight;
                  }

                  return new Size(tb.Width, System.Math.Min(preferredHeight, _maxTextboxHeight));
            }

            private int GetTotalLineRequirements(string text) {
                  int textWidth = GetPreferredTextWidth(text);
                  if (textWidth <= _maxTextWidth) {
                        return 1;

                  } else {
                        // This is an approximation. Might be wrong sometimes, especially if the text is too long, but it's simple
                        return (int)System.Math.Ceiling((double)textWidth / _maxTextWidth);
                  }
            }

            private int GetPreferredTextWidth(string text) {
                  LengthMeasurerLabel_Hidden.Text = text;
                  return LengthMeasurerLabel_Hidden.PreferredWidth;
            }

            private Size DefaultTextBoxSize() {
                  return new Size(_windowWidth - _padding * 2, _maxTextboxHeight);
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

            private void PreviousVersion(object sender, EventArgs e) {
                  this._currentChangelogIndex++;
                  if (this._currentChangelogIndex == _changelogs.Length - 1) {
                        this.previousVersionButton.Enabled = false;
                  }

                  this.nextVersionButton.Enabled = true;

                  ResetAllTextToDefault();
                  PopulateTextBoxes(CurrentChangelog);
                  ResizeWindowAndElements();

                  CorruptFormText();
                  LeftAlignControls();
            }

            private void NextVersion(object sender, EventArgs e) {
                  this._currentChangelogIndex--;
                  if (this._currentChangelogIndex == 0) {
                        this.nextVersionButton.Enabled = false;
                  }

                  this.previousVersionButton.Enabled = true;

                  ResetAllTextToDefault();
                  PopulateTextBoxes(CurrentChangelog);
                  CorruptFormText();
                  LeftAlignControls();

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
                  LeftAlignControls();

                  CenterHeaderAndVersionText();
            }

            public override void On_Deactivate(object sender, EventArgs e) {
                  base.On_Deactivate(sender, e);
            }

            private void LeftAlignControls() {
                  nextVersionButton.LeftAlign(ReleaseDateLabel);
            }

            private const string changelogFileName = "Changelog.txt";
            
            private const int _padding = 15;
            
            private const int _maxTextboxHeight = 105;
            private const int _baseTextboxHeight = 5;
            private const int _lineHeight = 20;
            
            private const int _windowWidth = 750;

            private const int _maxTextWidth = _windowWidth - 27;
      }
}
