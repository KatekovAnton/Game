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
        public override void DoFrame(GameTime gametime)
        {
            moved = CurrentPosition.Translation != new Vector3( actor.GlobalPose.M41,actor.GlobalPose.M42,actor.GlobalPose.M43);
            CurrentPosition = actor.GlobalPose.toXNAM();
        }
    }
}
