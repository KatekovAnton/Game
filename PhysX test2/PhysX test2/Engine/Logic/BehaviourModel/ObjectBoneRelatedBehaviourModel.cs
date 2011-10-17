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
        Matrix localMatrix;
        public ObjectBoneRelatedBehaviourModel(Engine.Animation.Character _parentCharacter, int _parentBone)
        {
            parentCharacter = _parentCharacter;
            parentBone = _parentBone;
        }

        public override void SetGlobalPose(Matrix GlobalPoseMatrix, object Additionaldata)
        {
            localMatrix = GlobalPoseMatrix;
            Matrix resultMatrix = parentCharacter._currentFames[parentBone] * localMatrix;
            mov = CurrentPosition != resultMatrix;
            CurrentPosition = resultMatrix;
        }

        public override void DoFrame(GameTime gametime)
        {
            Matrix newpos = parentCharacter._currentFames[parentBone] * localMatrix;
            moved = newpos!=CurrentPosition|| mov;
            CurrentPosition = newpos;
            mov = false;
        }

        public override void Move(Vector3 displacement)
        {
            localMatrix = localMatrix * Matrix.CreateTranslation(displacement);
            Matrix resultMatrix = parentCharacter._currentFames[parentBone] * localMatrix;
            mov = CurrentPosition != resultMatrix;
            CurrentPosition = resultMatrix;
        }
    }
}
