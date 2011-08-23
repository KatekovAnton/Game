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
namespace PhysX_test2.Content
{
    public class CollisionMesh:PackContent
    {
        public Vector3[] Vertices
        {
            get;
            protected set;
        }

        public int[] Indices
        {
            get;
            protected set;
        }

        public CollisionMesh()
        {
 
         
        }

  
        public override void loadbody(byte[] buffer)
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
            {
                triangleMeshDesc.VerticesStream.Write(vec);
            }
            triangleMeshDesc.Flags = 0;

            foreach (int ui in this.Indices)
            {
                triangleMeshDesc.TriangleStream.Write<int>(ui);
            }


           

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
        public float? IntersectionClosestSingle(Microsoft.Xna.Framework.Ray ray, Matrix transform)
        {
            var detransform = Matrix.Invert(transform);

            Vector3 p1 = Vector3.Transform(ray.Position, detransform);
            Vector3 p2 = Vector3.Transform(ray.Direction + ray.Position, detransform);
            p2 -= p1;

            var isIntersected = false;
            var distance = 0.0f;
            for (int i = 0; i < Indices.Length; i += 3)
            {
                Vector3 v0 = Vertices[Indices[i]];
                Vector3 v1 = Vertices[Indices[i + 1]] - Vertices[Indices[i]];
                Vector3 v2 = Vertices[Indices[i + 2]] - Vertices[Indices[i]];

                // solution of linear system
                // finds line and plane intersection point (if exists)
                float determinant =
                    -p2.Z * v1.Y * v2.X + p2.Y * v1.Z * v2.X + p2.Z * v1.X * v2.Y
                    - p2.X * v1.Z * v2.Y - p2.Y * v1.X * v2.Z + p2.X * v1.Y * v2.Z;

                if (determinant * determinant < 0.000000001f)
                    continue;

                float kramer = 1.0f / determinant;

                float t1 =
                     (p1.Z * p2.Y * v2.X - p1.Y * p2.Z * v2.X + p2.Z * v0.Y * v2.X
                    - p2.Y * v0.Z * v2.X - p1.Z * p2.X * v2.Y + p1.X * p2.Z * v2.Y
                    - p2.Z * v0.X * v2.Y + p2.X * v0.Z * v2.Y + p1.Y * p2.X * v2.Z
                    - p1.X * p2.Y * v2.Z + p2.Y * v0.X * v2.Z - p2.X * v0.Y * v2.Z) *
                    kramer;

                if (t1 < 0)
                    continue;

                float t2 =
                    -(p1.Z * p2.Y * v1.X - p1.Y * p2.Z * v1.X + p2.Z * v0.Y * v1.X
                    - p2.Y * v0.Z * v1.X - p1.Z * p2.X * v1.Y + p1.X * p2.Z * v1.Y
                    - p2.Z * v0.X * v1.Y + p2.X * v0.Z * v1.Y + p1.Y * p2.X * v1.Z
                    - p1.X * p2.Y * v1.Z + p2.Y * v0.X * v1.Z - p2.X * v0.Y * v1.Z) *
                    kramer;

                if (t2 < 0)
                    continue;

                float t3 =
                    (-p1.Z * v1.Y * v2.X + v0.Z * v1.Y * v2.X + p1.Y * v1.Z * v2.X
                    - v0.Y * v1.Z * v2.X + p1.Z * v1.X * v2.Y - v0.Z * v1.X * v2.Y
                    - p1.X * v1.Z * v2.Y + v0.X * v1.Z * v2.Y - p1.Y * v1.X * v2.Z
                    + v0.Y * v1.X * v2.Z + p1.X * v1.Y * v2.Z - v0.X * v1.Y * v2.Z) *
                    (-kramer);

                if (t3 < 0)
                    continue;

                // (t1>=0 && t2>=0 && t1+t2<=0.5)  => point is on face
                // (t3>0)  =>  point is on positive ray direction
                if (t1 + t2 > 1.0f)
                    continue;

                if (!isIntersected || distance > t3)
                {
                    isIntersected = true;
                    distance = t3;
                }
            }
            if (isIntersected)
                return new float?(distance);
            return null;
        }
        public Vector3? IntersectionClosest(Microsoft.Xna.Framework.Ray ray, Matrix transform)
        {
            var detransform = Matrix.Invert(transform);

            Vector3 p1 = Vector3.Transform(ray.Position, detransform);
            Vector3 p2 = Vector3.Transform(ray.Direction + ray.Position, detransform);
            p2 -= p1;

            var isIntersected = false;
            var distance = 0.0f;
            for (int i = 0; i < Indices.Length; i += 3)
            {
                Vector3 v0 = Vertices[Indices[i]];
                Vector3 v1 = Vertices[Indices[i + 1]] - Vertices[Indices[i]];
                Vector3 v2 = Vertices[Indices[i + 2]] - Vertices[Indices[i]];

                // solution of linear system
                // finds line and plane intersection point (if exists)
                float determinant =
                    -p2.Z * v1.Y * v2.X + p2.Y * v1.Z * v2.X + p2.Z * v1.X * v2.Y
                    - p2.X * v1.Z * v2.Y - p2.Y * v1.X * v2.Z + p2.X * v1.Y * v2.Z;

                if (determinant * determinant < 0.000000001f)
                    continue;

                float kramer = 1.0f / determinant;

                float t1 =
                     (p1.Z * p2.Y * v2.X - p1.Y * p2.Z * v2.X + p2.Z * v0.Y * v2.X
                    - p2.Y * v0.Z * v2.X - p1.Z * p2.X * v2.Y + p1.X * p2.Z * v2.Y
                    - p2.Z * v0.X * v2.Y + p2.X * v0.Z * v2.Y + p1.Y * p2.X * v2.Z
                    - p1.X * p2.Y * v2.Z + p2.Y * v0.X * v2.Z - p2.X * v0.Y * v2.Z) *
                    kramer;

                if (t1 < 0)
                    continue;

                float t2 =
                    -(p1.Z * p2.Y * v1.X - p1.Y * p2.Z * v1.X + p2.Z * v0.Y * v1.X
                    - p2.Y * v0.Z * v1.X - p1.Z * p2.X * v1.Y + p1.X * p2.Z * v1.Y
                    - p2.Z * v0.X * v1.Y + p2.X * v0.Z * v1.Y + p1.Y * p2.X * v1.Z
                    - p1.X * p2.Y * v1.Z + p2.Y * v0.X * v1.Z - p2.X * v0.Y * v1.Z) *
                    kramer;

                if (t2 < 0)
                    continue;

                float t3 =
                    (-p1.Z * v1.Y * v2.X + v0.Z * v1.Y * v2.X + p1.Y * v1.Z * v2.X
                    - v0.Y * v1.Z * v2.X + p1.Z * v1.X * v2.Y - v0.Z * v1.X * v2.Y
                    - p1.X * v1.Z * v2.Y + v0.X * v1.Z * v2.Y - p1.Y * v1.X * v2.Z
                    + v0.Y * v1.X * v2.Z + p1.X * v1.Y * v2.Z - v0.X * v1.Y * v2.Z) *
                    (-kramer);

                if (t3 < 0)
                    continue;

                // (t1>=0 && t2>=0 && t1+t2<=0.5)  => point is on face
                // (t3>0)  =>  point is on positive ray direction
                if (t1 + t2 > 1.0f)
                    continue;

                if (!isIntersected || distance > t3)
                {
                    isIntersected = true;
                    distance = t3;
                }
            }
            if (isIntersected)
                return new Vector3?(Vector3.Transform((p1 + p2 * distance), transform));
            return null;
        }
        public bool IntersectionExist(Microsoft.Xna.Framework.Ray ray, Matrix detransform)
        {
            // var detransform = Matrix.Invert(transform);

            Vector3 p1 = Vector3.Transform(ray.Position, detransform);
          
            
          
            
            Vector3 p2 = Vector3.Transform(ray.Direction + ray.Position, detransform);
            p2 -= p1;


            for (int i = 0; i < Indices.Length; i += 3)
            {
                Vector3 v0 = Vertices[Indices[i]];
                Vector3 v1 = Vertices[Indices[i + 1]] - Vertices[Indices[i]];
                Vector3 v2 = Vertices[Indices[i + 2]] - Vertices[Indices[i]];

                // solution of linear system
                // finds line and plane intersection point (if exists)
                float determinant =
                    -p2.Z * v1.Y * v2.X + p2.Y * v1.Z * v2.X + p2.Z * v1.X * v2.Y
                    - p2.X * v1.Z * v2.Y - p2.Y * v1.X * v2.Z + p2.X * v1.Y * v2.Z;

                if (determinant * determinant < 0.000000001f)
                    continue;

                float kramer = 1.0f / determinant;

                float t1 =
                     (p1.Z * p2.Y * v2.X - p1.Y * p2.Z * v2.X + p2.Z * v0.Y * v2.X
                    - p2.Y * v0.Z * v2.X - p1.Z * p2.X * v2.Y + p1.X * p2.Z * v2.Y
                    - p2.Z * v0.X * v2.Y + p2.X * v0.Z * v2.Y + p1.Y * p2.X * v2.Z
                    - p1.X * p2.Y * v2.Z + p2.Y * v0.X * v2.Z - p2.X * v0.Y * v2.Z) *
                    kramer;

                if (t1 < 0)
                    continue;

                float t2 =
                    -(p1.Z * p2.Y * v1.X - p1.Y * p2.Z * v1.X + p2.Z * v0.Y * v1.X
                    - p2.Y * v0.Z * v1.X - p1.Z * p2.X * v1.Y + p1.X * p2.Z * v1.Y
                    - p2.Z * v0.X * v1.Y + p2.X * v0.Z * v1.Y + p1.Y * p2.X * v1.Z
                    - p1.X * p2.Y * v1.Z + p2.Y * v0.X * v1.Z - p2.X * v0.Y * v1.Z) *
                    kramer;

                if (t2 < 0)
                    continue;

                float t3 =
                    (-p1.Z * v1.Y * v2.X + v0.Z * v1.Y * v2.X + p1.Y * v1.Z * v2.X
                    - v0.Y * v1.Z * v2.X + p1.Z * v1.X * v2.Y - v0.Z * v1.X * v2.Y
                    - p1.X * v1.Z * v2.Y + v0.X * v1.Z * v2.Y - p1.Y * v1.X * v2.Z
                    + v0.Y * v1.X * v2.Z + p1.X * v1.Y * v2.Z - v0.X * v1.Y * v2.Z) *
                    (-kramer);

                if (t3 < 0)
                    continue;

                // (t1>=0 && t2>=0 && t1+t2<=0.5)  => point is on face
                // (t3>0)  =>  point is on positive ray direction
                if (t1 + t2 > 1.0f)
                    continue;

                return true;
            }

            return false;
        }
    }

    

   
}
