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

        private bool _locatedAtLastFrame = false;

        public GameSimpleObject(string __objectName, GameLevel __level, Engine.Logic.PivotObjectDependType __dependType = PivotObjectDependType.Body, bool __mouseRC = false, bool __bulletRC = false)
            : base(__level, __mouseRC, __bulletRC)
        {
            _object = Engine.GameEngine.LoadObject(__objectName, null, __mouseRC, __bulletRC, __dependType) as LevelObject;
        }

        public void CalcParameters(PivotObject __parent)
        {
            if (!_onLevel || _locatedAtLastFrame)
            {
                _locatedAtLastFrame = false;
                return;
            }

            if (_object._needCalcAcxis)
            {
                Vector3 axis = Vector3.UnitY;
                if(__parent != null)
                    axis = Vector3.TransformNormal(Vector3.UnitY, __parent.transform);
                else
                    axis = Vector3.TransformNormal(Vector3.UnitY, _object.transform);
                _object._objectConstrAxis = axis;
            }
        }

        public override void LocateToLevel(LevelObject __parentObject)
        {
            if (!_onLevel)
            {
                _onLevel = true;
                _hisLevel.AddEngineObject(_object, __parentObject);
                _object.CreateRenderSimple();

                _locatedAtLastFrame = true;
            }
        }

        public void LocateBillboardToLevel(LevelObject __parentObject)
        {
            if (!_onLevel)
            {
                _onLevel = true;
                _hisLevel.AddEngineObject(_object, __parentObject);
                _object.CreateRenderBillboard();

                _locatedAtLastFrame = true;
            }
        }

        public void LocateConstrainedToLevel(LevelObject __parentObject, Vector3 __delta)
        {
            if (!_onLevel)
            {
                _onLevel = true;
                _object.CreateRenderConstrBillboard();
                CalcParameters(__parentObject);
                _hisLevel.AddEngineObject(_object, Matrix.CreateTranslation(__delta), __parentObject);

                _locatedAtLastFrame = true;
            }
        }

        public override void RemoveFromLevel()
        {
            if(_onLevel)
                _hisLevel.RemoveObject(_object);
            _onLevel = false;
        }

        public override void Unload()
        {
            if (!_object._unloaded)
                PhysX_test2.Engine.ContentLoader.ContentLoader.UnloadPivotObject(_object);
        }

        ~GameSimpleObject()
        {
            RemoveFromLevel();
            Unload();
        }
    }
}
