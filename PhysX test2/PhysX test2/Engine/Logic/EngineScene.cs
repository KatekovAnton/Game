using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Logic
{
    public class EngineScene
    {
        public MyContainer<PivotObject> _shadowObjects;
        public MyContainer<PivotObject> _visibleObjects;
        public MyContainer<PivotObject> _objects;
        //мы же хотим переключать сцены?? поэтому каждой - свой сценграф
        public SceneGraph.SceneGraph _sceneGraph;


         public Scene Scene;
         public Core Core;

        public EngineScene()
        {
            //инит ФизиХ-а
            var coreDesc = new CoreDescription();
            var output = new UserOutput();

            Core = new Core(coreDesc, output);
            Core.SetParameter(PhysicsParameter.ContinuousCollisionDetection, false);
            Core.SetParameter(PhysicsParameter.ContinuousCollisionDetectionEpsilon, 0.01f);

            var sceneDesc = new SceneDescription
            {
                SimulationType = SimulationType.Software, //Hardware,
                MaximumBounds = new Bounds3(-1000, -1000, -1000, 1000, 1000, 1000),
                UpAxis = 2,
                Gravity = new StillDesign.PhysX.MathPrimitives.Vector3(0.0f, -9.81f * 1.7f, 0.0f),
                GroundPlaneEnabled = false
            };
            Scene = Core.CreateScene(sceneDesc);
            //для обработки столкновений
            Scene.UserContactReport = new ContactReport(MyGame.Instance);


            _objects = new MyContainer<PivotObject>(100, 10, true);
            _visibleObjects = new MyContainer<PivotObject>(100, 2);
            _shadowObjects = new MyContainer<PivotObject>(100, 2);
            _sceneGraph = new SceneGraph.SceneGraph(this);

        }

        public EngineScene(EngineScene s)
        {
            _shadowObjects = s._shadowObjects;
            _visibleObjects = s._visibleObjects;
            _objects = s._objects;
            _sceneGraph = s._sceneGraph;
        }

        /// <summary>
        /// TODO - TEST
        /// </summary>
        /// <param name="__oldObject"></param>
        /// <param name="__newObject"></param>
        /// <param name="__recalculate"></param>
        public bool SwapObjects(PivotObject __oldObject, PivotObject __newObject, bool __recalculate)
        {
            bool finded = false;
            foreach(PivotObject obj in _objects)
                if (obj == __oldObject)
                {
                    finded = true;
                    break;
                }

            if (!finded)
            {
                __newObject.behaviourmodel.Enable();
                _objects.Add(__newObject);
                _sceneGraph.AddObject(__newObject);
                return false;
            }

            List<MyContainer<PivotObject>.MyContainerRule> rules = _objects.FindAllRulesForObject(__oldObject);
            foreach (MyContainer<PivotObject>.MyContainerRule rule in rules)
            {
                if (rule.firstObject == __oldObject)
                {
                    rule.firstObject = __newObject;
                    rule.secondObject.behaviourmodel.SetParentObject(__newObject);
                }
                else if (rule.secondObject == __oldObject)
                    rule.secondObject = __newObject;
            }
            __newObject.behaviourmodel.Enable();
            __oldObject.behaviourmodel.Disable();
            _objects.Swap(__oldObject, __newObject, false);
            _sceneGraph.SwapObjects(__oldObject, __newObject, __recalculate);
           
            return true;
        }

        public void Clear()
        {
            _visibleObjects.Clear();
            _shadowObjects.Clear();
            _sceneGraph.Clear();
            _objects.Clear();

            //вот тут странно- вдруг мы хотим юзать редактор идешников в какомто другом
            //месте? получается идешники одни на весь пак? а если паков несколько? 
            //надобе сделать его не статическим- тогда будем сохранять где надо
            //а при загрузке инициализировать просто и сё.
            IdGenerator.ClearIdsCounter();
            // LevelEditor.Cleared();
        }

        public PivotObject GetObjectWithID(uint id)
        {
            foreach (PivotObject t in _objects)
                if (t.editorAspect.id == id)
                    return t;
            //   ConsoleWindow.TraceMessage("Unable to find object with id = " + id.ToString());
            return null;
        }

        public void AddObject(PivotObject newObject, bool needUpdate = true)
        {
            if(needUpdate)
                newObject.Update();
            newObject.behaviourmodel.Enable();
            _objects.Add(newObject);
            _sceneGraph.AddObject(newObject);
        }

        public void DeleteObjects(MyContainer<PivotObject> deletingobjects)
        {
            foreach (PivotObject t in deletingobjects)
            {
                _objects.Remove(t);
                _sceneGraph.RemoveObject(t);
            }
            // счетчик идов будет начинать все делать с 0
            if (_objects.Count == 0)
            {
                IdGenerator.ClearIdsCounter();
            }
        }

        public void RemoveObject(PivotObject deletingobjects)
        {
            _objects.Remove(deletingobjects);
            _sceneGraph.RemoveObject(deletingobjects);
            deletingobjects.behaviourmodel.Disable();
            // счетчик идов будет начинать все делать с 0
            if (_objects.Count == 0)
            {
                IdGenerator.ClearIdsCounter();
            }
        }

        public void AddObjects(MyContainer<PivotObject> newobjects)
        {
            foreach (PivotObject t in newobjects)
            {
                _objects.Add(t);
                _sceneGraph.AddObject(t);
            }
        }

        public void UpdateScene()
        {
            foreach (PivotObject po in _objects)
            {
                po.Update();
            }
            _sceneGraph.NewFrame();

        }

        public void CalculateVisibleObjects()
        {
            _sceneGraph.calculateVisibleObjects(GameEngine.Instance.Camera.cameraFrustum, _visibleObjects);
            _sceneGraph.calculateShadowVisibleObjects(GameEngine.Instance.GraphicPipeleine.frustumForShadow, _shadowObjects);
        }
        private MyContainer<PivotObject> _intersectedObjects = new MyContainer<PivotObject>();
        public bool SearchPoint(Microsoft.Xna.Framework.Ray __ray, float __distance, out Vector3 __resultPoint, out PivotObject __resultObject, out Vector3 __pointNormal)
        {
            Engine.Logic.PivotObject clickedlo = null;
            Vector3 newpoint = new Vector3();
            Vector3 normal = new Vector3();
            float distance = __distance;
            bool finded = false;
            foreach (Engine.Logic.PivotObject lo in this._intersectedObjects)
            {
                if (!lo._needBulletCast)
                    continue;
                Vector3? point = lo.raycastaspect.IntersectionClosest(__ray, lo.transform, ref normal);
                if (point != null)
                {
                    StatisticContainer.Instance().UpdateParameter("totalBulletFaces", lo.raycastaspect.RCCM.Indices.Length / 3);
                    float range = (point.Value - __ray.Position).Length();
                    if (range < distance)
                    {
                        clickedlo = lo;
                        distance = range;
                        newpoint = point.Value;
                        finded = true;
                    }
                }
            }
            __pointNormal = normal;
            __resultObject = clickedlo;
            __resultPoint = newpoint;
            return finded;
            
        }
        public PivotObject SearchBulletIntersection(Vector3 __start, Vector3 __end, out Vector3 __intersectionPoint, out Vector3 __pointNormal)
        {
            Vector3 min = new Vector3(__start.X < __end.X ? __start.X : __end.X,
               __start.Y < __end.Y ? __start.Y : __end.Y,
               __start.Z < __end.Z ? __start.Z : __end.Z);
            Vector3 max = new Vector3(__start.X > __end.X ? __start.X : __end.X,
                __start.Y > __end.Y ? __start.Y : __end.Y,
                __start.Z > __end.Z ? __start.Z : __end.Z);

            Microsoft.Xna.Framework.BoundingBox bb = new BoundingBox(min, max);

            _sceneGraph.calculateBoxIntersectedObjects(bb, _intersectedObjects);
            StatisticContainer.Instance().UpdateParameter("totalBulletObjects", _intersectedObjects.Count);
            Vector3 raydir = __end - __start;
            float length = raydir.Length();
            raydir.Normalize();
            Microsoft.Xna.Framework.Ray bulletRay = new Microsoft.Xna.Framework.Ray(__start, raydir);
            PivotObject resultObject = null;
            SearchPoint(bulletRay, length, out __intersectionPoint, out resultObject, out __pointNormal);
            
            return resultObject;
        }

    }
}
