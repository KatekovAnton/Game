using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.TheGame.Objects;

namespace PhysX_test2.TheGame.ObjectLogic
{
    public class LogicWeapon
    {
        public GameWeapon _weaponObject;

        public LogicWeapon(GameWeapon __weaponObject)
        {
            _weaponObject = __weaponObject;
        }

    }
}
