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
        public LevelObject _hisObject;
        public string _name;

        public GameBulletSimple(GameLevel __level, string __name)
            : base(__level)
        {
            _name = __name;
        }
    }
}
