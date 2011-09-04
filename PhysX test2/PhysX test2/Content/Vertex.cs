using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace PhysX_test2.Content
{
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

