using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace ToolBoxLib
{

    public delegate void InvokeDelegate(CEventInfo cInfo);
    /// <summary>
    /// 將事件加入列表的界面
    /// </summary>
    /// <param name="cTaskInof"></param>
    /// <returns></returns>
    public delegate int InvokeDelegateAddEvent(CEventInfo cTaskInof);

    public class CEventInfo
    {
        /// <summary>
        /// 事件代號
        /// </summary>
        public UInt32 nEventCode;
        /// <summary>
        /// 事件細項代號
        /// </summary>
        public UInt32 nEventSubCode; 
        /// <summary>
        /// 事件資訊內容
        /// </summary>
        public List<byte[]> listEventByteData = new List<byte[]>();
        /// <summary>
        /// 自動填入目前時間
        /// </summary>
        /// 

        public DateTime tEventTime = DateTime.Now;

        public Object objExtra=null;

        public String strInfo;




        public CEventInfo(UInt32 _nEventCode =0, UInt32 _nEventSubCode = 0)
        {
            nEventCode = _nEventCode;

            nEventSubCode = _nEventSubCode;
           
        }


    }

    public class CProcEventTask
    {

        
        public bool doStop() { m_stop = true; return m_stop; }
        public bool IsAlive { get { return thprocEventLoop.IsAlive; } private set { return; } }
        public InvokeDelegateAddEvent delgAddEvent;

        private List<CEventInfo> listEventData = new List<CEventInfo>();
        private List<Thread> listProcThread = new List<Thread>();
        private Dictionary<UInt32, InvokeDelegate> listProcFuncitons = new Dictionary<UInt32, InvokeDelegate>();
        private volatile bool m_stop = true;
        private int nMaxEvenCont = 100;
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


        public int addTask(CEventInfo cTaskInof)
        {
            CEventInfo _cTaskInof = new CEventInfo();
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
                    CEventInfo _cTaskInof = new CEventInfo();
                    Monitor.Enter(listEventData);
                    _cTaskInof = listEventData[0];
                    listEventData.RemoveAt(0);
                    if (_cTaskInof == null)
                        continue;
                    
                    Monitor.Enter(listEventData);

                    InvokeDelegate fnProcThis;
                    try
                    {
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
