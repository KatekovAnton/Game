using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using PhysX_test2.Engine.Logic;

using PhysX_test2.TheGame.Level;

namespace PhysX_test2.TheGame.LogicControllers
{
    public abstract class BaseLogicController
    {
        public uint _id;
        public TimeSpan _createdTime;
        public abstract Parameters.InteractiveObjectParameters GetParameters();
        public abstract void TakeHit(Parameters.BulletParameters __bulletParameters);
        public abstract void Update(GameTime __gameTime);
        public GameLevel _itsLevel;

        public BaseLogicController()
        {
            _id = IdGenerator.StaticGenerator.NewId();
        }
    }
}
