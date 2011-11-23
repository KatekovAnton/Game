using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Logic.SceneGraph
{
    public class SceneGraph
    {
        EngineScene _scene;
        SGQdTree _octree;

        public SceneGraph(EngineScene __scene)
        {
            _octree = new SGQdTree();
            _scene = __scene;
        }

        public void AddObject(PivotObject __wo)
        {
            _octree.AddObject(__wo);
        }

        public void RemoveObject(PivotObject __wo)
        {
            _octree.RemoveObject(__wo);
        }

        public void NewFrame()
        {
            _octree.Update(_scene._objects);
        }

        public void calculateVisibleObjects(BoundingFrustum __viewFrustum, MyContainer<PivotObject> __container)
        {
            __container.Clear();
            _octree.Query(__viewFrustum, __container);
        }

        public void calculateShadowVisibleObjects(BoundingFrustum __lightViewFrustum, MyContainer<PivotObject> __container)
        {
            __container.Clear();
            _octree.Query(__lightViewFrustum, __container);
        }

        public void calculateBoxIntersectedObjects(BoundingBox __objectBox, MyContainer<PivotObject> __container)
        {
            __container.Clear();
            _octree.Query(__objectBox, __container);
        }

        public void Clear()
        {
            foreach (PivotObject lo in _scene._objects)
            {
                _octree.RemoveObject(lo);
            }
            //objects.Clear();
        }

        public int recalulcalated()
        {
            return _octree._entityRecalculateCount;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="__oldObject"></param>
        /// <param name="__newObject"></param>
        /// <param name="__recalculate"></param>
        public void SwapObjects(PivotObject __oldObject, PivotObject __newObject, bool __recalculate)
        {
            if (__recalculate)
            {
                _octree.RemoveObject(__oldObject);
                _octree.AddObject(__newObject);
            }
            else
            {
                _octree.SwapObjects(__oldObject, __newObject);
            }

        }
    }
}
