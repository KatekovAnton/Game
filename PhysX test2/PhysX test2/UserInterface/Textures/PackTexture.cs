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

        public PackTexture(string name, bool Retain_now = false)        // retain_now - хорошо помогает, когда надо создать текстуру и передать ее как параметр в какой-то метод или конструктор
            : base(name, null)
        {
            _packTexture = Engine.ContentLoader.ContentLoader.LoadTexture(name);
            _texture = _packTexture.texture;
            if (Retain_now) Retain();
            IsReleased = false;
        }
        /// <summary>
        /// выполняет загрузку текстуры из пака
        /// </summary>
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
