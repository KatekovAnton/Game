using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PhysX_test2.Content;
using StillDesign.PhysX;
namespace PhysX_test2.Engine.Logic
{
    public abstract class RenderObject
    {
        public string PictureTehnique;
        public string ShadowTehnique;
        public bool isanimaated;
        public bool isshadowreceiver;
        public bool isshadowcaster;



        protected RenderObject()
        {
        
        }
       /* public void Render(int lod, Engine.Render.Materials.Material mat = null)
        {
            
            PhysX_test2.Engine.Render.Materials.Material.ObjectRenderEffect.Parameters["World"].SetValue(behaviourmodel.GetGlobalPose());
            SelfRender(lod, mat);
        }*/
        public abstract void SelfRender(int lod, Engine.Render.Materials.Material mat = null);



    }
}
