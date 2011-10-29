using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2;

namespace PhysX_test2.BaseExtensions.Graph
{
    public delegate void _objectEventHandler(IGraphUser __object);
    public class GraphNode
    {
        public string _name;
        public MyContainer<GraphEdge> _outEdges;

        private MyContainer<GraphEdge> buffer;


        public _objectEventHandler _onDeactivate;
        public _objectEventHandler _onActivate;
        public _objectEventHandler _onUpdate;


        public GraphNode()
        {
            buffer = new MyContainer<GraphEdge>(10, 1);
            _outEdges = new MyContainer<GraphEdge>();
        }

        public GraphEdge GetEdge(string __eventName)
        {
            buffer.Clear();
            float chanceSum = 0.0f;
            foreach (GraphEdge edge in _outEdges)
                if (edge._eventName.CompareTo(__eventName) == 0)
                {
                    buffer.Add(edge);
                    chanceSum += edge._chance;
                }

            if (buffer.Count == 0)
                return null;
            if (buffer.Count == 1)
                return buffer[0];

            //несколько нодов, надо найти в какой нод мы переходим
            float chance = (((float)Extensions.GetRandomizer().Next(0, 10000)) / 10000.0f) * chanceSum;
            float currentChance = 0.0f;
            foreach (GraphEdge edge in buffer)
            {
                if ((chance >= currentChance) && (chance <= currentChance + edge._chance))
                    return edge;

                currentChance += edge._chance;
            }
            throw new Exception("Wrong calculation of chance");
            return null;
        }

        public virtual void OnDeactivate(IGraphUser __object)
        {
            _onDeactivate(__object);
        }

        public virtual void OnActivate(IGraphUser __object)
        {
            _onActivate(__object);
        }

        public virtual void OnUpdate(IGraphUser __object)
        {
            _onUpdate(__object);
        }

        //чтото я тут хотел сделать публ вирт
    }
}
