using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PhysX_test2
{
    public class ExcLog
    {
        private static bool needCrash = true;
        public static void LogException(string exceptionName)
        {
            LogProvider.logMessage("ExcLog: " + exceptionName);


            // write call stack method names
            StackTrace stackTrace = new StackTrace();           
            StackFrame[] stackFrames = stackTrace.GetFrames(); 
            foreach (StackFrame stackFrame in stackFrames)
                LogProvider.logMessage(stackFrame.GetMethod().Name);
            LogProvider.SaveLog();
#if DEBUG
            if(needCrash)
                throw new Exception(exceptionName);
#endif
        }

        public static void LogException_ToScreenLog(Exception ee)
        {
            MyGame.ScreenLogMessage(ee.Message, Microsoft.Xna.Framework.Color.Yellow);  
        }
        
    }
}
