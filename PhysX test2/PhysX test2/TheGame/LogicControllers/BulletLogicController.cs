using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using PhysX_test2.TheGame.Objects;
using PhysX_test2.TheGame.Level;
namespace PhysX_test2.TheGame.LogicControllers
{
    public class BulletLogicController : BaseLogicController
    {
        public Parameters.BulletParameters _baseParameters;
        public Parameters.BulletDynamicParameters _instanceParameters;
        public Vector3 _startPosition;
        public Vector3 _moveVector;
        
       
       
        public GameBulletSimple _hisObject;

        public BulletLogicController(
            GameLevel __level,
            GameBulletSimple __itsObject, 
            TimeSpan __nowTime,
            Parameters.BulletParameters __baseParameters,
            Parameters.BulletDynamicParameters __itsParameters, 
            Matrix __startPos, 
            Vector3 __moveVector)
            :base(__level)
        {
            _hisObject = __itsObject;
            _createdTime = __nowTime;
            _baseParameters = __baseParameters;
            _instanceParameters = __itsParameters;
            _startPosition = __startPos.Translation;
            _moveVector = __moveVector;
            _moveVector.Normalize();
            _hisObject._object.SetGlobalPose(__startPos);
        }

        public override void Update(GameTime __gameTime)
        {
            if ((__gameTime.TotalGameTime.TotalMilliseconds - _createdTime.TotalMilliseconds) > _baseParameters._lifeTime)
            {
                //remove visible object
                _hisObject.RemoveFromLevel();
                //stop the update
                _itsLevel.RemoveController(this);
            }
            else
            {
                StatisticContainer.Instance().UpdateParameter("totalBullets", 1);
                Vector3 delta = _moveVector * _baseParameters._speed * (float)(__gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0);
                Vector3 resulpoint = new Vector3();
                Vector3 resulnormal = new Vector3();
                Engine.Logic.PivotObjectMaterialType mattype = Engine.Logic.PivotObjectMaterialType.Metal;
                if (!_itsLevel.SearchBulletIntersection(this, delta, out resulpoint, out resulnormal, out mattype))
                {
                    //move forward
                    Matrix newmatrix = _hisObject._object.behaviourmodel.CurrentPosition * Matrix.CreateTranslation(delta);
                    _hisObject._object.SetGlobalPose(newmatrix);
                }
                else
                {
                    RemoveFromLevel();
                    _itsLevel.CreateIntersectionEffect(_moveVector, resulpoint, resulnormal, mattype, _baseParameters._type);
                }
            }
        }

        public override void RemoveFromLevel()
        {
            //remove visible object
            _hisObject.RemoveFromLevel();
            //stop the update
            _itsLevel.RemoveController(this);
        }
        

        public void LocateToLevel()
        {
            _hisObject.LocateToLevel();
            _hisObject._object._gameObject = this;
        }


        public override void TakeHit(Parameters.BulletDynamicParameters __bulletParameters)
        {
            return;
        }

        ~BulletLogicController()
        {
 
        }
    }
}
