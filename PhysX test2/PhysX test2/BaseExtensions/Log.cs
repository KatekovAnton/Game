﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2
{
    public class LogProvider
    {
        private static LogBase _log;
        
        public static bool NeedTracePhysXMessages
        {
            get;
            set;
        }
        public static bool NeedTraceContentSystemMessages
        {
            get;
            set;
        }
        public static bool NeedTraceContactReportMessages
        {
            get;
            set;
        }

        public static void SaveLog()
        {
            _log.SaveLog();
        }

        public static void Close()
        {
            _log.Close();
        }

        public static void logMessage(string __msg)
        {
            if (_log == null)
            {
                _log = new LogToFile();
                LogProvider.logMessage("Opening application");
                NeedTraceContentSystemMessages = true;
                NeedTracePhysXMessages = true;
            }

            _log.WriteMessage(__msg); 
        }
    }
    public abstract class LogBase
    {
        protected bool closed = false;

        public abstract void WriteMessage(string __message);
        public abstract void Close();

        public abstract void SaveLog();

        ~LogBase()
        {
            if (!closed)
                Close();
        }
    }

    public class LogToFile : LogBase
    {
        private System.IO.StreamWriter sw;
        private string filename;

        public LogToFile()
        {
          //  filename = System.IO.Path.GetDirectoryName(
          //      System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6) + DateTime.Now.ToString().Replace('.', ':') + ".txt";

            sw = new System.IO.StreamWriter("111.txt", false, Encoding.Default);
        }

        public override void WriteMessage(string __message)
        {
            sw.WriteLine(DateTime.Now.ToString() + "     " + __message);
        }

        public override void SaveLog()
        {
            sw.Flush();
        }

        public override void Close()
        {
            sw.Flush();
            sw.Close();
            sw = null;
            closed = true;
        }
    }
}