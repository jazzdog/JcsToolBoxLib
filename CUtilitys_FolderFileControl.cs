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
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Threading;


namespace ToolBoxLib
{


    public static partial class CUtil
    {

        public static bool saveFile(string strFolderPath, string strFileName,byte[] btArray)
        {
            isFolderExist(strFolderPath, true);
            string strFileFullPath= strFolderPath+ strFileName;
            File.WriteAllBytes(strFileFullPath, btArray); // Requires System.IO
            return isFile(strFileFullPath);
        }

        public static bool saveFile(string strFolderPath, string strFileName, string strB64)
        {
            string incoming = strB64.Replace('_', '/').Replace('-', '+');
            switch (strB64.Length % 4)
            {
                case 2: incoming += "=="; break;
                case 3: incoming += "="; break;
            }

            byte[] data = Convert.FromBase64String(incoming);
            return saveFile(strFolderPath, strFileName, data);
        }

        public static bool checkDirectory(string strFolderPath,bool blCreateDirectory=true)
        {
            if (!Directory.Exists(strFolderPath))
            {
                ///沒有要建立資料夾
                if (blCreateDirectory == false)
                    return false;

                try
                {
                    Directory.CreateDirectory(strFolderPath);
                    if (!Directory.Exists(strFolderPath))
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    //CUtil.jlogEx("[checkDirectory][Err][{0}]", ex.ToString());
                    return false;
                }
            }
            else
            {
                ///有資料夾
                return true;
            }
        }

        

        public static bool FolderMove(string strPathSource, string strPathTarget)
        {
            try
            {
                Directory.Move(strPathSource, strPathTarget);
                return true;
            }
            catch (IOException eMsg)
            {

                //CUtil.jlogEx("搬移檔案錯誤:\n{0}", eMsg.ToString());
                return false;
            }
        }

        /// <summary>
        /// 找到路徑最後的元件，不管是資料夾或者檔名
        /// 例如: 
        /// C:\\....\\DirectoryA
        /// C:\\....\\DirectoryA\\ ==>自動除去最後的斜線
        /// C:\\....\\DirectoryA\\FileB.txt
        /// 亦可
        /// </summary>
        /// <param name="strFullPath">路徑字串</param>
        /// <returns>最後元件字串</returns>
        public static string findPathLastItem(string strFullPath)
        {


            int nLastSlash = strFullPath.LastIndexOf("\\");
            int nStringLength = strFullPath.Length;
            if (nStringLength <= 0)
                return "";

            ///若路徑最後為"\\"則自動去除
            if (strFullPath.Length == nLastSlash+1)
            {
                string strDummy = strFullPath;
                strDummy = strDummy.Remove(nLastSlash, 1);
                nLastSlash = strDummy.LastIndexOf("\\");
                nStringLength = strDummy.Length;
            }
            try
            {
                string strlastItem = strFullPath.Substring(nLastSlash + 1, nStringLength - 1 - nLastSlash);
                return strlastItem;
            }
            catch (Exception e)
            {
                //CUtil.jlogEx("[Uitl-Err][{0}]",e.ToString());
                return "";
            }

        }

       /// <summary>
       /// 取得輸入路徑的上一層資料夾路徑
       /// 用在取得檔案路徑的所在目錄
       /// </summary>
       /// <param name="strFullPath"></param>
       /// <returns></returns>
        public static string findUpperPath(string strFullPath)
        {

            int nLastSlash = strFullPath.LastIndexOf("\\");
            int nStringLength = strFullPath.Length;
            if (nStringLength <= 0)
                return "";

            ///若路徑最後為"\\"則自動去除
            if (strFullPath.Length == nLastSlash + 1)
            {
                string strDummy = strFullPath;
                strDummy = strDummy.Remove(nLastSlash, 1);
                nLastSlash = strDummy.LastIndexOf("\\");
                nStringLength = strDummy.Length;
            }


            try
            {
                string strlastItem = strFullPath.Substring(0, nLastSlash);
                return strlastItem;
            }
            catch (Exception e)
            {
               // CUtil.jlogEx("[Uitl-Err][{0}]", e.ToString());
                return "";
            }

        }

