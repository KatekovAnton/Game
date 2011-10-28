using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Logic.SceneGraph
{
    public class SceneGraph
    {
        EngineScene scene;
        SGQdTree octree;

        public SceneGraph(EngineScene _scene)
        {
            octree = new SGQdTree();
            scene = _scene;
        }
        public void AddObject(PivotObject wo)
        {
            // objects.Add(wo);
            octree.AddEntity(wo);
        }
        public void DeleteObject(PivotObject wo)
        {
            // objects.Add(wo);
            octree.RemoveObject(wo);
        }
        public void NewFrame()
        {
            octree.Update(scene.objects);
        }
        public void calculateVisibleObjects(BoundingFrustum _viewFrustum, MyContainer<PivotObject> container)
        {
            container.Clear();
            octree.Query(_viewFrustum, container);
        }
        public void calculateShadowVisibleObjects(BoundingFrustum _lightViewFrustum, MyContainer<PivotObject> container)
        {
            container.Clear();
            octree.Query(_lightViewFrustum, container);
        }
        public void Clear()
        {
            foreach (PivotObject lo in scene.objects)
            {
                octree.RemoveObject(lo);
            }
            //objects.Clear();
        }
        public int recalulcalated()
        {
            return octree._entityRecalculateCount;
        }
    }
}
