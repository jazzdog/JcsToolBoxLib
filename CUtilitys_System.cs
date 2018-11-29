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
using System.Security.Cryptography;
using System.Threading;


namespace ToolBoxLib
{
    

    public static partial class CUtil
    {
        private static Random jRand = new Random();
       
        private static string m_strParentPath="";
       
#if DEBUG
        readonly static bool T_DEBUG = true;
#else
         readonly static bool T_DEBUG = false;
#endif

        public static string getCurrentPath()
        {
            //return System.Windows.Forms.Application.StartupPath; //執行程式的路徑;//Directory.GetCurrentDirectory();
            return m_strParentPath;
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


        //public static void jlogEx(string log_string, params Object[] args)
        public static void jlogEx(string strAbsFilePath, string strLogName, string log_string, params Object[] args)
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
            jlog(strAbsFilePath, strLogName,"\n\t\t\t[{0}]\n\t\t\t{1}", strFunction, strMsg);

        }

        private static string makeDaliyLogFileName(string strTheFilePath,string strFileName)
        {
            try
            {
                List<string> theMacs = getMacAddr();
                string strMac="unknowMac";
                if (theMacs.Count>0)
                    strMac = theMacs[0];
                if (isStringValid(strTheFilePath, 1) == false)
                    strTheFilePath = System.Windows.Forms.Application.StartupPath; //執行程式的路徑;//Directory.GetCurrentDirectory();
                string strFullFilePath = "";
                if (isStringValid(strFileName, 1) == false)
                    strFullFilePath = string.Format("{0}\\[{1:0000}_{2:00}_{3:00}]{4}.log", strTheFilePath,
                                                   DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, strMac);
                else
                    strFullFilePath = string.Format("{0}\\[{1:0000}_{2:00}_{3:00}]{4}_{5}.log", strTheFilePath,
                                                       DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, strFileName, strMac);
                return strFullFilePath;
            }
            catch(Exception eee)
            {
                string strEee = eee.ToString();
                return null;

            }
        }
        private static Object LOCKJLOG = new Object();
        public static bool jlog(string strAbsFilePath, string strLogName, string log_string, params Object[] args)
        {
            string strFullFilePath = makeDaliyLogFileName(strAbsFilePath, strLogName);
            string strMsg2Log;
            if (args.Length <= 0)
                strMsg2Log = log_string;
            else
                strMsg2Log = string.Format(log_string, args);

            //if (!Directory.Exists(strAbsFilePath))
            //    Directory.CreateDirectory(strAbsFilePath);

            isFolderExist(strAbsFilePath, true);

            try
            {
               

                
                bool blRes = false;
                bool blFileLock = true;
                lock (LOCKJLOG)
                {
                    
                    while (blFileLock == true)
                    {
                        using (StreamWriter stmwLog = new StreamWriter(strFullFilePath, true))
                        {
                            if (stmwLog == null)
                            {
                                SpinWait.SpinUntil(() => false, 1);
                                Debug.Write($"[jlog]wait...{strMsg2Log}\r\n");
                                continue;
                            }
                            else
                            {
                                stmwLog.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss.ffffff") + "]> " + strMsg2Log);
                                // stmwLog.Flush();
                                //stmwLog.Close();
                                //Debug.Write($"[jlog]OK...{strMsg2Log}\r\n");
                                blRes = true;
                                blFileLock = false;
                            }
                        }
                    }
                }
                
                return blRes;
            }
            catch (System.Exception eee)
            {
                return false;
            }
        }

        public static string downloadDatabyURL(string url)
        {
            using (WebClient client = new WebClient())
            {
                /*
                client.Headers["User-Agent"] =
                    "Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) " +
                    "(compatible; MSIE 6.0; Windows NT 5.1; " +
                    ".NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                */
                // Download data.
                try
                {
                    byte[] arr = client.DownloadData(url);
                    return convertBinaryArrayToB64String(arr);
                }
                catch (Exception eee)
                {
                    return "";
                }
            }

            return "";
        }

        public static bool jlog2( string strAbsFilePath, string strLogName, string log_string, params Object[] args)
        {
            string strFullFilePath = makeDaliyLogFileName(strAbsFilePath, strLogName);
           
            StreamWriter stmwLog = null;
            try
            {
                string strMsg2Log;
                if (args.Length <= 0)
                    strMsg2Log = log_string;
                else
                    strMsg2Log = string.Format(log_string, args);

                if (!Directory.Exists(strAbsFilePath))
                    Directory.CreateDirectory(strAbsFilePath);
                
                int nRetry = 0;


                //while (stmwLog==null && nRetry<10000)
                bool blFileLock = true;
                while (blFileLock == true)
                {
                    SpinWait.SpinUntil(() => false, 1);
                    try
                    {
                        using (File.Open(strFullFilePath, FileMode.Open, FileAccess.Write, FileShare.None))
                        {
                            blFileLock = false;
                            
                        }
                    }
                    catch
                    {
                        blFileLock = true;
                    }
                }

                //while (stmwLog == null)
                //{
                //    SpinWait.SpinUntil(() => false, 1);
                //    stmwLog = new StreamWriter(strFullFilePath, true);
                //    nRetry++;
                //}
                stmwLog = new StreamWriter(strFullFilePath, true);

                bool blRes = false;
                if (stmwLog != null)
                {
                    stmwLog.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss.ffffff") + "]> " + strMsg2Log);
                    stmwLog.Flush();
                    stmwLog.Close();
                    stmwLog = null;
                    blRes = true;
                }
                else
                    blRes= false;
                if (blRes == false)
                    blRes = false;
                return blRes;
            }
            catch (System.Exception eee)
            {
                if (stmwLog != null)
                {
                    stmwLog.Flush();
                    stmwLog.Close();
                    stmwLog = null;

                }
                return false;
            }
            finally
            {
                if (stmwLog != null)
                {
                    stmwLog.Flush();
                    stmwLog.Close();
                    stmwLog = null;

                }
                
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


        public static string getCurrentDateTimeString(string strDateTimeFormate = "yyyy_MM_dd_HHmmss",int nOffSetSec=0)
        {
            string strDateTime = "";
            if (nOffSetSec!=0)
                strDateTime = DateTime.Now.AddSeconds(nOffSetSec).ToString(strDateTimeFormate);
            else
                strDateTime = DateTime.Now.ToString(strDateTimeFormate);
            return strDateTime;
        }
        public static string convertStringToB64String(string inString)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(inString);
            return Convert.ToBase64String(bytes);
        }

