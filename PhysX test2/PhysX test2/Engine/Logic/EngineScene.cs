using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.Engine.Logic
{
    public class EngineScene
    {
        public MyContainer<PivotObject> _shadowObjects;
        public MyContainer<PivotObject> _visibleObjects;
        public MyContainer<PivotObject> _objects;
        //мы же хотим переключать сцены?? поэтому каждой - свой сценграф
        public SceneGraph.SceneGraph _sceneGraph;


        public EngineScene()
        {
            _objects = new MyContainer<PivotObject>(100, 10);
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
                __newObject.Update();
                _objects.Add(__newObject);
                _sceneGraph.AddObject(__newObject);
                return false;
            }

            MyContainer<MyContainer<PivotObject>.MyContainerRule> rules = _objects.FindAllRulesForObject(__oldObject);
            foreach (MyContainer<PivotObject>.MyContainerRule rule in rules)
            {
                if (rule.firstObject == __oldObject)
                    rule.firstObject = __newObject;
                else if (rule.secondObject == __oldObject)
                    rule.secondObject = __newObject;
            }
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

        public void AddObject(PivotObject newObject)
        {
            newObject.Update();
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
    }
}
