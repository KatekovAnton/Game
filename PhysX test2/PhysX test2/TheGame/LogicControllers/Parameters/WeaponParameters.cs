using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.TheGame.LogicControllers.Parameters
{
    public class WeaponParameters : ItemParameters
    {
        public double _fireTime;
        public double _fireRestartTime;
        public double _reloadTime;

        public float _hitBallisticMod;
        public float _hitEnergyMod;

        public float _magazineCapacity;

        public int _suitableAmmoType;

        public WeaponParameters(int __dbID, string __displayName, float __mass, object __selfObject)
            : base(__dbID, __displayName, __mass, __selfObject)
        { }

        public WeaponParameters(WeaponParameters _another, object __selfObject)
            : base(_another._dbID, _another._displayName, _another._mass, __selfObject)
        {
            _fireTime = _another._fireTime;
            _fireRestartTime = _another._fireRestartTime;
            _hitBallisticMod = _another._hitBallisticMod;
            _hitEnergyMod = _another._hitEnergyMod;
            _magazineCapacity = _another._magazineCapacity;
            _reloadTime = _another._reloadTime;
            _suitableAmmoType = _another._suitableAmmoType;

        }
    }
}
