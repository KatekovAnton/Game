using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.TheGame.Objects;

using Microsoft.Xna.Framework;

using PhysX_test2.TheGame.InputManagers;

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

        public InputProviderSuperClass _hisInput;
        public CharacterMoveState _moveState;

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

        public override void Update(GameTime __gameTime)
        {
            CalculateInput();

            if (MouseManager.Manager.lmbState == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                if (_hisWeapon.BeginFire(__gameTime))
                    _hisObject.Fire();
            }
        }

        public void CalculateInput()
        {
            CharacterMoveState oldstate = _hisInput._newInputState;
            _hisInput.Update(_hisObject._aliveObject.behaviourmodel.CurrentPosition.Translation);
            
            _hisObject.Rotate(_hisInput._angle);
            _hisObject._aliveObject.Move(_hisInput._moveVector);
            if (oldstate != _hisInput._newInputState)
            {
                Engine.Render.AnimRenderObject ro = _hisObject._aliveObject.renderaspect as Engine.Render.AnimRenderObject;
                ro.ReceiveEvent(GetEventName(_hisInput._newInputState), oldstate == CharacterMoveState.Stay);
            }
        }

        private string GetEventName(CharacterMoveState newstate)
        {
            switch (newstate)
            {
                case CharacterMoveState.Stay:
                    return "stopMove\0";
                case CharacterMoveState.WalkBackward:
                    return "beginWalkBack\0";
                case CharacterMoveState.WalkForward:
                    return "beginWalk\0";
                default: return "stopMove\0";
            }
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
