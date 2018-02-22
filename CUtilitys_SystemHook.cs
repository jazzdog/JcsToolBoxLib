using System;
using System.Diagnostics;
using System.Windows.Forms; //Keys
using System.Runtime.InteropServices;


namespace ToolBoxLib
{
    [StructLayout(LayoutKind.Explicit)]
    public struct KBLLHOOKSTRUCT
    {
        [FieldOffset(0)]
        public UInt16  wVk;
        [FieldOffset(2)]
        public UInt16 wScan;
        [FieldOffset(4)]
        public UInt32 dwFlags;
        [FieldOffset(4)]
        public UInt32 time;
        [FieldOffset(32)]
        UIntPtr dwExtraInfo;
    }
    [StructLayout(LayoutKind.Explicit)]
    public struct MSLLHOOKSTRUCT
    {
        [FieldOffset(0)]
        public POINT pt;
        [FieldOffset(0)]
        public Int32 X;
        [FieldOffset(4)]
        public Int32 Y;

        [Obsolete("use MouseData")]
        [FieldOffset(8)]
        public Int64 _mouseData;//Not correct

        [FieldOffset(10)]
        public Int16 MouseData;//Not shure

        [FieldOffset(16)]
        public Int64 flags;//Not shure
        /// <summary>
        /// Equals Environment.TickCount
        /// </summary>
        [FieldOffset(16)]
        public Int32 Time; //Environment.TickCount;

        [Obsolete("use Time")]
        [FieldOffset(24)]
        public Int64 _time;//Not correct

        [FieldOffset(32)]
        public UIntPtr dwExtraInfo;//Not shure
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public Int32 x;
        public Int32 y;
    }

    public delegate int HookDelegateProc(int code, IntPtr wParam, IntPtr lParam);
    
    
    /*
     * IDisposable 由於使用 unmanage 所以需要IDisposable與SuppressFinalize
     */
    public class cSystemHook : IDisposable
    {
        [DllImport("User32.dll")]
        static extern IntPtr SetWindowsHookEx(HookType idHookType, HookDelegateProc lpfn, IntPtr hmod, Int32 dwThreadId);
        [DllImport("User32.dll")]
        //public static extern IntPtr CallNextHookEx(IntPtr hHook, int nCode, IntPtr wParam, IntPtr lParam);
        static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern short GetKeyState(VirtualKeys nVirtKey);
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


        event EventHandler<HookEventArgs> evnHook=null;
        private IntPtr hookHandle = IntPtr.Zero;
        public Keys key = Keys.None;
        private HookType m_hookType=HookType.WH_NONE; //目前偵測的裝置行台
        private HookDelegateProc m_hookProc=null;
        private bool _bCallNext=true; ///是否將HOOK資訊傳遞給系統


        bool CallNextProc
        {
            get { return _bCallNext; }
            set { _bCallNext = value; }
        }

        bool isHooking
        {
            get { return _isHooking(); }
           
        }

        /// <summary>
        /// 設定HOOK型態
        /// </summary>
        /// <param name="hookDevice">HookType型態的HOOK類型</param>
        /// <param name="_evnHook">處理回傳訊息函式 void calbckHook(object sender, HookEventArgs strRequsetResult)</param>
        /// <param name="blCallNextProc">是否要將HOOK資訊攔截(預設:false)</param>
        public cSystemHook(HookType hookDevice, EventHandler<HookEventArgs> _evnHook, bool blBlockHookMessage =false)
        {
            _bCallNext = !blBlockHookMessage;
            m_hookType = hookDevice;
            evnHook += _evnHook;
            m_hookProc = new HookDelegateProc(procHookfnc);
          
        }

        bool _isHooking()
        {
            if ((hookHandle == IntPtr.Zero) && (m_hookType != HookType.WH_NONE))
                return false;
            else
                return true;
        }

        public void startHook()
        {
            if (_isHooking() == false)
            {
                using (Process curProcess = Process.GetCurrentProcess())
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    //全系統監控HOOK
                    if (m_hookType == HookType.WH_MOUSE_LL || m_hookType == HookType.WH_KEYBOARD_LL)
                        hookHandle = SetWindowsHookEx(m_hookType, m_hookProc, GetModuleHandle(curModule.ModuleName), 0);
                    //指監控HOOK此應用程式本身
                    else
                        hookHandle = SetWindowsHookEx(m_hookType, m_hookProc, GetModuleHandle(curModule.ModuleName), CUtil.GetCurrentThreadId());
                }
            }
        }

