using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.TheGame.LogicControllers.Parameters
{
    public class WeaponParameters : ItemParameters
    {

        public string _shortName;
        public int _caliber;
        public string _inhandObject;
        public string _onfloorObject;
        public string _addonObject;
        public float _reloadTime;
        public float _fireTime;
        public float _fireRestartTime;
        public string _fireObject;
        public float _damageBallMod;
        public float _damageEnMod;
        public int _magazineCapacity;
        public int _oneShootCount;
        public float _accuracy;
        public string _caliberName;


        public WeaponParameters(int __dbID, string __displayName, float __mass,
                             string __shortName,
                             int __caliber,
                             string __inhandObject,
                             string __onfloorObject,
                             string __addonObject,
                             float __reloadTime,
                             float __fireTime,
                             float __fireRestartTime,
                             string __fireObject,
                             float __damageBallMod,
                             float __damageEnMod,
                             int __magazineCapacity,
                             int __oneShootCount,
                             float __accuracy,
                             string __description,
                             string __caliberName)
            : base(__dbID, __displayName, __mass)
        {
            _shortName = __shortName;
            if (!_shortName.EndsWith("\0"))
                _shortName += "\0";
            _caliber = __caliber;
            _inhandObject = __inhandObject;
            if (!_inhandObject.EndsWith("\0"))
                _inhandObject += "\0";
            _onfloorObject = __onfloorObject;
            if (!_onfloorObject.EndsWith("\0"))
                _onfloorObject += "\0";
            _addonObject = __addonObject;
            _reloadTime = __reloadTime;
            _fireTime = __fireTime;
            _fireRestartTime = __fireRestartTime;
            _fireObject = __fireObject;
            if (!_fireObject.EndsWith("\0"))
                _fireObject += "\0";
            _damageBallMod = __damageBallMod;
            _damageEnMod = __damageEnMod;
            _magazineCapacity = __magazineCapacity;
            _oneShootCount = __oneShootCount;
            _accuracy = __accuracy;
            _description = __description;
            if (!_description.EndsWith("\0"))
                _description += "\0";
            _caliberName = __caliberName;
        }

        public WeaponParameters(WeaponParameters _another)
            : base(_another._dbID, _another._name, _another._mass)
        {
            _fireTime           =  _another._fireTime;
            _fireRestartTime    = _another._fireRestartTime;
            _damageEnMod        = _another._damageEnMod;
            _damageBallMod      = _another._damageBallMod;
            _magazineCapacity   = _another._magazineCapacity;
            _reloadTime         = _another._reloadTime;
            _caliber            = _another._caliber;
            _description        = _another._description;
            _shortName          = _another._shortName;
            _inhandObject       = _another._inhandObject;
            _onfloorObject      = _another._onfloorObject;
            _addonObject        = _another._addonObject;
            _fireObject         = _another._fireObject;
            _oneShootCount      = _another._oneShootCount;
            _accuracy           = _another._accuracy;

        }
    }
}
