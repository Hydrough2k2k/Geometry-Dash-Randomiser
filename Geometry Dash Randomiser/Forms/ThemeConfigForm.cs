using System;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser.Forms {

      public partial class ThemeConfigForm : ThemedFormBase {

            public ThemeConfigForm() {
                  InitializeComponent();

                  StoreOriginalText();

                  this.Text = "Theme Settings";
                  this.originalTitle = this.Text;

                  this.RandomThemeCheckbox.Checked = AdvancedConfig.Instance.EnableRandomTheme;
                  this.SystemThemeCheckbox.Checked = AdvancedConfig.Instance.EnableSystemTheme;
            }

            public override void On_FormClosing(object sender, FormClosingEventArgs e) {
                  base.On_FormClosing(sender, e);
            }

            public override void On_Activated(object sender, EventArgs e) {
                  this.SetTheme();
                  ResetAndCorruptFormText();

                  CenterControlHorizontally(this.headerLabel);
            }

            public override void On_Deactivate(object sender, EventArgs e) {
                  base.On_Deactivate(sender, e);
            }

            private void RandomThemeCheckbox_Click(object sender, EventArgs e) {
                  AdvancedConfig.Instance.EnableRandomTheme = (sender as CheckBox).Checked;
            }

            private void SystemThemeCheckbox_Click(object sender, EventArgs e) {
                  AdvancedConfig.Instance.EnableSystemTheme = (sender as CheckBox).Checked;
            }
      }
}
