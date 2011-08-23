using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;


namespace PhysX_test2.Content
{
    class Texture:PackContent
    {
        public Texture2D texture
        {
            get;
            private set;
        }
        private GraphicsDevice dev;
        public Texture(GraphicsDevice device)
        {
            dev = device;
        }


        public override void loadbody(byte[] array)
        {
            texture = Texture2D.FromStream(dev, new System.IO.MemoryStream(array));
        }
    }
}
