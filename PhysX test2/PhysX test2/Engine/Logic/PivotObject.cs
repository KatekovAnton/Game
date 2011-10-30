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
    public abstract class PivotObject
    {
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

        public PivotObject()
        { }

        public bool moved;
       
        public abstract RenderObject HaveRenderAspect();
        public abstract Render.Materials.Material HaveMaterial();
        public abstract void DoFrame(GameTime gt);


        public void BeginDoFrame()
        {
            moved = false;
            behaviourmodel.BeginDoFrame();
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

        public void SetGlobalPose(Microsoft.Xna.Framework.Matrix newPose)
        {
            behaviourmodel.SetGlobalPose(newPose, null);
            transform = newPose;
            moved = true;
        }

        public void SetPosition(Vector3 position)
        {
            behaviourmodel.SetPosition(position);
            transform = behaviourmodel.globalpose ;
            moved = true;
        }

        public void Update()
        {
            transform = behaviourmodel.globalpose;
            renderMatrix = useDeltaMatrix ? deltaMatrix * transform : transform;
            if (moved)
                raycastaspect.boundingShape.Update(transform);

        }

        /*from editor
         public abstract void SetActive(bool active);
         */

    }
}
