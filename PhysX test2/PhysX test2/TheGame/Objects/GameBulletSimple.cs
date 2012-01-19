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
            _object = Engine.GameEngine.LoadObject(__name, null, false, false) as LevelObject; 
        }

        public void Delete()
        {
            RemoveFromLevel();
            PhysX_test2.Engine.ContentLoader.ContentLoader.UnloadPivotObject(_object);
        }

        public override void RemoveFromLevel()
        {
            if (_onLevel)
                _hisLevel.RemoveObject(_object);
            _onLevel = false;
        }

        public override void Unload()
        {
            RemoveFromLevel();
            Engine.ContentLoader.ContentLoader.UnloadPivotObject(_object);
        }

        public override void LocateToLevel(LevelObject __parent)
        {
#if DEBUG
            if (__parent != null)
                ExcLog.LogException("GameBulletSimple.LocateToLevel with parent!!!");

            if(_onLevel)
                ExcLog.LogException("GameBulletSimple.LocateToLevel but already located!!!");
#endif
            if (!_onLevel)
                _hisLevel.AddEngineObject(_object);
            _onLevel = true;
        }

        ~GameBulletSimple()
        {
            Delete();
        }
    }
}
