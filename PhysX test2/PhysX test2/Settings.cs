using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2 {
    static class Settings {
        public static float movingSpeed = 1.0f;
        public static float rotateSpeed = 0.01f;
        public static float zoomSpeed = 1.01f;
    }
    public class DictionaryMethods
    {
        public static Dictionary<string, string> CreateCopy(Dictionary<string, string> parameter)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (string key in parameter.Keys)
            {
                result.Add(key, parameter[key]);
            }
            return result;
        }

       

        public static Dictionary<string, string> FromStream(System.IO.BinaryReader bw)
        {
            int count = bw.ReadInt32();
            Dictionary<string, string> result = new Dictionary<string, string>(count);
            for (int i = 0; i < count; i++)
            {
                result.Add(bw.ReadPackString(), bw.ReadPackString());
            }
            return result;
        }
    }
}
