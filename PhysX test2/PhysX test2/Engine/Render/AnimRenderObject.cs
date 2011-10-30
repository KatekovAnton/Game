using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.Engine.Animation;
using PhysX_test2.Content;
using PhysX_test2.Engine.Render.Materials;

namespace PhysX_test2.Engine.Render
{
    public class AnimRenderObject : RenderObject
    {
        public Character character;

        #region RENDER

        public class SubSet : System.IDisposable
        {
            public EngineSkinnedMesh mesh;
            public bool Disposed = false;
            public SubSet(EngineSkinnedMesh m)
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

        public class Model : System.IDisposable
        {
            public SubSet[] subsets;
            public bool Disposed = false;
            public Model(SubSet[] array)
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

            ~Model()
            {
                Dispose();
            }
        }

        public Model[] LODs
        {
            get;
            private set;
        }

        #endregion

        public void ReceiveEvent(string __eventname)
        {
            character.ReceiveEvent(__eventname);
        }

        public AnimRenderObject(Character _char, AnimRenderObject.Model[] models, bool shadowcaster, bool shadowreceiver)
            : base()
        {
            character = _char;
            LODs = models;
            isanimated = true;
            isshadowcaster = shadowcaster;
            isshadowreceiver = shadowreceiver;
        }

        public override void SelfRender(int lod, Engine.Render.Materials.Material mat = null)
        {
            Microsoft.Xna.Framework.Graphics.Effect currentEffect = PhysX_test2.Engine.Render.Materials.Material.ObjectRenderEffect;
            currentEffect.Parameters["Frame"].SetValue(character._currentFames);
            if (mat != null)
            {
                for (int i = 0; i < LODs[lod].subsets.Length; i++)
                {
                    mat.Apply(lod, i);
                    foreach (var pass in currentEffect.CurrentTechnique.Passes)
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
                    foreach (var pass in currentEffect.CurrentTechnique.Passes)
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

        public override void setPosition(Microsoft.Xna.Framework.Matrix m)
        {
            character.Position = m;
        }
    }
}
