using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Logic.SceneGraph
{
    public class SceneGraph
    {
        MyContainer<PivotObject> objects;
        SGQdTree octree;
        public MyContainer<PivotObject> visibleObjects;
        public MyContainer<PivotObject> shadowObjects;
        public SceneGraph()
        {
            octree = new SGQdTree();
            objects = new MyContainer<PivotObject>();
            shadowObjects = new MyContainer<PivotObject>();
            visibleObjects = new MyContainer<PivotObject>();
        }
        public void AddObject(PivotObject wo)
        {
            objects.Add(wo);
            octree.AddEntity(wo);
        }
        public void NewFrame()
        {
            octree.Update(objects);
        }
        public void calculateVisibleObjects(BoundingFrustum _viewFrustum)
        {
            visibleObjects.Clear();
            octree.Query(_viewFrustum, visibleObjects);
        }
        public void calculateShadowVisibleObjects(BoundingFrustum _lightViewFrustum)
        {
            shadowObjects.Clear();
            octree.Query(_lightViewFrustum, shadowObjects);
        }
        public int recalulcalated()
        {
            return octree._entityRecalculateCount;
        }
    }
}
