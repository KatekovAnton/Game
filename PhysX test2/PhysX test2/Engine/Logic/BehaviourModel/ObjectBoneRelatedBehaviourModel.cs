using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Logic.BehaviourModel
{
    public class ObjectBoneRelatedBehaviourModel: ObjectStaticBehaviourModel
    {
        public int parentBone;
        public Engine.Animation.Character parentCharacter;
        public Matrix localMatrix = Matrix.Identity;
        public ObjectBoneRelatedBehaviourModel(Engine.Animation.Character _parentCharacter, int _parentBone)
        {
            parentCharacter = _parentCharacter;
            parentBone = _parentBone;
            CurrentPosition = Matrix.Identity;
        }

        public override void SetGlobalPose(Matrix GlobalPoseMatrix, object Additionaldata)
        {
            localMatrix = GlobalPoseMatrix;
            Matrix resultMatrix = localMatrix * parentCharacter._currentFames[parentBone] * parentCharacter.Position; ;
            mov = CurrentPosition != resultMatrix;
            CurrentPosition = resultMatrix;
        }

        public override void DoFrame(GameTime gametime)
        {
            Matrix newpos = localMatrix * parentCharacter._currentFames[parentBone] * parentCharacter.Position; ;
            moved = newpos!=CurrentPosition|| mov;
            CurrentPosition = newpos;
            mov = false;
        }

        public override void Move(Vector3 displacement)
        {
            localMatrix = localMatrix * Matrix.CreateTranslation(displacement);
            Matrix resultMatrix = localMatrix * parentCharacter._currentFames[parentBone] * parentCharacter.Position;
            mov = CurrentPosition != resultMatrix;
            CurrentPosition = resultMatrix;
        }
    }
}
