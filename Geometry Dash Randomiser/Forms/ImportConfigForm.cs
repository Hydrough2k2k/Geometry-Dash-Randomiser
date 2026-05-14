using System;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      public partial class ImportConfigForm : Form {

            public Theme theme;
            public TextCorruptor textCorruptor;

            public delegate void ConfigDataReceivedEventHandler(object sender, string configData, EventArgs e);
            public event ConfigDataReceivedEventHandler ConfigDataChanged;

            public ImportConfigForm() {
                  InitializeComponent();
            }

            private void SetTheme() {
                  this.BackColor = theme.BackgroundColour.AdjustBrightness(0.60f);
                  this.importTextBox.BackColor = theme.ObjectBackColour.AdjustBrightness(0.50f);
                  this.importTextBox.ForeColor = theme.ObjectTextColour.AdjustBrightness(0.80f);
                  this.importLabel.ForeColor = theme.TextColour;

                  this.acceptButton.ForeColor = theme.ObjectTextColour;
                  this.acceptButton.BackColor = theme.ObjectBackColour.AdjustBrightness(0.70f);
                  this.cancelButton.ForeColor = theme.ObjectTextColour;
                  this.cancelButton.BackColor = theme.ObjectBackColour.AdjustBrightness(0.70f);

                  this.pasteFromClipboardButton.ForeColor = theme.ObjectTextColour;
                  this.pasteFromClipboardButton.BackColor = theme.ObjectBackColour.AdjustBrightness(0.70f);
            }

            private void _FormClosing(object sender, FormClosingEventArgs e) {
                  e.Cancel = true;
                  this._Deactivate(sender, e as EventArgs);
            }

            private void _Activated(object sender, EventArgs e) {
                  SetTheme();

                  this.Text = "Import Config Data";
                  this.importTextBox.Text = string.Empty;
            }

            private void acceptButton_Click(object sender, EventArgs e) {
                  ConfigDataChanged?.Invoke(this, this.importTextBox.Text, EventArgs.Empty);

                  _Deactivate(sender, e);
            }

            private void cancelButton_Click(object sender, EventArgs e) {
                  _Deactivate(sender, e);
            }

            private void pasteFromClipboardButton_Click(object sender, EventArgs e) {

                  IDataObject iData = Clipboard.GetDataObject();
                  if (iData.GetDataPresent(DataFormats.Text)) {
                        importTextBox.Text = (String)iData.GetData(DataFormats.Text);
                  }
            }

            private void _Deactivate(object sender, EventArgs e) {
                  this.Hide();
            }
      }
}
