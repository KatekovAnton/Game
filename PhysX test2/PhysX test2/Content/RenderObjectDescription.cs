using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysX_test2.Content
{
    
    public class RenderObjectDescription: PackContent
    {
        
        public Pack pack;
        //отбрасывает ли тени
        public bool IsShadowCaster = false;

        //принимает ли тени(затеняется или нет)
        public bool IsShadowReceiver = false;

        public bool NeedRotate;


        public void addlod()
        {
            LODs.Add(new Model());
        }

   
        public RenderObjectDescription()
        {
           // this.ShapeType = 1;
            LODs = new List<Model>();
           
        }

        //к меш листу надо ассоциировать текстуру. это субсет. при загрузке мешлист группируется в один меш.
        //набор субсетов - это модель одного лода. рендеробжект - набор нескольких моделей(разной детализации)
        public class SubSet : ICloneable
        {
            public string[] MeshNames;
           // public string TextureName;


            public SubSet(string[] meshnames/*, string texturename*/)
            {
                MeshNames = new string[meshnames.Length];
                meshnames.CopyTo(MeshNames, 0);
             //   TextureName = texturename;
            }
            public object Clone()
            {
                return new SubSet(this.MeshNames/*, this.TextureName*/);
            }
            public override string ToString()
            {
                return ("Count of meshes: " + MeshNames.Length.ToString());
                    //+ "; " + (TextureName == "" ? "Texture name empty" : TextureName.Substring(0, TextureName.Length - 1));
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


        public override void loadbody(byte[] array)
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
                    LodSubset[j] = new SubSet(names/*, br.ReadPackString()*/);
                }
                LODs.Add(new Model(LodSubset));
            }

            IsShadowCaster = br.ReadBoolean();
            IsShadowReceiver = br.ReadBoolean();
            NeedRotate = br.ReadBoolean();
        }
    }
}
