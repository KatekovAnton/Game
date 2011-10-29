using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhysX_test2. BaseExtensions.Graph;

namespace PhysX_test2.TheGame.Objects.StateGraphs
{
    public class ObjectGraphController : GraphController
    {
        public ObjectGraphController(ObjectGraphStatic __baseGraph, IGraphUser __linkedObject)
            : base(__baseGraph, __linkedObject)
        {
        }
    }
}
