using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace PhysX_test2.ContentNew
{
    public class RenderObjectDescription: IPackContentEntity
    {
        public bool IsShadowCaster = false;
        public bool IsShadowReceiver = false;

        public bool NeedRotate;
        public bool isTransparent;
        public bool isSelfIllumination;

        public class SubSet : ICloneable
        {
            public string[] MeshNames;
            public SubSet(string[] meshnames)
            {
                MeshNames = new string[meshnames.Length];
                meshnames.CopyTo(MeshNames, 0);
            }
            public object Clone()
            {
                return new SubSet(this.MeshNames);
            }
            public override string ToString()
            {
                return ("Count of meshes: " + MeshNames.Length.ToString());
            }
        }

        public class Model
        {
            public List<SubSet> subsets
            {
                get;
                private set;
            }
            public Model()
            {
                subsets = new List<SubSet>();
            }
            public Model(SubSet[] array)
            {
                subsets = new List<SubSet>(array);
            }
        }

        public List<Model> LODs
        {
            get;
            private set;
        }

        public void LoadBody(byte[] array)
        {
            BinaryReader br = new BinaryReader(new MemoryStream(array));
            // matname = br.ReadPackString();
            int lodcount = br.ReadInt32();
            LODs = new List<Model>();
            for (int i = 0; i < lodcount; i++)
            {
                SubSet[] LodSubset = new SubSet[br.ReadInt32()];
                for (int j = 0; j < LodSubset.Length; j++)
                {
                    string[] names = new string[br.ReadInt32()];
                    for (int n = 0; n < names.Length; n++)
                        names[n] = br.ReadPackString();
                    LodSubset[j] = new SubSet(names);
                }
                LODs.Add(new Model(LodSubset));
            }

            IsShadowCaster = br.ReadBoolean();
            IsShadowReceiver = br.ReadBoolean();
            NeedRotate = br.ReadBoolean();
            isTransparent = br.ReadBoolean();
            isSelfIllumination = br.ReadBoolean();

        }
    }
}
