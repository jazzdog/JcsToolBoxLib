using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.IO;

namespace ToolBoxLib
{
    public static partial class CUtil
    {
        public static MemoryStream BitmapStreamResize(MemoryStream IntputStream, double Scale)
        {
            if (IntputStream != null && Scale > 0)
            {
                try
                {
                    Bitmap OriginImg = new Bitmap(IntputStream);
                    return BitmapStreamResize(OriginImg, Scale);
                }
                catch
                {
                    CDebug.jmsgEx("BitmapStreamResize Stream Error");
                    return null;
                }
            }
            else
            {
                CDebug.jmsgEx("BitmapStreamResize Parameter Error");
                return null;
            }
        }
        public static MemoryStream BitmapStreamResize(Bitmap IntputBitmap, double Scale)
        {
            if (IntputBitmap != null && Scale > 0 && IntputBitmap.Width > 0 && IntputBitmap.Height > 0)
            {
                try
                {
                    Bitmap ResizeImg = ResizeProcess(IntputBitmap, IntputBitmap.Width, IntputBitmap.Height, (int)(IntputBitmap.Width * Scale), (int)(IntputBitmap.Height * Scale));

                    MemoryStream ResizeStream = new MemoryStream();
                    ResizeImg.Save(ResizeStream, System.Drawing.Imaging.ImageFormat.Png);
                    return ResizeStream;
                }
                catch
                {
                    CDebug.jmsgEx("BitmapStreamResize Transfer Error");
                    return null;
                }
            }
            else
            {
                CDebug.jmsgEx("BitmapStreamResize Parameter Error");
                return null;
            }
        }
        private static Bitmap ResizeProcess(Bitmap originImage, int oriwidth, int oriheight, int width, int height)
        {
            Bitmap resizedbitmap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(resizedbitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(System.Drawing.Color.Transparent);
            g.DrawImage(originImage, new Rectangle(0, 0, width, height), new Rectangle(0, 0, oriwidth, oriheight), GraphicsUnit.Pixel);
            return resizedbitmap;
        }

    }
}