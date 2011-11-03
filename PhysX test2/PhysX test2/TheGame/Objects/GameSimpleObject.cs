using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.Engine;
using PhysX_test2.Engine.Logic;
using PhysX_test2.BaseExtensions.Graph;

using PhysX_test2.TheGame;

using PhysX_test2.TheGame.Level;
using PhysX_test2.TheGame.Objects.StateGraphs;

namespace PhysX_test2.TheGame.Objects
{
    public class GameSimpleObject : GameObject
    {
        public PivotObject _object;
        public GameSimpleObject(PivotObject __object, GameLevel __level, bool __mouseRC, bool __bulletRC)
            : base(__level, __mouseRC, __bulletRC)
        {
            _object = __object;
        }
    }
}
