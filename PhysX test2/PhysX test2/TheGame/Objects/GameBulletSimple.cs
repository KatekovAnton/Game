using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.TheGame.Level;

using PhysX_test2.Engine;
using PhysX_test2.Engine.Logic;

using Microsoft.Xna.Framework;

namespace PhysX_test2.TheGame.Objects
{
    public class GameBulletSimple:GameBulletBase
    {
        public Vector3 _currentPosition;
        public Vector3 _lastPosition;
        public LevelObject _object;
        public string _name;
        public bool _onLevel;

        public GameBulletSimple(GameLevel __level, string __name)
            : base(__level)
        {
            _name = __name;
            _object = Engine.GameEngine.LoadObject(__name, null, false) as LevelObject; 
        }


        public void LocateToLevel()
        {
            if (!_onLevel)
                _hisLevel.AddEngineObject(_object);
            _onLevel = true;
        }

        public void RemoveFromLevel()
        {
            if (_onLevel)
                _hisLevel.RemoveObject(_object);
            _onLevel = false;
        }

        ~GameBulletSimple()
        {
            RemoveFromLevel();
            PhysX_test2.Engine.ContentLoader.ContentLoader.UnloadPivotObject(_object);
        }
    }
}
