using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using PhysX_test2.Engine.Logic;

namespace PhysX_test2.TheGame.LogicControllers
{
    public abstract class BaseLogicController
    {
        public GameTime _createdTime;
        public abstract Parameters.InteractiveObjectParameters GetParameters();
        public abstract void TakeHit(Parameters.BulletParameters __bulletParameters);
        public abstract void Update(GameTime __gameTime);
    }
}
