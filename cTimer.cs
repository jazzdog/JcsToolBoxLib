using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace ToolBoxLib
{
    /*Example:
    *  cTimer theTimer = new cTimer(1000); //give init timer (ms)
    *  cTimer theTimer = new cTimer(); // give time when start
    *  theTimer.addProc(_timerUp); //give timeup call back funcitons(could be more than one call back function)
    *  
    *   void _timerUp(){  //do something.... }
    *   theTimer.start(); // time by constructor
    *   theTimer.start(3000); //call start (stop first than start) to new time
    *   
    *   notice:
    *    if time set less then 0, can't go start timer
    *    if there is no set any callback function , cant go start timer
    */
    public delegate void InvokeTimerupEventproc();
    public class cTimer : IDisposable
    {

        private  System.Timers.Timer _timer = null;
        private  Object LOCKTIMER = new Object();
        private List<InvokeTimerupEventproc> listProcFuncitons = new List<InvokeTimerupEventproc>();
        private bool m_blCounting { get { return _timer.Enabled; } }

        private  System.Timers.Timer m_timer
        {
            get
            {
                if (_timer == null)
                {
                    lock (LOCKTIMER)
                    {
                        if (_timer == null)
                        {

                            _timer = new System.Timers.Timer();
                            _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timeupEvent);
                        }
                        else
                        {

                        }
                    }
                   
                }

                return _timer;
            }
        }
        void _timeupEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
           
                SpinWait.SpinUntil(() => listProcFuncitons.Count > 0 || m_blCounting == true, -1);
                if (m_blCounting == false)
                {
                    return;
                }
                foreach (InvokeTimerupEventproc theproc in listProcFuncitons)
                {
                    Thread thProcEvent = new Thread(() => theproc());
                    thProcEvent.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                    thProcEvent.Start();
                }


            
        }

        bool checkBeforeStart()
        {
            if ((m_timer.Interval > 0) && (listProcFuncitons.Count > 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public cTimer(double dbMillisecond = -1)
        {
            m_timer.Interval = dbMillisecond;
        }

        public void start(double dbMillisecond=-1)
        {
            if (dbMillisecond > 0)
                m_timer.Interval = dbMillisecond;

            if (checkBeforeStart() == false)
                return;

            m_timer.Stop();
            while (m_blCounting == true)
            {
                SpinWait.SpinUntil(() => false, 100);
            }


            m_timer.Start();

        }

        public bool stop()
        {
            m_timer.Stop();
            while (m_blCounting == true)
            {
                SpinWait.SpinUntil(() => false, 100);
            }
            return m_blCounting;
        }

        public int addProc(InvokeTimerupEventproc _fnProc)
        {
            listProcFuncitons.Add(_fnProc);
            return listProcFuncitons.Count;

        }

        ~cTimer()
        {
            Dispose();
        }

        public void Dispose()
        {
            m_timer.Stop();
            m_timer.Dispose();
        }

    }
}
