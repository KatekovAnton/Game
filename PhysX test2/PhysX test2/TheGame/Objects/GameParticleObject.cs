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
    public class GameParticleObject:GameObject
    {
        public bool _onLevel;
        public ParticleObject _object;

        public GameParticleObject(string __objectName, GameLevel __level, Vector3 __maxSize, int __count, Vector3 __direction, float __dispersionRadius, float __gravityMult)
            : base(__level, false, false)
        {
            _onLevel = false;
            _object = GameEngine.LoadParticleObject("EffectParticles\0", __maxSize);
            _object.SetParticlesParameters(__count, __direction, __dispersionRadius, __gravityMult);
        }

        public override void LocateToLevel(LevelObject __parentObject)
        {
            if (!_onLevel)
            {
                _onLevel = true;
                _hisLevel.AddEngineObject(_object, __parentObject);
                _object._isBillboard = false;
                _object._isBillboardCostrained = false;
                _object.FirstEmition();
            }
        }

        public override void RemoveFromLevel()
        {
            if (_onLevel)
                _hisLevel.RemoveObject(_object);
            _onLevel = false;
        }

        public override void Unload()
        {
            if (!_object._unloaded)
                PhysX_test2.Engine.ContentLoader.ContentLoader.UnloadParticleObject(_object);
        }

        ~GameParticleObject()
        {
            RemoveFromLevel();
            Unload();
        }
    }
}
