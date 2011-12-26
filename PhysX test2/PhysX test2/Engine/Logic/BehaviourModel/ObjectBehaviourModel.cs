using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace PhysX_test2.Engine.Logic.BehaviourModel
{
    public class BehaviourModelDescription
    {
        public int BehaviourType;
        public bool IsStatic;
        public Vector3 CenterOfMass;
        public Vector3 ShapeSize;
        public Vector3 ShapeRotationAxis;
        public int ShapeType;
        public int PhysXShapeType;
        public string RCCMName;
        public float Mass;
        public float ShapeRotationAngle;
    }

    public abstract class ObjectBehaviourModel
    {
        public Matrix PreviousPosition;
        public Matrix CurrentPosition;
        public Matrix globalpose;
        public virtual Matrix GetGlobalPose()
        {
            return globalpose;
        }
        public abstract void SetGlobalPose(Matrix GlobalPoseMatrix, object Additionaldata, PivotObject __parent = null);
        /// <summary>
        /// запоминаем что надо
        /// </summary>
        public virtual void BeginDoFrame()
        {
            PreviousPosition = globalpose;
            moved = false;
        }

        public virtual void SetParentObject(PivotObject __object)
        { }

        public virtual void Disable()
        { }

        public virtual void Enable()
        { }

        public virtual void Rotate(float __angle)
        { }

        public virtual void MakeJolt(Vector3 __point, Vector3 __direction, float __mass)
        { }

        public virtual void SetPosition(Vector3 newPosition)
        {
            SetGlobalPose(Matrix.CreateTranslation(newPosition), null);
        }

        public bool moved
        {
            get;
            protected set;
        }
        public abstract void Move(Vector3 displacement);
        /// <summary>
        /// симулярует
        /// </summary>
        /// <param name="gametime"></param>
        public abstract void DoFrame(GameTime gametime);
        /// <summary>
        /// завершаем всё и копируем параматеры в выходной интерфейс
        /// </summary>
        public void EndDoFrame()
        {
            moved = (globalpose.Translation != CurrentPosition.Translation);
            globalpose = CurrentPosition;
            
        }
    }
}
