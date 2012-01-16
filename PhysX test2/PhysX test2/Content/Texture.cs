using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;


namespace PhysX_test2.Content
{
   public  class Texture:PackContent
    {
        public Texture2D texture
        {
            get;
            private set;
        }

        public override void loadbody(byte[] array)
        {
            texture = Texture2D.FromStream(MyGame.Device, new System.IO.MemoryStream(array));
        }
    }
}
