﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.TheGame.Level;

namespace PhysX_test2.TheGame.Objects
{
    public abstract class GameBulletBase : GameObject
    {
        public GameBulletBase(GameLevel __level)
            : base(__level, false, false)
        { }
    }
}
