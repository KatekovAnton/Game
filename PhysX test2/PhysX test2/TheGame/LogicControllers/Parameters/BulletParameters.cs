using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.TheGame.LogicControllers.Parameters
{
    public class BulletParameters:ItemParameters
    {
        public float _damageBallistic;
        public float _damageEnergy;

        public float _damageRadius;

        public int _ammoType;
        public BulletParameters(int __dbID, string __displayName, float __mass, object __selfObject)
            : base(__dbID, __displayName, __mass, __selfObject)
        { }
    }
}
