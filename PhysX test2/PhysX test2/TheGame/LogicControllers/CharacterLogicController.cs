﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.TheGame.Objects;

using Microsoft.Xna.Framework;

using PhysX_test2.TheGame.Level;
using PhysX_test2.TheGame.LogicControllers.Parameters;
using PhysX_test2.TheGame.InputManagers;

namespace PhysX_test2.TheGame.LogicControllers
{
    public class CharacterLogicController:BaseLogicController 
    {
        public Parameters.CharacterParameters _baseParameters;
        public Parameters.CharacterDynamicParameters _instanceParameters;


        public bool _isAlive;
        public bool _isMe;

        public GameCharacter _hisObject;
        public GameSimpleObject _hisHead;

        public WeaponLogicController _hisWeapon;

        public InputProviderSuperClass _hisInput;
        public CharacterMoveState _moveState;

        public CharacterLogicController(GameLevel __level, GameCharacter __hisObject, GameSimpleObject __hisHead, bool __isMe = false)
            :base(__level)
        {
            _hisHead = __hisHead;
            _hisObject = __hisObject;

            _isAlive = false;
            _isMe = __isMe;

            _hisObject._levelObject._needMouseCast = _hisObject._levelObject._needMouseCast && !_isMe;
            _hisHead._object._needMouseCast = _hisHead._object._needMouseCast && !_isMe;
            _hisObject._levelObject._gameObject = this;
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
            AnimationInfo ai = GetAnimInfo();
            _hisObject._controllerAlive.MakeUnconditionalTransition(ai._stayTopAnim, 1, false, ai._stayTopAnim);
            SwitchGun();
        }

        public override void RemoveFromLevel()
        {
            if (_hisWeapon != null)
                _hisWeapon.RemoveFromLevel();
            _hisObject.RemoveFromLevel();
            _hisHead.RemoveFromLevel();
            _itsLevel.RemoveController(this);
           
        }

        public void SwitchGun()
        {
            MyGame.ScreenLogMessage("switching my gun");
            if (_hisWeapon._weaponObject._state == GameWeaponState.InHand)
                _hisWeapon.SetState(GameWeaponState.OnFloor);
            else
                _hisWeapon.SetState(GameWeaponState.InHand, this);
        }

        private AnimationInfo GetAnimInfo()
        {
            AnimationInfo animinfo = null;
            if (_hisWeapon == null)
                animinfo = _baseParameters._animations[1];
            else
                animinfo = _baseParameters._animations[_hisWeapon._baseParameters._type];
            return animinfo;
        }

        public void SetAlive(bool __liveState)
        {
            if (__liveState == _isAlive)
                return;

            if (__liveState)
            {
                MyGame.ScreenLogMessage("character alive");
                AnimationInfo animinfo = GetAnimInfo();
                _hisObject.SetAlive(animinfo._stayBotAnim, animinfo._stayTopAnim);
                _hisObject._levelObject._gameObject = this;
            }
            else
            {
                MyGame.ScreenLogMessage("character dead");
                _hisObject.SetDead();
            }

            _hisHead.LocateToLevel(_hisObject._levelObject);
            _isAlive = __liveState;
        }

        public override void Update(GameTime __gameTime)
        {
            CalculateInput();

            if (_hisInput == null)
                return;
            if (_hisInput._tryAttackFirst)
            {
                if (_hisWeapon.BeginFire(__gameTime))
                {
                    AnimationInfo info = GetAnimInfo();
                    //start fire animation and other
                    _hisObject.Fire(info._fireAniml, info._stayTopAnim);
                    _itsLevel.CreateBullet(_hisWeapon, __gameTime.TotalGameTime);
                }
            }
        }

        public void CalculateInput()
        {
            //TODO assert
            if (_hisInput == null)
                return;
            CharacterMoveState oldstate = _hisInput._newInputState;
           
            _hisInput.Update(_hisObject._levelObject.behaviourmodel.CurrentPosition.Translation);
            
            _hisObject.Rotate(_hisInput._angle);
            _hisInput._angle = 0;
            _hisObject._levelObject.Move(_hisInput._moveVector);
            Engine.Render.AnimRenderObject ro = _hisObject._levelObject.renderaspect as Engine.Render.AnimRenderObject;
            ro.character.SetTopAngle(_hisInput._bodyRotation);
            if (oldstate != _hisInput._newInputState)
            {
                
                ro.ReceiveEvent(GetEventName(_hisInput._newInputState), oldstate == CharacterMoveState.Stay);
                Engine.GameEngine.Instance.playerState = _hisInput._newInputState;
                
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
                case CharacterMoveState.WalkLeft:
                    return "beginWalkLeft\0";
                case CharacterMoveState.WalkRight:
                    return "beginWalkRight\0";
                default: return "stopMove\0";
            }
        }

        public override void TakeHit(Parameters.BulletDynamicParameters __bulletParameters)
        {
            //VOVA
            MyGame.ScreenLogMessage("character taking hit", Color.Red);
        }

      /*  public override Parameters.DynamicParameters GetParameters()
        {
            return _instanceParameters;
        }*/

        public static CharacterLogicController CreateCharacter(string __nameIsId, GameLevel __level, bool _needMC)
        {
            CharacterParameters parameters = StaticObjects.CharacterParameters[__nameIsId];


            GameCharacter myCharacter = new GameCharacter(parameters._levelObjectName, Matrix.Identity, __level);
            GameSimpleObject myHead = new GameSimpleObject(parameters._headObjectName,  __level, Engine.Logic.PivotObjectDependType.Head, false, false);
            CharacterLogicController result = new CharacterLogicController(__level, myCharacter, myHead, !_needMC);
            result._baseParameters = parameters;
            return result;
        }
    }
}
