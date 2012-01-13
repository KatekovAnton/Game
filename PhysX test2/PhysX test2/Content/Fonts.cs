using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace PhysX_test2.Content
{
    public static class Fonts
    {
        public static SpriteFont _font1;
        public static void Load(string skin)
        {
            _font1 = Program.game.Content.Load<SpriteFont>("Courier New");
        }
    }
}
