using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.TheGame.LogicControllers.Parameters
{
    public class WeaponParameters : ItemParameters
    {
        public float _fireTime;
        public float _fireRestartTime;
        public float _reloadTime;

        public float _hitBallisticMod;
        public float _hitEnergyMod;

        public float _magazineCapacity;

        public int _suitableAmmoType;
    }
}
