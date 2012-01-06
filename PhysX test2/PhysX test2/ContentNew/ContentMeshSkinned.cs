using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using System.IO;

namespace PhysX_test2.ContentNew
{
    public class ContentMeshSkinned:IPackContentEntity
    {
        public SkinnedVertex[] vertices;
        public int[] indices;
        public static CharacterBase skeleton;

        public void LoadBody(byte[] buffer)
        {
            //  vertexdeclaration = new VertexPositionNormalTexture();
            BinaryReader br = new BinaryReader(new MemoryStream(buffer));
            vertices = new SkinnedVertex[br.ReadInt32()];
            indices = new int[br.ReadInt32()];

            for (int bv = 0; bv < vertices.Length; bv++)
            {
                Vector3 pos = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                Vector3 nor = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                Vector2 tcr = new Vector2(br.ReadSingle(), br.ReadSingle());

                Vector3 bindx = new Vector3();
                Vector3 bwigs = new Vector3();

                int relationBoneCount = br.ReadInt32();
                if (relationBoneCount > 3)
                    throw new Exception("Too many bones in skin!!!");
                var boneIndices = new int[] { 0, 0, 0 };
                for (var j = 0; j < relationBoneCount; j++)
                {
                    string name = br.ReadPackString();
                    boneIndices[j] = skeleton.IndexOf(name);
                }
                bindx.X = boneIndices[0];
                bindx.Y = boneIndices[1];
                bindx.Z = boneIndices[2];


                relationBoneCount = br.ReadInt32();
                float[] boneWeight = new float[] { 0.0f, 0.0f, 0.0f };
                for (var j = 0; j < relationBoneCount; j++)
                    boneWeight[j] = br.ReadSingle();
                bwigs.X = boneWeight[0];
                bwigs.Y = boneWeight[1];
                bwigs.Z = boneWeight[2];


                vertices[bv] = new SkinnedVertex(
                    pos,
                    nor,
                    tcr,
                    bindx,
                    bwigs);
            }

            for (int bv = 0; bv < indices.Length; bv++)
                indices[bv] = br.ReadInt32();

            br.Close();
        }

        public int GetContentType()
        {
            return ElementType.MeshOptimazedForLoading;
        }
    }
}
