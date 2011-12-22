using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.Content
{
    
    public class PackList
    {
        public Pack[] packs;
        public static PackList Instance;
        public PackList()
        {
            Instance = this;
            string path = System.IO.Path.GetDirectoryName(
      System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6)+"\\Data";
           
            string[] filenames = System.IO.Directory.GetFiles(path,@"*.pack");
            packs = new Pack[filenames.Length]; 
            PhysX_test2.LogProvider.logMessage("Content system: Number of packs: " + packs.Length.ToString());
            for (int i = 0; i < filenames.Length; i++)
                packs[i] = new Pack(filenames[i]);
                
        }
        
        public PackContent FindObject(string name, ref Pack pp)
        {
            for (int j = 0; j < packs.Length; j++)
                if (packs[j].Objects != null)
                    for (int i = 0; i < packs[j].Objects.Length; i++)
                        if (packs[j].Objects[i].name.CompareTo(name) == 0)
                        {
                            pp = packs[j];
                            return packs[j].Objects[i];
                        }
            return null;
        }
        
        public PackContent GetObject(string objectname, PackContent pc)
        {
            Pack containedpack = null;
            PackContent pch = FindObject(objectname, ref containedpack);

            if (pch.objectReaded)
            {
                PhysX_test2.LogProvider.logMessage("Content system: Object loaded:\t\t" + objectname.Substring(0,objectname.Length-1) + "\t\t type of " + ElementType.ReturnString(pch.loadedformat));
            }
            else
            {
                PhysX_test2.LogProvider.logMessage("Content system: Loading object:\t\t" + objectname.Substring(0, objectname.Length - 1) + "\t\t type of " + ElementType.ReturnString(pch.loadedformat));
                System.IO.BinaryReader br = new System.IO.BinaryReader(containedpack.fi.OpenRead());
                br.BaseStream.Seek(containedpack.headersize + pch.offset, System.IO.SeekOrigin.Begin);
                byte[] objBuffer = br.ReadBytes(pch.size);
                br.Close();
                pc.loadbody(objBuffer);
                pch.objectReaded = true;
                pch.ReadedContentObject = pc;
            }
            pch.ReadedContentObject.name = objectname;

            return pch.ReadedContentObject;
        }
        
        public void Unload()
        {
            for (int j = 0; j < packs.Length; j++)
                if (packs[j].Objects != null)
                    for (int i = 0; i < packs[j].Objects.Length; i++)
                    {
                        if (packs[j].Objects[i].objectReaded)
                            packs[j].Objects[i].Unload();
                    }
        }

    }
}
