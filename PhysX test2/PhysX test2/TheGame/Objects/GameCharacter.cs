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
        public LevelObject _levelObject;

        public Engine.Animation.CharacterController _controllerAlive;

        public ObjectGraphController _characterStateController;

        

        public GameCharacter(string __aliveName, Matrix __aliveMatrix, GameLevel __level)
            : base(__level, true, true)
        {
            _levelObject = Engine.GameEngine.LoadObject(__aliveName, __aliveMatrix, true) as LevelObject;

            _controllerAlive = (_levelObject.renderaspect as Engine.Render.AnimRenderObject).character;

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
            if (_onLevel)
                _levelObject.behaviourmodel.Disable();
            else
            {
                _hisLevel.AddEngineObject(_levelObject);
                _onLevel = true;
                _levelObject.behaviourmodel.Disable();
            }

            if (_controllerAlive == null)
                return;

         
            _controllerAlive.MakeUnconditionalTransition("dead01\0", true);
        }

        public void Fire()
        {
            if (!_isAlive)
                return;

            

            _controllerAlive.MakeUnconditionalTransition("fire01\0", false);
        }

        public void SetAlive()
        {
            if (_isAlive)
                throw new Exception();
            _isAlive = true;
            //method
            _levelObject.SetPosition(_hisLevel.GetSpawnPlace());
            if (_onLevel)
            {
                _levelObject.behaviourmodel.Enable();
            }
            else
            {
                _hisLevel.AddEngineObject(_levelObject);
                _onLevel = true;
            }
            
            if (_controllerAlive == null)
                return;

            _controllerAlive.MakeUnconditionalTransition("stay1\0", false);
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

        public override void Rotate(float __angle)
        {
            _levelObject.behaviourmodel.Rotate(__angle);
        }
    }

}
