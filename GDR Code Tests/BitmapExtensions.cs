using RectpackSharp;
using System.Drawing;

namespace GDR_Code_Tests {

      public static class BitmapExtensions {

            public enum Corner { TopRight, TopLeft, BottomRight, BottomLeft };

            public static Bitmap cropImage(this Bitmap img, Rectangle cropArea) {
                  // TO-DO: Add safeguards by having throws or by it cropping as much as possible without an out of memory error

                  //if (cropArea.X + cropArea.Width > img.Width || cropArea.Y + cropArea.Height > img.Height) {
                  //    cropArea = new Rectangle(something)
                  //}
                  if (cropArea.Width <= 0 || cropArea.Height <= 0)
                        return new Bitmap(1, 1);

                  return img.Clone(cropArea, img.PixelFormat);
            }

            // Overload to support Lists and not just arrays
            public static Bitmap[] Multicrop(this Bitmap source, List<Rectangle> crops, int sliceSize = 512, float sliceThresholdMuiltiplier = 1.25f) {
                  return Multicrop(source, crops.ToArray(), sliceSize);
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

            public static Bitmap CopyTo(this Bitmap to, Bitmap copy, int xCoords, int yCoords, Corner corner = Corner.TopRight) {
                  ArgumentNullException.ThrowIfNull(to);
                  ArgumentNullException.ThrowIfNull(copy);

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

            public static Bitmap[] Shuffle(this Bitmap[] bmps, Random random = null, bool allowDuplicates = false) {
                  if (random == null)
                        new Random(Guid.NewGuid().GetHashCode());

                  int[] values = random.GetShuffledIntRange(bmps.Length).ToArray();
                  Bitmap[] ret = new Bitmap[bmps.Length];
                  for (int i = 0; i < bmps.Length; i++)
                        ret[i] = bmps[values[i]].GetClone();
                  return ret;
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
