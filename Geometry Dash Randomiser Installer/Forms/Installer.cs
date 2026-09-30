using Geometry_Dash_Randomiser_Installer.Enums;
using Geometry_Dash_Randomiser_Installer.Properties;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Geometry_Dash_Randomiser_Installer {

      public partial class Installer : Form {

            private const int _installationPageID = 2;
            private const int _quitPageID = 4;

            private int _pageID = 0;
            private Control[] _allControls;

            private string _installationPath = string.Empty;
            private bool _createStartMenuShortcut = true;
            private bool _createDesktopShortcut = true;

            private Timer UI_UpdateTimer = new Timer { Interval = 25 };
            private Thread _installThread = null;

            private bool _isFinished = false;

            private int PageID {
                  get { return _pageID; }
                  set {
                        if (value < 0) {
                              _pageID = 0;

                        } else if (value > _quitPageID) {
                              _pageID = _quitPageID;

                        } else {
                              _pageID = value;
                        }

                        Update_UI();
                  }
            }

            private Control[] AllControls {
                  get {
                        if (_allControls == null) {
                              _allControls = GetAllControls(this).ToArray();
                        }
                        return _allControls;
                  }
            }

            public Installer() {
                  InitializeComponent();
            }

            private void NextPage(object sender, EventArgs e) {
                  PageID++;
                  if (PageID == _installationPageID) {

                        // Get the state for the install when we get on the installation page
                        ReadyState state = InstallerCore.GetReadyState(_installationPath);

                        // If any of the blocking states are present, we prevent the installation from starting and go back to the previous page
                        if (state.IsBlocking()) {
                              Console.WriteLine(state.GetBlockingFlag().ToString());
                              PageID--;
                              return;
                        }

                        // Now we check for non-blocking states that need the user's input
                        if (state.HasFlag(ReadyState.FolderNotEmpty)) {
                              DialogResult dialog = MessageBox.Show("The provided folder is not empty.\nDo you still want to continue?", "Folder is not empty", MessageBoxButtons.YesNo);
                              if (dialog == DialogResult.No) {
                                    PageID--;
                                    return;
                              }
                        }

                        // Otherwise we are good to start the installation
                        StartInstalling();

                  } else if (PageID == _quitPageID) {
                        Application.Exit();
                  }
            }

            private void PreviousPage(object sender, EventArgs e) {
                  PageID--;
            }

            private void Update_UI() {
                  TogglePageSpecificControls();

                  this.BackButton.Visible = (PageID > 0 && PageID < _installationPageID);
                  this.NextButton.Visible = (PageID != _installationPageID);
                  this.WarningIcon.Visible = (PageID == _installationPageID);

                  if (PageID < _installationPageID - 1) {
                        this.NextButton.Text = "Next >";
                        this.NextButton.Enabled = true;

                        this.Header_4.Text = "Click \'Next\' to continue, or \'Cancel\' to exit.";

                  } else if (PageID == _installationPageID - 1) {
                        this.NextButton.Text = "Install >";

                        ReadyState rs = InstallerCore.GetReadyState(_installationPath);
                        bool isBlocking = rs.IsBlocking();
                        this.NextButton.Enabled = !isBlocking;
                        this.WarningIcon.Visible = isBlocking;
                        if (isBlocking) {
                              string toolTipText = rs.GetStateMessageString();

                              this.toolTip1.SetToolTip(this.WarningIcon, toolTipText);
                              Console.WriteLine($"Set tooltip for warning icon: {toolTipText}");
                              this.WarningIcon.Image = GetImageForWarningIcon(rs);
                        }

                        this.Header_4.Text = "Click \'Install\' to start installing, or \'Cancel\' to exit.";

                  } else if (PageID == _installationPageID) {
                        this.NextButton.Text = "Next >";

                        this.Header_4.Text = "Installing GD Randomiser...";

                  } else if (_pageID > _installationPageID) {
                        this.CustomCancelButton.Text = "Finish";

                        this.Header_4.Text = "Installation complete! Click \'Finish\' to exit.";
                  }
            }

            private Bitmap GetImageForWarningIcon(ReadyState rs) {
                  if (rs.IsBlocking()) {
                        return Resources.Error_half_size;

                  } else if (!rs.IsWarning()) {
                        return Resources.Warning_half_size;
                  }

                  return null;
            }

            private void TogglePageSpecificControls() {
                  Control[] allControls = AllControls;

                  for (int i = 0; i < allControls.Length; i++) {
                        int index = allControls[i].Name.IndexOf('_');
                        if (index != -1) {
                              string trimmedName = allControls[i].Name.Substring(0, index);

                              if (trimmedName.Length > 3 || trimmedName.StartsWith("P") == false)
                                    continue;

                              // Remove the 'P' prefix
                              trimmedName = trimmedName.Substring(1);

                              if (int.TryParse(trimmedName, out int pageNumber)) {
                                    allControls[i].Visible = (pageNumber == _pageID);
                              }
                        }
                  }
            }

            private void StartInstalling() {
                  bool success = false;
                  InstallerCore.InstallFailReason reason = InstallerCore.InstallFailReason.None;

                  _installThread = new Thread(() => {
                        try {
                              success = InstallerCore.Install(_installationPath, _createStartMenuShortcut, _createDesktopShortcut, out reason);

                        } catch (Exception ex) {
                              MessageBox.Show($"An error occurred during installation:\n\n{ex.Message}", "Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                              _isFinished = true;
                              Application.Exit();
                        }
                  });

                  _installThread.Start();
                  UI_UpdateTimer.Start();

                  UI_UpdateTimer.Tick += (s, e) => {
                        Console.WriteLine("tick");
                  };

                  Thread.Sleep(1000);

                  _installThread.Join();
                  UI_UpdateTimer.Stop();

                  if (success) {
                        PageID++;
                        _isFinished = true;

                  } else {
                        PageID--;
                        MessageBox.Show($"Installation was unsuccessful. Reason: {reason}");
                  }
            }

            private IEnumerable<Control> GetAllControls(Control control) {
                  var controls = control.Controls.Cast<Control>();

                  return controls
                        .SelectMany(ctrl => GetAllControls(ctrl))
                        .Concat(controls);
            }

            private void CancelInstallation(object sender, EventArgs e) {
                  Application.Exit();
            }

            private void Installer_Shown(object sender, EventArgs e) {
                  Update_UI();
            }

            #region Page 1 Controls

            private void InstallationPathTextBox_TextChanged(object sender, EventArgs e) {
                  TextBox textBox = sender as TextBox;
                  if (textBox != null) {
                        _installationPath = textBox.Text;
                  }

                  Update_UI();
            }

            private void BrowsePathButton_Click(object sender, EventArgs e) {
                  string initialDir = string.IsNullOrEmpty(_installationPath) ? "C\\" : _installationPath;

                  if (GetFolderViaExplorer(initialDir, true, out string folder)) {
                        _installationPath = Path.Combine(folder, "Geometry Dash Randomiser");
                        P1_InstallationPathTextBox.Text = _installationPath;
                  }

                  Update_UI();
            }

            private void StartMenuShortcutCheckBox_Click(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  if (checkBox != null) {
                        _createStartMenuShortcut = checkBox.Checked;
                  }
            }

            private void DesktopShortcutCheckBox_Click(object sender, EventArgs e) {
                  CheckBox checkBox = sender as CheckBox;
                  if (checkBox != null) {
                        _createDesktopShortcut = checkBox.Checked;
                  }
            }

            #endregion

            private bool GetFolderViaExplorer(string InitialDirectory, bool IsFolderPicker, out string folder) {
                  CommonOpenFileDialog dialog = new CommonOpenFileDialog {
                        InitialDirectory = InitialDirectory,
                        IsFolderPicker = IsFolderPicker
                  };

                  if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                        folder = dialog.FileName;
                        return true;
                  }

                  folder = null;
                  return false;
            }

            private void Installer_FormClosing(object sender, FormClosingEventArgs e) {
                  if (_isFinished)
                        return;

                  DialogResult dialog = MessageBox.Show("Are you sure you want to cancel installation of GDR?", "Exit", MessageBoxButtons.YesNo);
                  if (dialog == DialogResult.No) {
                        e.Cancel = true;
                  }
            }
      }
}
