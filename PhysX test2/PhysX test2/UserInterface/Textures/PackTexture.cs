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
            IsReleased = false;
        }

        public PackTexture(string name)
            : base(name, null)
        {
            _packTexture = Engine.ContentLoader.ContentLoader.LoadTexture(name);
            _texture = _packTexture.texture;
            IsReleased = false;
        }

        public override void Retain()
        {
            Engine.ContentLoader.ContentLoader.LoadTexture(base._name);
        }

        public override void Release()
        {
            Engine.ContentLoader.ContentLoader.UnloadTexture(_packTexture);
            IsReleased = true;
        }
    }
}
