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
    public class GameCharacter : GameObject, IGraphUser
    {
        public bool _onLevel;
        public bool _isAlive;
        public LevelObject _deadObject;
        public LevelObject _aliveObject;

        public LevelObject _currentObject;

        public Engine.Animation.CharacterController _controllerAlive;
        public Engine.Animation.CharacterController _controllerDead; 

        public ObjectGraphController _characterStateController;

        

        public GameCharacter(string __aliveName, Matrix __aliveMatrix, string __deadName, Matrix __deadMatrix, GameLevel __level)
            : base(__level, true, true)
        {
            _aliveObject = Engine.GameEngine.loadObject(__aliveName, __aliveMatrix, true) as LevelObject;
            _deadObject = Engine.GameEngine.loadObject(__deadName, __deadMatrix, true) as LevelObject;

            _controllerAlive = (_aliveObject.renderaspect as Engine.Render.AnimRenderObject).character;
            _controllerDead = (_deadObject.renderaspect as Engine.Render.AnimRenderObject).character;

            _isAlive = false;
            _onLevel = false;
        }

        public void dropParameters()
        {
            //method
        }

        public void SetDead()
        {
            if (!_isAlive)
                throw new Exception();
            _isAlive = false;
            //method
            _deadObject.SetGlobalPose(_aliveObject.transform);
            if (_onLevel)
                _hisLevel._scene.SwapObjects(_aliveObject, _deadObject, false);
            else
            {
                _hisLevel.AddObject(_deadObject);
                _onLevel = true;
            }

            if (_controllerAlive == null)
                return;

            if (_controllerDead == null)
                return;

            _controllerDead.CompyPoseFromAnother(_controllerAlive);
            _controllerDead.MakeUnconditionalTransition("dead01\0", true);

            _currentObject = _deadObject;
        }

        public void Fire()
        {
            if (!_isAlive)
                return;

            _controllerAlive.ReceiveEvent("fire01\0", true);
        }

        public void SetAlive()
        {
            if (_isAlive)
                throw new Exception();
            _isAlive = true;
            //method
            _aliveObject.SetPosition(_hisLevel.GetSpawnPlace());
            if (_onLevel)
            {
                _hisLevel._scene.SwapObjects(_deadObject, _aliveObject, false);
            }
            else
            {
                _hisLevel.AddObject(_aliveObject);
                _onLevel = true;
            }
            
            if (_controllerAlive == null)
                return;

            _controllerAlive.MakeUnconditionalTransition("stay1\0", false);
            _currentObject = _aliveObject;
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
