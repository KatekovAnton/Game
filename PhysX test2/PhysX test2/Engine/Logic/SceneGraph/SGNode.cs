using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Logic.SceneGraph
{
    public class SGNode
    {
        public static int nodeContainerParam1 = 10;
        public static int nodeContainerParam2 = 3;
        public SGNode ParentNode;
        public SGNode[] Children;
        public BoundingBox boundingBox;
        public int nestingLevel;
        public MyContainer<PivotObject> Entities;

        public SGNode(SGNode _parentNode,BoundingBox _bb, int _level)
        {
            boundingBox = _bb;
            ParentNode = _parentNode;
            nestingLevel = _level;
            Entities = new MyContainer<PivotObject>(nodeContainerParam1, nodeContainerParam2);
           // Entities = new List<WorldObject>();
 
        }
    }
}
