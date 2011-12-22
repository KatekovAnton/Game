using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Helpers
{
    public class ScreenLog
    {
        //VOVA - макс кол-во строчек на экране
        private static int maxLines = 50;
        public static List<string> Messages
        {
            get;
            private set;
        }
        public static List<Color> MessageColors
        {
            get;
            private set;
        }

        public int MaxLines
        {
            get
            {
                return maxLines;
            }
            set
            {
                maxLines = value;
                ClampMessages();
            }
        }

        private static void ClampMessages()
        {
            if (Messages.Count > maxLines)
            {
                Messages.RemoveRange(maxLines, Messages.Count - maxLines);
                MessageColors.RemoveRange(maxLines, MessageColors.Count - maxLines);
            }
        }

        public ScreenLog()
        {
            Messages = new List<string>();
            MessageColors = new List<Color>();
        }

        public static void TraceMessage(string __message, Color __color)
        {
            Messages.Insert(0, __message);
            MessageColors.Insert(0, __color);
            ClampMessages();
        }
    }
}
