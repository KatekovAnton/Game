using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.TheGame.Objects;
using Microsoft.Xna.Framework;

namespace PhysX_test2.TheGame.LogicControllers
{
    public class WeaponLogicController : BaseLogicController
    {
        public Parameters.WeaponParameters _baseParameters;
        public Parameters.WeaponParameters _instanceParameters;

        public Parameters.BulletParameters _chargedBullets;

        public GameWeapon _weaponObject;
        public GameSimpleObject _weaponFire;

        public TimeSpan _lastfiretime;
        public bool _isFiring;

        public WeaponLogicController(GameWeapon __weaponObject, GameSimpleObject __weaponFire)
        {
            _weaponObject = __weaponObject;
            _weaponFire = __weaponFire;
            _isFiring = false;
            _chargedBullets = StaticObjects.BulletParameters["1"];
        }

        public void SetState(GameWeaponState __newState, CharacterLogicController __owner = null)
        {
            if (__newState == _weaponObject._state)
                return;
            _weaponFire.RemoveFromLevel();
            switch (__newState)
            {
                case GameWeaponState.None:
                    _weaponObject.RemoveFromScene();
                    break;
                case GameWeaponState.InHand:
                    _weaponObject.TakeInHand(__owner._hisObject);
                    break;
                case GameWeaponState.OnFloor:
                    _weaponObject.DropOnFloor();
                    break;
                default: break;
            }

            _isFiring = false;
        }

        private bool CanFire(GameTime __gametime)
        {
            return _weaponObject._state == GameWeaponState.InHand && (__gametime.TotalGameTime.TotalMilliseconds - _lastfiretime.TotalMilliseconds) > _instanceParameters._fireRestartTime;
        }

        public bool BeginFire(GameTime __gameTime)
        {
            if (!CanFire(__gameTime))
                return false;

            _lastfiretime = __gameTime.TotalGameTime;
            _weaponFire.LocateToLevel(_weaponObject._inHandObject);
            return true;
        }

        public override void  Update(GameTime __gametime)
        {
            if (_weaponFire._onLevel && (__gametime.TotalGameTime.TotalMilliseconds - _lastfiretime.TotalMilliseconds) > _instanceParameters._fireTime)
                _weaponFire.RemoveFromLevel();
 	       // throw new NotImplementedException();
        }

        public override Parameters.InteractiveObjectParameters GetParameters()
        {
            return _instanceParameters;
        }

        public override void TakeHit(Parameters.BulletParameters __bulletParameters)
        {
            return;
        }
        

    }
}
 