using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.TheGame.Objects;

namespace PhysX_test2.TheGame.ObjectLogic
{
    public class LogicCharacter
    {
        public bool _isAlive;

        GameCharacter _hisObject;
        GameObject _hisHead;

        public LogicCharacter(GameCharacter __hisObject, GameObject __hisHead)
        {
            _hisHead = __hisHead;
            _hisObject = __hisObject;

            _isAlive = false;
        }

        public void SetAlive(bool __liveState)
        {
            if (__liveState == _isAlive)
                return;

            if (__liveState)
                _hisObject.setAlive();
            else
                _hisObject.setDead();

            _isAlive = __liveState;
        }
    }
}
