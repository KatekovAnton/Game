﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;

namespace PhysX_test2.Engine.Render.Materials
{

    public abstract class Material:ContentNew.IPackEngineObject
    {
        public string TehniqueName
        {
            get;
            set;
        }

        private EffectTechnique Technique
        {
            get 
            {
                return ObjectRenderEffect.Techniques[TehniqueName];
            }
            set
            {
                TehniqueName = value.Name;
            }
        }

        public static Effect ObjectRenderEffect;

        public abstract void Apply(int lod, int subset);

        public void Dispose() 
        {

        }

        public void CreateFromContentEntity(ContentNew.IPackContentEntity[] __entities)
        { }

        public bool needAutoDispose()
        {
            return false;
        }
    }
}
