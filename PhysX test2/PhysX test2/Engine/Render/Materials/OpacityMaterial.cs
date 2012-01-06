using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;

namespace PhysX_test2.Engine.Render.Materials
{
    class OpacityMaterial: Material
    {
        public class SubsetMaterial
        {
            public Texture2D diffuseTexture;
            public Texture2D opacityTexture;

            public SubsetMaterial(Texture2D __diffuseTexture, Texture2D __opacityTexture)
            {
                diffuseTexture = __diffuseTexture;
                opacityTexture = __opacityTexture;
            }
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

        public OpacityMaterial(Lod[] _lodmats)
        {
            lodmats = _lodmats;
            type = MaterialType.OpacityDiffuse;
        }

        public override void Apply(int lod, int subset)
        {
            Material.ObjectRenderEffect.Parameters["DiffuseTexture"].SetValue(lodmats[lod].mats[subset].diffuseTexture);
            Material.ObjectRenderEffect.Parameters["OpacityTexture"].SetValue(lodmats[lod].mats[subset].opacityTexture);
        }
    }
}
