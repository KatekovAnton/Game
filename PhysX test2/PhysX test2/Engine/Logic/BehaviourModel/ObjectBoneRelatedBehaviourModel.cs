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
        public PivotObjectDependType _dependType;
        private Engine.Animation.CharacterController _parentCharacterController;

        public ObjectBoneRelatedBehaviourModel(PivotObject __parentCharacter, int _parentBone)
        {
            parentBone = _parentBone;
            CurrentPosition = Matrix.Identity;
            if (__parentCharacter != null)
                SetParentObject(__parentCharacter);
        }

        public ObjectBoneRelatedBehaviourModel()
        {
            
        }

        public override void SetParentObject(PivotObject __object)
        {
            parentCharacter = __object;
            LevelObject lo = __object as LevelObject;
            Engine.Render.AnimRenderObject ro = lo.renderaspect as Engine.Render.AnimRenderObject;
            _parentCharacterController = ro.character;
        }

        public override void SetGlobalPose(Matrix GlobalPoseMatrix, object Additionaldata, PivotObject __parent)
        {
            Matrix position = Matrix.Identity;
            if (__parent != null)
            {
                LevelObject lo = __parent as LevelObject;
                if (lo == null)
                    throw new Exception();
                Render.AnimRenderObject ro = lo.renderaspect as Render.AnimRenderObject;
                if (ro == null)
                    throw new Exception();

                if (_parentCharacterController == null)
                    _parentCharacterController = ro.character;
                
                switch (_dependType)
                {
                    case PivotObjectDependType.Head:
                        {
                            position = ro.character._baseCharacter.skeleton.HeadMatrix;
                            parentBone = ro.character._baseCharacter.skeleton.HeadIndex;
                        } break;
                    case PivotObjectDependType.Weapon:
                        {
                            position = ro.character._baseCharacter.skeleton.WeaponMatrix;
                            parentBone = ro.character._baseCharacter.skeleton.WeaponIndex;
                        } break;
                    default: break;
                }
            }

            localMatrix =  position *GlobalPoseMatrix;
            Matrix resultMatrix = localMatrix * _parentCharacterController._currentFrames[parentBone] * _parentCharacterController.Position;
   
            CurrentPosition = resultMatrix;
        }

        public override void DoFrame(GameTime gametime)
        {
            Matrix newpos = localMatrix * _parentCharacterController._currentFrames[parentBone] * _parentCharacterController.Position;
            CurrentPosition = newpos;
        }

        public override void Move(Vector3 displacement)
        {
            localMatrix = localMatrix * Matrix.CreateTranslation(displacement);
            Matrix resultMatrix = localMatrix * _parentCharacterController._currentFrames[parentBone] * _parentCharacterController.Position;
            CurrentPosition = resultMatrix;
        }
    }
}
