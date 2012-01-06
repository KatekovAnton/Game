using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PhysX_test2.ContentNew
{
    public class Texture:IPackEngineObject, IPackContentEntity
    {
        public Texture2D texture
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (!texture.IsDisposed)
                Dispose();
        }

        public void CreateFromContentEntity(IPackContentEntity[] __contentEntities) 
        {
            //looks like dont need...
        }

        public void LoadBody(byte[] array)
        {
            texture = Texture2D.FromStream(MyGame.Device, new System.IO.MemoryStream(array));
        }

        public int GetContentType()
        {
            return ElementType.PNGTexture;
        }
    }
}
