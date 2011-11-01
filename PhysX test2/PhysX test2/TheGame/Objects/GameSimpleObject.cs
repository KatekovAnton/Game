using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.TheGame.Level;

namespace PhysX_test2.TheGame.Objects
{
    public class GameSimpleObject : GameObject
    {
        public GameSimpleObject(GameLevel __level, bool __mouseRC, bool __bulletRC)
            : base(__level, __mouseRC, __bulletRC)
        { }
    }
}
