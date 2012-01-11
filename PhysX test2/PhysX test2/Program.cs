using System;
using System.Diagnostics;

namespace PhysX_test2
{
#if WINDOWS || XBOX
    static class Program
    {
        static MyGame game;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg == "-git")
                { 
                   string git_commander = @"D:\projects\GitCommander\GitCommander\bin\Debug\GitCommander.exe";
                   Process.Start(git_commander);                    
                }
            }

            game = new MyGame();
            game.Run();
        }
    }
#endif
}

