using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

#pragma warning disable

namespace Geometry_Dash_Randomiser {

      public static class BitmapExtensions {

            public enum Corner { TopRight, TopLeft, BottomRight, BottomLeft };

            public static Bitmap BlackAndWhiteRecolour(this Bitmap image, Color blackReplacement, Color whiteReplacement) {
                  if (image.PixelFormat != PixelFormat.Format32bppArgb) {
                        throw new Exception($"Error: Bitmap must be in 32bpp ARGB format to use BlackAndWhiteRecolour. Current format is {image.PixelFormat}.");
                  }

                  Bitmap newImage = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);

                  for (int y = 0; y < newImage.Height; y++) {
                        for (int x = 0; x < newImage.Width; x++) {
                              Color pixel = image.GetPixel(x, y);
                              float interpolation = (float)((pixel.R + pixel.G + pixel.B) / 3) / 255;

                              newImage.SetPixel(x, y,
                                    Color.FromArgb(
                                          (int)(blackReplacement.R + (whiteReplacement.R - blackReplacement.R) * interpolation),
                                          (int)(blackReplacement.G + (whiteReplacement.G - blackReplacement.G) * interpolation),
                                          (int)(blackReplacement.B + (whiteReplacement.B - blackReplacement.B) * interpolation)
                                    )
                              );
                        }
                  }

                  return newImage;
            }

            public static Bitmap cropImage(this Bitmap img, int x, int y, int width, int height) {
                  return cropImage(img, new Rectangle(new Point(x, y), new Size(width, height)));
            }

            public static Bitmap cropImage(this Bitmap img, Point point, Size size) {
                  return cropImage(img, new Rectangle(point, size));
            }

            public static Bitmap cropImage(this Bitmap img, Rectangle cropArea) {

                  if (cropArea.X + cropArea.Width > img.Width || cropArea.Y + cropArea.Height > img.Height) {
                        Console.WriteLine($"Warning: Cropping area {cropArea} is partially out of the image {img.Size}. Adjusting crop area to fit within the image bounds.");
                        cropArea = new Rectangle(new Point(cropArea.X, cropArea.Y), new Size(img.Width - cropArea.X, img.Height - cropArea.Y));
                  }

                  if (cropArea.Width <= 0 || cropArea.Height <= 0) {

                        return new Bitmap(1, 1);
                  }

                  Bitmap target = new Bitmap(cropArea.Width, cropArea.Height);
                  using (Graphics g = Graphics.FromImage(target)) {
                        g.DrawImage(img, new Rectangle(0, 0, target.Width, target.Height),
                            cropArea,
                            GraphicsUnit.Pixel);
                  }
                  return target;
            }

            // Overload to support Lists and not just arrays
            public static Bitmap[] Multicrop(this Bitmap source, List<Rectangle> crops, int sliceSize = 512, float sliceThresholdMuiltiplier = 1.25f) {
                  return Multicrop(source, crops.ToArray(), sliceSize, sliceThresholdMuiltiplier);
            }

            public static Bitmap[] Multicrop(this Bitmap source, Rectangle[] crops, int sliceSize = 512, float sliceThresholdMuiltiplier = 1.25f) {
                  if (source.Width > sliceSize * sliceThresholdMuiltiplier || source.Height > sliceSize * sliceThresholdMuiltiplier) {
                        Bitmap[,] slices = source.Slice(sliceSize);
                        Bitmap[] ret = MulticropFromSlices(slices, crops, sliceSize);
                        slices.Dispose();

                        return ret;
                  } else {
                        return Multicrop(source, crops);
                  }
            }

            public static Bitmap[] MulticropFromSlices(this Bitmap source, Rectangle[] crops, int sliceSize = 512) {
                  Bitmap[,] slices = source.Slice(sliceSize);
                  Bitmap[] ret = MulticropFromSlices(slices, crops, sliceSize);
                  slices.Dispose();
                  return ret;
            }

            static Bitmap[] Multicrop(this Bitmap source, Rectangle[] crops) {
                  Bitmap[] ret = new Bitmap[crops.Length];
                  for (int i = 0; i < crops.Length; i++)
                        ret[i] = source.cropImage(crops[i]);
                  return ret;
            }

