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
       // public abstract Parameters.DynamicParameters GetParameters();
        public abstract void TakeHit(Parameters.BulletDynamicParameters __bulletParameters);
        public abstract void Update(GameTime __gameTime);
        public abstract void RemoveFromLevel();
        protected GameLevel _itsLevel;

        public BaseLogicController(GameLevel __level)
        {
            _itsLevel = __level;
            _itsLevel.AddController(this); 
            _id = IdGenerator.StaticGenerator.NewId();
        }

        ~BaseLogicController()
        { }
    }
}
