using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.Engine.Animation
{
    public class CharacterEvent                     // событие обрабатываемое анимацией
    {
        public string eventName;
        public DateTime createdTime;

        public CharacterEvent(string _eventname)
        {
            eventName = _eventname;
            createdTime = DateTime.Now;
        }

        public bool CompareTo(CharacterEvent tmp)
        {
            if (this.eventName.CompareTo(tmp.eventName) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool operator ==(CharacterEvent _event1, CharacterEvent _event2)
        {
            return _event1.eventName.CompareTo(_event2.eventName) == 0;
        }

        public static bool operator !=(CharacterEvent _event1, CharacterEvent _event2)
        {
            return _event1.eventName.CompareTo(_event2.eventName) != 0;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return this.eventName;
        }
    }

    public class CharacterPart                          //класс частей персонажа (например верх и низ)
    {
        public AnimationGraph animGraph;

        public int rootBoneIndex;

        public CharacterPart(AnimationGraph _baseGraph)
        {
            animGraph = _baseGraph;
        }
    }
}
