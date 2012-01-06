using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PhysX_test2.ContentNew
{
    public class EngineMesh : IPackEngineObject
    {
        public bool Disposed;
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;

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
            MyGame.Device.SetVertexBuffer(vertexBuffer);
            MyGame.Device.Indices = indexBuffer;
            MyGame.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexBuffer.VertexCount, 0, indexBuffer.IndexCount / 3);
        }

        public EngineMesh()
        {
            
        }

        public void CreateFromContentEntity(IPackContentEntity[] __contentEntities)
        {
            if (!Disposed)
                Dispose();

            ContentMesh[] buffers = new ContentMesh[__contentEntities.Length];
            for (int i = 0; i < __contentEntities.Length; i++)
            {
                ContentMesh mesh = __contentEntities[i] as ContentMesh;
                if (mesh == null)
                    throw new Exception("wrong object in EngineMesh.CreateFromContentEntity");
                buffers[i] = mesh;
            }

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

            vertexBuffer = new VertexBuffer(MyGame.Device, typeof(Vertex), vertices.Length, BufferUsage.None);
            vertexBuffer.SetData<Vertex>(vertices);

            indexBuffer = new IndexBuffer(MyGame.Device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);
            indexBuffer.SetData<ushort>(indices);

            Disposed = false;
        }

        public void Dispose()
        {
            indexBuffer.Dispose();
            vertexBuffer.Dispose();
            Disposed = true;
        }
    }

    public struct Vertex : IVertexType
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 textureCoordinate;

        public Vertex(
            Vector3 position = new Vector3(),
            Vector3 normal = new Vector3(),
            Vector2 textureCoordinate = new Vector2())
        {
            this.position = position;
            this.normal = normal;
            this.textureCoordinate = textureCoordinate;
        }

        public readonly static VertexDeclaration declaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)

        );

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return declaration; }
        }
    }
}
