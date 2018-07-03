using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace ToolBoxLib
{
    public class dinfo
    {
        public string filename;
        public string functionname;
        public int nLine;
        public int nCol;
    }
    public class CDebug
    {
        /// <summary>
        /// 在DEBUG模式中顯示資訊(不顯示行號)
        /// </summary>
        /// <param name="strBugMsg"></param>
        /// <param name="args"></param>
        public static void jmsg(String strBugMsg, params Object[] args)
        {
            try
            {
                string _fix_strBugMsg = CUtil.fixStringFormateError(strBugMsg);

                if (args.Length > 0)
                    _fix_strBugMsg = String.Format(_fix_strBugMsg, args);

                Debug.Write(_fix_strBugMsg+"\n");
            }
            catch (Exception e)
            {
                /*System.FormatException: 輸入字串格式不正確。
                 * 因為輸入JSON 包含{} 造成arg以為是參數序號{0},{1},...=>輸入字串格式不正確
                 */

                string _fix_strBugMsg = CUtil.fixStringFormateError(strBugMsg);
                _fix_strBugMsg = _fix_strBugMsg.Replace("{", "{{"); //如果要列印過複雜的字串將引號去除
                _fix_strBugMsg = _fix_strBugMsg.Replace("}", "}}"); //如果要列印過複雜的字串將引號去除
                Debug.Write(String.Format(_fix_strBugMsg + ":" + e.ToString() + "\n", args));
                //Debug.Write(strBugMsg);
            }
        }

        public static void jmsgt(String strBugMsg, params Object[] args)
        {
            try
            {
                string strNowTime = CUtil.getCurrentDateTimeString("MM-dd hh:mm:ss.ffffff");
                string _fix_strBugMsg = CUtil.fixStringFormateError(strBugMsg);

                if (args.Length > 0)
                    _fix_strBugMsg = String.Format(_fix_strBugMsg, args);

                Debug.Write("["+ strNowTime+"]"+_fix_strBugMsg + "\n");
            }
            catch (Exception e)
            {
                /*System.FormatException: 輸入字串格式不正確。
                 * 因為輸入JSON 包含{} 造成arg以為是參數序號{0},{1},...=>輸入字串格式不正確
                 */

                string _fix_strBugMsg = CUtil.fixStringFormateError(strBugMsg);
                _fix_strBugMsg = _fix_strBugMsg.Replace("{", "{{"); //如果要列印過複雜的字串將引號去除
                _fix_strBugMsg = _fix_strBugMsg.Replace("}", "}}"); //如果要列印過複雜的字串將引號去除
                Debug.Write(String.Format(_fix_strBugMsg + ":" + e.ToString() + "\n", args));
                //Debug.Write(strBugMsg);
            }
        }



        public static dinfo dInfo(int nSkeepLevel=1)
        {
            StackTrace stack = new StackTrace(nSkeepLevel, true); //●取得stackframe的階層(視架構可能需要改變)
            StackFrame sf = stack.GetFrame(0); //●0 取最遠的
            dinfo dinfo = new dinfo();
            dinfo.filename = sf.GetFileName();
            dinfo.functionname = sf.GetMethod().ToString();
            dinfo.nLine = sf.GetFileLineNumber();
            dinfo.nCol = sf.GetFileColumnNumber();
            return dinfo;
        }
       


        /// <summary>
        /// 在DEBUG模式中顯示資訊顯示行號，並連結回程式碼
        /// </summary>
        /// <param name="strBugMsg"></param>
        /// <param name="args"></param>
        public static void jmsgEx(String strBugMsg, params Object[] args)
        {
            String strInfo;
            StackTrace stack = new StackTrace(1, true); //●取得stackframe的階層(視架構可能需要改變)
            var sf = stack.GetFrame(0); //●取最遠的
            string strNowTime = CUtil.getCurrentDateTimeString("MM-dd hh:mm:ss.ffffff");
            string strFile = sf.GetFileName();
            string strFunction = sf.GetMethod().ToString();
            int iLine = sf.GetFileLineNumber();
            int iColumn = sf.GetFileColumnNumber();
            strInfo = strFile + "(" + iLine + ")" + ":[" + strFunction + "]"; //●可以連結回來的格式
            try
            {
                Debug.Write(String.Format(strInfo + ">>\n" + "[" + strNowTime + "]"+strBugMsg + "\n", args));
            }
            catch //(Exception e)
            {
                /*System.FormatException: 輸入字串格式不正確。
                 * 因為輸入JSON 包含{} 造成arg以為是參數序號{0},{1},...=>輸入字串格式不正確
                 */

                string _fix_strBugMsg = CUtil.fixStringFormateError(strBugMsg);
                _fix_strBugMsg = _fix_strBugMsg.Replace("{", "{{"); //如果要列印過複雜的字串將引號去除
                _fix_strBugMsg = _fix_strBugMsg.Replace("}", "}}"); //如果要列印過複雜的字串將引號去除

                //strBugMsg = CUtil.fixStringFormateError(strBugMsg);
                Debug.Write(String.Format(strInfo + ">>\n" + _fix_strBugMsg + "\n", args));
            }
        }

        public static string getDebugLink(int nFrameLayer)
        {
            String strInfo;
            StackTrace stack = new StackTrace(nFrameLayer, true); //●取得stackframe的階層(視架構可能需要改變)
            var sf = stack.GetFrame(0); //●取最遠的

            string strFile = sf.GetFileName();
            string strFunction = sf.GetMethod().ToString();
            int iLine = sf.GetFileLineNumber();
            int iColumn = sf.GetFileColumnNumber();
            strInfo = strFile + "(" + iLine + ")" + ":[" + strFunction + "]"; //●可以連結回來的格式

            return strInfo;
        }


    }
}
