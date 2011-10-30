using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2. BaseExtensions.Graph;

namespace PhysX_test2.TheGame.Objects.StateGraphs
{
    public class ObjectGraphEdge:GraphEdge
    {
        _objectEventHandler _eventHandler;

        public override void OnActivate(IGraphUser __parameter)
        {
            _eventHandler(__parameter);
        }
    }
}
