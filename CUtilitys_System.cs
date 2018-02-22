using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;



namespace ToolBoxLib
{
    

    public static partial class CUtil
    {
        private static Random jRand = new Random();
        private static string m_FileLogName = null;
        private static string m_strLogFileName = "JLOG";
        private static string m_strFullFilePath;
        private static StreamWriter _flog;
#if DEBUG
        readonly static bool T_DEBUG = true;
#else
         readonly static bool T_DEBUG = false;
#endif

        public static string getCurrentPath()
        {
            return System.Windows.Forms.Application.StartupPath; //執行程式的路徑;//Directory.GetCurrentDirectory();
        }

        public static bool isFolderExist(string strPath, bool blCreate)
        {
            string strLocalPath = "";
            if (strPath.IndexOf('/') == 0)
            {
                strLocalPath = getCurrentPath();
                strLocalPath += strPath;
            }
            else
                strLocalPath = strPath;

            bool blExist = Directory.Exists(strLocalPath);
            if ((blCreate == true) && (blExist == false))
            {
                Directory.CreateDirectory(strLocalPath);
                return true;
            }

            return blExist;
        }

        private static bool initFileLogger()
        {
            string logPathname = System.Windows.Forms.Application.StartupPath; //執行程式的路徑;//Directory.GetCurrentDirectory();
            string strMac = getMacAddr()[0];

            logPathname += "\\SaveLog";
            if (!Directory.Exists(logPathname))
            {
                Directory.CreateDirectory(logPathname);
            }
            if (m_FileLogName == null)
                m_strFullFilePath = string.Format("{0}\\[{1:0000}_{2:00}_{3:00}]{4}_{5}.log", logPathname,
                                               DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, m_strLogFileName, strMac);
            else
                m_strFullFilePath = string.Format("{0}\\[{1:0000}_{2:00}_{3:00}]{4}_{5}_{6}.log", logPathname,
                                                   DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, m_strLogFileName, m_FileLogName, strMac);
            _flog = new StreamWriter(m_strFullFilePath, true);
            string strAppFileVersion = CUtil.getAppilcationFileVersion();
            
            if (m_FileLogName==null)
                jlog("\n===================[LOG_HEAD -Ver.{0}-]===================\n\t\t\t{1}\n\t\t\t================================================", strAppFileVersion,m_strFullFilePath);
            else
                jlog("\n===================[LOG_HEAD -Ver.{0}- <<{1}>> ]===================\n\t\t\t{2}\n\t\t\t================================================", strAppFileVersion,m_FileLogName, m_strFullFilePath);


            return true;

        }

        public static void ChangeLogFileName(string FileName)
        {
            if (FileName != null && FileName.Length > 0)
            {
                m_FileLogName = FileName;
                if (_flog != null)
                {
                    _flog.Flush();
                    _flog = null;
                }

                initFileLogger();

            }
        }


        public static void jlogEx(string log_string, params Object[] args)
        {
            StackTrace stack = new StackTrace(1, true); //●取得stackframe的階層(視架構可能需要改變)
            var sf = stack.GetFrame(0); //●取最遠的
            string strFunction = sf.GetMethod().ToString();
            string strMsg;
            try
            {
                strMsg = string.Format(log_string, args);
            }
            catch //(Exception e)
            {
                log_string = fixStringFormateError(log_string);
                strMsg = string.Format(log_string, args);
            }
            jlog("\n\t\t\t[{0}]\n\t\t\t{1}", strFunction, strMsg);

        }
        public static void jlog(string log_string, params Object[] args)
        {
            if (_flog == null)
            {
                initFileLogger();
            }
            updateFileNamebyDate();
            if (!File.Exists(m_strFullFilePath))///更換日期
            {
                string logPathname = System.Windows.Forms.Application.StartupPath; //執行程式的路徑;//Directory.GetCurrentDirectory();
                if (m_FileLogName==null)
                m_strFullFilePath = string.Format("{0}\\[{1:0000}_{2:00}_{3:00}]{4}.log", logPathname,
                                              DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, m_strLogFileName);
                else
                m_strFullFilePath = string.Format("{0}\\[{1:0000}_{2:00}_{3:00}]{4}_{5}.log", logPathname,
                                                   DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, m_strLogFileName, m_FileLogName);
                _flog = File.CreateText(m_strFullFilePath);

            }
            string strMsg2Log;
            if (args.Length <= 0)
                strMsg2Log = log_string;
            else
                strMsg2Log = string.Format(log_string, args);
            _flog.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]> " + strMsg2Log);
            _flog.Flush();

            if (T_DEBUG == true)
            {
                CDebug.jmsg("{0}", strMsg2Log);
            }

        }

