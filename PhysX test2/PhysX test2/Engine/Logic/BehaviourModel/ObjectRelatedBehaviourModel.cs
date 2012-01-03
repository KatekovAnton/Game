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
        public ObjectRelatedBehaviourModel(PivotObject __parentCharacter)
        {
            CurrentPosition = Matrix.Identity;
            SetParentObject(__parentCharacter);
        }

        public ObjectRelatedBehaviourModel()
        {
            CurrentPosition = Matrix.Identity;
        }

        public override void SetParentObject(PivotObject __object)
        {
            _parentCharacter = __object;
        }

        public override void SetGlobalPose(Matrix GlobalPoseMatrix, object Additionaldata, PivotObject __parent)
        {
            _parentCharacter = __parent;

            localMatrix = GlobalPoseMatrix;
            Matrix resultMatrix = localMatrix * __parent.transform;
            CurrentPosition = resultMatrix;
        }

        public override void DoFrame(GameTime gametime)
        {
            Matrix newpos = localMatrix * _parentCharacter.behaviourmodel.CurrentPosition;
        
            CurrentPosition = newpos;
            
        }

        public override void Move(Vector3 displacement)
        {
            localMatrix = localMatrix * Matrix.CreateTranslation(displacement);
            Matrix resultMatrix = localMatrix * _parentCharacter.transform;
            CurrentPosition = resultMatrix;
        }
    }
}
