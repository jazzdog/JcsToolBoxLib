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
                Debug.Write(String.Format(strBugMsg + "\n", args));
            }
            catch (Exception e)
            {
                //strBugMsg = strBugMsg.Replace(System.Environment.NewLine, string.Empty);
                //strBugMsg = strBugMsg.Trim();
                //strBugMsg = strBugMsg.Replace("{", "{{");
                //strBugMsg = strBugMsg.Replace("}", "}}");
                strBugMsg = CUtil.fixStringFormateError(strBugMsg);
                Debug.Write(String.Format(strBugMsg + ":"+e.ToString()+"\n", args));
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

            string strFile = sf.GetFileName();
            string strFunction = sf.GetMethod().ToString();
            int iLine = sf.GetFileLineNumber();
            int iColumn = sf.GetFileColumnNumber();
            strInfo = strFile + "(" + iLine + ")" + ":[" + strFunction + "]"; //●可以連結回來的格式
            try
            {
                Debug.Write(String.Format(strInfo + ">>\n" + strBugMsg + "\n", args));
            }
            catch //(Exception e)
            {
                strBugMsg = CUtil.fixStringFormateError(strBugMsg);
                Debug.Write(String.Format(strInfo + ">>\n" + strBugMsg + "\n", args));
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
