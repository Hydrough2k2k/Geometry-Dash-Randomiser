using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser.Forms {

      public partial class ThemedFormBase : Form {

            protected string originalTitle = string.Empty;
            private string[] originalText = Array.Empty<string>();

            public Theme theme;
            public TextCorruptor textCorruptor;

            public virtual void SetTheme() {
                  this.BackColor = theme.BackgroundColour.AdjustBrightness(0.60f);

                  Control[] controls = GetAllControls(this);

                  foreach (Control control in controls) {
                        if (control is TextBox) {
                              control.BackColor = theme.ObjectBackColour.AdjustBrightness(0.50f);
                              control.ForeColor = theme.ObjectTextColour.AdjustBrightness(0.80f);

                        } else if (control is Label) {
                              control.ForeColor = theme.TextColour;

                        } else if (control is Button) {
                              control.ForeColor = theme.ObjectTextColour;
                              control.BackColor = theme.ObjectBackColour.AdjustBrightness(0.70f);

                        } else if (control is RichTextBox) {
                              control.BackColor = theme.ObjectBackColour.AdjustBrightness(0.50f);
                              control.ForeColor = theme.ObjectTextColour.AdjustBrightness(0.80f);

                        } else if (control is GroupBox) {
                              control.ForeColor = theme.TextColour;

                        } else if (control is CheckBox) {
                              control.ForeColor = theme.TextColour;

                        } else if (control is RadioButton) {
                              control.ForeColor = theme.TextColour;

                        } else if (control is NumericUpDown) {
                              control.BackColor = theme.ObjectBackColour.AdjustBrightness(0.50f);
                              control.ForeColor = theme.ObjectTextColour.AdjustBrightness(0.80f);

                        } else if (control is DomainUpDown) {
                              control.BackColor = theme.ObjectBackColour.AdjustBrightness(0.50f);
                              control.ForeColor = theme.ObjectTextColour.AdjustBrightness(0.80f);
                        }
                  }
            }

            public void ResetAndCorruptFormText() {
                  ResetAllTextToDefault();
                  CorruptFormText();
            }

            public virtual void ResetAllTextToDefault() {
                  Control[] controls = GetAllControls(this);
                  for (int i = 0; i < controls.Length; i++) {
                        Control control = controls[i];
                        if (control is NumericUpDown) {
                              continue;
                        }

                        control.Text = originalText[i];
                  }
            }

            public virtual void CorruptFormText() {
                  if (textCorruptor.CorruptionLevel == 0) {
                        return;
                  }

                  Control[] controls = GetAllControls(this);

                  for (int i = 0; i < controls.Length; i++) {
                        Control control = controls[i];

                        // Filter all control types you don't want to modify the text contents of via the TextCorruptor
                        if (control is NumericUpDown) {
                              continue;
                        }

                        control.Text = textCorruptor.CorruptText(control.Text);
                  }

                  this.Text = textCorruptor.CorruptText(this.originalTitle);
            }

            public void StoreOriginalText() {
                  Control[] controls = GetAllControls(this);
                  List<string> ogText = new List<string>();

                  for (int i = 0; i < controls.Length; i++) {
                        Control control = controls[i];

                        if (control is NumericUpDown) {
                              continue;
                        }

                        ogText.Add(controls[i].Text);
                  }

                  originalText = ogText.ToArray();
            }

            public void CenterControlsHorizontally(Control[] controls) {
                  foreach (Control control in controls) {
                        control.Left = (this.ClientSize.Width - control.Width) / 2;
                  }
            }

            public void CenterControlHorizontally(Control control) {
                  control.Left = (this.ClientSize.Width - control.Width) / 2;
            }

            public virtual Control[] GetAllControls(Control control) {
                  return control.Controls.Cast<Control>().ToArray();
            }

            public IEnumerable<Control> GetAll(Control control, Type type) {
                  var controls = control.Controls.Cast<Control>();

                  return controls
                        .SelectMany(ctrl => GetAll(ctrl, type))
                        .Concat(controls)
                        .Where(c => c.GetType() == type);
            }

            public virtual void On_FormClosing(object sender, FormClosingEventArgs e) {
                  e.Cancel = true;
                  this.On_Deactivate(sender, e as EventArgs);
            }

            public virtual void On_Activated(object sender, EventArgs e) {
                  SetTheme();
                  CorruptFormText();
            }

            public virtual void On_Deactivate(object sender, EventArgs e) {
                  this.Hide();
            }
      }
}
