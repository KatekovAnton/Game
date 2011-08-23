using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.Content
{
    class TextureList:PackContent
    {
        public string[] Names
        {
            private set;
            get;
        }

        public TextureList()
        {
            
        }



        public override void loadbody(byte[] buffer)
        {
            BinaryReader br = new BinaryReader(new MemoryStream(buffer));
            string name = br.ReadPackString();


            Names = new string[br.ReadInt32()];
            for (int i = 0; i < Names.Length; i++)
            {
                Names[i] = br.ReadPackString();
            }

        }
    }
}
