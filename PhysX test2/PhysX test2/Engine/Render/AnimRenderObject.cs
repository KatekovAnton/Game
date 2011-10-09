using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.Engine.Animation;

namespace PhysX_test2.Engine.Render
{
    public class AnimRenderObject : RenderObject
    {
        Character character;

        public AnimRenderObject(Character _char)
            : base()
        {
            character = _char;
            isanimaated = true;
        }

        public override void SelfRender(int lod, Engine.Render.Materials.Material mat = null)
        {

        }
    }
}
