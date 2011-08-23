using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;



namespace PhysX_test2.Engine.Logic.SceneGraph
{
    public class OTBoundingShape
    {
        public PhysX_test2.Content.EngineCollisionMesh cm;
        public OTAABoundingBox aabb;
        public BB bb;
        public AABS aabr;

        public OTBoundingShape(Vector3 v)
        {
            aabb = new OTAABoundingBox(v);
            bb = new BB(aabb.XNAbb.Min, aabb.XNAbb.Max);
            aabr = new AABS(aabb);
        }

        public OTBoundingShape(PhysX_test2.Content.EngineCollisionMesh _cm)
        {
            cm = _cm;
            aabb = new OTAABoundingBox();
            bb = new BB(cm);
            aabr = new AABS(aabb);
        }

        public void Update(Matrix newglobalpose)
        {
            bb.Transform(newglobalpose);
            aabr.create(bb.TransformedPoints);
        }
    }
}
