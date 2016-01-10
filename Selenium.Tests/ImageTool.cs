using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Selenium.Tests
{
    public class ImageTool
    {
        public static unsafe Bitmap GetDifferenceImage(Bitmap image1, Bitmap image2, Color matchColor)
        {
            if (image1 == null | image2 == null)
                return null;

            if (image1.Height != image2.Height || image1.Width != image2.Width)
                return null;

            Bitmap diffImage = image2.Clone() as Bitmap;

            int height = image1.Height;
            int width = image1.Width;

            BitmapData data1 = image1.LockBits(new Rectangle(0, 0, width, height), 
                ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData data2 = image2.LockBits(new Rectangle(0, 0, width, height), 
                ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData diffData = diffImage.LockBits(new Rectangle(0, 0, width, height), 
                ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            byte* data1Ptr = (byte*)data1.Scan0;
            byte* data2Ptr = (byte*)data2.Scan0;
            byte* diffPtr = (byte*)diffData.Scan0;

            byte[] swapColor = new byte[3];
            swapColor[0] = matchColor.B;
            swapColor[1] = matchColor.G;
            swapColor[2] = matchColor.R;

            int rowPadding = data1.Stride - (image1.Width * 3);

            // iterate over height (rows)
            for (int i = 0; i < height; i++)
            {
                // iterate over width (columns)
                for (int j = 0; j < width; j++)
                {
                    int same = 0;

                    byte[] tmp = new byte[3];

                    // compare pixels and copy new values into temporary array
                    for (int x = 0; x < 3; x++)
                    {
                        tmp[x] = data2Ptr[0];
                        if (data1Ptr[0] == data2Ptr[0])
                        {
                            same++;
                        }
                        data1Ptr++; // advance image1 ptr
                        data2Ptr++; // advance image2 ptr
                    }

                    // swap color or add new values
                    for (int x = 0; x < 3; x++)
                    {
                        diffPtr[0] = (same == 3) ? swapColor[x] : tmp[x];
                        diffPtr++; // advance diff image ptr
                    }
                }

                // at the end of each column, skip extra padding
                if (rowPadding > 0)
                {
                    data1Ptr += rowPadding;
                    data2Ptr += rowPadding;
                    diffPtr += rowPadding;
                }
            }

            image1.UnlockBits(data1);
            image2.UnlockBits(data2);
            diffImage.UnlockBits(diffData);

            return diffImage;
        }

        /// <summary>
        /// Counts the difference image.
        /// </summary>
        /// <returns>The difference image.</returns>
        /// <param name="image1">Image1.</param>
        /// <param name="match">Match.</param>
        public static unsafe int CountDifferenceImage(Bitmap image1, Color match)
        {
            int height = image1.Height;
            int width = image1.Width;
            int diff = 0;

            BitmapData data1 = image1.LockBits(new Rectangle(0, 0, width, height), 
                ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            byte* data1Ptr = (byte*)data1.Scan0;

            byte[] swapColor = new byte[3];
            swapColor[0] = match.B;
            swapColor[1] = match.G;
            swapColor[2] = match.R;
            int rowPadding = data1.Stride - (image1.Width * 3);

            // iterate over height (rows)
            for (int i = 0; i < height; i++) {
                // iterate over width (columns)
                for (int j = 0; j < width; j++) {

                    for (int x = 0; x < 3; x++)
                    {
                        if (data1Ptr[0] != swapColor[x])
                        {
                            diff++;
                        } 
                        data1Ptr++;
                    }
                }
                // at the end of each column, skip extra padding
                if (rowPadding > 0)
                {
                    data1Ptr += rowPadding;
                }
            }
            image1.UnlockBits(data1);
            return diff;
        }
    }
}

