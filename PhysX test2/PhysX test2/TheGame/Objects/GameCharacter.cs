﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using PhysX_test2.Engine;
using PhysX_test2.Engine.Logic;
using PhysX_test2.BaseExtensions.Graph;

using PhysX_test2.TheGame;

using PhysX_test2.TheGame.Level;
using PhysX_test2.TheGame.Objects.StateGraphs;

namespace PhysX_test2.TheGame.Objects
{
    public class GameCharacter : GameObject,IGraphUser
    {
        GameLevel _hisLevel;

        PivotObject _deadObject;
        PivotObject _aliveObject;

        ObjectGraphController _characterStateController;

        public GameCharacter(PivotObject __aliveObject, PivotObject __deadObject, GameLevel __level)
        {
            _deadObject = __deadObject;
            _aliveObject = __aliveObject;
            _hisLevel = __level;

            _characterStateController = new ObjectGraphController(StaticObjects.Instance()._graphCharacter, this);
        }

        public void dropParameters()
        {
            //method
        }

        public void setDead()
        {
            //method
            _deadObject.SetGlobalPose(_aliveObject.transform);
            _hisLevel.RemoveObject(_aliveObject);
            _hisLevel.AddObject(_deadObject);
            
        }

        public void setAlive(Vector3 location)
        {
            //method
            _aliveObject.SetPosition(_hisLevel.GetSpawnPlace());
            _hisLevel.RemoveObject(_deadObject);
            _hisLevel.AddObject(_aliveObject);
        }

        public static void edgeDeadToAlive(GameCharacter __object)
        {
            //method
            __object.setAlive(__object._hisLevel.GetSpawnPlace());
        }

        public static void edgeAliveToDead(GameCharacter __object)
        {
            //method
            __object.setDead();
        }
    }

}