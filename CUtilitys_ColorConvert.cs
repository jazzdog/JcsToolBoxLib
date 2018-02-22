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


namespace ToolBoxLib
{
    public static partial class CUtil
    {
        /// <summary>
        /// ARGB string to System.Windows.Media.Color
        /// </summary>
        /// <param name="strColorCode"></param>
        /// <returns></returns>
        public static System.Windows.Media.Color colorConvert2WinMediaColorByColorCode(string strColorCode)
        {
            return (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(strColorCode);
        }

        /// <summary>
        /// System.Drawing.Color => System.Windows.Media.Color
        /// </summary>
        /// <param name="drawColor"></param>
        /// <returns></returns>
        public static System.Windows.Media.Color colorConvert2WinMediaColor(System.Drawing.Color drawColor)
        {
            return System.Windows.Media.Color.FromArgb(drawColor.A, drawColor.R, drawColor.G, drawColor.B);
        }

        /// <summary>
        /// string Color Name(Red,Blue) or Color Code #RRGGBB => System.Drawing.Color
        /// </summary>
        /// <param name="strColorCode"></param>
        /// <returns></returns>
        public static System.Drawing.Color colorConvert2DrawingColorByColorString(string strColor)
        {
            if (strColor.Substring(0, 1).CompareTo("#") == 0)//可以直接輸入字串ARGB值 "#FFADFF"
                return System.Drawing.ColorTranslator.FromHtml(strColor);
            else
                return System.Drawing.Color.FromName(strColor);

        }


        /// <summary>
        /// System.Windows.Media.Color => System.Drawing.Color 
        /// </summary>
        /// <param name="WinMediaColor"></param>
        /// <returns></returns>
        public static System.Drawing.Color colorConvert2DrawingColor(System.Windows.Media.Color WinMediaColor)
        {
            return System.Drawing.Color.FromArgb(WinMediaColor.A, WinMediaColor.R, WinMediaColor.G, WinMediaColor.B);
        }


        /// <summary>
        /// string ARGB => System.Windows.Media.Brush
        /// </summary>
        /// <param name="strColorCode"></param>
        /// <returns></returns>
        public static System.Windows.Media.Brush colorConvert2BrushByColorCode(string strColorCode)
        {
            BrushConverter bc = new BrushConverter();
            //return (System.Windows.Media.Brush)bc.ConvertFrom(strColorCode); 
            //if (strColorCode.Substring(0, 1).CompareTo("#") == 0)//可以直接輸入字串ARGB值 "#FFADFF"
            try
            {
                return (System.Windows.Media.Brush)bc.ConvertFromString(strColorCode);
            }
            catch (Exception e)
            {
                CDebug.jmsg("[colorConvert2BrushByColorCode]err: [{0}]",e.ToString());
                return null;
            }

        }
    }
}
