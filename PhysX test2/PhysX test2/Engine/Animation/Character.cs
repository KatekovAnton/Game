using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.Engine.Animation
{
    public class Character
    {
        public CharacterStatic _baseCharacter;
        public AnimationNode[] _currentNodes;           // текущие узлы

        public void ReceiveEvent(CharacterEvent _event)
        {
            foreach (AnimationNode node in _currentNodes)
                node.Advance(_event.eventName);
        }
    }
}
