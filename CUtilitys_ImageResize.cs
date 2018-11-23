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

        public static bool isvalidRectangle(Rectangle rectTest, int nMinWH = -1)
        {
            if (rectTest.Width < nMinWH || rectTest.Height < nMinWH)
                return false;
            else
                return true;
        }

        public static Bitmap cutImage(string strB64Image, Rectangle recCutPoints)
        {
            byte[] btAryImage;
            convertB64StringToBinaryArray(strB64Image, out btAryImage);
            return cutImage(btAryImage, recCutPoints);
        }

        public static Bitmap cutImage(byte[] btaryImage, Rectangle recCutPoints)
        {
            Image _image = convertByteArrayToImage(btaryImage);
            return cutImage(_image, recCutPoints);
        }

        public static Rectangle fixRectangleofImage(Image img, Rectangle recCutPoints)
        {
            int nImage_W = img.Width;
            int nImage_H = img.Height;
            if (recCutPoints.X < 0)
                recCutPoints.X = 0;
            if (recCutPoints.Y < 0)
                recCutPoints.Y = 0;
            if ((recCutPoints.X + recCutPoints.Width) > nImage_W)
                recCutPoints.Width = (nImage_W - recCutPoints.X);
            if ((recCutPoints.Y + recCutPoints.Height) > nImage_H)
                recCutPoints.Height = (nImage_H - recCutPoints.Y);

            return recCutPoints;
        }

        public static Bitmap cutImage(Image img, Rectangle recCutPoints)
        {
            Rectangle _RecFixed = fixRectangleofImage(img, recCutPoints);
            if ((img != null) && (isvalidRectangle(_RecFixed, 1)))
            {


                Bitmap outbitmap = new Bitmap(_RecFixed.Width, _RecFixed.Height);
                Graphics g = Graphics.FromImage(outbitmap);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.Clear(System.Drawing.Color.Transparent);
                g.DrawImage(img, new Rectangle(0, 0, _RecFixed.Width, _RecFixed.Height), _RecFixed, GraphicsUnit.Pixel);
                return outbitmap;
            }
            else
            {
                return null;
            }
        }

        public static string convertImagetoB64String(Image theImage)
        {
            byte[] btimage;
            btimage = convertImagetoByteArray(theImage);
            return convertBinaryArrayToB64String(btimage);
        }

        public static byte[] convertImagetoByteArray(Image theImage)
        {
            if (theImage != null)
            {
                ImageConverter _imageConverter = new ImageConverter();
                byte[] theArray = (byte[])_imageConverter.ConvertTo(theImage, typeof(byte[]));
                return theArray;
            }
            else
            {
                return null;
            }

        }

        public static Image convertB64StringToImage(string b64string)
        {
            byte[] theImage;
            convertB64StringToBinaryArray(b64string, out theImage);
            return convertByteArrayToImage(theImage);
        }

        public static Image convertByteArrayToImage(byte[] btaryImage)
        {
            using (var ms = new MemoryStream(btaryImage))
            {
                return Image.FromStream(ms);
            }
        }


        public static Image resizeImage_fromPICBase64String(string strPICBase64, double theScale)
        {
            byte[] bytImg;
            convertB64StringToBinaryArray(strPICBase64, out bytImg);
            Image theImg = CUtil.convertByteArrayToImage(bytImg);
            MemoryStream memsRawImage = BitmapStreamResize((Bitmap)theImg, theScale);
            return convertByteArrayToImage(memsRawImage.ToArray());
        }

        public static String resizeImage_fromPICBase64StringToBase64(string strPICBase64, double theScale)
        {
            Image theResizedImg = resizeImage_fromPICBase64String( strPICBase64,  theScale);
            byte[] bytImg = convertImagetoByteArray(theResizedImg);
            return convertBinaryArrayToB64String(bytImg);
        }

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