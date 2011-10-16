using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Animation
{
    public class Character
    {
        public CharacterStatic _baseCharacter;
        public AnimationNode[] _currentNodes;           // текущие узлы- по одному на часть

        public Matrix[] _currentFames;

        public Character(CharacterStatic characterBase, string[] startNodes)
        {

            _baseCharacter = characterBase;
            _currentNodes = new AnimationNode[characterBase.parts.Length];
            _currentFames = new Matrix[_baseCharacter.skeleton.baseskelet.bones.Length];

            if (startNodes != null)
                for (int i = 0; i < startNodes.Length; i++)
                    SetCurrentNode(i, startNodes[i]);
        }

        public void SetCurrentNode(int index, string name)
        {
            _currentNodes[index] = _baseCharacter.parts[index].animGraph.FindNodeWithName(name);
        }

        public void ReceiveEvent(CharacterEvent _event)
        {
            foreach (AnimationNode node in _currentNodes)
                node.Advance(_event.eventName);
        }
    }
}
