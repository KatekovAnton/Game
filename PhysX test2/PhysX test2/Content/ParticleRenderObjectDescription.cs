using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysX_test2.Content
{
    public class ParticleRenderObjectDescription : PackContent, ContentNew.IPackContentEntity
    {
        public string texturename;
        public string meshname;

        public override void loadbody(byte[] array)
        {
            BinaryReader br = new BinaryReader(new MemoryStream(array));
            meshname = br.ReadPackString();
            texturename = br.ReadPackString();
            br.Close();
        }

        public void LoadBody(byte[] array)
        {
            loadbody(array);
        }

        public int GetContentType()
        {
            return ElementType.ParticelRenderObjectDescription;
        }

        public ContentNew.IPackEngineObject CreateEngineObject()
        {
            return null;
        }

        public Pack Pack
        {
            get;
            set;
        }
    }
}
