using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.TheGame.LogicControllers.Parameters
{
    public class BulletParameters : ItemParameters
    {

        public int _caliber;
        public float _damage;
        public float _pierce;
        public float _speed;
        public float _lifeTime;
        public float _accuracy;
        
        public string _levelObjectName;
        public float _bulletMass;

        public BulletParameters(int __dbID, string __displayName, float __boxMass, int __caliber,
         float __damage,         float __pierce,            float __speed,         float __lifeTime,
         float __accuracy,       string __levelObjectName,  float __bulletMass)
            : base(__dbID, __displayName, __boxMass)
        {
            _caliber = __caliber;
            _damage = __damage;
            _pierce = __pierce;
            _speed = __speed;
            _lifeTime = __lifeTime;
            _accuracy = __accuracy;

            _levelObjectName = __levelObjectName;
            _bulletMass = __bulletMass;
        }
        
    }
}
