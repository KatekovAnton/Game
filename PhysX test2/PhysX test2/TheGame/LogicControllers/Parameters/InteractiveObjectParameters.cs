using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.TheGame.LogicControllers.Parameters
{
    public class ControllerParameters
    {
        public int _dbID;
        public string _name;

        public ControllerParameters(int __dbID, string __displayName)
        {
            _dbID = __dbID;
            _name = __displayName;

            if (!_name.EndsWith("\0"))
                _name += "\0";
        }
    }

    
}
