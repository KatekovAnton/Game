using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PhysX_test2.Engine;

namespace PhysX_test2.Content
{
    public class EngineMesh : IDisposable
    {
        protected VertexBuffer vertexBuffer;
        protected IndexBuffer indexBuffer;
        //public VertexDeclaration vertexdeclaration;


        public EngineMesh(Vertex[] vertices, ushort[] indices)
        {
            vertexBuffer = new VertexBuffer(GameEngine.Device, typeof(Vertex), vertices.Length, BufferUsage.None);
            vertexBuffer.SetData<Vertex>(vertices);

            indexBuffer = new IndexBuffer(GameEngine.Device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);
            indexBuffer.SetData<ushort>(indices);
        }
        
        public EngineMesh()
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
            Disposed = true;
        }
      
        public static EngineMesh FromContentMeshes(ContentMesh[] buffers)
        {
            //  vertexdeclaration = new VertexPositionNormalTexture();
            Vertex[] vertices;
            ushort[] indices;
            int indicescount = 0, verticescount = 0;
            for (int i = 0; i < buffers.Length; i++)
            {
                verticescount += buffers[i].vertices.Length;
                indicescount += buffers[i].indices.Length;
       
            }

            vertices = new Vertex[verticescount];
            indices = new ushort[indicescount];
            int vertexoffset = 0;
            int indexoffset = 0;
            for (int i = 0; i < buffers.Length; i++)
            {
                ContentMesh cm = buffers[i];
                int currentvert = cm.vertices.Length;
                int currentindx = cm.indices.Length;


                for (int ci = 0; ci < currentvert; ci++)
                {
                    vertices[ci + vertexoffset] = new Vertex(cm.vertices[ci].position, cm.vertices[ci].normal, cm.vertices[ci].textureCoordinate);
                    vertices[ci + vertexoffset].textureCoordinate.Y = 1.0f - vertices[ci + vertexoffset].textureCoordinate.Y;
                }

                for (int ci = 0; ci < currentindx; ci++)
                    indices[ci + indexoffset] = Convert.ToUInt16(cm.indices[ci] + vertexoffset);

                vertexoffset += currentvert;
                indexoffset += currentindx;
            }
            return new EngineMesh(vertices, indices);
        }
        ~EngineMesh()
        {
            if (!Disposed)
                Dispose();
        }
    }
}
