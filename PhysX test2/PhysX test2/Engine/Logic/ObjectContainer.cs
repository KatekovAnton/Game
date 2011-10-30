using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.Engine.Logic
{
    public class ObjectContainer
    {
        MyContainer<PivotObject> mouseRaycastObjects;
        MyContainer<PivotObject> bulletRayCastObjects;
        MyContainer<PivotObject> shadowCasters;
        MyContainer<PivotObject> visibleObjects;
        MyContainer<PivotObject> ligths;

        public ObjectContainer()
        {
            mouseRaycastObjects = new MyContainer<PivotObject>();
            bulletRayCastObjects = new MyContainer<PivotObject>();
            shadowCasters = new MyContainer<PivotObject>();
            visibleObjects = new MyContainer<PivotObject>();
            ligths = new MyContainer<PivotObject>();
        }

        public void addObject(PivotObject newObject)
        { }

        public void removeObject(PivotObject newObject)
        { }
    }
}
