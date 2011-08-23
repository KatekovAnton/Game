using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PhysX_test2.Engine;

namespace PhysX_test2.Content
{
    public class EngineMesh :/*PackContent,*/ IDisposable
    {
        protected VertexBuffer vertexBuffer;
        protected IndexBuffer indexBuffer;
        public VertexDeclaration vertexdeclaration;


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

        protected static void LoadBuffers(Stream stream, out Vertex[] vertices, out int[] indices)
        {
            var reader = new BinaryReader(stream);

            var vertexBufferLength = reader.ReadInt32();
            var indexBufferLength = reader.ReadInt32();

            vertices = new Vertex[vertexBufferLength];
            indices = new int[indexBufferLength];

            for (int i = 0; i < vertexBufferLength; i++)
            {
                vertices[i].position.X = reader.ReadSingle();
                vertices[i].position.Y = reader.ReadSingle();
                vertices[i].position.Z = reader.ReadSingle();

                vertices[i].normal.X = reader.ReadSingle();
                vertices[i].normal.Y = reader.ReadSingle();
                vertices[i].normal.Z = reader.ReadSingle();

                vertices[i].textureCoordinate.X = reader.ReadSingle();
                vertices[i].textureCoordinate.Y = 1.0f - reader.ReadSingle();


            }

            for (int i = 0; i < indexBufferLength; i += 3)
            {
                indices[i + 2] = reader.ReadInt32();
                indices[i + 1] = reader.ReadInt32();
                indices[i] = reader.ReadInt32();
            }
        }

        
        public static EngineMesh FromBuffers(byte[][] buffers)
        {
            //  vertexdeclaration = new VertexPositionNormalTexture();
            Vertex[] vertices;
            ushort[] indices;
            int indicescount = 0, verticescount = 0;
            for (int i = 0; i < buffers.GetLength(0); i++)
            {
                BinaryReader br = new BinaryReader(new MemoryStream(buffers[i]));
                verticescount += br.ReadInt32();
                indicescount += br.ReadInt32();
                br.Close();
            }
            
            vertices = new Vertex[verticescount];
            indices = new ushort[indicescount];
            int vertexoffset = 0;
            int indexoffset = 0;
            for (int i = 0; i < buffers.GetLength(0); i++)
            {
                BinaryReader br = new BinaryReader(new MemoryStream(buffers[i]));
                int currentvert = br.ReadInt32();
                int currentindx  = br.ReadInt32();
               

                for (int bv = vertexoffset; bv < currentvert+vertexoffset; bv++)
                {
                    vertices[bv] = new Vertex(
                        new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                        new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                        new Vector2(br.ReadSingle(), 1.0f - br.ReadSingle()));


                    //скин////////////////////////////////////////////////////////////////////////////////////
                    string sss;
                    int t = br.ReadInt32();
                    for (int b = 0; b < t; b++)
                        sss = br.ReadPackString();

                    int d = br.ReadInt32();
                    br.BaseStream.Seek(d * 4, SeekOrigin.Current);
                    //скин////////////////////////////////////////////////////////////////////////////////////
                }

                for (int bv = indexoffset; bv < currentindx + indexoffset; bv++)
                    indices[bv] = Convert.ToUInt16 (br.ReadInt32() + vertexoffset);
               
                vertexoffset += currentvert;
                indexoffset += currentindx;
                
                br.Close();
            }
            return new EngineMesh(vertices, indices);
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

                int currentvert = buffers[i].vertices.Length;
                int currentindx = buffers[i].indices.Length;


                for (int bv = vertexoffset; bv < currentvert + vertexoffset; bv++)
                {
                    int currentvertex = bv - vertexoffset;
                    vertices[bv] = buffers[i].vertices[currentvertex];


                    //скин////////////////////////////////////////////////////////////////////////////////////
                   /* string sss;
                    int t = br.ReadInt32();
                    for (int b = 0; b < t; b++)
                        sss = br.ReadPackString();

                    int d = br.ReadInt32();
                    br.BaseStream.Seek(d * 4, SeekOrigin.Current);*/
                    //скин////////////////////////////////////////////////////////////////////////////////////
                }

                for (int bv = indexoffset; bv < currentindx + indexoffset; bv++)
                {
                    int currentindex = bv - indexoffset;
                    indices[bv] = Convert.ToUInt16(buffers[i].indices[currentindex]);
                }

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
