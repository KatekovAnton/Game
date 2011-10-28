using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.Engine.Logic;

namespace PhysX_test2.TheGame.Level
{
    public class GameLevel
    {
        public EngineScene _scene;

        public GameLevel(EngineScene __scene)
        {
            _scene = __scene;
        }

        public void AddLight(Engine.Logic.LigthSource theLight)
        { }

        public void RemoveLight(Engine.Logic.LigthSource theLight)
        { }

        public void AddObject(Engine.Logic.PivotObject theObjecr)
        { }

        public void RemoveObject(Engine.Logic.PivotObject theObjecr)
        { }
    }
}
