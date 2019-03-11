using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace ToolBoxLib
{
    /*Example:
     *    [Declare]
                static CProcEventTask m_procMainEventTask=null; //[對內]處理各頁面的事件
                public static CProcEventTask mainEventProc  ///(static)事件處理handle 對外任何頁面都可以取(read only)
                {
                    get
                    {
                        if (m_procMainEventTask == null)
                        {
                            m_procMainEventTask = new CProcEventTask();
                        }
                        return m_procMainEventTask;
                    }
                }

                /// 事件主代碼
                public enum eEcode
                {
                    DEF_NONE=0,
                    EVENT_SYS,
                    EVENT_SYS_ERR,
                }
                /// 事件子代碼
                public enum eSubEcode
                {
                    _DEF_NONE = 0,
                    _EVENT_SYS_WHAT_HAPPENT,
                    _EVENT_SYS_ERR_SOMETHING_WRONE,
                }
            [Init]
                void initRegEventProc() //建立事件代碼對應表
                {
                    mainEventProc.buildProcFuncs((int)eEcode.EVENT_CALL, procEVENT_CALL);
                    mainEventProc.buildProcFuncs((int)eEcode.EVENT_REPLY, procEVENT_REPLY);
                    mainEventProc.buildProcFuncs((int)eEcode.SYS_ERR, procEvent_SYS_ERR);
                    mainEventProc.doRun();  //啟動
                }
            [Process]
                 void procEVENT_CALL(CEventInfo cInfo)
                {
                    //根據子代號處理
                    switch ((eSubEcode)cInfo.nEventSubCode)
                    {
                        case eSubEcode._EVENT_CALL_GET_NAMELIST: 
                            send_GET_NAMELIST();
                            break;
                    }
                }
            [Add Task]
                private void bt_getNameList(object sender, EventArgs e)
                {

                    CEventInfo eventInfo = new CEventInfo((int)eEcode.EVENT_CALL, (int)eSubEcode._EVENT_CALL_GET_NAMELIST);
                    eventInfo.objExtra = eEcode.EVENT_CALL;
                    mainEventProc.addTask(eventInfo); //發動事件
                }

     */
    public delegate void InvokeDelegate(CEventInfo cInfo);
    /// <summary>
    /// 將事件加入列表的界面
    /// </summary>
    /// <param name="cTaskInof"></param>
    /// <returns></returns>
    public delegate int InvokeDelegateAddEvent(CEventInfo cTaskInof);

    /// <summary>
    /// 給listEventByteData事件處理時,傳遞的資料結構類別
    /// 主要以事件代碼，子碼來傳遞參數
    /// 會自動記錄事件資料建立時間(亦可修改)
    /// 另外亦可額外傳遞:binary,object,以及String資料
    /// </summary>
    public class CEventInfo
    {
        /// <summary>
        /// 事件代號
        /// </summary>
        public UInt32 nEventCode;
        /// <summary>
        /// 事件子代號
        /// </summary>
        public UInt32 nEventSubCode;
        /// <summary>
        /// [額外]事件binary資料=>
        /// listEventByteData.Add(theBinaryData);
        /// </summary>
        public List<byte[]> listEventByteData = new List<byte[]>();

        public List<object> listEventObject = new List<Object>();
        /// <summary>
        /// 自動填入目前時間，亦可修改
        /// </summary>
        public DateTime tEventTime = DateTime.Now;

        /// <summary>
        /// [額外]物件資料
        /// </summary>
        public Object objExtra=null;

        /// <summary>
        /// [額外]字串資料
        /// </summary>
        public String strInfo;

        /// <summary>
        /// 建立傳遞資料結構
        /// </summary>
        /// <param name="_nEventCode">事件主碼</param>
        /// <param name="_nEventSubCode">事件子碼</param>
        public CEventInfo(UInt32 _nEventCode, UInt32 _nEventSubCode = 0)
        {
            nEventCode = _nEventCode;
            nEventSubCode = _nEventSubCode;
            listEventObject = new List<object>();
            listEventByteData = new List<byte[]>();
        }
    }

    public class CProcEventTask
    {

        
        public bool doStop() { m_stop = true; return m_stop; }
        public bool IsAlive { get { return thprocEventLoop.IsAlive; } private set { return; } }
        public InvokeDelegateAddEvent delgAddEvent;

        private List<CEventInfo> listEventData = new List<CEventInfo>();
        public int eventLeftCount { get { return listEventData.Count; } private set { return; } }
        
        private List<Thread> listProcThread = new List<Thread>();
        private Dictionary<UInt32, InvokeDelegate> listProcFuncitons = new Dictionary<UInt32, InvokeDelegate>();
        private volatile bool m_stop = true;
        private int nMaxEvenCont = 5000;
        private Thread thMonitlistEventDataFull;
        private Thread thprocEventLoop;



        public UInt32 buildProcFuncs(UInt32 nEventCode, InvokeDelegate _fnProc)
        {
            if (!listProcFuncitons.ContainsKey(nEventCode))
            {
                listProcFuncitons.Add(nEventCode, _fnProc);
                return nEventCode;
            }
            else
                return 0;
            
        }

        public UInt32 removeProcFuncs(UInt32 nEventCode)
        {
            if (listProcFuncitons.ContainsKey(nEventCode))
            {
                listProcFuncitons.Remove(nEventCode);
                return nEventCode;
            }
            else
                return 0;

        }

        public int addTask(uint eMainCode, uint eSubCode, string strExtroInfo = "")
        {
            //CDebug.jmsgEx("[addTask]{0}:{1}:{2}", eMainCode, eSubCode, strExtroInfo);
            CEventInfo _cTaskInof = new CEventInfo(eMainCode, eSubCode);///初始化UInt32最大值
            if (CUtil.isStringValid(strExtroInfo, 1) == true)
                _cTaskInof.strInfo = strExtroInfo;
            return addTask(_cTaskInof);
        }

        public int addTask(CEventInfo cTaskInof)
        {
            CEventInfo _cTaskInof = new CEventInfo(4294967295);///初始化UInt32最大值
            _cTaskInof = cTaskInof;
            listEventData.Add(_cTaskInof);
            return listEventData.Count - 1;
        }

        public CProcEventTask()
        {
            delgAddEvent = new InvokeDelegateAddEvent(addTask);
            thMonitlistEventDataFull = new Thread(MonitlistEventDataFull);
            thMonitlistEventDataFull.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thMonitlistEventDataFull.Start();
            thprocEventLoop = new Thread(procEventLoop);
            thprocEventLoop.SetApartmentState(ApartmentState.STA); //Set the thread to STA
           
        }

        
        public void doRun() 
        { 
            m_stop = false;
            thprocEventLoop.Start();
        }

        private void MonitlistEventDataFull()
        {
            while (m_stop== false)
            {
                SpinWait.SpinUntil(() => (m_stop==true)||(listEventData.Count >= nMaxEvenCont), -1);
                if (m_stop == true)
                    break;
                Monitor.Enter(listEventData);
                listEventData.RemoveAt(0);
                Monitor.Exit(listEventData);
            }
        }

     
        private void procEventLoop()
        {
            while (m_stop== false)
            {
                SpinWait.SpinUntil(() => listEventData.Count > 0 || m_stop==true, -1);
                if (m_stop == true)
                {
                    break;
                }
                if (listEventData.Count > 0 )
                {
                    try
                    {
                        //取得第一筆資料
                        CEventInfo _cTaskInof = new CEventInfo(4294967295);///初始化UInt32最大值
                        Monitor.Enter(listEventData);
                        _cTaskInof = listEventData[0];
                        listEventData.RemoveAt(0);
                        if (_cTaskInof == null)
                            continue;
                    
                        Monitor.Exit(listEventData);

                        InvokeDelegate fnProcThis;
                    
                        if (listProcFuncitons.TryGetValue(_cTaskInof.nEventCode, out fnProcThis))
                        {
                            Thread thProcEvent = new Thread(() => fnProcThis(_cTaskInof));
                            thProcEvent.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                            thProcEvent.Start();
                        }
                        else
                        {
                            CDebug.jmsg("[錯誤]處理事件:{0}未設定處理函式\n", _cTaskInof.nEventCode);
                            continue;
                        }
                    }
                    catch (Exception e)
                    {
                        CDebug.jmsgEx("[EventPorc]Exception!!{0}",e.Message);
                        continue;
                    }
                }
            }
        }

    }
}
