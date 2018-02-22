using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace ToolBoxLib
{

    public static class ExtensionMethods
    {
        public static long LowWord(this int number)
        { return number & 0x0000FFFF; }
        public static long LowWord(this int number, int newValue)
        { return (number & 0xFFFF0000) + (newValue & 0x0000FFFF); }
        public static long HighWord(this int number)
        { return number & 0xFFFF0000; }
        public static long HighWord(this int number, int newValue)
        { return (number & 0x0000FFFF) + (newValue << 16); }
    }

    public class HookEventArgs : EventArgs
    {
        public HookType type = HookType.WH_NONE;
        public int HookCode=-1;
        public IntPtr wParam = IntPtr.Zero;
        public IntPtr lParam = IntPtr.Zero;
        //[Keyboard]
        public Keys key=Keys.None;
        public bool bAltKey =false;
        public bool bCtrlKey=false;
        //[Mouse]
        public int nX=-1;
        public int nY=-1;
        public WindowsMessages WMessages=WindowsMessages.WM_NULL;
    }

    public enum VirtualKeys
    {
        VK_SHIFT = 0x10,
        VK_CONTROL = 0x11,
        VK_MENU = 0x12,    //ALT
        VK_PAUSE = 0x13,
        VK_CAPITAL = 0x14
    }

    public enum WindowsMessages
    {
        WM_NULL = 0,
        WM_KEYDOWN = 256,
        WM_KEYUP=257,
        WM_MOUSEMOVE= 512,//0x200
        WM_LBUTTONDOWN = 513,
        WM_LBUTTONUP = 514,
        WM_LBUTTONDBLCLK= 515,
        WM_RBUTTONDOWN = 516,
        WM_RBUTTONUP = 517,
        WM_RBUTTONDBLCLK =518,
        WM_MBUTTONDOWN=519,
        WM_MBUTTONUP = 520,
        WM_MBUTTONDBLCLK=521,
        WM_MOUSEWHEEL = 522,



    }

    public enum MouseEventFlag : uint //滑鼠動作
    {
        Move = 0x0001,               
        LeftDown = 0x0002,           
        LeftUp = 0x0004,             
        RightDown = 0x0008,          
        RightUp = 0x0010,            
        MiddleDown = 0x0020,         
        MiddleUp = 0x0040,           
        XDown = 0x0080,
        XUp = 0x0100,
        Wheel = 0x0800,              
        VirtualDesk = 0x4000,        
        Absolute = 0x8000
    }
    public enum HookType : int
    {
        WH_NONE = -1,
        WH_JOURNALRECORD = 0,
        WH_JOURNALPLAYBACK = 1,
        WH_KEYBOARD = 2,
        WH_GETMESSAGE = 3,
        WH_CALLWNDPROC = 4,
        WH_CBT = 5,
        WH_SYSMSGFILTER = 6,
        WH_MOUSE = 7,
        WH_HARDWARE = 8,
        WH_DEBUG = 9,
        WH_SHELL = 10,
        WH_FOREGROUNDIDLE = 11,
        WH_CALLWNDPROCRET = 12,
        WH_KEYBOARD_LL = 13,
        WH_MOUSE_LL = 14
    }

    public static partial class CUtil
    {
        #region #####Mouse#####
        

        [STAThread]
        [DllImport("User32")]
        public extern static void mouse_event(int dwFlags, int dx, int dy, int dwData, IntPtr dwExtraInfo);
        [DllImport("User32")]
        public extern static void SetCursorPos(int x, int y);
        [DllImport("User32")]
        public extern static bool GetCursorPos(out  Point p);
        [DllImport("User32")]
        public extern static int ShowCursor(bool bShow);
        #endregion

        #region #####Window#####
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        #endregion

        #region #####Keyboard#####
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);
        #endregion

        #region #####Art#####

        //[DllImport("shell32.dll")] ///Retrieves information about an object in the file system, such as a file, folder, directory, or drive root.
        //public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref Art.GetIncoIntPtrStruct.SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        //[DllImport("user32.dll")]
        //public static extern bool ClientToScreen(IntPtr hWnd, ref Art.DismantleElmentStruct.POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        #endregion

        #region #####Device#####
        //[DllImport("User32.dll")]
        //public static extern bool GetLastInputInfo(ref Device.LASTINPUTINFO info);
        #endregion

        #region #####Hook#####

        //===========================================
        //copy from public class WindowsHookHelper
        [DllImport("kernel32.dll")]
            public static extern int GetCurrentThreadId();

       

        //[DllImport("User32.dll")]
        //public static extern IntPtr CallNextHookEx(
        //    IntPtr hHook, Int32 nCode, IntPtr wParam, IntPtr lParam);

        //[DllImport("User32.dll")]
        //public static extern IntPtr UnhookWindowsHookEx(IntPtr hHook);

        //=====================================================================


        //[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //public static extern int SetWindowsHookEx(int idHook, NativeStructs.HookDelegateProc lpfn, IntPtr hInstance, int threadId);
       

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

        #endregion

        #region #####Window#####
        [DllImport("user32.dll")]
        public static extern int GetForegroundWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(int hWnd, String str, Int32 count);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(int hWnd, string lpString);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(int hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point Point);
        [DllImport("user32.dll")]
        public static extern int GetFocus();
        #endregion

        #region #####Input Method Editor#####
        [DllImport("Imm32.dll")]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);
        [DllImport("Imm32.dll")]
        public static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);
        [DllImport("Imm32.dll")]
        public static extern Boolean ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);
        #endregion

        [DllImport("user32")]
        public static extern int GetDoubleClickTime();
        [DllImport("user32")]
        public static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);
        [DllImport("user32")]
        public static extern int GetKeyboardState(byte[] pbKeyState);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
    }
}
