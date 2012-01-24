using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;

namespace PhysX_test2.Engine.Render.Materials
{
    class DiffuseMaterial : Material
    {
        public class SubsetMaterial
        {
            public Texture2D diffuseTexture;

        }

        public class Lod
        {
            public SubsetMaterial[] mats;
            public Lod(SubsetMaterial[] _mats)
            {
                mats = _mats;
            }
        }

        public Lod[] lodmats;

        public DiffuseMaterial(Lod[] _lodmats)
        {
            lodmats = _lodmats;
        }

        public override void Apply(int lod, int subset)
        {
            Material.ObjectRenderEffect.Parameters["DiffuseTexture"].SetValue(lodmats[lod].mats[subset].diffuseTexture);
        }
    }
}
