using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
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
        /// 顯示對話視窗
        /// </summary>
        /// <param name="strBugMsg"></param>
        /// <param name="args"></param>
        public static void jbox(string strBugMsg, params Object[] args)
        {
            try
            {
                System.Windows.Forms.MessageBox.Show(string.Format(strBugMsg, args));
            }
            catch //(Exception e)
            {
                strBugMsg = fixStringFormateError(strBugMsg);
                System.Windows.Forms.MessageBox.Show(string.Format(strBugMsg, args));
            }
        }

        /// <summary>
        /// 顯示對話視窗，並且在設定秒數後自動消失
        /// </summary>
        /// <param name="nSec">顯示時間(秒)</param>
        /// <param name="strTitle">視窗標題</param>
        /// <param name="strBugMsg">訊息</param>
        /// <param name="args">訊息參數</param>
        public static void jboxAuto(int nSec,string strTitle,string strBugMsg, params Object[] args)
        {
            var w = new Form() { Size = new System.Drawing.Size(0, 0) };
            Task.Delay(TimeSpan.FromSeconds(nSec))
                .ContinueWith((t) => w.Close(), TaskScheduler.FromCurrentSynchronizationContext());
            try
            {
                System.Windows.Forms.MessageBox.Show(w, string.Format(strBugMsg, args), strTitle);
            }
            catch //(Exception e)
            {
                strBugMsg = fixStringFormateError(strBugMsg);
                System.Windows.Forms.MessageBox.Show(w, string.Format(strBugMsg, args), strTitle);
            }
        }

        public static bool IsNumeric(string strNumber)
        {
            try
            {
                int i = Convert.ToInt32(strNumber);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool isStringValid(string strTest, uint unMinLenth = 1)
        {
            if (strTest != null && strTest.Length >= unMinLenth)
                return true;
            else
                return false;
        }

        public static string Base64Encode(string AStr)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(AStr));
        }



        public static string Base64Decode(string ABase64)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(ABase64));
        }

        public static string fixStringFormateError(string strErroFormat)
        {
            string strFixed = strErroFormat;
            strFixed = strFixed.Replace(System.Environment.NewLine, string.Empty);
            strFixed = strFixed.Trim();
            strFixed = strFixed.Replace("{", "{{");
            strFixed = strFixed.Replace("}", "}}");
            return strFixed;
        }
        public static string StringToUnicode(string srcText)
        {
            string dst = "";
            char[] src = srcText.ToCharArray();
            for (int i = 0; i < src.Length; i++)
            {
                byte[] bytes = Encoding.Unicode.GetBytes(src[i].ToString());
                string str = @"\u" + bytes[1].ToString("X2") + bytes[0].ToString("X2");
                dst += str;
            }
            return dst;
        }

        public static string UnicodeToString(string srcText)
        {
            string dst = "";
            string src = srcText;
            int len = srcText.Length / 6;

            for (int i = 0; i <= len - 1; i++)
            {
                string str = "";
                str = src.Substring(0, 6).Substring(2);
                src = src.Substring(6);
                byte[] bytes = new byte[2];
                bytes[1] = byte.Parse(int.Parse(str.Substring(0, 2), System.Globalization.NumberStyles.HexNumber).ToString());
                bytes[0] = byte.Parse(int.Parse(str.Substring(2, 2), System.Globalization.NumberStyles.HexNumber).ToString());
                dst += Encoding.Unicode.GetString(bytes);
            }
            return dst;
        } 

    }
}
