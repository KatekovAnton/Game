using System;

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
            game = new MyGame();
            game.Run();
        }
    }
#endif
}

