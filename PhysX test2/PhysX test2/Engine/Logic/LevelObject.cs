using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.Engine.Render;

namespace PhysX_test2.Engine.Logic
{
    public class LevelObject : PivotObject, ContentNew.IPackEngineObject
    {        
        /// <summary>
        /// for group для графики
        /// </summary>
        public RenderObject renderaspect;
        
        /// <summary>
        /// for group для графики - материал отделен от RenderObject-а чтоб их можно было по-разному группировать
        /// </summary>
        public Render.Materials.Material material;

        public LevelObject(BehaviourModel.ObjectBehaviourModel _behaviourmodel, RenderObject _renderaspect, Render.Materials.Material _material, RaycastBoundObject _raycastaspect)
        {
            behaviourmodel = _behaviourmodel;
            renderaspect = _renderaspect;
            raycastaspect = _raycastaspect;
            material = _material;
        }

        public override void SetVisible(bool __visible)
        {
            _visible = __visible;
            renderaspect.visible = __visible;
        }

        public override Render.Materials.Material HaveMaterial()
        {
            return material;
        }

        public override RenderObject HaveRenderAspect()
        {
            return renderaspect;
        }

        public override void DoFrame(Microsoft.Xna.Framework.GameTime gt)
        {
            behaviourmodel.DoFrame(gt);
            renderaspect.SetPosition(behaviourmodel.CurrentPosition);
        }

        public void CreateFromContentEntity(ContentNew.IPackContentEntity[] __contentEntities)
        {
            
        }

        public void Dispose()
        {

        }

        public bool needAutoDispose()
        {
            return false;
        }
    }
}
