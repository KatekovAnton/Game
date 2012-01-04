using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;

using PhysX_test2.Content;
using PhysX_test2.Engine.Render.Materials;

using StillDesign.PhysX;

namespace PhysX_test2.Engine.Render
{
    public class SubSet : System.IDisposable
    {
        public EngineMesh mesh;
        public bool Disposed = false;
        public SubSet(EngineMesh m)
        {
            mesh = m;
        }
        public void Dispose()
        {
            if (!Disposed)
            {
                mesh.Dispose();
                mesh = null;
                Disposed = true;
                GC.SuppressFinalize(this);
            }
        }
        ~SubSet()
        {
            Dispose();
        }
    }

    public class MyModel : System.IDisposable
    {
        public SubSet[] subsets;
        public bool Disposed = false;
        public MyModel(SubSet[] array)
        {
            subsets = new SubSet[array.Length];
            array.CopyTo(subsets, 0);
        }
        public void Dispose()
        {
            if (!Disposed)
            {
                for (int i = 0; i < subsets.Length; i++)
                    subsets[i].Dispose();
                subsets = null;
                Disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        ~MyModel()
        {
            Dispose();
        }
    }

    public class UnAnimRenderObject : RenderObject
    {
        
       
        public MyModel[] LODs
        {
            get;
            private set;
        }

        public UnAnimRenderObject(MyModel[] lods)
        {
            LODs = lods;
        }

        public UnAnimRenderObject(
            MyModel[] lods,
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

        public override void Dispose()
        {
            if (!Disposed)
            {
                for (int i = 0; i < LODs.Length; i++)
                    LODs[i].Dispose();
                LODs = null;
                Disposed = true;
                GC.SuppressFinalize(this);
            }
        }

    }
}
