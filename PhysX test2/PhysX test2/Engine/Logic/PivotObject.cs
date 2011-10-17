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

        public PivotObject()
        { }

        public bool moved;
       
        public abstract void Move(Vector3 d);
        public abstract void SetGlobalPose(Matrix newPose);
        public abstract RenderObject HaveRenderAspect();
        public abstract Render.Materials.Material HaveMaterial();
        public abstract void Update();
        public abstract void BeginDoFrame();
        public abstract void EndDoFrame();
        public abstract void DoFrame(GameTime gt);

        /*from editor
         public abstract void SetActive(bool active);
         */

    }
}
