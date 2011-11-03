using System;
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
        

        public PivotObject _deadObject;
        public PivotObject _aliveObject;

        public ObjectGraphController _characterStateController;

        public GameCharacter(PivotObject __aliveObject, PivotObject __deadObject, GameLevel __level)
            : base(__level, true, true)
        {
            _deadObject = __deadObject;
            _aliveObject = __aliveObject;

            _characterStateController = new ObjectGraphController(StaticObjects.Instance()._graphCharacter, this);
        }

        public void dropParameters()
        {
            //method
        }

        public void SetDead()
        {
            //method
            _deadObject.SetGlobalPose(_aliveObject.transform);
             _hisLevel._scene.SwapObjects(_aliveObject, _deadObject, false);
            
        }

        public void SetAlive()
        {
            //method
            _aliveObject.SetPosition(_hisLevel.GetSpawnPlace());
            _hisLevel._scene.SwapObjects(_deadObject, _aliveObject, false);
            
        }

        public static void edgeDeadToAlive(GameCharacter __object)
        {
            //method
            __object.SetAlive();
        }

        public static void edgeAliveToDead(GameCharacter __object)
        {
            //method
            __object.SetDead();
        }
    }

}