        protected int procHookfnc(int code, IntPtr wParam, IntPtr lParam)
        {

            switch ((int)m_hookType)
            {
                case (int)HookType.WH_MOUSE:
                case (int)HookType.WH_MOUSE_LL:
                    procHookMouse(code, wParam, lParam);
                    break;
                case (int)HookType.WH_KEYBOARD:
                case (int)HookType.WH_KEYBOARD_LL:
                    procHookKeyboard(code, wParam, lParam);
                    break;
                default:
                    CallProcHookProcedure(code, wParam, lParam);
                    break;
            }


            if (CallNextProc == true)
            {
                //CDebug.jmsg("CallNextHookEx...");
                return CallNextHookEx(hookHandle, code, wParam, lParam);
            }
            else
                return -1;
        }
      

        protected void procHookMouse(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code != 0) //HC_ACTION(0) ; 
            {
                return ;
            }

            if (evnHook != null)
            {
                HookEventArgs eventArgs = new HookEventArgs();
                eventArgs.type = m_hookType;
                eventArgs.key = key;
                eventArgs.wParam = wParam; // virtual-key code 
                eventArgs.lParam = lParam;
                eventArgs.HookCode = code;
                eventArgs.WMessages = (WindowsMessages)wParam;
                MSLLHOOKSTRUCT theMouseInfo = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                eventArgs.nX = theMouseInfo.X;
                eventArgs.nY = theMouseInfo.Y;
                //CDebug.jmsg("[Mouse][0] X:{1} Y:{2}=>{3} ", eventArgs.WMessages.ToString(), eventArgs.nX, eventArgs.nY, theMouseInfo.Time);

                evnHook(this, eventArgs);
            }
            return;

        }
      

      
        protected void procHookKeyboard(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code != 0)
            {
                return;
            }
            if (evnHook != null)
            {
                Keys key = (Keys)lParam.ToInt32();
                HookEventArgs eventArgs = new HookEventArgs();
                eventArgs.type = m_hookType;
                eventArgs.lParam = lParam;
                eventArgs.wParam = wParam;
                eventArgs.HookCode = code;

                eventArgs.WMessages = (WindowsMessages)wParam;
                eventArgs.bAltKey = GetKeyState(VirtualKeys.VK_MENU) <= -127;
                eventArgs.bCtrlKey = GetKeyState(VirtualKeys.VK_CONTROL) <= -127;
                KBLLHOOKSTRUCT theKeyBordInfo = (KBLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBLLHOOKSTRUCT));
                eventArgs.key = (Keys)theKeyBordInfo.wVk;
                CDebug.jmsg("[Keyboard][{0}] X:{1} Y:{2}=>{3} ", eventArgs.WMessages.ToString(), eventArgs.bAltKey, eventArgs.bCtrlKey,(Keys) theKeyBordInfo.wVk);
                evnHook(this, eventArgs);
            }
            return;
        }

        protected void CallProcHookProcedure(int code, IntPtr wParam, IntPtr lParam)
        {
          
            if (evnHook != null)
            {
                HookEventArgs eventArgs = new HookEventArgs();
                eventArgs.type = m_hookType;
                eventArgs.lParam = lParam;
                eventArgs.wParam = wParam;
                eventArgs.HookCode = code;
                CDebug.jmsg("[OtherHook] lParam:{0} wParam:{1} HookCode:{2}", eventArgs.lParam, eventArgs.wParam, eventArgs.HookCode);
                evnHook(this, eventArgs);
            }
            return;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            bool ret = false;
            if (isHooking == true)
            {
                ret=UnhookWindowsHookEx(hookHandle);

                hookHandle = IntPtr.Zero;
                if (ret == false)
                    CDebug.jmsg("呼叫 UnhookWindowsHookEx 失敗!");
            }
        }

        ~cSystemHook()
        {
            Dispose(false);
        }

    }


}