        public static bool FileRename(string strPathSource, string strNewFileName)
        {

            string strFolderUpperPath = findUpperPath(strPathSource);
            if (strFolderUpperPath == "" || strFolderUpperPath.Length <= 1)
            {
                CUtil.jlogEx("搬移檔案錯誤:[{0}]=>上層檔案錯誤[{1}]", strPathSource, strFolderUpperPath);
                return false;
            }
            string strTargetPath = strFolderUpperPath + "\\" + strNewFileName;
            try
            {

                File.Move(strPathSource, strTargetPath);
                return true;
            }
            catch (IOException eMsg)
            {

                CUtil.jlogEx("檔案更名錯誤:[{0}]=>[{1}]\nerr:{2}", strPathSource, strTargetPath, eMsg.ToString());
                return false;
            }
        }

        public static bool FolderRename(string strPathSource, string strNewFolderName)
        {

            string strFolderUpperPath = findUpperPath(strPathSource);
            if (strFolderUpperPath == "" || strFolderUpperPath.Length <= 1)
            {
                CUtil.jlogEx("搬移檔案錯誤:[{0}]=>上層檔案錯誤[{1}]", strPathSource, strFolderUpperPath);
                return false;
            }
            string strTargetPath = strFolderUpperPath + "\\" + strNewFolderName;
            try
            {
                
                Directory.Move(strPathSource, strTargetPath);
                return true;
            }
            catch (IOException eMsg)
            {

                CUtil.jlogEx("搬移檔案錯誤:[{0}]=>[{1}]\nerr:{2}",strPathSource,strTargetPath, eMsg.ToString());
                return false;
            }
        }

        public static bool clearDirectory(string strPath)
        {
            if (isFolder(strPath) == false)
            {
                //CUtil.jlogEx("[clearDirectory]Error: invalid Folder Path:{0}",strPath);
                return false;
            }

            System.IO.DirectoryInfo di = new DirectoryInfo(strPath);
            

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
            return true;
        }

        public static bool deleteDirectory(string strPath)
        {
            if (isFolder(strPath) == false)
            {
                //CUtil.jlogEx("[deleteDirectory]Error: invalid Folder Path:{0}", strPath);
                return false;
            }

            System.IO.DirectoryInfo di = new DirectoryInfo(strPath);


            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                try
                {
                    dir.Delete(true);
                }
                catch (IOException ioExc)
                {
                    //CUtil.jlogEx("[deleteDirectory]Error:{0}", ioExc.ToString());
                }
            }

            try
            {
                di.Delete();
            }
            catch (IOException ioExc)
            {
                //CUtil.jlogEx("[deleteDirectory]Error:{0}", ioExc.ToString());
                return false;
            }
            return true;
        }

        public static bool isFolder(string strPath)
        {
            // get the file attributes for file or directory
            FileAttributes attr = File.GetAttributes(strPath);

            //detect whether its a directory or file
            if (attr.HasFlag(FileAttributes.Directory))
                return true;
            else
                return false;
        }

        public static long getFileSizeByte(string strPath)
        {
            if (isFile(strPath) == false)
                return -1;

            System.IO.FileInfo fileMediaInfo = new System.IO.FileInfo(strPath);
            return fileMediaInfo.Length;
        }

        public static int delFileinFolder(string strPathFolder, string strPartialFileName="")
        {
            int nFindFileCount = -1;
            try
            {

                DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(strPathFolder);
                FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + strPartialFileName + "*.*");
                nFindFileCount = filesInDir.Count();
               
                foreach (FileInfo foundFile in filesInDir)
                {
                    string fullName = foundFile.FullName;

                    CUtil.delFile(fullName);
                }
                return nFindFileCount;
            }
            catch (Exception ee)
            {
                CDebug.jmsg("[delFileinFolder]:[{0}]:[{1}->{2}]", ee.ToString(), strPathFolder, strPartialFileName);
                return -1;
                
            }
           
        }

        public static bool delFile(string strPath)
        {
            if (isFile(strPath) == false)
                return false;
            try
            {
                File.Delete(strPath);
            }
            catch //(Exception e)
            {
                return false;
            }
            if (isFile(strPath) == false)
                return true;///確認檔案已經被刪除
            else
                return false;


        }

        public static bool isFile(string strPath, int nMinFileSize=-1)
        {
            if (File.Exists(strPath) == false)
                return false;
            // get the file attributes for file or directory
            FileAttributes attr = File.GetAttributes(strPath);

            //detect whether its a directory or file
            if (attr.HasFlag(FileAttributes.Directory) == false)
            {
                ///set minSize
                if ((nMinFileSize > 0)&&(getFileSizeByte(strPath)<nMinFileSize))
                    return false;    
                else
                    return true;
                
            }
            else
                return false;
        }
    }
}
