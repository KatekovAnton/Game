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
            _levelObject = Engine.GameEngine.LoadObject(__aliveName, __aliveMatrix, true, true) as LevelObject;
            _levelObject.matrialType = PivotObjectMaterialType.DynamicHuman;
            _controllerAlive = (_levelObject.renderaspect as Engine.Render.AnimRenderObject).character;

            _isAlive = false;
            _onLevel = false;
        }

        public override void RemoveFromLevel()
        {
            _hisLevel.RemoveObject(_levelObject);
        }

        public void dropParameters()
        {
            //method
        }

        public void SetDead()
        {
            #if DEBUG
            if (!_isAlive)
                ExcLog.LogException("GameCharacter.SetDead whereever its already dead!");
            #endif
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


            _controllerAlive.MakeUnconditionalTransition("dead01\0", true, "dead01\0");
        }

        public override void LocateToLevel(LevelObject __parent)
        {
            #if DEBUG
            if (__parent != null)
                ExcLog.LogException("GameCharacter.LocateToLevel with parent not null!");
            #endif
            _hisLevel.AddEngineObject(_levelObject);
            _onLevel = true;
        }

        public void Fire(string __animName, string __idleName)
        {
            if (!_isAlive)
                return;

            _controllerAlive.MakeUnconditionalTransition(__animName, false, __idleName);
        }

        public void SetAlive(string __stayBottomName, string __stayTopName)
        {
#if DEBUG
            if (_isAlive)
                ExcLog.LogException("GameCharacter.SetAlive but its already alive!");
#endif
            _isAlive = true;
            //method
            _levelObject.SetPosition(_hisLevel.GetSpawnPlace());
           // _levelObject.behaviourmodel.Rotate(0.9f);
            if (_onLevel)
            {
                _levelObject.behaviourmodel.Enable();
            }
            else
            {
                LocateToLevel(null);
            }
            
            if (_controllerAlive == null)
                return;

            _controllerAlive.MakeUnconditionalTransition(__stayBottomName, 0, false, __stayBottomName);
            _controllerAlive.MakeUnconditionalTransition(__stayTopName, 1, false, __stayTopName);
        }

        public static void edgeDeadToAlive(GameCharacter __object)
        {
            //method
          //  __object.SetAlive();
        }

        public static void edgeAliveToDead(GameCharacter __object)
        {
            //method
           // __object.SetDead();
        }

        public override void Rotate(float __angle)
        {
            _levelObject.behaviourmodel.Rotate(__angle);
        }

        public override void Unload()
        {
            RemoveFromLevel();
            Engine.ContentLoader.ContentLoader.UnloadPivotObject(_levelObject);

            _controllerAlive = null;

            _characterStateController = null;
        }
    }

}
