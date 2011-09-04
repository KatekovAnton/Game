using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.CharacterControllers
{
    class CharacterControllerPerson:CharacterControllerSuperClass
    {
        private Logic.BehaviourModel.ObjectPhysicControllerBehaviourModel _charcterBehaviour;
        private float _oldRotation;

        public CharacterControllerPerson(Logic.LevelObject charecter)
        {
            _charcterBehaviour = charecter.behaviourmodel as Logic.BehaviourModel.ObjectPhysicControllerBehaviourModel;
        }

        private float CreateAngleForCharacter(Microsoft.Xna.Framework.Vector3 target)
        {
            double buffer = 0;
            double result = 0;

            float cursorX = target.X;
            float cursorZ = target.Z;

            Vector3 charPos = _charcterBehaviour.PreviousPosition.Translation;
            float charX = charPos.X;
            float charZ = charPos.Z;
            double difX = charX - cursorX;
            double difZ = charZ - cursorZ;
            if (Math.Abs(difX) > Math.Abs(difZ))
            {
                if (difX != 0) {
                    buffer = difZ / difX;
                    result = _oldRotation - buffer;
                }
                else {
                    result = -0.01f;
                }
            }
            else if ( Math.Abs(difX) < Math.Abs(difZ))
            {
                if (difZ != 0) {
                    buffer = difX / difZ;
                    result = buffer - _oldRotation;
                }
                else {
                    result = 0.01f;
                }
            }

            //Console.WriteLine("result = " + result);
            _oldRotation = (float) buffer;
            return (float) result*MathHelper.PiOver4;
        }
        public override void Update(Vector3 _target)
        {
            _charcterBehaviour.Rotate(CreateAngleForCharacter(_target));
        }
    }
}
