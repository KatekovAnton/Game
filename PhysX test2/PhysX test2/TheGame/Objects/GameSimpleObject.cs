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

      

        public GameSimpleObject(string __objectName, PivotObject __parentObject, Engine.Logic.PivotObjectDependType __dependType, GameLevel __level, bool __mouseRC, bool __bulletRC)
            :base(__level, __mouseRC, __bulletRC)
        {
            _object = Engine.GameEngine.loadObject(__objectName, null, __mouseRC, __parentObject, __dependType) as LevelObject; 
        }

        public void LocateToLevel(LevelObject __parentObject)
        {
            if(!_onLevel)
                _hisLevel.AddObject(_object, __parentObject);
            _onLevel = true;
        }

        public void RemoveFromLevel()
        {
            if(_onLevel)
                _hisLevel.RemoveObject(_object);
            _onLevel = false;
        }
    }
}
