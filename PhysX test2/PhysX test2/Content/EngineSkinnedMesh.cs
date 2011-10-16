using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PhysX_test2.Engine;

namespace PhysX_test2.Content
{
    public class EngineSkinnedMesh : IDisposable
    {
        protected VertexBuffer vertexBuffer;
        protected IndexBuffer indexBuffer;

        public EngineSkinnedMesh(SkinnedVertex[] vertices, ushort[] indices)
        {
            vertexBuffer = new VertexBuffer(GameEngine.Device, typeof(SkinnedVertex), vertices.Length, BufferUsage.None);
            vertexBuffer.SetData<SkinnedVertex>(vertices);

            indexBuffer = new IndexBuffer(GameEngine.Device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);
            indexBuffer.SetData<ushort>(indices);
        }

        public EngineSkinnedMesh()
        {
        }

        public VertexBuffer VertexBuffer
        {
            get { return vertexBuffer; }
        }

        public IndexBuffer IndexBuffer
        {
            get { return indexBuffer; }
        }

        public CharacterContent Skeleton
        {
            get;
            set;
        }

        public void Render()
        {
            GameEngine.Device.SetVertexBuffer(vertexBuffer);
            GameEngine.Device.Indices = indexBuffer;
            GameEngine.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexBuffer.VertexCount, 0, indexBuffer.IndexCount / 3);
        }

        public bool Disposed;
        public void Dispose()
        {
            indexBuffer.Dispose();
            vertexBuffer.Dispose();
        }


        public static EngineSkinnedMesh FromContentMeshes(ContentSkinnedMesh[] buffers)
        {
            //  vertexdeclaration = new VertexPositionNormalTexture();
            SkinnedVertex[] vertices;
            ushort[] indices;
            int indicescount = 0, verticescount = 0;
            for (int i = 0; i < buffers.Length; i++)
            {
                verticescount += buffers[i].vertices.Length;
                indicescount += buffers[i].indices.Length;

            }

            vertices = new SkinnedVertex[verticescount];
            indices = new ushort[indicescount];
            int vertexoffset = 0;
            int indexoffset = 0;
            for (int i = 0; i < buffers.Length; i++)
            {
                ContentSkinnedMesh cm = buffers[i];
                int currentvert = cm.vertices.Length;
                int currentindx = cm.indices.Length;


                for (int ci = 0; ci < currentvert; ci++)
                {
                    vertices[ci + vertexoffset] = new SkinnedVertex(
                        cm.vertices[ci].position, 
                        cm.vertices[ci].normal, 
                        cm.vertices[ci].textureCoordinate,
                        cm.vertices[ci].boneIndices,
                        cm.vertices[ci].boneWeights);
                    vertices[ci + vertexoffset].textureCoordinate.Y = 1.0f - vertices[ci + vertexoffset].textureCoordinate.Y;
                
                }

                for (int ci = 0; ci < currentindx; ci++)
                    indices[ci + indexoffset] = Convert.ToUInt16(cm.indices[ci] + vertexoffset);

                vertexoffset += currentvert;
                indexoffset += currentindx;
            }
            return new EngineSkinnedMesh(vertices, indices);
        }
        

       ~EngineSkinnedMesh()
        {
            if (!Disposed)
                Dispose();
        }
    }
}
