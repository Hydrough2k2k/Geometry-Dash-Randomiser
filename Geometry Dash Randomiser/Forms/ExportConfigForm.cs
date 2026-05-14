using System;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      public partial class ExportConfigForm : Form {

            public Theme theme;
            public TextCorruptor textCorruptor;

            public ExportConfigForm() {
                  InitializeComponent();
            }

            private void SetTheme() {
                  this.BackColor = theme.BackgroundColour.AdjustBrightness(0.60f);
                  this.exportTextBox.BackColor = theme.ObjectBackColour.AdjustBrightness(0.50f);
                  this.exportTextBox.ForeColor = theme.ObjectTextColour.AdjustBrightness(0.80f);
                  this.exportLabel.ForeColor = theme.TextColour;

                  this.copyToClipboardButton.ForeColor = theme.ObjectTextColour;
                  this.copyToClipboardButton.BackColor = theme.ObjectBackColour.AdjustBrightness(0.70f);
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