        public static DateTime convertStringToDateTime(string strDateTime,string strDateTimeFormate = "yyyy/MM/dd HH:mm")
        {
            if (strDateTimeFormate == null)
                return Convert.ToDateTime(strDateTime);
            else
            {
                DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();
                dtFormat.FullDateTimePattern = strDateTimeFormate;
                //dtFormat.ShortDatePattern = strDateTimeFormate;

                try
                {
                    return DateTime.ParseExact(strDateTime, strDateTimeFormate, CultureInfo.InvariantCulture);
                    //return Convert.ToDateTime(strDateTime, dtFormat);
                }
                catch (Exception eee)
                {
                    return Convert.ToDateTime(strDateTime);
                }
            }
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
                //CUtil.jlogEx("[clearMemory]MaxWorkingSet={0} MinWorkingSet={1}", loProcess.MaxWorkingSet, loProcess.MinWorkingSet);
            }
            catch (System.Exception)
            {

                loProcess.MaxWorkingSet = (IntPtr)((int)1024*1024);
                loProcess.MinWorkingSet = (IntPtr)((int)1024*8);
                //CUtil.jlogEx("[clearMemory][●catch●]MaxWorkingSet={0} MinWorkingSet={1}", loProcess.MaxWorkingSet, loProcess.MinWorkingSet);
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
                //CUtil.jlogEx("[setMaxMemory]MaxWorkingSet={0} MinWorkingSet={1}", loProcess.MaxWorkingSet, loProcess.MinWorkingSet);
            }
            catch (System.Exception e)
            {

                //loProcess.MaxWorkingSet = (IntPtr)((int)1024 * 1024);
                //loProcess.MinWorkingSet = (IntPtr)((int)1024 * 8);
                //loProcess.MaxWorkingSet = (IntPtr)(loProcess.MinWorkingSet+1024);
                clearMemory();
                //loProcess.MinWorkingSet = (IntPtr)((int)loProcess.MinWorkingSet * (int)(2));
                //CUtil.jlogEx("[setMaxMemory][●catch●]MaxWorkingSet={0} MinWorkingSet={1} [e: {2}]", loProcess.MaxWorkingSet, loProcess.MinWorkingSet,e.ToString());
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

        public static double getDateTimeinMillisec(string strDateTime="", string strDateTimeFormate = "yyyy/MM/dd HH:mm")
        {
            DateTime theDateTime;
            //if (isStringValid(strDateTime, 8) == true)
            if (isDateTimeString(strDateTime) == true)
                theDateTime = convertStringToDateTime(strDateTime, strDateTimeFormate);
            else
                theDateTime = DateTime.Now;
            //double theTime = (int)(theDateTime.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
            double theTime = (int)(theDateTime.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;
            return theTime;
        }

        public static double getDateTimeinMillisec(DateTime theDt)
        {
            DateTime theDateTime;
            //if (isStringValid(strDateTime, 8) == true)
            if (theDt!= null)
                theDateTime = theDt;
            else
                theDateTime = DateTime.Now;
            double theTime =(double)(theDateTime.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;
            return theTime;
        }

        public static DateTime convertMillisec2DateTime(double dbDatetimeInMillisec)
        {
            DateTime dt0 = new DateTime(1970, 1, 1);
            DateTime dtfommls = dt0.AddMilliseconds(dbDatetimeInMillisec);

            return dtfommls.ToLocalTime();
        }

        public static DateTime convertSec2DateTime(double dbDatetimeInSec)
        {
            DateTime dt0 = new DateTime(1970, 1, 1);
            DateTime dtfommls = dt0.AddMilliseconds(dbDatetimeInSec * 1000);

            return dtfommls.ToLocalTime();
        }

        public static string makeMD5(string strInput)
        {
            MD5 md5 = MD5.Create();//建立一個MD5
            // byte[] source = Encoding.Default.GetBytes(strInput);//將字串轉為Byte[]
            //byte[] crypto = md5.ComputeHash(source);//進行MD5加密
            //string result = Convert.ToBase64String(crypto);//把加密後的字串從Byte[]轉為字串
            byte[] bresult = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(strInput));
            string result = "";
            for (int i = 0; i < bresult.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                string strTheCode = bresult[i].ToString("x");
                if (strTheCode.Length <= 1)
                    strTheCode = "0" + strTheCode;
                result = result + strTheCode;
            }
            //string result = System.Text.Encoding.Default.GetString(bresult);
            return result;
        }

    }
}