            static Bitmap[] MulticropFromSlices(this Bitmap[,] source, Rectangle[] crops, int sliceSize) {
                  Bitmap[] ret = new Bitmap[crops.Length];

                  for (int i = 0; i < crops.Length; i++) {
                        Rectangle cropRect = crops[i];

                        Point point = new Point(cropRect.X % sliceSize, cropRect.Y % sliceSize);
                        Size size = new Size(cropRect.Width, cropRect.Height);

                        if (point.X + size.Width <= sliceSize && point.Y + size.Height <= sliceSize) {
                              // Get what subsheet you need to crop the image from
                              int column = cropRect.X / sliceSize;
                              int row = cropRect.Y / sliceSize;

                              cropRect = new Rectangle(point, size);

                              // Crop the image from a subsheet, since it's entirely within it
                              ret[i] = source[column, row].cropImage(cropRect);

                        } else {
                              // Cropping from the subsheets. Way faster than copying from the big gamesheet
                              ret[i] = source.CropFromSlices(crops[i], sliceSize);
                        }
                  }
                  return ret;
            }

            static Bitmap CropFromSlices(this Bitmap[,] source, Rectangle cropRect, int sliceSize) {
                  int imageStartX = cropRect.X;
                  int imageEndX = imageStartX + cropRect.Width;

                  int imageStartY = cropRect.Y;
                  int imageEndY = imageStartY + cropRect.Height;

                  int startingColumn = imageStartX / sliceSize;
                  int startingRow = imageStartY / sliceSize;

                  // Calculate how many rows and columns you need to crop from
                  int columns = (imageEndX - 1) / sliceSize - (imageStartX / sliceSize) + 1;
                  int rows = (imageEndY - 1) / sliceSize - (imageStartY / sliceSize) + 1;

                  // Coordinates for cropping the data from the subsheets
                  int[] X_coords = Enumerable.Repeat(0, columns).ToArray();
                  int[] Y_coords = Enumerable.Repeat(0, rows).ToArray();

                  // Set the first crop's parameters
                  X_coords[0] = imageStartX % sliceSize;

                  // Set the last crop's parameters
                  Y_coords[0] = imageStartY % sliceSize;

                  // Make the default values equal to "sliceSize" 
                  int[] columnWidths = Enumerable.Repeat(sliceSize, columns).ToArray();
                  int[] rowHeights = Enumerable.Repeat(sliceSize, rows).ToArray();

                  // Set the first and last column widths separately
                  if (columnWidths.Length > 1) {
                        columnWidths[0] = sliceSize - (imageStartX % sliceSize);
                        columnWidths[columns - 1] = imageEndX % sliceSize;

                        // Set the width to the size of the subsheet to avoid making it 0
                        if (columnWidths[columns - 1] == 0)
                              columnWidths[columns - 1] = sliceSize;

                  } else {
                        columnWidths[0] = cropRect.Width;
                  }

                  // Set the first and last row heights separately
                  if (rowHeights.Length > 1) {
                        rowHeights[0] = sliceSize - (imageStartY % sliceSize);
                        rowHeights[rows - 1] = imageEndY % sliceSize;

                        // Set the height to the size of the subsheet to avoid making it 0
                        if (rowHeights[rows - 1] == 0)
                              rowHeights[rows - 1] = sliceSize;

                  } else {
                        rowHeights[0] = cropRect.Height;
                  }

                  Bitmap ret = new Bitmap(cropRect.Width, cropRect.Height);

                  // X and Y offsets are for keeping track of where the image should be printed onto the compiled texture
                  int Y_offset = 0;
                  for (int y = startingRow; y < startingRow + rows; y++) {

                        int X_offset = 0;
                        for (int x = startingColumn; x < startingColumn + columns; x++) {
                              Point point = new Point(X_coords[x - startingColumn], Y_coords[y - startingRow]);
                              Size size = new Size(columnWidths[x - startingColumn], rowHeights[y - startingRow]);

                              Rectangle crop = new Rectangle(point, size);
                              Bitmap fragment = source[x, y].cropImage(crop);
                              ret.CopyTo(fragment, X_offset, Y_offset);

                              X_offset += columnWidths[x - startingColumn];
                        }
                        Y_offset += rowHeights[y - startingRow];
                  }

                  return ret;
            }

