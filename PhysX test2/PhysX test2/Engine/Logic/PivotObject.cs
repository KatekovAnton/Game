using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StillDesign.PhysX;

namespace PhysX_test2.Engine.Logic
{
    public abstract class PivotObject
    {
        public EditorData editorAspect;
        //mixed
        public RaycastBoundObject raycastaspect;
        public Matrix transform = Matrix.Identity;
      //  public bool neetforceupdate = false;
        public PivotObject()
        { }

        public bool moved;
       
        public abstract void Move(Vector3 d);
        public abstract void SetGlobalPose(Matrix newPose);
        public abstract RenderObject HaveRenderAspect();
        public abstract Render.Materials.Material HaveMaterial();
        public abstract void Update();
        public abstract void BeginDoFrame();
        public abstract void EndDoFrame();
        public virtual void DoFrame(GameTime gt)
        {
              
        }
        /*from editor
         public abstract void SetActive(bool active);
         */

    }
}
