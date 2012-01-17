using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.Engine.Render;
using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Logic
{
    public class ParticleEmitterObject:ParticleObject
    {
        /// <summary>
        /// Источник частиц, частицы запихивает в парента, но 
        /// сам при апдейте создает новые
        /// </summary>
        public ParticleEmitterObject(TimeSpan __currentTime, Vector3 __maxSize, int __particleCount, Vector3 __position, Vector3 __direction, float __gravityRelationMultiplier)
            : base(__currentTime, __maxSize, __particleCount, __position, __direction, __gravityRelationMultiplier)
        {
        }

        public override void DoFrame(GameTime gt)
        {
            //calculate particle data
            UpdateParticles();

            GenerateParticles();

            SortParticles();

            //set particle data
            renderaspect.SetParticleData(GetParticleData());
        }

        protected void GenerateParticles()
        {
            double time = MyGame.UpdateTime.ElapsedGameTime.TotalSeconds;
            //TODO 
            //generate new particles
        }
    }
}
