using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.TheGame.Objects;
using Microsoft.Xna.Framework;
using PhysX_test2.TheGame.Level;
using PhysX_test2.TheGame.LogicControllers.Parameters;
namespace PhysX_test2.TheGame.LogicControllers
{
    public class WeaponLogicController : BaseLogicController
    {
        public Parameters.WeaponParameters _baseParameters;
        public Parameters.WeaponDynamicParameters _instanceParameters;

        public Parameters.BulletParameters _chargedBullets;
        public Parameters.BulletDynamicParameters _bulletFinalParameters;

        public GameWeapon _weaponObject;
        public GameSimpleObject _weaponFire;

        public TimeSpan _lastfiretime;
        public bool _isFiring;

        public WeaponLogicController(GameLevel __level, GameWeapon __weaponObject, GameSimpleObject __weaponFire)
            :base(__level)
        {
            _weaponObject = __weaponObject;
            _weaponFire = __weaponFire;
            _isFiring = false;
            _chargedBullets = StaticObjects.BulletParameters["1"];
            _bulletFinalParameters = new BulletDynamicParameters(_chargedBullets);
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
                    _weaponFire.LocateConstrainedToLevel(_weaponObject._inHandObject);
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
            _weaponFire.LocateConstrainedToLevel(_weaponObject._inHandObject);
            return true;
        }

        public override void Update(GameTime __gametime)
        {
            if (_weaponFire._onLevel && (__gametime.TotalGameTime.TotalMilliseconds - _lastfiretime.TotalMilliseconds) > _baseParameters._fireTime) ;
               // _weaponFire.RemoveFromLevel();
           
                _weaponFire.CalcParameters();
        }

        public override Parameters.DynamicParameters GetParameters()
        {
            return _instanceParameters;
        }

        public override void TakeHit(Parameters.BulletDynamicParameters __bulletParameters)
        {
            //newer for weapons
            return;
        }

        public static WeaponLogicController CreateWeapon(string __nameAsId, GameLevel __level)
        {
            WeaponParameters parameters = StaticObjects.WeaponParameters[__nameAsId];
            GameWeapon myWeapon = new GameWeapon(parameters._inhandObject, parameters._onfloorObject, parameters._addonObject, __level);
            GameSimpleObject weaponFire = new GameSimpleObject(parameters._fireObject, Engine.Logic.PivotObjectDependType.Body, __level, false, false);
            WeaponLogicController newGun = new WeaponLogicController(__level, myWeapon, weaponFire);
            newGun._baseParameters = parameters;
            newGun._instanceParameters = new WeaponDynamicParameters(parameters);
            return newGun;
        }
    }
}
 