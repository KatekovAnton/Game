using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Reflection;
using IronPython;
using IronPython.Hosting;
using IronPython.Runtime;
using IronPython.Modules;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace PhysX_test2.Scripting
{
    //Аттрибуты можно получить во время рантайм выполнения скрипта. Можно запихать сюда инфу для описания метода и прочее
    public class Comment : System.Attribute    {        public string comment;      public Comment(string comment)      {    this.comment = comment; }    }

    // Dictionary<string,dynamic> - using for UserData such as Config и т.п.
    public abstract class MyPythonEngine : Dictionary<string,dynamic>
    {
        public string file_name = "";
        public SCRIPTS scripts;
        public ScriptEngine scriptengine;
        public ScriptScope scriptscope;
        public virtual void SetVariables() { }
        
        public MyPythonEngine(string path)
            : base()
        {
            file_name = path;
            scripts = new SCRIPTS(path);
            scriptengine = IronPython.Hosting.Python.CreateEngine();
            scriptscope = scriptengine.CreateScope();
            scriptscope.SetVariable("result", "");
        }

        /// <summary>
        /// Please, do not use while game is running and user is killing monsters.
        /// It's extra slow. Do not use in scripts!!!
        /// </summary>
        public Dictionary<string, dynamic> Variables
        {
            get
            {
                List<string> names = new List<string>(scriptscope.GetVariableNames());
                Dictionary<string, dynamic> vars = new Dictionary<string, dynamic>(names.Count);
                foreach (string name in names)
                    try
                    {
                        vars[name] = scriptscope.GetVariable(name);
                    }
                    catch
                    {
                        vars[name] = "ERROR on Getvariable";
                    }
                return vars;
            }
        }
        Exception LastException;
        public void Execute(string str)
        {
            if (str != scripts.Empty.data)
            {
                try
                {
                    scriptengine.Execute(str, scriptscope);
                }
                catch (Exception ee)
                {
                    LastException = ee;
                    ExcLog.LogException_ToScreenLog(ee);
                }
               /* try { return scriptscope.GetVariable<string>("result"); }       catch { }
                try { return scriptscope.GetVariable<object>("result"); }       catch { }*/
            }
           // return "";
        }

        public virtual void ExScript(string script_name)
        {
               scriptscope.SetVariable("counter", scripts[script_name].counter++);
               Execute(scripts[script_name].data);
               if (LastException!=null)
                   ExcLog.LogException("Executing Script " + script_name + " : " + LastException.Message);
            //return scripts[script_name].Execute();
        }

        public virtual void FillByVariables(string name_pattern)
        {
            Regex regex = new Regex(name_pattern);
            Dictionary<string, dynamic> vars = Variables;
            foreach (string name in vars.Keys)
            {
                if (regex.IsMatch(name))
                Add(name, vars[name]);
            }
        }


    }

    public class SE : MyPythonEngine
    {
        public SE(string path) : base(path) { }
        private static SE _instance;
        public static SE Instance {
            get {
                if (_instance == null)
                    _instance = new SE(Config.Instance["_scripts_path"]);
            return _instance;
            }
        }
    }



    public class SCRIPTS : UserInterface.AutoLoadingContent
    {
        public void SaveAll()
        { foreach (string s in Names) this[s].Save(path + s + ".py"); }

        public SCRIPTS(string path)
            : base(path)
        {
            Empty = new SCRIPT();
        }

        public override dynamic Load(string _name)
        {
            SCRIPT script = new SCRIPT();
            try
            {
                StreamReader sr;
                string filename = "";
                if (!_name.Contains("\\")) filename = path; //default directory
                filename += _name;
                if (!_name.Contains(".")) filename += ".py"; //default extension
                if (File.Exists(filename))
                {
                    sr = new StreamReader(filename);
                    script.name = _name;
                    script.data = sr.ReadToEnd();
                    sr.Close();
                }
                else
                {
                    ExcLog.LogException("Loading Script ERROR: " + filename + " doesn't exist");
                }
            }
            catch (Exception ee)
            {
                // insert your pack loading here
                ExcLog.LogException("Loading Script ERROR: " + ee.Message);
            }
            return script;
        }


    }
    
    public class SCRIPT
    {
        public string data = "# ", name = "Empty";
        public int counter = 0;
        public SCRIPT() { }
        public void Dispose() { data = ""; name = ""; }
    }

}
