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
        private float _oldRotation = -MathHelper.PiOver2;
        

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
            if(difX>0)
            {
                if (difX == 0)
                    difX = 0.01;
                buffer = Math.Atan(difZ / difX);
            }
            else
            {
                if (difX == 0)
                    difX = 0.01;
                buffer = Math.Atan(difZ / difX)-Math.PI;
            }
            result = _oldRotation - buffer;

            _oldRotation = (float) buffer;
            return (float) result;
        }
        public override void Update(Vector3 _target)
        {
            _charcterBehaviour.Rotate(CreateAngleForCharacter(_target) );
        }
    }
}
