using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Render.Materials
{
    public class AnimatedMaterial : Material
    {
        public Material[] _frameMaterials;
        public Material _currentMaterial;

        public override void Apply(int lod, int subset)
        {
            _currentMaterial.Apply(lod, subset);
        }

    }
}
