using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using PhysX_test2;
using PhysX_test2.Engine;
using PhysX_test2.Content;
using PhysX_test2.Engine.Render;
using PhysX_test2.Engine.Render.Materials;


/*
namespace PhysX_test2.Engine.Logic
{
    //хуй теперь знает для чего этот класс... подумать надо
    abstract class GameBeing : PivotObject
    {
        RenderObject ro;
        public readonly bool clickable;
        public bool animate
        {
            get;
            private set;
        }
        /// <summary>
        /// Мышь над объектом или нет
        /// </summary>
        /// <param name="_ray"></param>
        /// <returns></returns>
        public virtual bool getmousehover(Ray _ray)
        {
            throw new NotImplementedException("getmousehover at " + name);
        }
        /// <summary>
        /// если мышь над объектом - в _Point записать локальные кооординаты точки
        /// </summary>
        /// <param name="_ray"></param>
        /// <returns></returns>
        public virtual bool getlocalpoint(Ray _ray, out Vector3 _Point)
        {
            throw new NotImplementedException("getlocalpoint at " + name);
        }
        /// <summary>
        /// если мышь над объектом - в _Point записать глобальные кооординаты точки
        /// </summary>
        /// <param name="_ray"></param>
        /// <returns></returns>
        public virtual bool getglobalpoint(Ray _ray, out Vector3 _Point)
        {
            throw new NotImplementedException("getglobalpoint at " + name);
        }
        public GameBeing(RenderObject ro)
        {
            this.ro = ro;
            clickable = false;
        }
        public virtual void Draw()
        {
            Material.ObjectRenderEffect.Parameters["World"].SetValue(this.Transform);
            ro.Render(0);
        }

    }//общий так сказать объект. 
}
*/