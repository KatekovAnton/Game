using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.TheGame.LogicControllers.Parameters
{
    public class ItemParameters : InteractiveObjectParameters
    {
        //load from database
        public static Dictionary<string, ItemParameters> _allItems;

        public float _mass;
        public object _selfObject;
    }
}
