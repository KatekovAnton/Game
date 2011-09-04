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

        private float CreateAngleForCharacter(Microsoft.Xna.Framework.Vector3 _target)
        {
            float buffer = 0;
            float result = 0;

            float cursorX = _target.X;
            float cursorZ = _target.Z;

            Vector3 charPos = _charcterBehaviour.globalpose.Translation;
            float charX = charPos.X;
            float charZ = charPos.Z;
            float difX = charX - cursorX;
            float difY = charZ - cursorZ;
            if (difX != 0 && Math.Abs(difX) > Math.Abs(difY))
            {
                buffer = difY / difX;
                result = _oldRotation - buffer;
            }
            else if (difY != 0 && Math.Abs(difX) < Math.Abs(difY))
            {
                buffer = difX / difY;
                result = buffer - _oldRotation;
            }

            //Console.WriteLine("result = " + result);
            _oldRotation = buffer;
            return result;
        }
        public override void Update(Vector3 _target)
        {
            _charcterBehaviour.Rotate(CreateAngleForCharacter(_target));
        }
    }
}
