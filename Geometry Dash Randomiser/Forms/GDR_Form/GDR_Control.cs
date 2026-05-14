using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      public partial class GDR_Form : Form {

            /// <summary>
            /// This populates all Lists with Controls and other similar items to help simplify code.<br/>
            /// This should only be called once
            /// </summary>
            internal void InitialiseControlContainers() {

                  // Check if this method has already run. If so, short circuit.
                  if (dataArraysArePopulated == true) {
                        return;
                  }

                  // Get all controls that are related to icon randomisation
                  this.iconCheckBoxes = GetAll(this.IconTextureBox, typeof(CheckBox)).Select(c => c as CheckBox).ToArray();
                  this.iconGroupDisplays = GetAll(this.IconTextureBox, typeof(NumericUpDown)).Select(c => c as NumericUpDown).ToArray();

                  // Get all controls we are interested in changing later for themes all at once to speed up later references
                  labels = GetAll(this, typeof(Label)).Select(c => c as Label).ToArray();
                  checkBoxes = GetAll(this, typeof(CheckBox)).Select(c => c as CheckBox).ToArray();
                  buttons = GetAll(this, typeof(Button)).Select(c => c as Button).ToArray();
                  numericUpDowns = GetAll(this, typeof(NumericUpDown)).Select(c => c as NumericUpDown).ToArray();
                  textBoxes = GetAll(this, typeof(TextBox)).Select(c => c as TextBox).ToArray();
                  domainUpDowns = GetAll(this, typeof(DomainUpDown)).Select(c => c as DomainUpDown).ToArray();
                  richTextBoxes = GetAll(this, typeof(RichTextBox)).Select(c => c as RichTextBox).ToArray();
                  groupBoxes = GetAll(this, typeof(GroupBox)).Select(c => c as GroupBox).ToArray();
                  pictureBoxes = GetAll(this, typeof(PictureBox)).Select(c => c as PictureBox).ToArray();
                  radioButtons = GetAll(this, typeof(RadioButton)).Select(c => c as RadioButton).ToArray();

                  // Finally, signal that the arrays are populated
                  dataArraysArePopulated = true;
            }

            private IEnumerable<Control> GetAll(Control control, Type type) {
                  var controls = control.Controls.Cast<Control>();

                  return controls
                        .SelectMany(ctrl => GetAll(ctrl, type))
                        .Concat(controls)
                        .Where(c => c.GetType() == type);
            }

            // This is needed for this to script be able to print to the console
            private void Control_AllocConsole() {
                  AllocConsole();
            }

            #region Animations And Related Methods

            private void AnimateImageRotation(PictureBox pb, Bitmap image, float rotationSpeed, Action onAnimationComplete = null, int updateInterval = 25) {
                  float elapsed = 0f;

                  Timer timer = new Timer { Interval = updateInterval };

                  timer.Tick += (s, e) => {
                        elapsed += updateInterval;
                        float angle = elapsed / 1000 * rotationSpeed;

                        if (angle >= 360) {
                              timer.Stop();
                              onAnimationComplete?.Invoke();
                              pb.Image = image;

                        } else {
                              pb.Image = image.RotateImage(angle % 360);
                        }
                  };

                  timer.Start();
            }

            #endregion
      }
}
