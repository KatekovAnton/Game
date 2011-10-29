using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.TheGame.Objects.StateGraphs;

namespace PhysX_test2.TheGame.Objects
{
    public class CharacterMethods
    {
        public static void edgeDeadToAlive(GameObject __object)
        {
            GameCharacter character = __object as GameCharacter;
            if (character == null)
                return;//throw new Exception("wrong object in method CharacterMethods.deadToAlive");

            //method
        }

        public static void edgeAliveToDead(GameObject __object)
        {
            GameCharacter character = __object as GameCharacter;
            if (character == null)
                return;//throw new Exception("wrong object in method CharacterMethods.aliveToDead");

            //method
        }

        public static void emptyAction(GameObject __object)
        { }
    }
}
