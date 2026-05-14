using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      public partial class CreditsForm : Form {

            private string[] originalText = Array.Empty<string>();

            // Data received from the main Form
            public Theme theme;
            public TextCorruptor textCorruptor;

            public CreditsForm() {
                  InitializeComponent();

                  originalText = GetAllLabels().Select(l => l.Text).ToArray();
            }

            private void SetTheme() {
                  this.BackColor = theme.BackgroundColour.AdjustBrightness(0.60f);

                  CenterAndRecolourAllText();
            }

            private void CenterAndRecolourAllText() {
                  Label[] labels = GetAllLabels();

                  for (int i = 0; i < labels.Length; i++) {
                        labels[i].Left = (this.Size.Width - labels[i].Width) / 2;
                        labels[i].ForeColor = theme.TextColour;
                  }
            }

            private void CorruptAllText() {
                  if (textCorruptor.CorruptionLevel == 0) {
                        return;
                  }

                  Label[] labels = GetAllLabels();

                  for (int i = 0; i < labels.Length; i++) {
                        labels[i].Text = textCorruptor.CorruptText(originalText[i]);
                  }
            }

            private void On_FormClosing(object sender, FormClosingEventArgs e) {
                  e.Cancel = true;
                  this.On_Deactivate(sender, e as EventArgs);
            }

            private void On_Activated(object sender, EventArgs e) {
                  CorruptAllText();
                  SetTheme();

                  this.Text = "Credits";
            }

            private void On_Deactivate(object sender, EventArgs e) {
                  this.Hide();
            }

            private IEnumerable<Control> GetAll(Control control, Type type) {
                  var controls = control.Controls.Cast<Control>();

                  return controls
                        .SelectMany(ctrl => GetAll(ctrl, type))
                        .Concat(controls)
                        .Where(c => c.GetType() == type);
            }

            private Label[] GetAllLabels() => GetAll(this, typeof(Label)).Select(c => c as Label).ToArray();
      }
}
