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
        public PivotObject parentCharacter;
        public Matrix localMatrix = Matrix.Identity;

        private Engine.Animation.CharacterController _parentCharacterController;

        public ObjectBoneRelatedBehaviourModel(PivotObject __parentCharacter, int _parentBone)
        {
            parentBone = _parentBone;
            CurrentPosition = Matrix.Identity;
            if (__parentCharacter != null)
                SetParentObject(__parentCharacter);
        }

        public override void SetParentObject(PivotObject __object)
        {
            parentCharacter = __object;
            LevelObject lo = __object as LevelObject;
            Engine.Render.AnimRenderObject ro = lo.renderaspect as Engine.Render.AnimRenderObject;
            _parentCharacterController = ro.character;
        }

        public override void SetGlobalPose(Matrix GlobalPoseMatrix, object Additionaldata)
        {
            localMatrix = GlobalPoseMatrix;
            Matrix resultMatrix = localMatrix * _parentCharacterController._currentFrames[parentBone] * _parentCharacterController.Position;
            mov = CurrentPosition != resultMatrix;
            CurrentPosition = resultMatrix;
        }

        public override void DoFrame(GameTime gametime)
        {
            Matrix newpos = localMatrix * _parentCharacterController._currentFrames[parentBone] * _parentCharacterController.Position;
            moved = newpos!=CurrentPosition|| mov;
            CurrentPosition = newpos;
            mov = false;
        }

        public override void Move(Vector3 displacement)
        {
            localMatrix = localMatrix * Matrix.CreateTranslation(displacement);
            Matrix resultMatrix = localMatrix * _parentCharacterController._currentFrames[parentBone] * _parentCharacterController.Position;
            mov = CurrentPosition != resultMatrix;
            CurrentPosition = resultMatrix;
        }
    }
}
