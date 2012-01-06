using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Microsoft.Xna.Framework;

namespace PhysX_test2.ContentNew
{
    public class ContentMesh: PackContentEntity
    {
        public Vertex[] vertices;
        public int[] indices;

        public ContentMesh()
        {
 
        }

        public void LoadBody(byte[] buffer)
        {
            //  vertexdeclaration = new VertexPositionNormalTexture();
            BinaryReader br = new BinaryReader(new MemoryStream(buffer));
            vertices = new Vertex[br.ReadInt32()];
            indices = new int[br.ReadInt32()];

            for (int bv = 0; bv < vertices.Length; bv++)
            {
                vertices[bv] = new Vertex(
                    new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                    new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                    new Vector2(br.ReadSingle(), br.ReadSingle()));

                string sss;
                int t = br.ReadInt32();
                for (int i = 0; i < t; i++)
                    sss = br.ReadPackString();

                t = br.ReadInt32();

                br.BaseStream.Seek(t * 4, SeekOrigin.Current);
            }

            for (int bv = 0; bv < indices.Length; bv++)
                indices[bv] = br.ReadInt32();

            br.Close();
        }
    }
}
