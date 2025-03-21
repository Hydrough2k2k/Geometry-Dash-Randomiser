using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using static Geometry_Dash_Randomiser.GameFiles;
using System.IO;

#pragma warning disable

namespace Geometry_Dash_Randomiser {

      internal static class BitmapExtensions {

            public enum Corner { TopRight, TopLeft, BottomRight, BottomLeft };

            public static Bitmap cropImage(this Bitmap img, int x, int y, int width, int height) {
                  return cropImage(img, new Rectangle(new Point(x, y), new Size(width, height)));
            }

            public static Bitmap cropImage(this Bitmap img, Point point, Size size) {
                  return cropImage(img, new Rectangle(point, size));
            }

            public static Bitmap cropImage(this Bitmap img, Rectangle cropArea) {
                  // TO-DO: Add safeguards by having throws or by it cropping as much as possible without an out of memory error

                  return img.Clone(cropArea, img.PixelFormat);
            }

            /// <summary>
            /// Method to rotate an Image object. The result can be one of three cases:
            /// - upsizeOk = true: output image will be larger than the input, and no clipping occurs 
            /// - upsizeOk = false & clipOk = true: output same size as input, clipping occurs
            /// - upsizeOk = false & clipOk = false: output same size as input, image reduced, no clipping
            /// 
            /// A background color must be specified, and this color will fill the edges that are not 
            /// occupied by the rotated image. If color = transparent the output image will be 32-bit, 
            /// otherwise the output image will be 24-bit.
            /// 
            /// Note that this method always returns a new Bitmap object, even if rotation is zero - in 
            /// which case the returned object is a clone of the input object. 
            /// </summary>
            /// <param name="inputImage">input Image object, is not modified</param>
            /// <param name="angleDegrees">angle of rotation, in degrees</param>
            /// <param name="upsizeOk">see comments above</param>
            /// <param name="clipOk">see comments above, not used if upsizeOk = true</param>
            /// <param name="backgroundColor">color to fill exposed parts of the background</param>
            /// <returns>new Bitmap object, may be larger than input image</returns>
            public static Bitmap RotateImage(this Bitmap inputImage, float angleDegrees, bool upsizeOk = true,
                                             bool clipOk = false) {
                  Color backgroundColor = Color.Transparent;

                  // Test for zero rotation and return a clone of the input image
                  if (angleDegrees == 0f)
                        return (Bitmap)inputImage.Clone();

                  // Set up old and new image dimensions, assuming upsizing not wanted and clipping OK
                  int oldWidth = inputImage.Width;
                  int oldHeight = inputImage.Height;
                  int newWidth = oldWidth;
                  int newHeight = oldHeight;
                  float scaleFactor = 1f;

                  // If upsizing wanted or clipping not OK calculate the size of the resulting bitmap
                  if (upsizeOk || !clipOk) {
                        double angleRadians = angleDegrees * Math.PI / 180d;

                        double cos = Math.Abs(Math.Cos(angleRadians));
                        double sin = Math.Abs(Math.Sin(angleRadians));
                        newWidth = (int)Math.Round(oldWidth * cos + oldHeight * sin);
                        newHeight = (int)Math.Round(oldWidth * sin + oldHeight * cos);
                  }

                  // If upsizing not wanted and clipping not OK need a scaling factor
                  if (!upsizeOk && !clipOk) {
                        scaleFactor = Math.Min((float)oldWidth / newWidth, (float)oldHeight / newHeight);
                        newWidth = oldWidth;
                        newHeight = oldHeight;
                  }

                  // Create the new bitmap object. If background color is transparent it must be 32-bit, 
                  //  otherwise 24-bit is good enough.
                  Bitmap newBitmap = new Bitmap(newWidth, newHeight, backgroundColor == Color.Transparent ?
                                                   PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb);
                  newBitmap.SetResolution(inputImage.HorizontalResolution, inputImage.VerticalResolution);

                  // Create the Graphics object that does the work
                  using (Graphics graphicsObject = Graphics.FromImage(newBitmap)) {
                        graphicsObject.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphicsObject.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphicsObject.SmoothingMode = SmoothingMode.HighQuality;

                        // Fill in the specified background color if necessary
                        if (backgroundColor != Color.Transparent)
                              graphicsObject.Clear(backgroundColor);

                        // Set up the built-in transformation matrix to do the rotation and maybe scaling
                        graphicsObject.TranslateTransform(newWidth / 2f, newHeight / 2f);

                        if (scaleFactor != 1f)
                              graphicsObject.ScaleTransform(scaleFactor, scaleFactor);

                        graphicsObject.RotateTransform(angleDegrees);
                        graphicsObject.TranslateTransform(-oldWidth / 2f, -oldHeight / 2f);

                        // Draw the result 
                        graphicsObject.DrawImage(inputImage, 0, 0);
                  }

                  return newBitmap;
            }

            public static Bitmap[,] Subdivide(this Bitmap b, int maxDimensionSize) {
                  int horizontalSlices = (b.Height - 1) / maxDimensionSize + 1;
                  int verticalSlices = (b.Width - 1) / maxDimensionSize + 1;

                  // Create 2D array
                  Bitmap[,] sub = new Bitmap[verticalSlices, horizontalSlices];

                  for (int y = 0; y < horizontalSlices; y++) {

                        for (int x = 0; x < verticalSlices; x++) {

                              // How big the cropped subsheet will be, max will be "maxDimensionSize"
                              int cropWidth = Math.Min(maxDimensionSize, b.Width - x * maxDimensionSize);
                              int cropHeight = Math.Min(maxDimensionSize, b.Height - y * maxDimensionSize);

                              Point point = new Point(x * maxDimensionSize, y * maxDimensionSize);
                              Size size = new Size(cropWidth, cropHeight);

                              sub[x, y] = cropImage(b, new Rectangle(point, size));
                        }
                  }
                  return sub;
            }

            public static Bitmap CopyTo(this Bitmap to, Bitmap copy, int xCoords, int yCoords, Corner corner = Corner.TopRight) {
                  //ArgumentNullException.ThrowIfNull(to);
                  //ArgumentNullException.ThrowIfNull(copy);

                  // Adjust the X and Y coordinates based on the picked corner
                  switch (corner) {
                        case Corner.TopLeft:
                              xCoords += copy.Width;
                              break;
                        case Corner.BottomRight:
                              yCoords -= copy.Height;
                              break;
                        case Corner.BottomLeft:
                              xCoords += copy.Width;
                              yCoords -= copy.Height;
                              break;
                        default:
                              break;
                  }

                  Rectangle destRect = new Rectangle(new Point(xCoords, yCoords), new Size(copy.Width, copy.Height));
                  using (Graphics g = Graphics.FromImage(to)) {
                        g.DrawImageUnscaledAndClipped(copy, destRect);
                  }
                  return to;
            }

            // Code and comment courtesy of mpen on Stack Overflow: https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp
            /// <summary>
            /// Resize the image to the specified width and height.
            /// </summary>
            /// <param name="image">The image to resize.</param>
            /// <param name="width">The width to resize to.</param>
            /// <param name="height">The height to resize to.</param>
            /// <returns>The resized image.</returns>
            public static Bitmap ResizeImage(this Image image, int width, int height) {
                  var destRect = new Rectangle(0, 0, width, height);
                  var destImage = new Bitmap(width, height);

                  destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                  using (var graphics = Graphics.FromImage(destImage)) {
                        graphics.CompositingMode = CompositingMode.SourceCopy;
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        using (var wrapMode = new ImageAttributes()) {
                              wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                              graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                        }
                  }

                  return destImage;
            }
      }
}
