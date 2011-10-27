using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.BaseExtensions.Graph
{
    public class GraphInstance
    {
        public GraphStatic _baseGraph;

        public GraphNode _currentNode;
        public GraphNodeInctance _currentNodeInstance;

        public GraphInstance(GraphStatic __baseGraph)
        {
            _baseGraph = __baseGraph;
            _currentNode = _baseGraph._nodes[0];
        }

        protected GraphNode Advance(string __eventName)
        {
            GraphNode nextNode = _currentNode.GetNextNode(__eventName);
            if (nextNode != null)
                _currentNode = nextNode;
            else
            {
                //event unused
            }
            return _currentNode;
        }
    }
}
