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
        public MyContainer<BaseLogicController> _objectsForRemove;

        public GameLevel(EngineScene __scene)
        {
            _scene = __scene;
            _allLogicObjects = new MyContainer<BaseLogicController>();
            _objectsForRemove = new MyContainer<BaseLogicController>();
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
        }

        public void RemoveController(BaseLogicController __object)
        {
            _objectsForRemove.Add(__object);
        }
      
        public void AddEngineObject(Engine.Logic.PivotObject __object, Engine.Logic.PivotObject __parentObject = null)
        {
            LevelObject loNew = __object as LevelObject;
            bool added = false;
            if (loNew != null)
            {
                AddLevelObject(loNew, __parentObject);
                added = true;
            }
            if (!added)
            {
                ParticleObject ponew = __object as ParticleObject;
                if (ponew != null)
                {
                    AddParticleObject(ponew);
                    added = true;
                }
            }
            
        }

        public void AddEngineObject(Engine.Logic.PivotObject __object, Matrix __position, Engine.Logic.PivotObject __parentObject = null)
        {
            LevelObject loNew = __object as LevelObject;
            bool added = false;
            if (loNew != null)
            {
                AddLevelObject(loNew, __parentObject);
            }
            if (!added)
            {
                ParticleObject ponew = __object as ParticleObject;
                if (ponew != null)
                {
                    AddParticleObject(ponew);
                    added = true;
                }
            }
            __object.SetGlobalPose(__position, true, __parentObject);
        }

        private void AddLevelObject(LevelObject loNew, Engine.Logic.PivotObject __parentObject = null)
        {
            if (loNew.renderaspect.isanimated)
            {
                Engine.Render.AnimRenderObject ro = loNew.renderaspect as Engine.Render.AnimRenderObject;
                Engine.AnimationManager.AnimationManager.Manager.AddAnimationUser(ro.Update, ro.character);
            }

            _scene.AddObject(loNew);

            if (__parentObject != null)
            {
                loNew.SetGlobalPose(Matrix.Identity, true, __parentObject);
                _scene._objects.AddRule(__parentObject, loNew);
            }
        }

        private void AddParticleObject(ParticleObject loNew, Engine.Logic.PivotObject __parentObject = null)
        {
            _scene.AddObject(loNew);

            if (__parentObject != null)
            {
                loNew.SetGlobalPose(Matrix.Identity, true, __parentObject);
                _scene._objects.AddRule(__parentObject, loNew);
            }
        }

        public void RemoveObject(Engine.Logic.PivotObject theObjecr)
        {
            _scene.RemoveObject(theObjecr);
        }

        public void Update(GameTime __gameTime)
        {
            _objectsForRemove.Clear();
            foreach (BaseLogicController controller in _allLogicObjects)
                controller.Update(__gameTime);
            foreach (BaseLogicController controller in _objectsForRemove)
                _allLogicObjects.Remove(controller);
        }

        public Vector3 CreateRandomizedPoint(Vector3 StartPoint, float accuracy)
        {
            float dispersion = (1.0f - accuracy);
            if (dispersion < 0)
                dispersion = 0;
           

            float rx = MyRandom.Instance.Next(0, 10);
            float ry = MyRandom.Instance.Next(0, 10);
            float rz = MyRandom.Instance.Next(0, 10);


            float dx = (float)MyRandom.Instance.NextDouble();
            float dy = (float)MyRandom.Instance.NextDouble();
            float dz = (float)MyRandom.Instance.NextDouble();

            const float mult = 1.2f;

            float multx = (rx > 5 ? 1 : -1) * (dx) * mult;
            float multy = (ry > 5 ? 1 : -1) * (dy) * mult;
            float multz = (rz > 5 ? 1 : -1) * (dz) * mult;

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
            
            LogicControllers.Parameters.BulletDynamicParameters parameters = __weapon._bulletFinalParameters;
            Objects.GameBulletSimple bullet = new Objects.GameBulletSimple(this, __weapon._chargedBullets._levelObjectName);

            Vector3 moveVector;
            Vector3 targetpoint = MyGame.Instance._mousepoint;
            Vector3 startpoint = __weapon._weaponObject._inHandObject.transform.Translation;

            moveVector = targetpoint - startpoint;
            moveVector.Normalize();

            targetpoint = startpoint + moveVector * 5.0f;
            targetpoint = CreateRandomizedPoint(targetpoint, __weapon._baseParameters._accuracy);

            moveVector = targetpoint - startpoint;
            moveVector.Normalize();

           // Matrix transform = __weapon._weaponObject._inHandObject.transform * Matrix.CreateTranslation(moveVector);
            Matrix transform = Matrix.CreateTranslation(0, -0.5f, 0.08f) * __weapon._weaponObject._inHandObject.transform;

            BulletLogicController result = new BulletLogicController(
                this,
                bullet,
                __nowTime,
                __weapon._chargedBullets,
                parameters,
                transform,
                moveVector);

            
            result.LocateToLevel();
        }

        public void CreateIntersectionEffect(Vector3 __axis, Vector3 __position, Vector3 __normal, Engine.Logic.PivotObjectMaterialType __matType, LogicControllers.Parameters.BulletType __bulletType)
        {
            string paramid = StaticObjects.GetEffectParameters(__matType, __bulletType);
            if (paramid == null)
                return;
            EffectLogicController newController = LogicControllers.EffectLogicController.CreateEffect(paramid, this, __position, __normal);
            newController.LocateOnLevel(MyGame.UpdateTime.TotalGameTime);
        }

        public bool SearchBulletIntersection(BulletLogicController __bullet, Vector3 __moveVector, out Vector3 __resultPoint, out Vector3 __resultNormal, out PivotObjectMaterialType __resulttype)
        {
            Vector3 startPosition = __bullet._hisObject._object.behaviourmodel.CurrentPosition.Translation;
            Vector3 endPosition = startPosition + __moveVector;
            Vector3 intersectionPoint = new Vector3();
            Vector3 intersectionNormal = new Vector3();

            PivotObject intersectedObject = _scene.SearchBulletIntersection(startPosition, endPosition, out intersectionPoint, out intersectionNormal);

            if (intersectedObject != null)
            {
                intersectedObject.behaviourmodel.MakeJolt(intersectionPoint, __moveVector, __bullet._baseParameters._bulletMass);

                BaseLogicController blc = intersectedObject._gameObject as BaseLogicController;
                if (blc != null)
                    blc.TakeHit(__bullet._instanceParameters);

                __resulttype = intersectedObject.matrialType;
                __resultPoint = intersectionPoint;
                __resultNormal = intersectionNormal;
                return true;
            }

            __resultPoint = new Vector3();
            __resulttype = PivotObjectMaterialType.Metal;
            __resultNormal = new Vector3();
            return false;            
        }
    }
}
