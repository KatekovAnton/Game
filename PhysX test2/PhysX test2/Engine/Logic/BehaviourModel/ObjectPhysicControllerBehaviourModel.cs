using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using StillDesign.PhysX;


namespace PhysX_test2.Engine.Logic.BehaviourModel
{
    /// <summary>
    /// Челобарики
    /// </summary>
    class ObjectPhysicControllerBehaviourModel:ObjectBehaviourModel
    {
        private Actor actor;
        private Vector3 lastposition;
        private Vector3 move;

        public ObjectPhysicControllerBehaviourModel(Actor _actor)
        {
            actor = _actor;
        }

        public override void SetGlobalPose(Matrix GlobalPoseMatrix, object Additionaldata)
        {
            this.globalpose = GlobalPoseMatrix;
            actor.GlobalPose = GlobalPoseMatrix;
        }

        public override void Move(Vector3 displacement)
        {
            move.X += displacement.X;
            move.Z += displacement.Z;
        }

        public override void DoFrame(GameTime gametime)
        {
            if (move.LengthSquared() != 0)
            {
                move.Y = 0;
                move.Normalize();
                move.X *= 8;
                move.Z *= 8;

                actor.LinearVelocity = new Vector3(move.X, move.Y + actor.LinearVelocity.Y, move.Z);
            }
            else
                actor.LinearVelocity = new Vector3(0, actor.LinearVelocity.Y, 0);

            lastposition = CurrentPosition.Translation;
            CurrentPosition = actor.GlobalPose;
            move.X = move.Y = move.Z =0;
        }
    }
}
