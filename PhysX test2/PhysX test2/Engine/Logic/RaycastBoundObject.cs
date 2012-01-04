using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using PhysX_test2.Content;
using Microsoft.Xna.Framework;
namespace PhysX_test2.Engine.Logic
{
    public class RaycastBoundObject
    {
        public EngineCollisionMesh RCCM;
        public SceneGraph.OTBoundingShape boundingShape;
        public float? IntersectionClosestSingle(Ray ray, Matrix transform)
        {
            float? disttobb = boundingShape.aabb.XNAbb.Intersects(ray);
            if (disttobb == null)
                return null;
            disttobb = RCCM.IntersectionClosestSingle(ray, transform);
            return disttobb;
        }

        public Vector3? IntersectionClosest(Ray ray, Matrix transform, ref Vector3 normal)
        {
            float? disttobb = boundingShape.aabb.XNAbb.Intersects(ray);
            if (disttobb == null)
                return null;


            Vector3? point = RCCM.IntersectionClosest(ray, transform, ref normal);
            return point;
        }

        public RaycastBoundObject(SceneGraph.OTBoundingShape bb, EngineCollisionMesh _RCCM)
        {
            boundingShape = bb;
            RCCM = _RCCM != null ? _RCCM : bb.cm;
        }

    }
}
