using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysX_test2.UserInterface
{
    public class ExternalTexture:CashedTexture2D
    {
        public ExternalTexture(string __name)
            : base(__name, MyGame.Instance.Content.Load<Texture2D>(__name))
        {
            IsReleased = false;
        }

        public override void Release()
        {
            if (IsReleased)
                return;
            IsReleased = true;
            _texture.Dispose();
        }
    }
}
