using Geometry_Dash_Randomiser.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
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

            public ChangelogForm() {
                  InitializeComponent();

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
                  this.HeaderLabel.Text = "Changelog";
                  this.VersionLabel.Text = data.Version;

                  this.ReleaseDateLabel.Text = data.ReleaseDate;

                  this.previousVersionButton.Text = "Previous Version";
                  this.nextVersionButton.Text = "Next Version";

                  StringBuilder sb = new StringBuilder();
                  AppendDataToChangeLog(data.NewStuff, sb);
                  AppendDataToChangeLog(data.Changes, sb);
                  AppendDataToChangeLog(data.Bugfixes, sb);
                  AppendDataToChangeLog(data.KnownBugs, sb);
                  AppendDataToChangeLog(data.Notes, sb);

                  this.ChangeLogTextBox.Text = sb.ToString();
            }

            private void AppendDataToChangeLog(ChangelogData.ChangeLogProperty data, StringBuilder to) {
                  if (data.Data.Length != 0) {
                        if (to.Length != 0) {
                              to.Append("\n\n");
                        }

                        to.Append($"{data.TypeName}\n");
                        to.Append($"{string.Join("\n", data.Data)}");
                  }
            }

            private void ResizeWindowAndElements() {
                  Size size = new Size(_windowWidth, 0);

                  this.ChangeLogTextBox.Size = GetTextBoxHeightFromContents(this.ChangeLogTextBox);

                  // Calculate the new window height
                  size.Height = this.ChangeLogTextBox.Location.Y + this.ChangeLogTextBox.Height + 3 * _padding;

                  // Set the size of the window based on the calculated values
                  this.Size = new Size(size.Width + _padding, size.Height + (int)(_padding * 0.5f));

                  // Move the buttons to the correct location
                  this.previousVersionButton.Location = new Point(_padding, _padding);
                  this.nextVersionButton.Location = new Point(this.Width - this.nextVersionButton.Width - 2 * _padding, _padding);

                  RelocateHeaderLabels();
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

            private void RelocateHeaderLabels() {
                  int windowWidth = this.Width;

                  int headerMargin = (windowWidth - HeaderLabel.Width) / 2;
                  // Position the header label
                  this.HeaderLabel.Location = new Point(
                        headerMargin,
                        this.HeaderLabel.Location.Y
                  );

                  // Position the changelog label
                  this.VersionLabel.Location = new Point(
                        this.HeaderLabel.Location.X - this.VersionLabel.Width / 2,
                        this.VersionLabel.Location.Y
                  );

                  // Position the release date label
                  this.ReleaseDateLabel.Location = new Point(
                        this.HeaderLabel.Right - this.ReleaseDateLabel.Width / 2,
                        this.ReleaseDateLabel.Location.Y
                  );
            }

            private void PreviousVersion(object sender, EventArgs e) {
                  this._currentChangelogIndex++;
                  if (this._currentChangelogIndex == _changelogs.Length - 1) {
                        this.previousVersionButton.Enabled = false;
                  }

                  this.nextVersionButton.Enabled = true;

                  ResetAllTextToDefault();
                  PopulateTextBoxes(CurrentChangelog);
                  CorruptFormText();

                  ResizeWindowAndElements();
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

                  ResizeWindowAndElements();
            }

            public override void On_FormClosing(object sender, FormClosingEventArgs e) {
                  base.On_FormClosing(sender, e);
            }

            public override void On_Activated(object sender, EventArgs e) {
                  SetTheme();

                  ResetAllTextToDefault();
                  PopulateTextBoxes(CurrentChangelog);
                  CorruptFormText();

                  ResizeWindowAndElements();
            }

            public override void On_Deactivate(object sender, EventArgs e) {
                  base.On_Deactivate(sender, e);
            }

            private const string changelogFileName = "Changelog.txt";
            
            private const int _padding = 15;
            
            private const int _maxTextboxHeight = 1000;
            private const int _baseTextboxHeight = 5;
            private const int _lineHeight = 20;
            
            private const int _windowWidth = 750;

            private const int _maxTextWidth = _windowWidth - 27;
      }
}
