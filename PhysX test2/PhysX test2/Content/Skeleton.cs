using System;
using System.Collections.Generic;
using System.Linq;
using BoneMap = System.Collections.Generic.Dictionary<string, int>;
using System.IO;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Content
{
    public class Bone
    {
        public int level;
        public List<Bone> Childrens;
        public string Name
        {
            get;
            set;
        }

        public Matrix BaseInverseMatrix
        {
            get;
            set;
        }

        public float Length
        {
            get;
            set;
        }

        public Bone Parent
        {
            get;
            set;
        }
    }

    public class Skeleton
    {
        public Bone[] bones;
        public Bone Root;
        public BoneMap map;

        protected Skeleton(Bone root, Bone[] bones)
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

        public static Skeleton FromStream(Stream stream)
        {
            var self = new BinaryReader(stream);
            var bones = new Bone[self.ReadInt32()];
            var parentNames = new string[bones.Length];
            Bone root = null;

            for (int i = 0; i < bones.Length; i++)
            {
                bones[i] = new Bone();
                bones[i].Name = self.ReadPackString();
                parentNames[i] = self.ReadPackString();
                if (parentNames[i] == "-\0")
                {
                    root = bones[i];
                }
            }

            if (root == null)
            {
                throw new Exception("Root bone can not be null");
            }

            var skeleton = new Skeleton(root, bones);

            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].BaseInverseMatrix = self.ReadMatrix();
                if (bones[i] != root)
                    bones[i].Parent = bones[skeleton.IndexOf(parentNames[i])];
                
            }

            return skeleton;
        }
    }
}
