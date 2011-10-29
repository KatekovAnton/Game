using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.Engine.Logic;

using Microsoft.Xna.Framework;

namespace PhysX_test2.TheGame.Level
{
    public class GameLevel
    {
        public EngineScene _scene;

        public GameLevel(EngineScene __scene)
        {
            _scene = __scene;
        }

        public Vector3 GetSpawnPlace()
        {
            return new Vector3(0, 15, 0);
        }

        public void AddLight(Engine.Logic.LigthSource theLight)
        {
            _scene.AddObject(theLight);
        }

        public void RemoveLight(Engine.Logic.LigthSource theLight)
        {
            _scene.RemoveObject(theLight);
        }

        public void AddObject(Engine.Logic.PivotObject theObjecr)
        {
            _scene.AddObject(theObjecr);
        }

        public void RemoveObject(Engine.Logic.PivotObject theObjecr)
        {
            _scene.RemoveObject(theObjecr);
        }
    }
}
