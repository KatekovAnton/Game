using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PhysX_test2.ContentNew
{
    public class EngineMeshSkinned : IPackEngineObject
    {
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

        public CharacterBase Skeleton;
        
        public bool Disposed;

        public void CreateFromContentEntity(IPackContentEntity[] __contentEntities)
        {
            if (!Disposed)
                Dispose();

            ContentMeshSkinned[] buffers = new ContentMeshSkinned[__contentEntities.Length];
            for (int i = 0; i < __contentEntities.Length; i++)
            {
                ContentMeshSkinned mesh = __contentEntities[i] as ContentMeshSkinned;
                if (mesh == null)
                    throw new Exception("wrong object in EngineMesh.CreateFromContentEntity");
                buffers[i] = mesh;
            }

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
                ContentMeshSkinned cm = buffers[i];
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

            vertexBuffer = new VertexBuffer(MyGame.Device, typeof(SkinnedVertex), vertices.Length, BufferUsage.None);
            vertexBuffer.SetData<SkinnedVertex>(vertices);

            indexBuffer = new IndexBuffer(MyGame.Device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);
            indexBuffer.SetData<ushort>(indices);

            Disposed = false;
        }

        public void Render()
        {
            MyGame.Device.SetVertexBuffer(vertexBuffer);
            MyGame.Device.Indices = indexBuffer;
            MyGame.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexBuffer.VertexCount, 0, indexBuffer.IndexCount / 3);
        }


        public void Dispose()
        {
            indexBuffer.Dispose();
            vertexBuffer.Dispose();
            Disposed = true;
        }

        public bool needAutoDispose()
        {
            return true;
        }
    }

    public struct SkinnedVertex : IVertexType
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 textureCoordinate;
        public Vector3 boneIndices;
        public Vector3 boneWeights;

        public SkinnedVertex(
            Vector3 _position = new Vector3(),
            Vector3 _normal = new Vector3(),
            Vector2 _textureCoordinate = new Vector2(),
            Vector3 _idices = new Vector3(),
            Vector3 _weights = new Vector3())
        {
            this.position = _position;
            this.normal = _normal;
            this.textureCoordinate = _textureCoordinate;
            this.boneIndices = _idices;
            this.boneWeights = _weights;
        }

        public readonly static VertexDeclaration declaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(32, VertexElementFormat.Vector3, VertexElementUsage.BlendIndices, 0),
            new VertexElement(44, VertexElementFormat.Vector3, VertexElementUsage.BlendWeight, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return declaration; }
        }
    }
}
