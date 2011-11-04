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

using Microsoft.Xna.Framework;

namespace PhysX_test2.TheGame.Objects
{
    
    public class GameSimpleObject : GameObject
    {
        public bool _onLevel;
        public LevelObject _object;

        public GameSimpleObject(LevelObject __object, GameLevel __level, bool __mouseRC, bool __bulletRC)
            : base(__level, __mouseRC, __bulletRC)
        {
            _object = __object;
        }

        public GameSimpleObject(string __objectName, PivotObject __parentObject, Engine.Logic.PivotObjectDependType __dependType, GameLevel __level, bool __mouseRC, bool __bulletRC)
            :base(__level, __mouseRC, __bulletRC)
        {
            Engine.GameEngine.loadObject(__objectName, null, __mouseRC, __parentObject, __dependType); 
        }

        public void LocateToLevel(LevelObject __parentObject)
        {
            _hisLevel.AddObject(_object);

            if (__parentObject != null)
                _hisLevel.AddObjectSequence(__parentObject, _object);

            _onLevel = true;
        }

        public void RemoveFromLevel()
        {
            _hisLevel.RemoveObject(_object);
            _onLevel = false;
        }
    }
}
