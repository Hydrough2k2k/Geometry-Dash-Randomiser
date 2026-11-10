using Geometry_Dash_Randomiser.Forms;
using System;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      public partial class ImportConfigForm : ThemedFormBase {

            public delegate void ConfigDataReceivedEventHandler(object sender, ConfigUpdateEventArgs e);
            public event ConfigDataReceivedEventHandler ConfigDataChanged;

            public ImportConfigForm() {
                  InitializeComponent();

                  StoreOriginalText();

                  this.Text = "Import Config Data";
                  this.originalTitle = this.Text;
            }

            public override void On_FormClosing(object sender, FormClosingEventArgs e) {
                  e.Cancel = true;
                  this.On_Deactivate(sender, e as EventArgs);
            }

            public override void On_Activated(object sender, EventArgs e) {
                  SetTheme();
                  this.ResetAndCorruptFormText();

                  this.Text = "Import Config Data";
                  this.importTextBox.Text = string.Empty;
            }

            public override void On_Deactivate(object sender, EventArgs e) {
                  this.Hide();
            }

            private void AcceptButton_Click(object sender, EventArgs e) {
                  ConfigDataChanged?.Invoke(this, new ConfigUpdateEventArgs(this.importTextBox.Text));

                  On_Deactivate(sender, e);
            }

            private void CancelButton_Click(object sender, EventArgs e) {
                  On_Deactivate(sender, e);
            }

            private void PasteFromClipboardButton_Click(object sender, EventArgs e) {

                  IDataObject iData = Clipboard.GetDataObject();
                  if (iData.GetDataPresent(DataFormats.Text)) {
                        importTextBox.Text = (String)iData.GetData(DataFormats.Text);
                  }
            }
      }
}
