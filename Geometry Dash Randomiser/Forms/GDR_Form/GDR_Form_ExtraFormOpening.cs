using Geometry_Dash_Randomiser.Forms;
using System;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      public partial class GDR_Form : Form {

            private void Logo_Click(object sender, EventArgs e) {
                  if (creditsForm == null) {
                        creditsForm = new CreditsForm();
                  }

                  creditsForm.theme = this.CurrentTheme;
                  creditsForm.textCorruptor = this.textCorruptor;
                  creditsForm.Show();

                  return;
            }

            private void ShowImportConfigForm(object sender, EventArgs e) {
                  if (importConfigForm == null) {
                        importConfigForm = new ImportConfigForm();

                        this.importConfigForm.ConfigDataChanged += (s, data, ev) => {
                              ImportConfigThenRefreshUI(data);
                        };
                  }

                  importConfigForm.textCorruptor = this.textCorruptor;
                  importConfigForm.theme = this.CurrentTheme;
                  importConfigForm.Show();
            }

            private void ShowExportConfigForm(object sender, EventArgs e) {
                  if (exportConfigForm == null) {
                        exportConfigForm = new ExportConfigForm();
                  }

                  exportConfigForm.textCorruptor = this.textCorruptor;
                  exportConfigForm.theme = this.CurrentTheme;
                  exportConfigForm.Show();
            }

            private void ChangelogButton_Click(object sender, EventArgs e) {
                  if (changelogForm == null) {
                        changelogForm = new ChangelogForm();
                  }

                  changelogForm.theme = this.CurrentTheme;
                  changelogForm.textCorruptor = this.textCorruptor;
                  changelogForm.Show();
            }

            private void ThemeConfigButton_Click(object sender, EventArgs e) {
                  if (themeConfigForm == null) {
                        themeConfigForm = new ThemeConfigForm();
                  }

                  themeConfigForm.theme = this.CurrentTheme;
                  themeConfigForm.textCorruptor = this.textCorruptor;
                  themeConfigForm.Show();
            }
      }
}
