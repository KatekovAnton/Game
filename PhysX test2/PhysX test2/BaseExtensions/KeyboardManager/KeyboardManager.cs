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



        public static void Init()
        {
            string str_low = "0123456789abcdefghijklmnopqrstuvwxyz;=,-./`[\\]'`";
            string str_up = ")!@#$%^&*(ABCDEFGHIJKLMNOPQRSTUVWXYZ:+<_>?~{|}\"~";

             Keys_l = new Dictionary<int, string>();
             Keys_u = new Dictionary<int, string>();

             Keys_add(ref str_low, ref str_up, 48, 10);
             Keys_add(ref str_low, ref str_up, 65, 26);
             Keys_add(ref str_low, ref str_up, 186, 7);
             Keys_add(ref str_low, ref str_up, 219, 4);
             Keys_add(32, " ");
             Keys_add(9, "\t");
             
        }
        public static void Keys_add(int key, string str)
        {
            Keys_l.Add(key, str);
            Keys_u.Add(key, str);
        }

        public static void Keys_add(ref string str_low, ref string str_up, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Keys_l.Add(i + offset, str_low[i].ToString());
                Keys_u.Add(i + offset, str_up[i].ToString());
            }

            str_low = str_low.Remove(0, count);
            str_up = str_up.Remove(0,count);
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
                        for (int i = 0; i < k.Count; i++)
                        {
                            bool containkey = false;
                            for (int hk = 0; hk < scaningKeys.Count; hk++)
                            {
                                if (scaningKeys[hk].key == k[i])
                                {
                                    containkey = true;
                                    break;
                                }
                            }
                            if (!containkey)
                                scaningKeys.Add(new KeyScan(k[i]));
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

                if (keys_captured_by_user) // captured_user.hotkeys может содержать CaptureRelease, поэтому нужна вторая проверка
                    if (PressedKeys.Count >= 1)
                    {
                        foreach (HotKey k in captured_user.hotkeys)
                            if (k.Count == 1)
                            {
                                PressedKeys.Remove(k[0]);
                                LastPressedKeys.Remove(k[0]);
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

        static public Dictionary<int, string> Keys_l;
        static public Dictionary<int, string> Keys_u;

        static public string Key_To_Str(Keys Key)
        {
            try
            {
                if (Shift) return Keys_u[(int)Key];
                return Keys_l[(int)Key];
            }
            catch
            {
                Keys_u.Add((int)Key, "");
                Keys_l.Add((int)Key, "");
                return "";
            }
        }

        static public string Clipboard
        {
            set;
            get;
        }

        bool keys_captured_by_user = false;

        public static string key_buffer = "";

        public IKeyboardUser captured_user;
        public  IAllKeys captured_user_all_keys;


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

        public void Check(IKeyboardUser user)
        {
            HotKey kk1;
            foreach (IKeyboardUser _user in keyboardusers)    // я не знаю как можно это реализовать по-другому :)
                foreach (HotKey _kk in _user.hotkeys)
                    foreach (HotKey kk in user.hotkeys)
                    {
                        if (kk.Count > 0 && _kk.Count > 0)
                       {
                        bool eq = true;
                        foreach (Keys key in kk)
                            foreach (Keys _key in _kk)
                            {
                                if (key != _key)
                                {
                                    eq = false;
                                }
                            }
                       
                        if (eq)
                        {
                            string str = "";
                            foreach (Keys key in kk) str += "'"+key.ToString() + "'  ";
                            MyGame.ScreenLogMessage("<<<<<<<<<<<<<", GColors.CTextBack); 
                            MyGame.ScreenLogMessage("Keys are equals: " + str, GColors.CError);
                            MyGame.ScreenLogMessage(_user.GetType().ToString(), GColors.CError);
                            MyGame.ScreenLogMessage(user.GetType().ToString(), GColors.CError);
                            MyGame.ScreenLogMessage(">>>>>>>>>>>>>", GColors.CTextBack); 
                        }
                       }
                    }
        }

        public void AddKeyboardUser(IKeyboardUser newUser)
        {
#if DEBUG
            Check(newUser);  //проверка на совпадение горячих клавиш
#endif
            keyboardusers.Add(newUser);

        }
        public void RemoveKeyboardUser(IKeyboardUser user)
        {
            keyboardusers.Remove(user);
        }
    }
}
