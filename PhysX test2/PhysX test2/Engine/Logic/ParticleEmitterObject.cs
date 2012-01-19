using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.Engine.Render;
using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Logic
{
  /*  public class ParticleEmitterObject:ParticleObject
    {
        public double _emitionSpeed;

        /// <summary>
        /// Источник частиц, частицы запихивает в парента, но 
        /// сам при апдейте создает новые
        /// </summary>
        public ParticleEmitterObject(Vector3 __maxSize, int __particleCount,
            Vector3 __position, Vector3 __direction, float __dispersionRadius, float __gravityRelationMultiplier, float __emitionSpeed)
            : base(__maxSize, __particleCount, __position, __direction, __dispersionRadius, __gravityRelationMultiplier)
        {
            _emitionSpeed = __emitionSpeed;
        }

        public override void DoFrame(GameTime gt)
        {
            //calculate particle data
            UpdateParticles();

            GenerateParticles();
        }

        protected void GenerateParticles()
        {
            double time = MyGame.UpdateTime.ElapsedGameTime.TotalSeconds;
            //TODO 
            //generate new particles

            double particles = time / _emitionSpeed;
        }
    }*/
}
