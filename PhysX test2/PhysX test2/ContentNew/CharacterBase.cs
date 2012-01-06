using System;
using System.Collections.Generic;
using System.Linq;
using BoneMap = System.Collections.Generic.Dictionary<string, int>;
using System.IO;

using Microsoft.Xna.Framework;


namespace PhysX_test2.ContentNew
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

    public class CharacterBase : IPackContentEntity
    {
        public Bone[] bones;
        public Bone Root;
        public BoneMap map;

        public int[] botomindexes;
        public int[] topindexes;

        public int HeadIndex;
        public int WeaponIndex;
        public int RootIndex;
        public int TopRootIndex;
        public int BottomRootIndex;

        public Matrix HeadMatrix;
        public Matrix WeaponMatrix;

        public byte[] data;

        public CharacterBase()
        {

        }

        public int IndexOf(string name)
        {
            return map[name];
        }

        public void LoadBody(byte[] array)
        {
            data = array;
            var self = new BinaryReader(new MemoryStream(array));
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

            Root = root;
            this.bones = bones;

            map = new BoneMap(bones.Length);
            for (int i = 0; i < bones.Length; i++)
                map.Add(bones[i].Name, i);

            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].BaseInverseMatrix = self.ReadMatrix();
                if (bones[i] != root)
                    bones[i].Parent = bones[IndexOf(parentNames[i])];

            }



            HeadIndex = self.ReadInt32();
            WeaponIndex = self.ReadInt32();

            HeadMatrix = self.ReadMatrix();
            WeaponMatrix = self.ReadMatrix();

            RootIndex = self.ReadInt32();
            TopRootIndex = self.ReadInt32();
            BottomRootIndex = self.ReadInt32();
            botomindexes = new int[self.ReadInt32()];
            for (int i = 0; i < botomindexes.Length; i++)
                botomindexes[i] = self.ReadInt32();

            topindexes = new int[self.ReadInt32()];
            for (int i = 0; i < topindexes.Length; i++)
                topindexes[i] = self.ReadInt32();
        }

        public int GetContentType()
        {
            return ElementType.SkeletonWithAddInfo;
        }
    }
}