using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.BaseExtensions.Graph
{
    public class GraphEdge
    {
        public GraphNode _nodeFrom;
        public GraphNode _nodeTo;

        /// <summary>
        /// probability for multiple edges with same _eventName and diffr _nodeTo
        /// </summary>
        public float _chance = 1.0f;

        public string _eventName;
    }
}
