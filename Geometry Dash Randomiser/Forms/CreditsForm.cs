using Geometry_Dash_Randomiser.Forms;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      public partial class CreditsForm : ThemedFormBase {

            public CreditsForm() {
                  InitializeComponent();

                  StoreOriginalText();

                  this.Text = "Credits";
                  this.originalTitle = this.Text;
            }

            public new void SetTheme() {
                  base.SetTheme();

                  CenterControlsHorizontally(GetAllLabels());
            }

            private Label[] GetAllLabels() => GetAll(this, typeof(Label)).Select(c => c as Label).ToArray();

            public override void On_FormClosing(object sender, FormClosingEventArgs e) {
                  base.On_FormClosing(sender, e);
            }

            public override void On_Activated(object sender, EventArgs e) {
                  this.SetTheme();
                  ResetAndCorruptFormText();
            }

            public override void On_Deactivate(object sender, EventArgs e) {
                  base.On_Deactivate(sender, e);
            }
      }
}
