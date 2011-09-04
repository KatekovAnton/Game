using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Logic.BehaviourModel
{
    class ObjectStaticBehaviourModel:ObjectBehaviourModel
    {
        public ObjectStaticBehaviourModel()
        {
            CurrentPosition = globalpose =  Matrix.Identity;
        }
        bool mov;
        
        public override void SetGlobalPose(Matrix GlobalPoseMatrix, object Additionaldata)
        {
            mov = CurrentPosition != GlobalPoseMatrix;
            CurrentPosition = GlobalPoseMatrix;
        }
       
        public override void DoFrame(GameTime gametime)
        {
            moved = mov;
            mov = false;
        }
        public override void Move(Vector3 displacement)
        {
            Matrix newposition = Matrix.Multiply(globalpose, Matrix.CreateTranslation(displacement));
            mov = CurrentPosition != newposition;
            CurrentPosition = newposition;
        }
    }
}
