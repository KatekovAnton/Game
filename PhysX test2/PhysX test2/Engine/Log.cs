using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.Engine
{
    public class LogProvider
    {
        public LogProvider(bool needtrace, Log l)
        {
            needtracelog = needtrace;
            log = l;
            NeedTraceContentSystemMessages = true;
            NeedTracePhysXMessages = true;
        }
        public static bool needtracelog = true;
        private static Log log;
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
        public static void TraceMessage(string msg)
        {
            log.WriteMessage(msg); 
        }
    }
    public class Log
    {
        
        private System.IO.StreamWriter sw;
        string filename;
        public Log(bool needtracelog = true)
        {
            LogProvider l = new LogProvider(needtracelog,this);

            filename = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6) + DateTime.Now.ToString().Replace('.', ':') + ".txt";

            sw = new System.IO.StreamWriter("111.txt", false, Encoding.Default);
            if (LogProvider.needtracelog)
                LogProvider.TraceMessage("Opening application");
        }
        public void WriteMessage(string message)
        {
            sw.WriteLine(DateTime.Now.ToString() + "     " + message);
            try
            {
                sw.Flush();
            }
            catch (Exception ex)
            {
                sw = new System.IO.StreamWriter("111.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + "     " + message);
                sw.Flush();

            }

        }
        ~Log()
        {
            if (LogProvider.needtracelog)
                LogProvider.TraceMessage("Closing application");

            sw = null;
            if (!LogProvider.needtracelog && System.IO.File.Exists(filename))
                System.IO.File.Delete(filename);
        }
    }
}
