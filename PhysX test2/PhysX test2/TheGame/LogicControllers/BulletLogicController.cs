using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using PhysX_test2.TheGame.Objects;

namespace PhysX_test2.TheGame.LogicControllers
{
    public class BulletLogicController : BaseLogicController
    {
        public Parameters.BulletParameters _instanceParameters;
        public Vector3 _startPosition;
        public Vector3 _moveVector;
        
       
       
        public GameBulletSimple _hisObject;

        public BulletLogicController(
            GameBulletSimple __itsObject, 
            TimeSpan __nowTime, 
            Parameters.BulletParameters __itsParameters, 
            Matrix __startPos, 
            Vector3 __moveVector)
        {
            _hisObject = __itsObject;
            _createdTime = __nowTime;

            _instanceParameters = __itsParameters;
            _startPosition = __startPos.Translation;
            _moveVector = __moveVector;
            _moveVector.Normalize();
            _hisObject._object.SetGlobalPose(__startPos);
        }

        public override void Update(GameTime __gameTime)
        {
            if ((__gameTime.TotalGameTime.TotalMilliseconds - _createdTime.TotalMilliseconds) > _instanceParameters._lifeTime)
            {
                //remove visible object
                _hisObject.RemoveFromLevel();
                //stop the update
                _itsLevel.RemoveController(this);
            }
            else
            {
                Vector3 startPosition = _hisObject._object.behaviourmodel.CurrentPosition.Translation;
                Vector3 delta= _moveVector * _instanceParameters._moveSpeed * (float)(__gameTime.ElapsedGameTime.TotalMilliseconds/1000.0);
                Vector3 endPosition = startPosition + delta;

                BaseLogicController ooo = _itsLevel.SearchBulletIntersection(startPosition, endPosition);
                if (ooo != null)
                {
                    CharacterLogicController clc = ooo as CharacterLogicController;
                    if (clc != null && !clc._isMe)
                    {
                        int a = 0;
                        a++;
                    }
                }


                Matrix newmatrix = _hisObject._object.behaviourmodel.CurrentPosition * Matrix.CreateTranslation(delta);
                _hisObject._object.SetGlobalPose(newmatrix);
            }
        }

        public void LocateToLevel()
        {
            _hisObject.LocateToLevel();
            _hisObject._object._gameObject = this;
        }

        public override Parameters.InteractiveObjectParameters GetParameters()
        {
            return _instanceParameters;
        }

        public override void TakeHit(Parameters.BulletParameters __bulletParameters)
        {
            return;
        }

        ~BulletLogicController()
        {
 
        }
    }
}
