using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;


namespace ToolBoxLib
{
    /*[example]
     *   cWachDog theDog = new cWachDog();
     *     theDog.addWatch_processNoRun("AVC_MemberManager", @"D:\[ ]Job\[ ]Project\[ ]AVC_奇卓Androvideo_FaceDetect\展示間管理\AVC_MemberManager\AVC_MemberManager\bin\Debug\AVC_MemberManager.exe");
           theDog.addWatch_processMutilple("AVC_MemberManager");
           theDog.start(1000);
           
     */

    class wathdogInfo
    {
        public DateTime startTime { get { return _startTime; } }
        public DateTime lastTime { get { return _lastTime; } }
        public UInt64 processNoRunCount { get { return _processNoRunCount; } }
        public UInt64 processMutilpleCount { get { return _processMutilple; } }

        DateTime _startTime;
        DateTime _lastTime;
        UInt64 _processNoRunCount;
        UInt64 _processMutilple;

        public wathdogInfo()
        {
            reset();
        }

        public void reset()
        {
            _startTime = DateTime.Now;
            _lastTime = DateTime.Now;
            _processNoRunCount = 0;
            _processMutilple = 0;
        }

        public UInt64 addCount_NoRun()
        {
            _processNoRunCount++;
            _lastTime = DateTime.Now;
            return processNoRunCount;
        }

        public UInt64 addCount_Mutilple()
        {
            _processMutilple++;
            _lastTime = DateTime.Now;
            return processMutilpleCount;
        }



    }


    public delegate void InvokeWatchEventCall();
    public class cWachDog
    {

        public UInt64 countNoRun {get{ return info.processNoRunCount; } }
        public UInt64 countMultiple { get { return info.processMutilpleCount; } }
        public DateTime startTime { get { return info.startTime; } }
        public DateTime lastTime { get { return info.lastTime; } }

        cTimer _timerWatchDog = null;
        string m_ProcName_processNoRun = "";
        string m_ProcPath_processNoRun = "";
        string m_ProcName_processMutilple = "";
        wathdogInfo info;
        public InvokeWatchEventCall eventCallback_NoRun;
        public InvokeWatchEventCall eventCallback_Multiple;

        public cWachDog()
        {
            info = new wathdogInfo();
            _timerWatchDog = new cTimer();
            stop();
        }

        public void _stop()
        {
            while (_timerWatchDog.runing)
            {
                _timerWatchDog.stop();
                SpinWait.SpinUntil(() => false, 100);
               
            }
            CUtil.clearMemory();
        }

        public void stop()
        {
            _stop();
            info.reset();
            _timerWatchDog.Dispose();
            _timerWatchDog = null;
            _timerWatchDog = new cTimer();
            m_ProcName_processNoRun = "";
            m_ProcPath_processNoRun = "";
            m_ProcName_processMutilple = "";
            CUtil.clearMemory();
        }
        public void start(double ms=10000)
        {
            _timerWatchDog.start(ms);
            
        }





        public void addWatch_processMutilple(string procName)
        {
            m_ProcName_processMutilple = procName;
           
            if (CUtil.isStringValid(m_ProcName_processMutilple, 1))
            {
               
                _timerWatchDog.addProc(procwatch_processMutilple);
            }
        }
        void procwatch_processMutilple()
        {
            if (_timerWatchDog.runing)
            {
                Process[] theProcList;
                int nCount = CUtil.getProcessByName(m_ProcName_processMutilple, out theProcList);
                if (nCount > 1)
                {
                    info.addCount_Mutilple();
                    if (eventCallback_Multiple != null)
                        eventCallback_Multiple();
                    CUtil.closeProcess(m_ProcName_processMutilple);
                }
            }
        }

        public void addWatch_processNoRun(string procName, string procPath)
        {
            if ((CUtil.isStringValid(procName, 1)) &&
                (CUtil.isStringValid(procPath, 1)))
            {

                m_ProcName_processNoRun = procName;
                m_ProcPath_processNoRun = procPath;

                if (CUtil.isStringValid(m_ProcName_processNoRun, 1))
                {
                   
                    _timerWatchDog.addProc(procwatch_processNoRun);
                }
            }

        }
        void procwatch_processNoRun()
        {
            if (_timerWatchDog.runing)
            {
                Process[] theProcList;
                int nCount = CUtil.getProcessByName(m_ProcName_processNoRun, out theProcList);
                if (nCount <= 0)
                {
                    info.addCount_NoRun();
                    if (eventCallback_NoRun != null)
                        eventCallback_NoRun();
                    CUtil.startProcess(m_ProcPath_processNoRun);
                }
            }
        }
    }
}
