﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.Engine.Logic
{
    public class EngineScene
    {
        public MyContainer<PivotObject> ShadowObjects;
        public MyContainer<PivotObject> VisibleObjects;
        public MyContainer<PivotObject> objects;
        //мы же хотим переключать сцены?? поэтому каждой - свой сценграф
        public SceneGraph.SceneGraph sceneGraph;


        public EngineScene()
        {
            objects = new MyContainer<PivotObject>(100, 10);
            VisibleObjects = new MyContainer<PivotObject>(100, 2);
            ShadowObjects = new MyContainer<PivotObject>(100, 2);
            sceneGraph = new SceneGraph.SceneGraph(this);
        }

        public EngineScene(EngineScene s)
        {
            ShadowObjects = s.ShadowObjects;
            VisibleObjects = s.VisibleObjects;
            objects = s.objects;
            sceneGraph = s.sceneGraph;
        }

        public void Clear()
        {
            VisibleObjects.Clear();
            ShadowObjects.Clear();
            sceneGraph.Clear();
            objects.Clear();

            //вот тут странно- вдруг мы хотим юзать редактор идешников в какомто другом
            //месте? получается идешники одни на весь пак? а если паков несколько? 
            //надобе сделать его не статическим- тогда будем сохранять где надо
            //а при загрузке инициализировать просто и сё.
            IdGenerator.ClearIdsCounter();
            // LevelEditor.Cleared();
        }

        public PivotObject GetObjectWithID(uint id)
        {
            foreach (PivotObject t in objects)
                if (t.editorAspect.id == id)
                    return t;
            //   ConsoleWindow.TraceMessage("Unable to find object with id = " + id.ToString());
            return null;
        }

        public void AddObject(PivotObject newObject)
        {
            newObject.Update();
            objects.Add(newObject);
            sceneGraph.AddObject(newObject);
        }

        public void DeleteObjects(MyContainer<PivotObject> deletingobjects)
        {
            foreach (PivotObject t in deletingobjects)
            {
                objects.Remove(t);
                sceneGraph.DeleteObject(t);
            }
            // счетчик идов будет начинать все делать с 0
            if (objects.Count == 0)
            {
                IdGenerator.ClearIdsCounter();
            }
        }

        public void RemoveObject(PivotObject deletingobjects)
        {
            objects.Remove(deletingobjects);
            sceneGraph.DeleteObject(deletingobjects);

            // счетчик идов будет начинать все делать с 0
            if (objects.Count == 0)
            {
                IdGenerator.ClearIdsCounter();
            }
        }

        public void AddObjects(MyContainer<PivotObject> newobjects)
        {
            foreach (PivotObject t in newobjects)
            {
                objects.Add(t);
                sceneGraph.AddObject(t);
            }
        }

        public void UpdateScene()
        {
            foreach (PivotObject po in objects)
            {
                po.Update();
            }
            sceneGraph.NewFrame();

        }

        public void CalculateVisibleObjects()
        {
            sceneGraph.calculateVisibleObjects(GameEngine.Instance.Camera.cameraFrustum, VisibleObjects);
            sceneGraph.calculateShadowVisibleObjects(GameEngine.Instance.GraphicPipeleine.frustumForShadow, ShadowObjects);
        }
    }
}