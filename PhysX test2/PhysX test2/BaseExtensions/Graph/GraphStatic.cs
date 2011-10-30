using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.BaseExtensions.Graph
{
    public class GraphStatic
    {
        public GraphNode[] _nodes;
        public GraphEdge[] _edges;

        public GraphStatic(GraphNode[] __nodes, GraphEdge[] __edges)
        {
            _nodes = __nodes;
            _edges = __edges;
        }

        public GraphNode GetEdgeWithName(string __name)
        {
            foreach (GraphNode node in _nodes)
                if (node._name.CompareTo(__name) == 0)
                    return node;
            return null;
        }
    }
}
