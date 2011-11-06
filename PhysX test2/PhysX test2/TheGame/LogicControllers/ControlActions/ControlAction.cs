using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using PhysX_test2.Engine.Logic;



namespace PhysX_test2.TheGame.LogicControllers.ControlActions
{
    public abstract class ControlAction
    {
        public GameTime _startTime;


        public abstract void Update(PivotObject __object);
    }
}
