using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using StillDesign.PhysX;

namespace PhysX_test2.Engine.Logic.BehaviourModel
{
    class PhysicData
    {
        public Vector3 LinearMomentum;
        public Vector3 LinearVelocity;
        public Vector3 AngularMomentum;
        public Vector3 AngularVelocity;
        public PhysicData() { }
        public static readonly PhysicData ZeroParameters = new PhysicData();
 

    }
    //Всякие предметы окружения
    class ObjectPhysicBehaviourModel:ObjectBehaviourModel
    {
       
        public Actor actor;
        public ObjectPhysicBehaviourModel(Actor _actor)
        {
            actor = _actor;
            actor.LinearVelocity = Vector3.Zero.toPhysicV3();
            actor.LinearMomentum = Vector3.Zero.toPhysicV3();
            actor.AngularVelocity = Vector3.Zero.toPhysicV3();
            actor.AngularMomentum = Vector3.Zero.toPhysicV3();
        }
        public override void Move(Vector3 displacement)
        {
            SetGlobalPose(Matrix.Multiply(actor.GlobalPose.toXNAM(), Matrix.CreateTranslation(displacement)), null);
        }
        public override void SetGlobalPose(Matrix GlobalPoseMatrix, object Additionaldata)
        {
            this.globalpose = GlobalPoseMatrix;
            actor.GlobalPose = GlobalPoseMatrix.toPhysicM();
            if (Additionaldata != null)
            {
                PhysicData pd = Additionaldata as PhysicData;

                actor.LinearVelocity = pd.LinearVelocity.toPhysicV3();
                actor.LinearMomentum = pd.LinearMomentum.toPhysicV3();
                actor.AngularVelocity = pd.AngularVelocity.toPhysicV3();
                actor.AngularMomentum = pd.AngularMomentum.toPhysicV3();
            }
        }

        public override void Disable()
        {
            actor.ActorFlags.DisableCollision = true;
            actor.Sleep();
        }

        public override void MakeJolt(Vector3 __point, Vector3 __direction, float __mass)
        {
            if (actor.Mass == 0)
                return;
            __direction.Normalize();
            __direction *= __mass * 10;
            actor.AddForceAtPosition(__direction.toPhysicV3(), __point.toPhysicV3(), ForceMode.Impulse);
        }

        public override void Enable()
        {
            actor.ActorFlags.DisableCollision = false;
            actor.WakeUp();
        }

        public override void DoFrame(GameTime gametime)
        {
            moved = CurrentPosition.Translation != new Vector3( actor.GlobalPose.M41,actor.GlobalPose.M42,actor.GlobalPose.M43);
            CurrentPosition = actor.GlobalPose.toXNAM();
        }
    }
}
