using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.Engine.Animation
{
    public class NodeEvent                                  // событие которое возможно для конкретного узла графа анимаций- ребро графа анимаций
    {
        public string neededEvent;                          // необходимое событие
        public AnimationNode parentNode;                        // ссылка на владельца 
        public AnimationNode associatedNode;                // узел к которому ведет событие 
        public string description;// описание 

        public string parentName;
        public string associatedNodeName;

        public NodeEvent(string _description, AnimationNode _parent, AnimationNode _associatedNode, string _neededevent)
        {
            SetDescription(_description);
            associatedNode = _associatedNode;
            parentNode = _parent;


            SetNeededEvevntName(_neededevent);
            SetParentNodeName(parentNode.name);
            SetAssocNodeName(associatedNode.name);
        }

        public void SetNeededEvevntName(string _name)
        {
            neededEvent = _name;
            if (!(neededEvent[neededEvent.Length - 1] == '\0'))
                neededEvent += "\0";
        }

        public void SetDescription(string _description)
        {
            description = _description;
            if (!(description[description.Length - 1] == '\0'))
                description += "\0";
        }

        public void SetParentNodeName(string _name)
        {
            parentName = _name;
            if (!(parentName[parentName.Length - 1] == '\0'))
                parentName += "\0";
        }

        public void SetAssocNodeName(string _name)
        {
            associatedNodeName = _name;
            if (!(associatedNodeName[associatedNodeName.Length - 1] == '\0'))
                associatedNodeName += "\0";
        }

        public NodeEvent()
        {
        }

        public override string ToString()
        {
            return description;
        }

        public static NodeEvent NodeEventFromStream(System.IO.BinaryReader br)
        {
            NodeEvent @event = new NodeEvent();
            @event.neededEvent = br.ReadPackString();
            @event.description = br.ReadPackString();

            @event.parentName = br.ReadPackString();
            @event.associatedNodeName = br.ReadPackString();
            return @event;
        }
    }

    public class AnimationNode                              // узел графа анимаций
    {
        public string name;
        public NodeProperties Properties;
        public int index;
        public NodeEvent[] nodeEvents;                      // исходящие рёбра
        public Animation animation;                         // соответствующая узлу анимация
        
        public float animationSpeed = 0.075f;
        public Dictionary<string, string> parameters;

        public AnimationNode(string _name, Animation _animation)
        {
            Properties = new NodeProperties();
            SetName(_name);
            animation = _animation;
        }
        public AnimationNode(string _name, Animation _animation, NodeProperties _Properties)
        {
            SetName(_name);
            animation = _animation;
            Properties = _Properties;
        }
        public AnimationNode(string _name)
        {
            Properties = new NodeProperties();
            SetName(_name);
        }

        public void SetName(string _name)
        {
            name = _name;
            if (!(name[name.Length - 1] == '\0'))
                name += "\0";
        }

        public AnimationNode Advance(string _event) // обработка перехода к след узлу графа анмаций по событию
        {
            for (int i = 0; i < nodeEvents.Length; i++)
                if (nodeEvents[i].neededEvent.CompareTo(_event) == 0)
                    return nodeEvents[i].associatedNode;

            return this;
        }

        public static AnimationNode AnimationNodeFromStream(System.IO.BinaryReader br)
        {
            int index = br.ReadInt32();
            AnimationNode node = new AnimationNode(br.ReadPackString());
            node.index = index; // br.ReadInt32();
            int animType = br.ReadInt32();

            switch (animType)
            {
                case 0:
                    node.animation = FullAnimation.FullAnimationFromStream(br);
                    break;
                default:
                    break;
            }
            int count = br.ReadInt32();
            node.nodeEvents = new NodeEvent[count];
            for (int i = 0; i < count; i++)
            {
                node.nodeEvents[i] = NodeEvent.NodeEventFromStream(br);
            }
            return node;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
