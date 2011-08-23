using System;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace PhysX_test2.Content
{
    class MeshList:PackContent
    {
        public string[] MeshNames
        {
            protected set;
            get;
        }
        public override void loadbody(byte[] buffer)
        {
            BinaryReader br = new BinaryReader(new MemoryStream(buffer));
            string name = br.ReadPackString();
            int meshCount = br.ReadInt32();


            MeshNames = new string[meshCount];
            for (int i = 0; i < meshCount; i++)
                MeshNames[i] = br.ReadPackString();
        }
    }
}
