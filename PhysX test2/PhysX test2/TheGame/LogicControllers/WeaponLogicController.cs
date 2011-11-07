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
        public GameWeapon _weaponObject;

        public GameTime _lastfiretime;

        public WeaponLogicController(GameWeapon __weaponObject)
        {
            _weaponObject = __weaponObject;
            _lastfiretime = new GameTime();
        }

        public void SetState(GameWeaponState __newState, CharacterLogicController __owner = null)
        {
            if (__newState == _weaponObject._state)
                return;

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
        }

        public override void  Update(float __elapsedTime)
        {
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
