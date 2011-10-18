using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.Engine.Render;

namespace PhysX_test2.Engine.Logic
{
    public class LevelObject : PivotObject
    {
        /// <summary>
        /// for one механика поведения объекта с физ точки зрения
        /// </summary>
        public BehaviourModel.ObjectBehaviourModel behaviourmodel;
        
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

        public override Render.Materials.Material HaveMaterial()
        {
            return material;
        }

        public override RenderObject HaveRenderAspect()
        {
            return renderaspect;
        }

        public override void Move(Microsoft.Xna.Framework.Vector3 d)
        {
            behaviourmodel.Move(d);
            moved = true;
        }

        public override void SetGlobalPose(Microsoft.Xna.Framework.Matrix newPose)
        {
            behaviourmodel.SetGlobalPose(newPose, null);
            transform = newPose;
            moved = true;
        }

        public override void BeginDoFrame()
        {
            moved = false;
            behaviourmodel.BeginDoFrame();
        }
        
        public override void EndDoFrame()
        {
            behaviourmodel.EndDoFrame();
            moved= behaviourmodel.moved;
        }

        public override void DoFrame(Microsoft.Xna.Framework.GameTime gt)
        {
            behaviourmodel.DoFrame(gt);
            renderaspect.setPosition(behaviourmodel.CurrentPosition);
        }

        public override void Update()
        {
            transform = behaviourmodel.globalpose;
            renderMatrix = useDeltaMatrix ? deltaMatrix*transform: transform;
            if (moved)
                raycastaspect.boundingShape.Update(transform);
         
        }
        /*from editor
        public override void SetActive(bool active)
        {
            editorAspect.isActive = active;
        }
         */
    }
}
