using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.AnimationManager
{
    public interface IAnimationUser
    {
        void Update(float animTime);//animTime between 0.0 ....... animLength
        void onAnimEnded();
        float animLength
        {
            get;
        }
    }
}
