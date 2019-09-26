using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices; //DllImport
using System.Security.Principal;//WindowsIdentity 


namespace ToolBoxLib
{
    public static partial class CUtil
    {

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(
        string lpszUsername,
        string lpszDomain,
        string lpszPassword,
        int dwLogonType,
        int dwLogonProvider,
        ref IntPtr phToken);

        //登出
        [DllImport("kernel32.dll")]
        public extern static bool CloseHandle(IntPtr hToken);

        public static IntPtr connect_Neighborhood(string ip, string user, string psw)
        {
            string MachineName = ip;// "10.100.82.156";
            string UserName = user;// "administrator";
            string Pw = psw;// "86924447@Secom.com";
            /*
             * // logon providers
                const int LOGON32_PROVIDER_DEFAULT = 0;
                const int LOGON32_PROVIDER_WINNT50 = 3;
                const int LOGON32_PROVIDER_WINNT40 = 2;
                const int LOGON32_PROVIDER_WINNT35 = 1;
             */
            const int LOGON32_PROVIDER_DEFAULT = 0;

            /*
             * // logon types
                const int LOGON32_LOGON_INTERACTIVE = 2; //電腦存在於網域
                const int LOGON32_LOGON_NETWORK = 3;
                const int LOGON32_LOGON_NEW_CREDENTIALS = 9; //直接存取網路芳鄰
             */
            const int LOGON32_LOGON_NEW_CREDENTIALS = 9;


            IntPtr hToken = new IntPtr(0);
            hToken = IntPtr.Zero;

            bool returnValue = LogonUser(UserName, MachineName, Pw,
            LOGON32_LOGON_NEW_CREDENTIALS,
            LOGON32_PROVIDER_DEFAULT,
            ref hToken);

            WindowsIdentity w = new WindowsIdentity(hToken);
            w.Impersonate();
            if (false == returnValue)
            {
                hToken = IntPtr.Zero;
            }
            return hToken; // if(ip != IntPtr.Zero)
        }

        public static bool disconnect_Neighborhood(IntPtr hToken)
        {
            return CloseHandle(hToken);
        }
    }
}
