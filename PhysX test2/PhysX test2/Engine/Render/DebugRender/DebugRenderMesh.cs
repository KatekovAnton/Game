using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using StillDesign.PhysX;
using PhysX_test2.Content;
namespace PhysX_test2.Engine.DebugRender
{
    class DebugRenderMesh:System.IDisposable
    {
       
            protected VertexBuffer vertexBuffer;
            protected IndexBuffer indexBuffer;

            public DebugRenderMesh(GraphicsDevice device, CollisionMesh cm)
            {
                VertexPositionColor[] vertices = new VertexPositionColor[cm.Vertices.Length];
                int[] indices = new int[cm.Indices.Length];
                for (int i = 0; i < vertices.Length; i++)
                    vertices[i] = new VertexPositionColor(cm.Vertices[i], Color.LightGray);
                cm.Indices.CopyTo(indices, 0);

                vertexBuffer = new VertexBuffer(device, typeof(VertexPositionColor), vertices.Length, BufferUsage.None);
                vertexBuffer.SetData<VertexPositionColor>(vertices);

                indexBuffer = new IndexBuffer(device, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.None);
                indexBuffer.SetData<int>(indices);
            }

            public VertexBuffer VertexBuffer
            {
                get { return vertexBuffer; }
            }

            public IndexBuffer IndexBuffer
            {
                get { return indexBuffer; }
            }

            public void Render(GraphicsDevice device)
            {
                device.SetVertexBuffer(vertexBuffer);
                device.Indices = indexBuffer;
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexBuffer.VertexCount, 0, indexBuffer.IndexCount / 3);
            }

            public void Dispose()
            {
                indexBuffer.Dispose();
                vertexBuffer.Dispose();
            }
    }
}
