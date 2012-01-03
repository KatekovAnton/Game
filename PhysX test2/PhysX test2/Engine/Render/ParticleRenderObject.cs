using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Render
{
    public class ParticleRenderObject:RenderObject
    {
        private Matrix[] _matrices;

        public ParticleRenderObject(int __particleCount)
        {
            _matrices = new Matrix[__particleCount];
            for (int i = 0; i < __particleCount; i++)
                _matrices[i] = Matrix.Identity;
        }

        public override void SelfRender(int lod, Materials.Material mat = null)
        {
            //TODO: instancing!!!
        }

        /// <summary>
        /// Set data in update for using it in draw()
        /// </summary>
        /// <param name="matrices">Matrices per particle</param>
        public void SetParticleData(Matrix[] matrices)
        {
            _matrices = matrices;
        }
    }
}
