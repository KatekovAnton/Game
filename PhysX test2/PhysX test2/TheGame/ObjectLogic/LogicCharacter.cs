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

        public GameCharacter _hisObject;
        public GameSimpleObject _hisHead;

        public LogicWeapon _hisWeapon;

        public LogicCharacter(GameCharacter __hisObject, GameSimpleObject __hisHead)
        {
            _hisHead = __hisHead;
            _hisObject = __hisObject;

            _isAlive = false;
        }

        public void SetGun(LogicWeapon __newWeapon)
        {
            if (_hisWeapon != null)
            {
                switch (_hisWeapon._weaponObject._state)
                {
                    case GameWeaponState.InHand:
                    case GameWeaponState.OnFloor:
                        {
                            _hisWeapon._weaponObject.RemoveFromScene();
                        }break;
                    default: break;
                }

                _hisWeapon = null;
            }

            if(__newWeapon._weaponObject._state!= GameWeaponState.None)
                __newWeapon._weaponObject.RemoveFromScene();

            _hisWeapon = __newWeapon;
            SwitchGun();
        }

        public void SwitchGun()
        {
            if (_hisWeapon._weaponObject._state == GameWeaponState.InHand)
            {
                _hisWeapon._weaponObject.DropOnFloor();
            }
            else
                _hisWeapon._weaponObject.TakeInHand(_hisObject);
        }

        public void SetAlive(bool __liveState)
        {
            if (__liveState == _isAlive)
                return;

            if (__liveState)
                _hisObject.SetAlive();
            else
                _hisObject.SetDead();

            _hisHead.LocateToLevel(_hisObject._aliveObject);
            _isAlive = __liveState;
        }
    }
}
