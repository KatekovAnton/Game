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

        public ParticleObject _object;

        public GameParticleObject(GameLevel __level, Vector3 __maxSize, int __count, Vector3 __position, Vector3 __direction, float __dispersionRadius, float __gravityMult)
            :base(__level,false,false)
        {
            _object = new ParticleObject(__maxSize, __count, __position, __direction, __dispersionRadius, __gravityMult);
        }
    }
}
