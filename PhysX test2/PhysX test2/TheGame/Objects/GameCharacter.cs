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

        public LevelObject _deadObject;
        public LevelObject _aliveObject;


        public ObjectGraphController _characterStateController;

        

        public GameCharacter(string __aliveName, Matrix __aliveMatrix, string __deadName, Matrix __deadMatrix, GameLevel __level)
            : base(__level, true, true)
        {
            _aliveObject = Engine.GameEngine.loadObject(__aliveName, __aliveMatrix, true) as LevelObject;
            _deadObject = Engine.GameEngine.loadObject(__deadName, __deadMatrix, true) as LevelObject;
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

            Engine.Animation.CharacterController controllerAlive = (_aliveObject.renderaspect as Engine.Render.AnimRenderObject).character;
            if (controllerAlive == null)
                return;

            Engine.Animation.CharacterController controllerDead = (_deadObject.renderaspect as Engine.Render.AnimRenderObject).character;
            if (controllerDead == null)
                return;

            controllerAlive._currentFames.CopyTo(controllerDead._currentFames, 0);
            controllerDead.MakeUnconditionalTransition("dead\0", true);

        }

        public void SetAlive()
        {
            //method
            _aliveObject.SetPosition(_hisLevel.GetSpawnPlace());
            _hisLevel._scene.SwapObjects(_deadObject, _aliveObject, false);

            Engine.Animation.CharacterController controllerAlive = (_aliveObject.renderaspect as Engine.Render.AnimRenderObject).character;
            if (controllerAlive == null)
                return;
            controllerAlive.MakeUnconditionalTransition("stay01\0", false);
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
