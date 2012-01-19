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

namespace PhysX_test2.Scripting
{
    public class Comment : System.Attribute    {        public string comment;      public Comment(string comment)      {    this.comment = comment; }    }

    public static class SE
    {
        public static SCRIPTS scripts;
        public static ScriptEngine scriptengine;
        public static ScriptScope scriptscope;


        public static dynamic Ex(string script_name)
        {
           return scripts[script_name].Execute();
        }

        public static void Init()
        {
           scripts = new SCRIPTS(@"Content\Scripts\"); 
           scriptengine = IronPython.Hosting.Python.CreateEngine();
           scriptscope = scriptengine.CreateScope();
           // scriptscope.SetVariable("null", null);
           scriptscope.SetVariable("result", "");

            
        }

        public static dynamic Execute(string str)
        {
            if (str != scripts.Empty.data)
            {
               scriptengine.Execute(str, scriptscope);
               try { return scriptscope.GetVariable<string>("result"); } catch { }
               try { return scriptscope.GetVariable<object>("result"); } catch { }
            }
            return "";
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
                StreamReader sr = new StreamReader(path + _name + ".py");
                script.name = _name;
                script.data = sr.ReadToEnd();
                sr.Close();
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
        public void Execute()
        {
            try
            {
                SE.scriptscope.SetVariable("counter", counter++);
                SE.Execute(data);
            }
            catch (Exception ee)
            {
                ExcLog.LogException("Executing Script "+ name + " : " + ee.Message);
            }
        }

        public void Add(string data)
        {
            this.data += "\n" + data;
        }

        public void Save() { Save(SE.scripts.path + name + ".py"); }
        public void Save(string filename)
        {
            if (name!=SE.scripts.Empty.name)
            try
            {
               StreamWriter sw = new StreamWriter(filename, false); sw.Write(data); sw.Close(); 
            }
            catch (Exception ee)
            {
                ExcLog.LogException("Saving Script " + name + " : " + ee.Message);
            }
        }
    }

}
