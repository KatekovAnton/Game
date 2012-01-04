using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.TheGame.LogicControllers.Parameters
{
    public class ItemParameters : ControllerParameters
    {
        public float _mass;
        public string _inventoryImage;
        public string _description;

        public ItemParameters(int __dbID, string __displayName, float __mass)
            : base(__dbID, __displayName)
        {
            _mass = __mass;
        }
    }
}
