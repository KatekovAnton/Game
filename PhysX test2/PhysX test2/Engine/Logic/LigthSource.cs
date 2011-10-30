using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace PhysX_test2.Engine.Logic
{
    public enum LigthSourceType {Point, DirectPoint, Global};
    public class LigthSource: PivotObject
    {
        public LigthSourceType type;
        public override void DoFrame(GameTime gt)
        {
            throw new NotImplementedException();
        }

        public override Render.Materials.Material HaveMaterial()
        {
            throw new NotImplementedException();
        }

        public override Render.RenderObject HaveRenderAspect()
        {
            throw new NotImplementedException();
        }
    }
}
