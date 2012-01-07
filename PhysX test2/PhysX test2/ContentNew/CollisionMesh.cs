using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using StillDesign.PhysX;
using PhysX_test2.Content;
using System.IO;

namespace PhysX_test2.ContentNew
{
    public class CollisionMesh:IPackContentEntity
    {
        public Vector3[] Vertices;
        public int[] Indices;

        public CollisionMesh()
        {

        }

        public void LoadBody(byte[] buffer)
        {
            BinaryReader br = new BinaryReader(new MemoryStream(buffer));
            Vertices = new Vector3[br.ReadInt32()];
            for (int i = 0; i < Vertices.Length; i++)
                Vertices[i] = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            Indices = new int[br.ReadInt32()];
            for (int i = 0; i < Indices.Length; i++)
                Indices[i] = br.ReadInt32();
            br.Close();

        }

        public TriangleMeshShapeDescription CreateTriangleMeshShape(Core core)
        {
            var triangleMeshDesc = new TriangleMeshDescription()
            {
                VertexCount = this.Vertices.Length,
                TriangleCount = this.Indices.Length

            };

            triangleMeshDesc.AllocateVertices<Vector3>(this.Vertices.Length);
            triangleMeshDesc.AllocateTriangles<uint>(this.Indices.Length);

            foreach (Vector3 vec in this.Vertices)
                triangleMeshDesc.VerticesStream.Write(vec);

            triangleMeshDesc.Flags = 0;

            foreach (int ui in this.Indices)
                triangleMeshDesc.TriangleStream.Write<int>(ui);





            // Two ways on cooking mesh: 1. Saved in memory, 2. Saved in file	
            // Cooking from memory
            MemoryStream stream = new MemoryStream();
            Cooking.InitializeCooking(new ConsoleOutputStream());
            Cooking.CookTriangleMesh(triangleMeshDesc, stream);
            Cooking.CloseCooking();

            stream.Position = 0;

            TriangleMesh pMesh = core.CreateTriangleMesh(stream);


            // Create TriangleMesh above code segment.
            pMesh.SaveToDescription();


            TriangleMeshShapeDescription tmsd = new TriangleMeshShapeDescription();
            tmsd.TriangleMesh = pMesh;
            return tmsd;
        }

        public ConvexShapeDescription CreatreConvexShape(Core core)
        {
            ConvexMeshDescription convexMeshDesc = new ConvexMeshDescription();
            convexMeshDesc.PointCount = Vertices.Length;
            convexMeshDesc.Flags |= ConvexFlag.ComputeConvex;
            convexMeshDesc.AllocatePoints<Vector3>(Vertices.Length);
            for (int i = 0; i < Vertices.Length; i++)
            {
                convexMeshDesc.PointsStream.Write(Vertices[i]);
            }


            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            Cooking.InitializeCooking();
            Cooking.CookConvexMesh(convexMeshDesc, stream);
            Cooking.CloseCooking();

            stream.Position = 0;

            ConvexMesh convexMesh = core.CreateConvexMesh(stream);
            return new ConvexShapeDescription(convexMesh);
        }

        public int GetContentType()
        {
            return ElementType.CollisionMesh;
        }

        public IPackEngineObject CreateEngineObject()
        {
            EngineCollisionMesh ecm = new EngineCollisionMesh();
            
            return null;
        }
    }
}
