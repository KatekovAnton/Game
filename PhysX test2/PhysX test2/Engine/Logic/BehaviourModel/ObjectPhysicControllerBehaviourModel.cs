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
        private readonly Actor _actor;
        private Vector3 _lastposition;
        private Vector3 _move;
        private float _angle;

        public ObjectPhysicControllerBehaviourModel(Actor _actor)
        {
            this._actor = _actor;
            _angle = 0;
        }

        public override void SetGlobalPose(Matrix GlobalPoseMatrix, object Additionaldata)
        {
            this.globalpose = GlobalPoseMatrix;
            _actor.GlobalPose = GlobalPoseMatrix;
        }

        public override void Move(Vector3 displacement)
        {
            _move.X += displacement.X;
            _move.Z += displacement.Z;
        }


        public void Rotate( float angle) {
            _angle += angle;
            _actor.GlobalOrientation *= Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), angle);
        }



        public override void DoFrame(GameTime gametime)
        {
            //float elapsed = (float)(gametime.ElapsedGameTime.TotalMilliseconds / 1000.0);
            if (_move.LengthSquared() != 0)
            {
                _move.Y = 0;
                _move.Normalize();
                _move.X *= 8;
                _move.Z *= 8;

                _actor.LinearVelocity = new Vector3(_move.X, _move.Y + _actor.LinearVelocity.Y, _move.Z);
            }
            else
                _actor.LinearVelocity = new Vector3(0, _actor.LinearVelocity.Y, 0);

            _lastposition = CurrentPosition.Translation;
            CurrentPosition = _actor.GlobalPose;
            _move.X = _move.Y = _move.Z =0;
        }
    }
}
