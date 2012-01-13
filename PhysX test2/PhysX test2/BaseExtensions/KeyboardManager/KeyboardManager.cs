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
        public static KeyboardState currentState;
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
                    List<HotKey> hotkeys = user.hotkeys();
                    foreach (HotKey k in hotkeys)
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
            currentState = Keyboard.GetState();
            foreach (KeyScan KeyScan in scaningKeys)
                KeyScan.Update();

            foreach (IKeyboardUser user in keyboardusers)
            {
                if (captured_user!=null || user.GlobalUser)
                {
                    List<HotKey> hotkeys = user.hotkeys();
                    foreach (HotKey k in hotkeys)
                    {
                        k.TryExecute();
                    }
                }
            }

        }

        bool keys_captured_by_user = false;
        IKeyboardUser captured_user;

        public void Capture(IKeyboardUser IKeyboardUser)
        {
            captured_user = IKeyboardUser;
        }

        public void CaptureRelease()
        {
            keys_captured_by_user = false;
        }

        private List<IKeyboardUser> keyboardusers;
        public static bool IsMouseCaptured
        {
            get
            {
                foreach (IKeyboardUser user in Manager.keyboardusers)
                {
                    if (user.IsKeyboardCaptured())
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
