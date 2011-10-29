using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhysX_test2.BaseExtensions.Graph;

namespace PhysX_test2.TheGame.Objects.StateGraphs
{
    /// <summary>
    /// static object graph - graph must be created for each kind of objects
    /// </summary>
    public class ObjectGraphStatic:GraphStatic
    {
        public ObjectGraphStatic(ObjectGraphNode[] __nodes, ObjectGraphEdge[] __edges)
            :base(__nodes, __edges)
        {
 
        }
    }
}
