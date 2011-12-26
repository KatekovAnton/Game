using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.Content
{
    public class Pack
    {
        public string errors = string.Empty;
        public System.IO.FileInfo fi;
        public PackContent[] Objects;
        public int headersize;
        static int formayid = 43647457;
        public bool fullsucces;
        public Pack(string filename)
        {
            AddObjectsToPack(filename);
        }
        public void AddObjectsToPack(string filename)
        {
            List<int> needcalcsize = new List<int>();
            fi = new System.IO.FileInfo(filename);
            fullsucces = false;
            System.IO.FileStream str1 = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
            System.IO.BinaryReader br = new System.IO.BinaryReader(str1);


            int foratID = br.ReadInt32();
            if (foratID != formayid)
            {
                errors += DateTime.Now.ToString() + ": not a pack;";
                br.Close();
                return;
            }
            int objectcount = br.ReadInt32();

            Objects = new PackContent[objectcount];
            for (int i = 0; i < objectcount; i++)
            {
                Objects[i] = new PackContent(br, i);
                if (Objects[i].size == 0)
                    needcalcsize.Add(i);
            }

            headersize = Convert.ToInt32(br.BaseStream.Position);

            for (int i = 0; i < needcalcsize.Count - 1; i++)
            {
                Objects[needcalcsize[i]].size = Objects[needcalcsize[i] + 1].offset - Objects[needcalcsize[i]].offset;
            }
            if (needcalcsize[needcalcsize.Count - 1] + 1 == Objects.Length)
            {
                Objects[needcalcsize[needcalcsize.Count - 1]].size = Convert.ToInt32(this.fi.Length) -
                    (Objects[Objects.Length - 1].offset + headersize);
            }
            else
            {
                int processingobject = needcalcsize[needcalcsize.Count - 1];
                Objects[processingobject].size = Objects[processingobject + 1].offset - Objects[processingobject].offset;
            }
            br.Close();
            fullsucces = true;
        }
    }
}
