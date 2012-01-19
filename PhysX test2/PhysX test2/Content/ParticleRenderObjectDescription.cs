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
        public string materialname;
        public string meshname;

        public bool ShadowCaster;
        public bool ShadowReceiver;
        public bool Transparent;
        public bool SelfIlmn;

        public override void loadbody(byte[] array)
        {
            BinaryReader br = new BinaryReader(new MemoryStream(array));
            meshname = br.ReadPackString();
            materialname = br.ReadPackString();

            ShadowCaster = br.ReadBoolean(); ;
            ShadowReceiver = br.ReadBoolean();
            Transparent = br.ReadBoolean();
            SelfIlmn = br.ReadBoolean();
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
