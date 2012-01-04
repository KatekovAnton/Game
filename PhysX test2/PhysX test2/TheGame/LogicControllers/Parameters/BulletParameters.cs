using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.TheGame.LogicControllers.Parameters
{
    public enum BulletType
    {
        Bullet,
        Energy,
        Rocket
    }
    public class BulletParameters : ItemParameters
    {
        public BulletType _type = BulletType.Bullet;

        public int _caliber;
        public float _damage;
        public float _pierce;
        public float _speed;
        public double _lifeTime;
        public float _accuracy;
        
        public string _levelObjectName;
        public float _bulletMass;

        public BulletParameters(int __dbID, string __displayName, float __boxMass, int __caliber,
         float __damage, float __pierce, float __speed, double __lifeTime,
         float __accuracy,       string __levelObjectName,  float __bulletMass)
            : base(__dbID, __displayName, __boxMass)
        {
            _caliber = __caliber;
            _damage = __damage;
            _pierce = __pierce;
            _speed = __speed;
            _lifeTime = __lifeTime;
            _accuracy = __accuracy;
            if (_accuracy == 0)
                _accuracy = 0.1f;
            _levelObjectName = __levelObjectName;
            if (!_levelObjectName.EndsWith("\0"))
                _levelObjectName += "\0";
            _bulletMass = __bulletMass;
        }
    }

    public class BulletDynamicParameters : DynamicParameters
    {
        public float _damage;
        public float _pierce;
        public float _accuracy;

        public BulletDynamicParameters(BulletParameters __params)
        {
            _damage = __params._damage;
            _pierce = __params._pierce;
            _accuracy = __params._accuracy;
        }
    }
}
