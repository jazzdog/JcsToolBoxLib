using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolBoxLib;



namespace ToolBoxLib
{

    /* Example:
     *   [Declare]
     *          DispatcherTimer _timeout = new DispatcherTimer("TimeoutName");
     *   [init]
     *           _timeout.m_nEventDurationSec = 10;
                    if (_timeout.calbkEventTimeup == null)
                        _timeout.calbkEventTimeup = procTimeout;
                    _timeout.start();
         [Callback]
                void procTimeout(object sender, EventArgs hdlEvent)
                {
                    _timeout.stop();
                    CDebug.jmsg("CGI timout!!");
                }
     */
    public class DispatcherTimer : IDisposable
    {
        private System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };

        public EventHandler<EventArgs> calbkEventTimeup ;
        public int m_nEventDurationSec =-1;
        public bool isTicking { get { return (m_nEventDurationSec > 0)?true:false; } set { } }
        string m_strTimerName = "";

        public DispatcherTimer(string strTimerName="")
        {
            if (dispatcherTimer==null)
                dispatcherTimer = new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
            m_strTimerName = strTimerName;
            dispatcherTimer.Tick += TimerCount_Tick;
        }


        ~DispatcherTimer()
        {
            Dispose();
        }
        public void start()
        {
            if (dispatcherTimer == null)
                CDebug.jmsg("★★★[Err]★★★dispatcherTimer[{0}] == null", m_strTimerName);
            if (dispatcherTimer != null)
            {
                dispatcherTimer.Start();
                CDebug.jmsg("★★★dispatcherTimer.Start({0})=>{1}s", m_strTimerName,m_nEventDurationSec);
            }

        }
        public void stop()
        {
            if (dispatcherTimer == null)
                CDebug.jmsg("★★★[Err]★★★dispatcherTimer[{0}] == null", m_strTimerName);
            if (dispatcherTimer != null)
                dispatcherTimer.Stop();
        }


        public void Dispose()
        {
             dispatcherTimer = null;
        }

        void TimerCount_Tick(object sender, EventArgs e) 
        {

            if (m_nEventDurationSec > 0)
            {
                m_nEventDurationSec--;
               
            }
            else if (m_nEventDurationSec <= 0)
            {
                dispatcherTimer.Stop();
                CDebug.jmsg("[DispatcherTimer][{0}]時間到，呼叫註冊的函式....", m_strTimerName);
                if (calbkEventTimeup != null)
                NotifyModelChanged();
                
            }
        }

        public void NotifyModelChanged() 
        {
            if (calbkEventTimeup == null)
                CDebug.jmsg("★★★[Err]★★★NotifyModelChanged[{0}] == null", m_strTimerName);
            if (calbkEventTimeup!=null)
                calbkEventTimeup(this, new EventArgs());
        }
    }
}
