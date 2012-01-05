using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StillDesign.PhysX;

using PhysX_test2.Engine.Render;

namespace PhysX_test2.Engine.Logic
{
    public enum PivotObjectDependType
    {
        Weapon,
        Head,
        Body
    }

    public enum PivotObjectMaterialType
    {
        Stone,
        Metal,
        Wood,
        DynamicMetal,
        DynamicWood,
        DynamicStone,
        DynamicHuman,
        DynamicAlien
    }
    
    public abstract class PivotObject
    {
        public bool _unloaded = false;
        public PivotObjectMaterialType matrialType = PivotObjectMaterialType.Stone;

        /// <summary>
        /// некая инфа о происхождении объекта
        /// </summary>
        public EditorData editorAspect;

        /// <summary>
        /// mixed объект для рейкаста- тыкания мышкой и прочих взаимодействием с лучами
        /// </summary>
        public RaycastBoundObject raycastaspect;

        /// <summary>
        /// матрица преобразования- положение поворот объекта - расчитывается на каждом кадре в behaviourmodel-е при апдейте
        /// </summary>
        public Matrix transform = Matrix.Identity;

        /// <summary>
        /// использовать ли дополнительную матрицу преобразования для рисования
        /// </summary>
        public bool useDeltaMatrix = false;

        /// <summary>
        /// матрица, использующаяся совместано с предыдущим флагом
        /// </summary>
        public Microsoft.Xna.Framework.Matrix deltaMatrix;

        /// <summary>
        /// итоговая матрица для рендера
        /// </summary>
        public Microsoft.Xna.Framework.Matrix renderMatrix;

        /// <summary>
        /// for one механика поведения объекта с физ точки зрения
        /// </summary>
        public BehaviourModel.ObjectBehaviourModel behaviourmodel;
        public BehaviourModel.BehaviourModelDescription bmDescription;

        /// <summary>
        /// associated game object
        /// </summary>
        public object _gameObject;

        /// <summary>
        /// calc or not cast with mouse ray for searching target point
        /// </summary>
        public bool _needMouseCast;
        public bool _needBulletCast;

        public bool _isBillboardCostrained;
        public bool _isBillboard;
        public Vector3 _objectConstrAxis = Vector3.UnitX;
        public Vector3 _objectConstrForward = -Vector3.UnitY;

        protected bool _visible;
        public bool moved;

        public virtual void SetVisible(bool __visible)
        {
            _visible = __visible;
        }

        public PivotObject()
        {
            _isBillboard = false;
            _isBillboardCostrained = false;
        }


        public abstract RenderObject HaveRenderAspect();
        public abstract Render.Materials.Material HaveMaterial();
        public abstract void DoFrame(GameTime gt);


        public void BeginDoFrame()
        {
            moved = false;
            behaviourmodel.BeginDoFrame();
            SetVisible(false);
        }

        public void EndDoFrame()
        {
            behaviourmodel.EndDoFrame();
            moved = behaviourmodel.moved;
        }

        public void Move(Microsoft.Xna.Framework.Vector3 d)
        {
            behaviourmodel.Move(d);
            moved = true;
        }

        public void SetParentObject(PivotObject __object)
        {
            BehaviourModel.ObjectBoneRelatedBehaviourModel model = behaviourmodel as BehaviourModel.ObjectBoneRelatedBehaviourModel;
            if (model != null)
            {
                model.SetParentObject(__object);
                return;
            }

            BehaviourModel.ObjectRelatedBehaviourModel model1 = behaviourmodel as BehaviourModel.ObjectRelatedBehaviourModel;
            if (model1 != null)
            {
                model1.SetParentObject(__object);
                return;
            }
        }

        public void SetGlobalPose(Microsoft.Xna.Framework.Matrix newPose, PivotObject __parent = null)
        {
            behaviourmodel.SetGlobalPose(newPose, null, __parent);
            transform = newPose;
            moved = true;
        }

        public void SetPosition(Vector3 position)
        {
            behaviourmodel.SetPosition(position);
            transform = behaviourmodel.globalpose;
            moved = true;
        }

        public void Update()
        {
            if (_isBillboard)
                transform = Matrix.CreateBillboard(behaviourmodel.globalpose.Translation, MyGame.Instance._engine.Camera._position, Vector3.Up, MyGame.Instance._engine.Camera._direction);
            else if (_isBillboardCostrained)
                transform = Matrix.CreateConstrainedBillboard(behaviourmodel.globalpose.Translation, MyGame.Instance._engine.Camera._position, _objectConstrAxis, MyGame.Instance._engine.Camera._direction, _objectConstrForward);
            else
                transform = behaviourmodel.globalpose;

            if (moved)
                raycastaspect.boundingShape.Update(transform);


            renderMatrix = transform;
            if (useDeltaMatrix)
                renderMatrix = deltaMatrix * transform;
            else
                renderMatrix = transform;

        }

        /*from editor
         public abstract void SetActive(bool active);
         */

    }
}
