using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;

namespace PhysX_test2
{
    public class KeyScan
    {
        public bool pressed;
        public long timePressed;
        public Keys key;

        public KeyScan(Keys _key)
        {
            timePressed = 0;
            pressed = false;
            key = _key;
        }

        public static bool operator ==(KeyScan _key1, KeyScan _key2)
        {
            return _key1.key == _key2.key;
        }

        public static bool operator !=(KeyScan _key1, KeyScan _key2)
        {
            return _key1.key != _key2.key;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void Update()
        {
            if (KeyboardManager.currentState.IsKeyDown(key))
            {
                if (!pressed)
                {
                    pressed = true;
                    timePressed = DateTime.Now.Ticks;
                }
            }
            else
            {
                if (pressed)
                {
                    pressed = false;
                    timePressed = 0;
                }
            }
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}
