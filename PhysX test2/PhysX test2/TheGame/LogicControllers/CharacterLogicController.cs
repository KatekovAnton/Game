using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.TheGame.Objects;

namespace PhysX_test2.TheGame.LogicControllers
{
    public class CharacterLogicController:BaseLogicController 
    {
        public Parameters.CharacterParameters _baseParameters;
        public Parameters.CharacterParameters _instanceParameters;


        public bool _isAlive;
        public bool _isMe;

        public GameCharacter _hisObject;
        public GameSimpleObject _hisHead;

        public WeaponLogicController _hisWeapon;

        public CharacterLogicController(GameCharacter __hisObject, GameSimpleObject __hisHead, bool __isMe = false)
        {
            _hisHead = __hisHead;
            _hisObject = __hisObject;

            _isAlive = false;
            _isMe = __isMe;

            _hisObject._aliveObject._needMouseCast = _hisObject._aliveObject._needMouseCast && !_isMe;
            _hisHead._object._needMouseCast = _hisHead._object._needMouseCast && !_isMe;
        }

        public void SetGun(WeaponLogicController __newWeapon)
        {
            if (_hisWeapon != null)
            {
                switch (_hisWeapon._weaponObject._state)
                {
                    case GameWeaponState.InHand:
                    case GameWeaponState.OnFloor:
                        {
                            _hisWeapon.SetState(GameWeaponState.None);
                        } break;
                    default: break;
                }

                _hisWeapon = null;
            }

            __newWeapon.SetState(GameWeaponState.None);

            _hisWeapon = __newWeapon;
            SwitchGun();
        }

        public void SwitchGun()
        {
            if (_hisWeapon._weaponObject._state == GameWeaponState.InHand)
                _hisWeapon.SetState(GameWeaponState.OnFloor);
            else
                _hisWeapon.SetState(GameWeaponState.InHand, this);
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

        public override void Update(float __elapsedTime)
        {

        }

        public override void TakeHit(Parameters.BulletParameters __bulletParameters)
        {

        }

        public override Parameters.InteractiveObjectParameters GetParameters()
        {
            return _instanceParameters;
        }
    }
}
