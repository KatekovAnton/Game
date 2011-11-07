using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.TheGame.LogicControllers.Parameters
{
    public class CharacterParameters : MotralParameters
    {
        public int _socialGroup;
        public float _speed;

        public float _carryingMass;
        public CharacterParameters(int __dbID, string __displayName)
            : base(__dbID, __displayName)
        {

        }
    }
}
