using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.TheGame.LogicControllers.Parameters
{
    public class MotralParameters : InteractiveObjectParameters
    {
        //load from database
        public static Dictionary<string, MotralParameters> _allObjects;

        public float _healtPoints;
        public float _armorBallistic;
        public float _armorEnergy;

        public float _resistancePenetrating;
        public float _resistanceChemical;

        public MotralParameters(int __dbID, string __displayName)
            : base(__dbID, __displayName)
        { }
    }
}
