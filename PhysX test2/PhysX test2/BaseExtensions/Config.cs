using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using IronPython;
using IronPython.Hosting;
using IronPython.Runtime;
using IronPython.Modules;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Xml;

namespace PhysX_test2
{
    public class Config : Scripting.MyPythonEngine
    {
        private static Config _instance;
        public static Config Instance
        {
            get
            {
                if(_instance == null) 
                    _instance = new Config("config.cfg");
                return _instance;
            }
        }

        public Config(string config_file)
            : base(config_file)
        {
            scriptscope.SetVariable("false", false);
            scriptscope.SetVariable("true", true);
            scriptscope.SetVariable("no", false);
            scriptscope.SetVariable("yes", true);

            ExScript(config_file);
            FillByVariables("_*");
        }

     
    }
}
