using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;

using PhysX_test2.Content;
using PhysX_test2.Engine.Render.Materials;

using StillDesign.PhysX;

namespace PhysX_test2.Engine.Logic
{

    public class UnAnimRenderObject : RenderObject
    {
        #region RENDER

        public class SubSet 
        {
            public EngineMesh mesh;
            public SubSet(EngineMesh m)
            {
                mesh = m;
            }
        }

        public class Model
        {
            public SubSet[] subsets
            {
                get;
                private set;
            }

            public Model(SubSet[] array)
            {
                subsets = new SubSet[array.Length];
                array.CopyTo(subsets, 0);
            }

        }
       
        public Model[] LODs
        {
            get;
            private set;
        }

        #endregion

        public UnAnimRenderObject(Model[] lods)
        {
            LODs = lods;
        }

        public UnAnimRenderObject(
            Model[] lods,
            bool shadowcaster,
            bool shadowreceiver)
        {
            LODs = lods;
            isshadowcaster = shadowcaster;
            isshadowreceiver = shadowreceiver;
        }

        public override void SelfRender(int lod, Engine.Render.Materials.Material mat = null)
        {
            if (mat!=null)
            {
                for (int i = 0; i < LODs[lod].subsets.Length; i++)
                {
                    mat.Apply(lod, i);
                    foreach (var pass in PhysX_test2.Engine.Render.Materials.Material.ObjectRenderEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        LODs[lod].subsets[i].mesh.Render();
                    }
                }
            }
            else
            {
                foreach (SubSet s in LODs[lod].subsets)
                {
                    foreach (var pass in PhysX_test2.Engine.Render.Materials.Material.ObjectRenderEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        s.mesh.Render();
                    }
                }
            }
        }

    }
}
