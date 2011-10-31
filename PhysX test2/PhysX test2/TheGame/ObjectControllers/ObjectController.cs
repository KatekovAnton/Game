using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using PhysX_test2.Engine.Logic;

namespace PhysX_test2.TheGame.ObjectControllers
{
    public abstract class ObjectController
    {
        public PivotObject _controlledObject;

        public GameTime startTime;
    }
}
