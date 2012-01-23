using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.UserInterface
{
    public class PackTexture : CashedTexture2D
    {
        public Content.Texture _packTexture;

        public PackTexture(Content.Texture __texture)
            :base(__texture.name, __texture.texture)
        {
            _packTexture = __texture;
            _isReleased = false;
        }

        public override void Release()
        {
            Engine.ContentLoader.ContentLoader.UnloadTexture(_packTexture);
            _isReleased = true;
        }
    }
}
