using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.TheGame.LogicControllers.Parameters
{
    public class ItemParameters : InteractiveObjectParameters
    {
        //load from database
        private static Dictionary<string, ItemParameters> _allItems;
        public static Dictionary<string, ItemParameters> allItems
        { 
            get
            {
                if(_allItems==null)
                    _allItems = new Dictionary<string,ItemParameters>();
                return _allItems;
            }
        }

        public float _mass;
        public object _selfObject;

        public ItemParameters(int __dbID, string __displayName, float __mass, object __selfObject)
            : base(__dbID, __displayName)
        {
            _mass = __mass;
            _selfObject = __selfObject;
        }
    }
}
