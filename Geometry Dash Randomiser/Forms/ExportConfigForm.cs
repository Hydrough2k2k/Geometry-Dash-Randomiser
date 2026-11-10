using Geometry_Dash_Randomiser.Forms;
using System;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      public partial class ExportConfigForm : ThemedFormBase {

            public ExportConfigForm() {
                  InitializeComponent();

                  StoreOriginalText();

                  this.Text = "Export Config Data";
                  this.originalTitle = this.Text;
            }

            public override void On_FormClosing(object sender, FormClosingEventArgs e) {
                  e.Cancel = true;
                  this.On_Deactivate(sender, e as EventArgs);
            }

            public override void On_Activated(object sender, EventArgs e) {
                  this.SetTheme();
                  this.ResetAndCorruptFormText();

                  string exportString = RandomizationConfig.Instance.GetExportConfigData();
                  this.exportTextBox.Text = exportString;
            }

            private void CopyToClipboardButton_Click(object sender, EventArgs e) {
                  Clipboard.SetDataObject(exportTextBox.Text);
                  this.On_Deactivate(sender, e);
            }

            public override void On_Deactivate(object sender, EventArgs e) {
                  this.Hide();
            }
      }
}
