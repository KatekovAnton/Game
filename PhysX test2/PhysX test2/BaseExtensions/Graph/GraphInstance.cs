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

        public IGraphUser _linkedObject;

        public GraphInstance(GraphStatic __baseGraph)
        {
            _baseGraph = __baseGraph;
            _currentNode = _baseGraph._nodes[0];
        }

        public void setState(string __nodeName)
        {
            GraphNode node = _baseGraph.GetEdgeWithName(__nodeName);
            if (node == null)
                return;

            if (_currentNode != null)
                _currentNode.OnDeactivate(_linkedObject);

            node.OnActivate(_linkedObject);

            _currentNode = node;
        }

        protected GraphNode Advance(string __eventName)
        {
            GraphEdge nextEdge = _currentNode.GetEdge(__eventName);
            if (nextEdge != null)
            {
                _currentNode = nextEdge._nodeTo;
                nextEdge.OnActivate(_linkedObject);
            }
            else
            {
                //event not exists
            }
            return _currentNode;
        }
    }
}
