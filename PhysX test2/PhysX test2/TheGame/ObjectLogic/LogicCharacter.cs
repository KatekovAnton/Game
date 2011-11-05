using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.TheGame.Objects;

namespace PhysX_test2.TheGame.ObjectLogic
{
    public class LogicCharacter
    {
        public static float _marineGun01FireTime = 1.0f;
        public static float _marineGun01FireRestartTime = 1.0f;
        public static float _marineGun01ReloadTime = 1.0f;
        public static float _marineActionTime = 1.0f;

        public bool _isAlive;

        GameCharacter _hisObject;
        GameSimpleObject _hisHead;


        public LogicCharacter(GameCharacter __hisObject, GameSimpleObject __hisHead)
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
                _hisObject.SetAlive();
            else
                _hisObject.SetDead();

            _hisHead.LocateToLevel(_hisObject._currentObject);
            _isAlive = __liveState;
        }
    }
}
