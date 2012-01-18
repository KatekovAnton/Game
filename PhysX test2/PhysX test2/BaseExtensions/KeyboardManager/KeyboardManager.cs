using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PhysX_test2
{
    public class KeyboardManager
    {
        public static bool Shift, Ctrl, Alt, NumLock, etc;

        public static KeyboardState currentState;
        public static List<Keys> PressedKeys;
        public static List<Keys> LastPressedKeys;
        public static List<KeyScan> scaningKeys;
        public static KeyScan GetScanForKey(Keys key)
        {
            for (int i = 0; i < scaningKeys.Count; i++)
                if (scaningKeys[i].key == key)
                    return scaningKeys[i];
            return new KeyScan(key);
        }

        private sealed class KeyboardManagerCreator
        {
            private static readonly KeyboardManager instance = new KeyboardManager();

            public static KeyboardManager _KeyboardManager
            {
                get { return instance; }
            }
        }

        public static KeyboardManager Manager
        {
            get { return KeyboardManagerCreator._KeyboardManager; }
        }
        
        int lastusercount;
        public void Update()
        {
            if (lastusercount != keyboardusers.Count)
            {
                //scaningKeys.Clear();
                foreach (IKeyboardUser user in keyboardusers)
                {
                    foreach (HotKey k in user.hotkeys)
                    {
                        for (int i = 0; i < k.associatedKeys.Length; i++)
                        {
                            bool containkey = false;
                            for (int hk = 0; hk < scaningKeys.Count; hk++)
                            {
                                if (scaningKeys[hk].key == k.associatedKeys[i])
                                {
                                    containkey = true;
                                    break;
                                }
                            }
                            if (!containkey)
                                scaningKeys.Add(new KeyScan(k.associatedKeys[i]));
                        }
                    }
                }
                lastusercount = keyboardusers.Count;
            }

            LastPressedKeys = PressedKeys;//currentState.GetPressedKeys().ToList<Keys>();
            currentState = Keyboard.GetState();
            PressedKeys = currentState.GetPressedKeys().ToList<Keys>();
            Shift = currentState.IsKeyDown(Keys.LeftShift);
            Ctrl = currentState.IsKeyDown(Keys.LeftControl);
            Alt = currentState.IsKeyDown(Keys.LeftAlt);

            foreach (KeyScan KeyScan in scaningKeys)
                KeyScan.Update();

            if (keys_captured_by_user)
            {
                foreach (HotKey k in captured_user.hotkeys)
                {
                    k.TryExecute();
                }

                if (keys_captured_by_user)
                    if (PressedKeys.Count >= 1)
                    {
                        foreach (HotKey k in captured_user.hotkeys)
                            if (k.associatedKeys.Length == 1)
                            {
                                PressedKeys.Remove(k.associatedKeys[0]);
                                LastPressedKeys.Remove(k.associatedKeys[0]);
                            }

                        if (captured_user_all_keys != null)
                        {
                            List<Keys> NewKeys = PressedKeys.Except<Keys>(LastPressedKeys).ToList<Keys>();
                            foreach (Keys Key in NewKeys)
                                key_buffer += Key_To_Str(Key);
                            //   if (!LastPressedKeys.Contains(PressedKeys[0]))
                            captured_user_all_keys.KeyPress();
                        }
                    }
            }
            else
                foreach (IKeyboardUser user in keyboardusers)
                {
                    if (user.GlobalUser)
                    {
                        foreach (HotKey k in user.hotkeys)
                        {
                            k.TryExecute();
                        }
                    }
                }
        }

        static public string Key_To_Str(Keys Key)
        {
             int i = (int)Key;
             if (i > 31 && i < 127)
             {
                 string str = Convert.ToChar((int)Key).ToString();
                 if (Shift)
                 {
                     switch (i)
                     {
                         case 48: str = ")"; break;
                         case 57: str = "("; break;
                         default: ; break;
                     }
                     return str;
                 }
                 else
                 {
                     return str.ToLowerInvariant();
                 }
             }
             else
             {
                string str = "";
                switch (i)
                  {
                    case 187: str = Shift ? "+" : "="; break;
                    case 189: str = Shift ? "_" : "-"; break;
                      default: ; break;
                  }
                return str;
             }

             return "?";
        }

        bool keys_captured_by_user = false;

        public static string key_buffer = "";

        IKeyboardUser captured_user;
        IAllKeys captured_user_all_keys;


        public void Capture(IKeyboardUser IKeyboardUser)
        {
            captured_user = IKeyboardUser;
            captured_user_all_keys = IKeyboardUser as IAllKeys;
            captured_user.IsKeyboardCaptured = true;
            keys_captured_by_user = true;
        }

        public void CaptureRelease()
        {
            if (keys_captured_by_user)
            {
                keys_captured_by_user = false;
                captured_user.IsKeyboardCaptured = false;
                captured_user_all_keys = null;
                captured_user = null;
            }
        }

        private List<IKeyboardUser> keyboardusers;
        public static bool IsMouseCaptured
        {
            get
            {
                foreach (IKeyboardUser user in Manager.keyboardusers)
                {
                    if (user.IsKeyboardCaptured)
                        return true;
                }
                return false;
            }
        }
        protected KeyboardManager()
        {
            keyboardusers = new List<IKeyboardUser>();
            scaningKeys = new List<KeyScan>();
        }
        public void AddKeyboardUser(IKeyboardUser newUser)
        {
            keyboardusers.Add(newUser);
        }
        public void RemoveKeyboardUser(IKeyboardUser user)
        {
            keyboardusers.Remove(user);
        }
    }
}
