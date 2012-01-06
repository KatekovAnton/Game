using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.ContentNew
{
    public class MaterialDescription:IPackContentEntity
    {
        public Lod[] lodMats;

        public class SubsetMaterial
        {
            public string DiffuseTextureName;
        }

        public class Lod
        {
            public SubsetMaterial[] mats;
        }

        public MaterialDescription() 
        { }

        public void LoadBody(byte[] buffer)
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(new System.IO.MemoryStream(buffer));
            int count = br.ReadInt32();
            lodMats = new Lod[count];
            for (int i = 0; i < count; i++)
            {
                lodMats[i] = new Lod();
                int matc = br.ReadInt32();
                lodMats[i].mats = new SubsetMaterial[matc];
                for (int j = 0; j < matc; j++)
                {
                    lodMats[i].mats[j] = new SubsetMaterial();
                    lodMats[i].mats[j].DiffuseTextureName = br.ReadPackString();
                }
            }
        }

        public int GetContentType()
        {
            return ElementType.Material;
        }
    }
}
