using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2;


namespace PhysX_test2.Engine.Animation
{
    public class AnimationGraph                             // граф анимаций
    {
        public string description
        {
            get;
            private set;
        }

        public void SetDescription(string _description)
        {
            description = _description;
            if (!(description[description.Length - 1] == '\0'))
                description += "\0";
        }

        // массив узлов (анимация + её имя(название == идентификатор))
        public AnimationNode[] nodes;

        //индексы костей полного скелета, которые анимирует 
        //этот граф. это не правильно но так проще =(
        public int[] boneIndexes;
        public AnimationGraph(AnimationNode[] _nodes, int[] _indexes)
        {
            nodes = _nodes;
            description = "new_graph\0";

            boneIndexes = new int[_indexes.Length];
            _indexes.CopyTo(boneIndexes, 0);
        }

        public AnimationGraph(string _description, int[] _indexes)
        {
            SetDescription(_description);
            boneIndexes = new int[_indexes.Length];
            _indexes.CopyTo(boneIndexes, 0);
        }

        public string[] getAllEvents()
        {
            List<string> keys = new List<string>();
            for (int i = 0; i < nodes.Length; i++)
            {
                for (int j = 0; j < nodes[i].nodeEvents.Length; j++)
                {
                    if (!keys.Contains(nodes[i].nodeEvents[j].neededEvent))
                        keys.Add(nodes[i].nodeEvents[j].neededEvent);
                }
            }
            return keys.ToArray();
        }

        public AnimationNode FindNodeWithName(string nodeName)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                AnimationNode currentNode = nodes[i];
                if (currentNode != null && currentNode.name == nodeName)
                    return currentNode;
            }
            return null;
        }

        public static AnimationGraph AnimationGraphFromStream(System.IO.BinaryReader br)
        {
            int[] indexes = new int[br.ReadInt32()];
            for (int i = 0; i < indexes.Length; i++)
                indexes[i] = br.ReadInt32();
            string description = br.ReadPackString();
            AnimationGraph AGrf = new AnimationGraph(description, indexes);

            int lenth = br.ReadInt32();
            AGrf.nodes = new AnimationNode[lenth];
            for (int i = 0; i < lenth; i++)
            {
                AnimationNode node = AnimationNode.AnimationNodeFromStream(br);
                AGrf.nodes[i] = node;
            }
            for (int i = 0; i < AGrf.nodes.Length; i++)
                for (int j = 0; j < AGrf.nodes[i].nodeEvents.Length; j++)
                {
                    AGrf.nodes[i].nodeEvents[j].associatedNode = AGrf.FindNodeWithName(AGrf.nodes[i].nodeEvents[j].associatedNodeName);
                    AGrf.nodes[i].nodeEvents[j].parentNode = AGrf.FindNodeWithName(AGrf.nodes[i].nodeEvents[j].parentName);
                }
            return AGrf;
        }

        public override string ToString()
        {
            return this.description.Remove(this.description.Length - 1);
        }
    }

    public class NodeProperties
    {
        public bool OneTimeAnimation;

        
        public static NodeProperties NodePropertiesFromStream(System.IO.BinaryReader br)
        {
            NodeProperties result = new NodeProperties();
            result.LoadFromStream(br);
            return result;
        }
        public void LoadFromStream(System.IO.BinaryReader br)
        {
            OneTimeAnimation = br.ReadBoolean();
        }
    }
}
