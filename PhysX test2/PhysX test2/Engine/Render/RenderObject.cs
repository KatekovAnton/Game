using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PhysX_test2.Content;
using StillDesign.PhysX;
namespace PhysX_test2.Engine.Render
{
    public abstract class RenderObject:System.IDisposable
    {
        public string PictureTehnique;
        public string ShadowTehnique;
        public bool isanimated;
        public bool isshadowreceiver;
        public bool isshadowcaster;

        public bool Disposed = false;

        protected RenderObject()
        {
        
        }

        public virtual void setPosition(Microsoft.Xna.Framework.Matrix m)
        { }

        public abstract void SelfRender(int lod, Engine.Render.Materials.Material mat = null);

        public virtual void Dispose()
        {
            Disposed = true;
        }

    }
}
