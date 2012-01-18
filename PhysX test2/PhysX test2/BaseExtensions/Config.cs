using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace PhysX_test2
{
    public class Config
    {
        private string commentString = "//";
        private string lineEnd = ";";

        private static Config _instance;
        public static Config Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new Config();
                return _instance;
            }
        }

        private Dictionary<string, string> _config;

        public string GetParameterValue(string __paramName)
        {
            if (!_config.Keys.Contains(__paramName))
                return null;

            return _config[__paramName];
        }

        public bool GetBooleanParameter(string __paramName)
        {
            return GetParameterValue(__paramName).ToUpper().CompareTo("YES") == 0;
        }

        private Config()
        {
            StreamReader sr = new StreamReader("config.cfg");
            _config = new Dictionary<string, string>();

            int linenum =0;

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                line = RemoveComments(line);
                if (line.Length == 0)
                    continue;
                if (!CheckFinish(line))
                    LogProvider.logMessage("Config: error line ends on line " + linenum.ToString());

                AddParameter(line, linenum);

                linenum++;

            }
        }

        private string RemoveComments(string __line)
        {
            int location = __line.IndexOf(commentString);
            if (location < 0)
                return __line;

            return __line.Substring(0, location).Trim(' ', '\t', '\n');
        }

        private bool CheckFinish(string rs)
        {
            int location = rs.IndexOf(lineEnd);
            return location == rs.Length-1;
        }

        private void AddParameter(string __str, int line)
        {
            string[] data = __str.Split('=');

            if (data.Length != 2)
            {
                LogProvider.logMessage("Config: wrong number of \'=\' on line " + line.ToString());
                return;
            }

            for (int i = 0; i < data.Length; i++)
                data[i] = data[i].Trim(' ', '\t', '\n', ';');

            _config.Add(data[0], data[1]);
        }
    }
}
