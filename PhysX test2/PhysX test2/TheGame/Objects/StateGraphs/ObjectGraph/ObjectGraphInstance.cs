using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhysX_test2. BaseExtensions.Graph;

namespace PhysX_test2.TheGame.Objects.StateGraphs
{
    public class ObjectGraphInstance : GraphInstance
    {
        public ObjectGraphInstance(ObjectGraphStatic __baseGraph, IGraphUser __linkedObject)
            : base(__baseGraph)
        {
            _linkedObject = __linkedObject;
        }

        public void ReceiveEvent(string __eventName)
        {
            ObjectGraphNode lastnode = _currentNode as ObjectGraphNode;
            ObjectGraphNode newnode = base.Advance(__eventName) as ObjectGraphNode;
            if (newnode != null)
            {
                //evet received, 
                lastnode.OnDeactivate(_linkedObject);
                newnode.OnActivate(_linkedObject);
            }
        }
    }
}
