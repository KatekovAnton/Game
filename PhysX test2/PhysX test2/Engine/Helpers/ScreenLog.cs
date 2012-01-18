using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Helpers
{
    public class ScreenLog : List<ScreenLogMessage>
    {
        //VOVA - макс кол-во строчек на экране
        private int maxLines = 50;

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

        public string Text
        { get { string txt = ""; foreach (ScreenLogMessage slm in this) txt += slm.text + "\n"; return txt; } }

        private void ClampMessages()
        {
            if (Count > maxLines)
            {
                RemoveRange(maxLines, Count - maxLines);
            }
        }

        public new void Add(ScreenLogMessage __message)
        {
            this.Insert(0, __message);
            ClampMessages();
        }

        public void TraceMessage(string __text, Color __color)
        { 
            Add(new ScreenLogMessage(__text,__color));
        }
    }

    public class ScreenLogMessage  
    {
        public string text;
        public Color Color;
        public ScreenLogMessage(string __text, Color __color)
        {
            this.Color = __color;
            this.text = __text;
        }
    }
}
