using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using BoneMap = System.Collections.Generic.Dictionary<string, int>;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Animation
{
    public class Bone
    {
        public int level;
        public string Name;
        public int index;
        public Matrix BaseMatrix;
        public Bone[] Childrens;
        public Bone Parent;

        public Bone()
        {
            
        }

        public override string ToString()
        {
            return index.ToString() + " " + Name;
        }
    }

    public class Skeleton
    {
        public Bone[] bones;
        public BoneMap map;
        public Bone Root;

        public Skeleton()
        { }

        public void Init(Bone root, Bone[] bones)
        {
            Root = root;
            this.bones = bones;

            map = new BoneMap(bones.Length);
            for (int i = 0; i < bones.Length; i++)
                map.Add(bones[i].Name, i);
        }

        

        public int IndexOf(string name)
        {
            return map[name];
        }

        public int RootIndex
        {
            get { return map[Root.Name]; }
        }

        public static Skeleton FromStream(BinaryReader stream)
        {
            var bones = new Bone[stream.ReadInt32()];
            var parentNames = new string[bones.Length];
            Bone root = null;

            for (int i = 0; i < bones.Length; i++)
            {
                bones[i] = new Bone();
                bones[i].index = i;
                bones[i].Name = stream.ReadPackString();
                parentNames[i] = stream.ReadPackString();
                if (parentNames[i] == "-\0")
                    root = bones[i];
            }

            if (root == null)
                throw new Exception("Root bone can not be null");

            var skeleton = new Skeleton();
            skeleton.Init(root, bones);
            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].BaseMatrix = stream.ReadMatrix();
                if (bones[i] != root)
                    bones[i].Parent = bones[skeleton.IndexOf(parentNames[i])];
            }
            foreach (Bone b in bones)
            {
                List<Bone> chl = new List<Bone>();
                for (int i = 0; i < bones.Length; i++)
                    if (bones[i].Parent == b)
                        chl.Add(bones[i]);
                b.Childrens = chl.ToArray();
            }


            return skeleton;
        }
    }
}
