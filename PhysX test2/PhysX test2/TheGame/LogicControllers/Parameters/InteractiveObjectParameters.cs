using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.TheGame.LogicControllers.Parameters
{
    public class InteractiveObjectParameters
    {
        public int _dbID;
        public string _displayName;

        public InteractiveObjectParameters(int __dbID, string __displayName)
        {
            _dbID = __dbID;
            _displayName = __displayName;
        }
    }
}