            public static Bitmap[,] Slice(this Bitmap b, int maxSliceSize) {
                  int horizontalSlices = (b.Height - 1) / maxSliceSize + 1;
                  int verticalSlices = (b.Width - 1) / maxSliceSize + 1;

                  // Create 2D array
                  Bitmap[,] sub = new Bitmap[verticalSlices, horizontalSlices];

                  for (int y = 0; y < horizontalSlices; y++) {

                        for (int x = 0; x < verticalSlices; x++) {

                              // How big the cropped subsheet will be, max will be "maxSliceSize"
                              int cropWidth = Math.Min(maxSliceSize, b.Width - x * maxSliceSize);
                              int cropHeight = Math.Min(maxSliceSize, b.Height - y * maxSliceSize);

                              Point point = new Point(x * maxSliceSize, y * maxSliceSize);
                              Size size = new Size(cropWidth, cropHeight);

                              sub[x, y] = cropImage(b, new Rectangle(point, size));
                        }
                  }
                  return sub;
            }

            public static Bitmap RotateImage(this Bitmap bmp, float angle) {

                  //create a new empty bitmap to hold rotated image
                  Bitmap returnBitmap = new Bitmap(bmp.Width, bmp.Height);

                  using (Graphics g = Graphics.FromImage(returnBitmap)) {
                        // move rotation point to center of image
                        g.TranslateTransform((float)(bmp.Width - 2) / 2, (float)(bmp.Height - 2) / 2);

                        // rotate
                        g.RotateTransform(angle);

                        // move image back
                        g.TranslateTransform(-(float)(bmp.Width - 2) / 2, -(float)(bmp.Height - 2) / 2);

                        // draw passed in image onto graphics object
                        g.DrawImage(bmp, new Point(0, 0));
                  }

                  return returnBitmap;
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
            public static Bitmap RotateImage_2(this Bitmap inputImage, float angleDegrees, bool upsizeOk = true,
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

            // Maybe not the best name for this method?
            public static Bitmap CopyTo(this Bitmap to, Bitmap copy, int xCoords, int yCoords, Corner corner = Corner.TopRight) {
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

            public static Bitmap GetClone(this Bitmap bmp) {
                  return (Bitmap)bmp.Clone();
            }

            public static Bitmap[] GetClone(this Bitmap[] bmps) {
                  Bitmap[] ret = new Bitmap[bmps.Length];
                  for (int i = 0; i < bmps.Length; i++)
                        ret[i] = bmps[i].GetClone();
                  bmps.Dispose();
                  return ret;
            }

            public static Bitmap Assemble(Bitmap[] images, Rectangle[] rects, Size imageSize) {
                  Bitmap gamesheet = new Bitmap(imageSize.Width, imageSize.Height);
                  int loops = Math.Min(images.Length, rects.Length);

                  for (int i = 0; i < loops; i++) {
                        if (images[i] != null && images[i].Width != 0 && images[i].Height != 0) {
                              gamesheet.CopyTo(images[i], rects[i].X, rects[i].Y);
                        }
                  }
                  return gamesheet;
            }

            public static void Dispose(this Bitmap[] arr) {
                  if (arr == null) return;

                  for (int x = 0; x < arr.GetLength(0); x++)
                        if (arr[x] != null)
                              arr[x].Dispose();
            }

            public static void Dispose(this Bitmap[,] arr) {
                  if (arr == null) return;

                  for (int x = 0; x < arr.GetLength(0); x++)
                        for (int y = 0; y < arr.GetLength(1); y++)
                              if (arr[x, y] != null)
                                    arr[x, y].Dispose();
            }

            public static void Dispose(this Bitmap[][] arr) {
                  if (arr == null) return;

                  for (int x = 0; x < arr.GetLength(0); x++) {
                        if (arr[x] != null) {
                              arr[x].Dispose();
                        }
                  }
            }
      }
}
