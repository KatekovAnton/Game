﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace PhysX_test2
{
    public interface IKeyboardUser
    {
        bool GlobalUser { get; }
        bool IsKeyboardCaptured { set; get; }
        List<HotKey> hotkeys { get; }
    }

    public interface IAllKeys
    {
        void KeyPress();
    }

    public class HotKey : List<Keys>
    {
        public HotKey(Microsoft.Xna.Framework.Input.Keys[] keys, Action _action)
        {
            this.AddRange(keys);
            action = _action;
        }
        public Action action;
        bool active = false;

        public void TryExecute()
        {
            if (!active)
            {
                bool canexecute = true;
                long timelast = 0;
                KeyScan scanforcurrentkey = KeyboardManager.GetScanForKey(this[0]);
                if (scanforcurrentkey.pressed)
                {
                    timelast = scanforcurrentkey.timePressed;

                    for (int i = 1; i < Count;  i++)
                    {
                        scanforcurrentkey = KeyboardManager.GetScanForKey(this[i]);

                        if (!scanforcurrentkey.pressed || scanforcurrentkey.timePressed < timelast)
                        {
                            active = false;
                            canexecute = false;
                            return;
                        }

                        timelast = scanforcurrentkey.timePressed;
                    }
                    if (canexecute)
                    {
                        action();
                        active = true;
                    }
                }
            }
            else
            {
                KeyScan scanforcurrentkey = KeyboardManager.GetScanForKey(this[Count - 1]);
                if (!scanforcurrentkey.pressed)
                {
                    active = false;
                    return;
                }
            }
        }
    }
}
