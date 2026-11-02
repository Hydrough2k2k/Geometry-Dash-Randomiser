using System;
using System.Drawing;
using System.Windows.Forms;

namespace Geometry_Dash_Randomiser {

      public partial class GDR_Form : Form {

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
                              pb.Image = image.RotateImage(angle);
                        }
                  };

                  timer.Start();
            }
      }
}
