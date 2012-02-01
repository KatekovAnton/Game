using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysX_test2.UserInterface
{
    public abstract class CashedTexture2D
    {
        public string _name;
        public Texture2D _texture;

        public bool IsReleased
        {
            get;
            protected set;
        }

        public abstract void Release();

        /// <summary>
        /// выполняет загрузку текстуры из пака
        /// </summary>
        public abstract void Retain();

        public CashedTexture2D(string __name, Texture2D __texture, bool Retain_now = false)
        {
            _name = __name;
            _texture = __texture;
            if (Retain_now) Retain();
        }


        ~CashedTexture2D()
        {
            if (!IsReleased)
                Release();
        }
    }
}
