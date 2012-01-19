using System;
using System.Diagnostics;

namespace PhysX_test2
{
#if WINDOWS || XBOX
   public static class Program
    {
       public static MyGame game;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                foreach (string arg in args)
                {
                    if (arg == "-git")
                    {
                        string git_commander = @"C:\Users\shpengler\Desktop\git\GitCommander\GitCommander\bin\Debug\GitCommander.exe";
                        Process.Start(git_commander);
                    }
                }
            }
            catch { }
            Config.Init();
            game = new MyGame();
            game.Run();
        }
    }
#endif
}

