using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.TheGame.InputManagers
{
    public class InputProviderAI : InputProviderSuperClass
    {
        float angle = 0.9f;

        public InputProviderAI()
        {
            
        }

        public override void Update(Vector3 __position)
        {
            _angle = angle;
            angle = 0;
            _newInputState = CharacterMoveState.Stay;
        }
    }
}
