using System;
using System.Diagnostics.Tracing;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      public partial class ExportConfigForm : Form {

            public Theme theme;

            public ExportConfigForm() {
                  InitializeComponent();
            }

            private void SetTheme() {
                  this.BackColor = theme.backgroundColour.AdjustBrightness(0.60f);
                  this.exportTextBox.BackColor = theme.objectBackColour.AdjustBrightness(0.50f);
                  this.exportTextBox.ForeColor = theme.objectTextColour.AdjustBrightness(0.80f);
                  this.exportLabel.ForeColor = theme.textColour;

                  this.copyToClipboardButton.ForeColor = theme.objectTextColour;
                  this.copyToClipboardButton.BackColor = theme.objectBackColour.AdjustBrightness(0.70f);
            }

            private void _FormClosing(object sender, FormClosingEventArgs e) {
                  e.Cancel = true;
                  this._Deactivate(sender, e as EventArgs);
            }

            private void _Activated(object sender, EventArgs e) {
                  SetTheme();

                  this.Text = "Export Config Data";

                  string exportString = Config.Instance.GetExportConfigData();
                  this.exportTextBox.Text = exportString;
            }

            private void copyToClipboardButton_Click(object sender, EventArgs e) {
                  Clipboard.SetDataObject(exportTextBox.Text);
                  this._Deactivate(sender, e);
            }

            private void _Deactivate(object sender, EventArgs e) {
                  this.Hide();
            }

      }
}
