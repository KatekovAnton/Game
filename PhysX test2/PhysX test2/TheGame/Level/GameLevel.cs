using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.Engine.Logic;

using PhysX_test2.TheGame.LogicControllers;

using Microsoft.Xna.Framework;

namespace PhysX_test2.TheGame.Level
{
    public class GameLevel
    {
        public EngineScene _scene;
        public MyContainer<BaseLogicController> _allLogicObjects;


        public GameLevel(EngineScene __scene)
        {
            _scene = __scene;
            _allLogicObjects = new MyContainer<BaseLogicController>();
        }
        public float times = 0;
        public Vector3 GetSpawnPlace()
        {
            times += 1;
            return new Vector3(0 + times*2.0f, 16.0f, 0);
        }

        public void AddController(BaseLogicController __object)
        {
            _allLogicObjects.Add(__object);
            __object._itsLevel = this;
        }

        public void RemoveController(BaseLogicController __object)
        {
            _allLogicObjects.Remove(__object);
            __object._itsLevel = null;
        }
      
        public void AddEngineObject(Engine.Logic.PivotObject __object, Engine.Logic.PivotObject __parentObject = null)
        {
            LevelObject loNew = __object as LevelObject;
            if (loNew == null)
                return;

            if (loNew.renderaspect.isanimated)
            {
                Engine.Render.AnimRenderObject ro = loNew.renderaspect as Engine.Render.AnimRenderObject;
                Engine.AnimationManager.AnimationManager.Manager.AddAnimationUser(ro.Update, ro.character);
            }

            _scene.AddObject(__object);

            if (__parentObject != null)
            {
                __object.behaviourmodel.SetParentObject(__parentObject);
                _scene._objects.AddRule(__parentObject, __object);
            }
        }

        public void RemoveObject(Engine.Logic.PivotObject theObjecr)
        {
            _scene.RemoveObject(theObjecr);
        }

        public void Update(GameTime __gameTime)
        {

            foreach (BaseLogicController controller in _allLogicObjects)
                controller.Update(__gameTime);

        }

        public Vector3 CreateRandomizedPoint(Vector3 StartPoint, float accuracy)
        {
            float dispersion = (1.0f - accuracy) * 0.5f;

            float multx = MyRandom.Instance.Next(0, 10) > 5 ? 1 : -1;
            float multy = MyRandom.Instance.Next(0, 10) > 5 ? 1 : -1;
            float multz = MyRandom.Instance.Next(0, 10) > 5 ? 1 : -1;

            Vector3 deltaRadius = new Vector3(dispersion * multx, dispersion * multy, dispersion * multz);
            Vector3 result = StartPoint + deltaRadius;
            
            return result;
        }

        public void CreateBullet(WeaponLogicController __weapon, TimeSpan __nowTime)
        {
            //VOVA 
            //тут создается пуля, __weapon - из какой пушки выпущена
            //тут тебе ненадо ничего менять имхо
            MyGame.ScreenLogMessage("create bullet");
            
            LogicControllers.Parameters.BulletParameters parameters = __weapon._chargedBullets;
            Objects.GameBulletSimple bullet = new Objects.GameBulletSimple(this, parameters._levelObjectName);

            Vector3 moveVector;
            Vector3 targetpoint = MyGame.Instance._mousepoint;
            Vector3 startpoint = __weapon._weaponObject._inHandObject.transform.Translation;

            moveVector = targetpoint - startpoint;
            moveVector.Normalize();

            targetpoint = startpoint + moveVector*10;
            targetpoint = CreateRandomizedPoint(targetpoint, parameters._accuracy);

            moveVector = targetpoint - startpoint;
            moveVector.Normalize();

           // Matrix transform = __weapon._weaponObject._inHandObject.transform * Matrix.CreateTranslation(moveVector);
            Matrix transform = Matrix.CreateTranslation(0, -0.5f, 0.08f) * __weapon._weaponObject._inHandObject.transform;

            BulletLogicController result = new BulletLogicController(
                bullet,
                __nowTime,
                parameters,
                transform,
                moveVector);

            
            AddController(result);
            result.LocateToLevel();
        }

        public bool SearchBulletIntersection(BulletLogicController __bullet, Vector3 __moveVector)
        {
            Vector3 startPosition = __bullet._hisObject._object.behaviourmodel.CurrentPosition.Translation;
            Vector3 endPosition = startPosition + __moveVector;

            Vector3 intersectionPoint = new Vector3();

         
            PivotObject intersectedObject = _scene.SearchBulletIntersection(startPosition, endPosition, out intersectionPoint);

            if (intersectedObject != null)
            {
                intersectedObject.behaviourmodel.MakeJolt(intersectionPoint, __moveVector, __bullet._instanceParameters._bulletMass);

                BaseLogicController blc = intersectedObject._gameObject as BaseLogicController;
                if (blc != null)
                    blc.TakeHit(__bullet._instanceParameters);
                return true;
            }

            return false;            
        }
    }
}
