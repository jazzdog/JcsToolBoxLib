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
using System.Web.Script.Serialization;


namespace ToolBoxLib
{


    public static partial class CUtil
    {

        public static bool saveFile(string strFolderPath, string strFileName, byte[] btArray)
        {
            isFolderExist(strFolderPath, true);
            string strFileFullPath = strFolderPath + strFileName;
            File.WriteAllBytes(strFileFullPath, btArray); // Requires System.IO
            return isFile(strFileFullPath);
        }

        public static bool convertFileToByteArray(string strFileFullPath, out byte[] btBuffer)
        {
            if (isFile(strFileFullPath) == true)
            {
                var fileStream = new FileStream(strFileFullPath, FileMode.Open, FileAccess.Read);
                btBuffer = new byte[fileStream.Length];
                fileStream.Read(btBuffer, 0, btBuffer.Length);
                return true;
            }
            else
            {
                btBuffer = null;
                return false;
            }

        }

        public static string convertFileToB64String(string strFileFullPath)
        {
            if (isFile(strFileFullPath) == true)
            {
                byte[] fileBuffer;
                if (convertFileToByteArray(strFileFullPath, out fileBuffer) == true)
                    return convertBinaryArrayToB64String(fileBuffer);
                else
                    return "";
            }
            else
            {
                return "";
            }

        }

        public static void clearConcurrentBag<T>(ConcurrentBag<T> theconBag)
        {
            while (theconBag.Count > 0)
            {
                T element;
                theconBag.TryTake(out element);
            }
        }

        public static bool copyList<T>(List<T> sourceList, out List<T> outList)
        {
            try
            {
                outList = new List<T>(sourceList);
                return true;
            }
            catch
            {
                outList = null;
                return false;
            }
        }
        public static void clearList<listType>(List<listType> theListToClear)
        {
            theListToClear.Clear();
            theListToClear.Capacity = 0;
            theListToClear.TrimExcess();
        }

        public static bool copyDictionary<TKey, TValue>(Dictionary<TKey, TValue> sourceDic, out Dictionary<TKey, TValue> outDic)
        {
            try
            {
                outDic = new Dictionary<TKey, TValue>();
                foreach (KeyValuePair<TKey, TValue> _pair in sourceDic)
                {
                    outDic.Add(_pair.Key, _pair.Value);
                }
                return true;
            }
            catch
            {
                outDic = null;
                return false;
            }
        }

        public static bool copyArray<T>(T[] sourceArray, out T[] outputArray, int nCopyLength = -1)
        {
            try
            {
                if (nCopyLength < 0)
                    nCopyLength = sourceArray.Length;
                outputArray = resetArray<T>(nCopyLength, true);
                Array.Copy(sourceArray, outputArray, nCopyLength);
                return true;
            }
            catch (Exception eee)
            {
                outputArray = null;
                return false;
            }
        }


        public static T[] resetArray<T>(int nSize, bool blClear = false)
        {
            try
            {
                T[] _array = new T[1];
                Array.Resize<T>(ref _array, nSize);
                if(blClear==true)
                    Array.Clear(_array, 0, _array.Length);
                return _array;
            }
            catch (Exception eee)
            {
                return null;
            }

            
        }

        public static string convertBinaryArrayToB64String(byte[] theData)
        {
            string strDataBase64 = Convert.ToBase64String(theData);
            return strDataBase64; //for debug breakpoint 

        }

        public static string convertClassToJSONString(Object theClass)
        {
            var theJ = new JavaScriptSerializer();
            theJ.MaxJsonLength = 2147483647;
            string strJSONString = theJ.Serialize(theClass);
            return strJSONString;
        }

        public static string convertDictionaryToJSONString<T1, T2>(Dictionary<T1, T2> dict)
        {
            var entries = dict.Select(d =>
                string.Format("\"{0}\": {1}", d.Key, string.Join(",", d.Value)));
            return "{" + string.Join(",", entries) + "}";
        }
        public static System.Drawing.Image covnertB64String2Image(string strB64Image)
        {
            byte[] theImagebyte;
            convertB64StringToBinaryArray(strB64Image, out theImagebyte);
            return convertByteArrayToImage(theImagebyte);
        }

        public static bool convertB64StringToBinaryArray(string strB64,out byte[] theData)
        {
            string incoming = strB64.Replace('_', '/').Replace('-', '+');
            switch (strB64.Length % 4)
            {
                case 2: incoming += "=="; break;
                case 3: incoming += "="; break;
            }

            theData = Convert.FromBase64String(incoming);

            if (theData?.Length > 0)
                return true;
            else
                return false;

        }

        public static bool saveFile(string strFolderPath, string strFileName, string strB64)
        {

            byte[] data;
            if (convertB64StringToBinaryArray(strB64, out data))
                return saveFile(strFolderPath, strFileName, data);
            else
                return false;
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
