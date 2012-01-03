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

        public LevelObject _parent;

        public GameSimpleObject(string __objectName, Engine.Logic.PivotObjectDependType __dependType, GameLevel __level, bool __mouseRC, bool __bulletRC)
            :base(__level, __mouseRC, __bulletRC)
        {
            _object = Engine.GameEngine.LoadObject(__objectName, null, __mouseRC, __bulletRC, __dependType) as LevelObject; 
        }

        public void CalcParameters()
        {
            if (!_onLevel)
                return;

            if (_object._isBillboardCostrained)
            {
                Vector3 axis = Vector3.TransformNormal(Vector3.UnitZ, _parent.transform);
                _object._objectConstrAxis = axis;
            }
        }

        public void LocateToLevel(LevelObject __parentObject)
        {
            if (!_onLevel)
            {
                _parent = __parentObject;
                _hisLevel.AddEngineObject(_object, __parentObject);
                _object._isBillboard = false;
                _object._isBillboardCostrained = false;
            }
            _onLevel = true;
        }

        public void LocateBillboardToLevel(LevelObject __parentObject)
        {
            if (!_onLevel)
            {
                _parent = __parentObject;
                _hisLevel.AddEngineObject(_object, __parentObject);
                _object._isBillboard = true;
                _object._isBillboardCostrained = false;
            }
            _onLevel = true;
        }

        public void LocateConstrainedToLevel(LevelObject __parentObject)
        {
            if (!_onLevel)
            {
                _parent = __parentObject;
                _hisLevel.AddEngineObject(_object, __parentObject);
                _object._isBillboard = false;
                _object._isBillboardCostrained = true;
            }
            _onLevel = true;
        }

        public void RemoveFromLevel()
        {
            if(_onLevel)
                _hisLevel.RemoveObject(_object);
            _parent = null;
            _onLevel = false;
        }

        ~GameSimpleObject()
        {
            RemoveFromLevel();
            PhysX_test2.Engine.ContentLoader.ContentLoader.UnloadPivotObject(_object);
        }
    }
}
