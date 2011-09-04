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

        public EngineSkinnedMesh(GraphicsDevice device, SkinnedVertex[] vertices, ushort[] indices)
        {
            vertexBuffer = new VertexBuffer(device, typeof(SkinnedVertex), vertices.Length, BufferUsage.None);
            vertexBuffer.SetData<SkinnedVertex>(vertices);

            indexBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);
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

        public Skeleton Skeleton
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

        

        

       ~EngineSkinnedMesh()
        {
            if (!Disposed)
                Dispose();
        }
    }
}
