using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;


using Microsoft.Xna.Framework.Graphics;

namespace PhysX_test2.Engine.Logic.SceneGraph
{
    /// <summary>
    /// Axis-aligned bounding shape
    /// </summary>
    public class AABS
    {
        float y;
        public float wx, lz;
       
        public Vector3[] points
        {
            get;
            private set;
        }
        Engine.Logic.SceneGraph.OTAABoundingBox _bb;

        public AABS(Engine.Logic.SceneGraph.OTAABoundingBox bb)
        {
            points = new Vector3[4];
            _bb = bb;
        }

        public void create(Vector3[] Points)
        {
            int indexXmin = 0;
            int indexXmax = 0;
            float sizeXmin = Points[0].X;
            float sizeXmax = Points[0].X;


            int indexZmin = 0;
            int indexZmax = 0;
            float sizeZmin = Points[0].Z;
            float sizeZmax = Points[0].Z;

            float sizeYmin = Points[0].Y;
            float sizeYmax = Points[0].Y;
            for (int i = 0; i < Points.Length; i++)
            {

                if (sizeXmin > Points[i].X)
                {
                    sizeXmin = Points[i].X;
                    indexXmin = i;
                }
                if (sizeXmax < Points[i].X)
                {
                    sizeXmax = Points[i].X;
                    indexXmax = i;
                }

                if (sizeYmin > Points[i].Y)
                    sizeYmin = Points[i].Y;

                if (sizeYmax < Points[i].Y)
                    sizeYmax = Points[i].Y;


                if (sizeZmin > Points[i].Z)
                {
                    sizeZmin = Points[i].Z;
                    indexZmin = i;
                }
                if (sizeZmax < Points[i].Z)
                {
                    sizeZmax = Points[i].Z;
                    indexZmax = i;
                }
            }
            y = ((sizeYmax - sizeYmin) / 2) + sizeYmin;
            wx = sizeXmax - sizeXmin;
            lz = sizeZmax - sizeZmin;
            points[0].X = sizeXmax;
            points[1].X = sizeXmax;
            points[2].X = sizeXmin;
            points[3].X = sizeXmin;

            points[0].Z = sizeZmax;
            points[2].Z = sizeZmax;
            points[1].Z = sizeZmin;
            points[3].Z = sizeZmin;

            points[0].Y =
            points[1].Y =
            points[2].Y =
            points[3].Y = y;

            _bb.XNAbb.Max.X = sizeXmax;
            _bb.XNAbb.Max.Y = sizeYmax;
            _bb.XNAbb.Max.Z = sizeZmax;

            _bb.XNAbb.Min.X = sizeXmin;
            _bb.XNAbb.Min.Y = sizeYmin;
            _bb.XNAbb.Min.Z = sizeZmin;
        }
    }
    public class BB
    {
        float wx, hy, lz;
        public Vector3[] BasicPoints
        {
            get;
            private set;
        }
        public Vector3[] TransformedPoints
        {
            get;
            private set;
        }
        public BB(Content.EngineCollisionMesh cm)
        {
            //bb from basic collision mesh (not transformed)
            if (cm.Vertices.Length > 1)
            {
                int indexXmin = 0;
                int indexXmax = 0;
                float sizeXmin = cm.Vertices[0].X;
                float sizeXmax = cm.Vertices[0].X;


                int indexYmin = 0;
                int indexYmax = 0;
                float sizeYmin = cm.Vertices[0].Y;
                float sizeYmax = cm.Vertices[0].Y;


                int indexZmin = 0;
                int indexZmax = 0;
                float sizeZmin = cm.Vertices[0].Z;
                float sizeZmax = cm.Vertices[0].Z;

                for (int i = 0; i < cm.Vertices.Length; i++)
                {
                    if (sizeXmin > cm.Vertices[i].X)
                    {
                        sizeXmin = cm.Vertices[i].X;
                        indexXmin = i;
                    }
                    if (sizeXmax < cm.Vertices[i].X)
                    {
                        sizeXmax = cm.Vertices[i].X;
                        indexXmax = i;
                    }


                    if (sizeYmin > cm.Vertices[i].Y)
                    {
                        sizeYmin = cm.Vertices[i].Y;
                        indexYmin = i;
                    }
                    if (sizeYmax < cm.Vertices[i].Y)
                    {
                        sizeYmax = cm.Vertices[i].Y;
                        indexYmax = i;
                    }


                    if (sizeZmin > cm.Vertices[i].Z)
                    {
                        sizeZmin = cm.Vertices[i].Z;
                        indexZmin = i;
                    }
                    if (sizeZmax < cm.Vertices[i].Z)
                    {
                        sizeZmax = cm.Vertices[i].Z;
                        indexZmax = i;
                    }
                }
                wx = sizeXmax - sizeXmin;
                hy = sizeYmax - sizeYmin;
                lz = sizeZmax - sizeZmin;
                BasicPoints = new Vector3[8];
                TransformedPoints = new Vector3[8];
                BasicPoints[0].Y = sizeYmax;
                BasicPoints[1].Y = sizeYmax;
                BasicPoints[3].Y = sizeYmax;
                BasicPoints[2].Y = sizeYmax;

                BasicPoints[4].Y = sizeYmin;
                BasicPoints[5].Y = sizeYmin;
                BasicPoints[7].Y = sizeYmin;
                BasicPoints[6].Y = sizeYmin;

                BasicPoints[3].Z = sizeZmax;
                BasicPoints[2].Z = sizeZmax;
                BasicPoints[7].Z = sizeZmax;
                BasicPoints[6].Z = sizeZmax;

                BasicPoints[0].Z = sizeZmin;
                BasicPoints[1].Z = sizeZmin;
                BasicPoints[5].Z = sizeZmin;
                BasicPoints[4].Z = sizeZmin;

                BasicPoints[0].X = sizeXmax;
                BasicPoints[2].X = sizeXmax;
                BasicPoints[6].X = sizeXmax;
                BasicPoints[4].X = sizeXmax;

                BasicPoints[1].X = sizeXmin;
                BasicPoints[3].X = sizeXmin;
                BasicPoints[7].X = sizeXmin;
                BasicPoints[5].X = sizeXmin;
            }
            else
            {
                BasicPoints = new Vector3[0];
                TransformedPoints = new Vector3[0];
            }
        }
        public BB(Vector3 min, Vector3 max)
        {
            BasicPoints = new Vector3[8];
            BasicPoints[0].Y = max.Y;
            BasicPoints[1].Y = max.Y;
            BasicPoints[3].Y = max.Y;
            BasicPoints[2].Y = max.Y;

            BasicPoints[4].Y = min.Y;
            BasicPoints[5].Y = min.Y;
            BasicPoints[7].Y = min.Y;
            BasicPoints[6].Y = min.Y;

            BasicPoints[3].Z = max.Z;
            BasicPoints[2].Z = max.Z;
            BasicPoints[7].Z = max.Z;
            BasicPoints[6].Z = max.Z;

            BasicPoints[0].Z = min.Z;
            BasicPoints[1].Z = min.Z;
            BasicPoints[5].Z = min.Z;
            BasicPoints[4].Z = min.Z;

            BasicPoints[0].X = max.X;
            BasicPoints[2].X = max.X;
            BasicPoints[6].X = max.X;
            BasicPoints[4].X = max.X;

            BasicPoints[1].X = min.X;
            BasicPoints[3].X = min.X;
            BasicPoints[7].X = min.X;
            BasicPoints[5].X = min.X;


            TransformedPoints = new Vector3[8];
        }
        public void Transform(Matrix transformmatrix)
        {
            for (int i = 0; i < BasicPoints.Length; i++)
                TransformedPoints[i] = Vector3.Transform(BasicPoints[i], transformmatrix);
        }

    }
}
