using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Logic.BehaviourModel
{
    public class ObjectRelatedBehaviourModel : ObjectStaticBehaviourModel
    {

        public PivotObject _parentCharacter;
        public Matrix localMatrix = Matrix.Identity;
        public ObjectRelatedBehaviourModel(PivotObject __parentCharacter, int _parentBone)
        {
            CurrentPosition = Matrix.Identity;
            SetParentCharacter(__parentCharacter);
        }

        public void SetParentCharacter(PivotObject __parent)
        {
            _parentCharacter = __parent;
        }

        public override void SetGlobalPose(Matrix GlobalPoseMatrix, object Additionaldata)
        {
            localMatrix = GlobalPoseMatrix;
            Matrix resultMatrix = localMatrix *  _parentCharacter.transform;
            mov = CurrentPosition != resultMatrix;
            CurrentPosition = resultMatrix;
        }

        public override void DoFrame(GameTime gametime)
        {
            Matrix newpos = localMatrix * _parentCharacter.transform;
            moved = newpos!=CurrentPosition|| mov;
            CurrentPosition = newpos;
            mov = false;
        }

        public override void Move(Vector3 displacement)
        {
            localMatrix = localMatrix * Matrix.CreateTranslation(displacement);
            Matrix resultMatrix = localMatrix * _parentCharacter.transform;
            mov = CurrentPosition != resultMatrix;
            CurrentPosition = resultMatrix;
        }
    }
}