        public static List<string> getIPAddr()
        {
            IPHostEntry host;
            List<string> listIPAddr = new List<string>();
            
           
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    listIPAddr.Add(ip.ToString());
                }
            }
            return listIPAddr; 
            
            
        }
        public static List<string> getMacAddr()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            List<string> macList = new List<string>();
           // List<NetworkInterface> macNicList = new List<NetworkInterface>();
            foreach (var nic in nics)
            {
                // 因為電腦中可能有很多的網卡(包含虛擬的網卡)，
                // 我只需要 Ethernet 網卡的 MAC
                if (((nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)||(nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))&&
                (nic.OperationalStatus== OperationalStatus.Up))
                {
                    macList.Add(nic.GetPhysicalAddress().ToString());
                    //macNicList.Add(nic);
                }
            }

            return macList;
        }

        /// <summary>
        /// 檢查網路狀態，輸入字串形式HTTP網址，或是IP位置
        /// </summary>
        /// <param name="strIP_HTTp"></param>
        /// <returns>bool</returns>
        public static bool IsInternetWrok(string strIP_Http)
        {
            //CDebug.jmsg("IsInternetWrok.....");
            Ping p = new Ping();
            PingReply reply;
            try
            {
                //取得網站的回覆
                reply = p.Send(strIP_Http);
                //如果回覆的狀態為Success則return true
                if (reply.Status == IPStatus.Success)
                {
                    //CDebug.jmsg("IsInternetWrok.....true");
                    return true;
                }

            }
            catch
            {
                //CDebug.jmsg("IsInternetWrok.....false");
                return false;
            }
            return false; 

        }


        public static void closeProcess(string strProcessName)
        {
            System.Diagnostics.Process[] MyProcess = System.Diagnostics.Process.GetProcessesByName(strProcessName);
            if (MyProcess.Length > 0)
                MyProcess[0].Kill();
        }


        public static string getCurrentDateTimeString()
        {
            string strDateTime = DateTime.Now.ToString("yyyy_MM_dd_HHmmss");
            return strDateTime;
        }

        public static DateTime convertStringToDateTime(string strDateTime,string strDateTimeFormate = "yyyy/MM/dd HH:mm")
        {
            DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();

            dtFormat.ShortDatePattern = strDateTimeFormate;
            return Convert.ToDateTime(strDateTime, dtFormat);
        }

        /// <summary>
        /// 清除程式占用的記憶體
        /// </summary>
        public static void clearMemory()
        {
            System.Diagnostics.Process loProcess = System.Diagnostics.Process.GetCurrentProcess();

            try
            {
                loProcess.MaxWorkingSet = (IntPtr)((int)loProcess.MaxWorkingSet - 1); //1,409,024
                loProcess.MinWorkingSet = (IntPtr)((int)loProcess.MinWorkingSet - 1); //  200,704
                CUtil.jlogEx("[clearMemory]MaxWorkingSet={0} MinWorkingSet={1}", loProcess.MaxWorkingSet, loProcess.MinWorkingSet);
            }
            catch (System.Exception)
            {

                loProcess.MaxWorkingSet = (IntPtr)((int)1024*1024);
                loProcess.MinWorkingSet = (IntPtr)((int)1024*8);
                CUtil.jlogEx("[clearMemory][●catch●]MaxWorkingSet={0} MinWorkingSet={1}", loProcess.MaxWorkingSet, loProcess.MinWorkingSet);
            }
        }

        /// <summary>
        /// 指定程式能使用最多記憶體(Byte)，同時亦清除記憶體
        /// </summary>
        /// <param name="nMaxMemByte"></param>
        public static void setMaxMemory(int nMaxMemByte)
        {
            System.Diagnostics.Process loProcess = System.Diagnostics.Process.GetCurrentProcess();

            try
            {
                loProcess.MinWorkingSet = (IntPtr)(nMaxMemByte/10);
                loProcess.MaxWorkingSet = (IntPtr)(nMaxMemByte);
                CUtil.jlogEx("[setMaxMemory]MaxWorkingSet={0} MinWorkingSet={1}", loProcess.MaxWorkingSet, loProcess.MinWorkingSet);
            }
            catch (System.Exception e)
            {

                //loProcess.MaxWorkingSet = (IntPtr)((int)1024 * 1024);
                //loProcess.MinWorkingSet = (IntPtr)((int)1024 * 8);
                //loProcess.MaxWorkingSet = (IntPtr)(loProcess.MinWorkingSet+1024);
                clearMemory();
                //loProcess.MinWorkingSet = (IntPtr)((int)loProcess.MinWorkingSet * (int)(2));
                CUtil.jlogEx("[setMaxMemory][●catch●]MaxWorkingSet={0} MinWorkingSet={1} [e: {2}]", loProcess.MaxWorkingSet, loProcess.MinWorkingSet,e.ToString());
            }
        }

        public static string getAppilcationFileVersion(string strAppExePath = null)
        {
            string logPathname = null;
            if (strAppExePath == null || strAppExePath.Length < 3)
                logPathname = System.Windows.Forms.Application.ExecutablePath;
            else
                logPathname = strAppExePath;
            //FileVersionInfo.GetVersionInfo(logPathname);
            FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(logPathname);

            return myFileVersionInfo.FileVersion;
            
        }

        public static bool isDateTimeString(string strDateTime)
        {
            try
            {
                DateTime dt = Convert.ToDateTime(strDateTime);
            }
            catch //(Exception e)
            {
                return false;
            }
            return true;
        }
        public static int rand(int nMini = 0, int nMax = 1000)
        {

            return jRand.Next(nMini, nMax);
        }
  
        public static string convertStringToDateOnly(string strDateTime)
        {

            DateTime dtDateTime = convertStringToDateTime(strDateTime);
            string strDayOnly = string.Format("{0}/{1}/{2}", dtDateTime.Year, dtDateTime.Month, dtDateTime.Date);
            return strDayOnly;
        }

        public static string getTodayDateOnly()
        {
            DateTime dtToday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            string strDayOnly = string.Format("{0:0000}/{1:00}/{2:00}", dtToday.Year, dtToday.Month, dtToday.Day);
            return strDayOnly;
        }
        

    }
}
