using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Logic.SceneGraph
{

    public class OTAABoundingBox
    {
        public BoundingBox XNAbb;
        public OTAABoundingBox() 
        {

        }
        public OTAABoundingBox(Vector3 shapesize)
        {
            XNAbb.Max = new Vector3(shapesize.X / 2.0f, shapesize.Y / 2.0f, shapesize.Z / 2.0f);
            XNAbb.Min = new Vector3(-shapesize.X / 2.0f, -shapesize.Y / 2.0f, -shapesize.Z / 2.0f);
        }
    }
}
