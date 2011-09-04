using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.Engine.CharacterControllers
{
    public class CharacterControllerSuperClass
    {
        public virtual void Update(Microsoft.Xna.Framework.Vector3 _target)
        { }
        /*
        //вообще като так в идеале но InputState-а нету покачто
        public virtual void Update(InputState inputState)
        { }
        
        abstract class InputState
        {}
        //управление игроком
        class PlayerInputState: InputState
        {}
        //управление по сети
        class NetworkInputState: InputState
        {}
        //неписи управляюстя иск интелектом
        class AIInputState: InputState
        {}
        */
    }
}
