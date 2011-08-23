using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;





namespace PhysX_test2.Content
{
    public class StringList : PackContent
    {
        

        public string[] Names
        {
            protected set;
            get;
        }

        public StringList()
        {


        }



        public override void loadbody(byte[] buffer)
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(new System.IO.MemoryStream(buffer));
            var name = br.ReadPackString();
            int meshCount = br.ReadInt32();
           

            Names = new string[meshCount];
            for (int i = 0; i < meshCount; i++)
            {
                Names[i] = br.ReadPackString();

               
            }
            br.Close();

        }

    }
}
