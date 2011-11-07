using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.TheGame.LogicControllers
{
    public class ImortalLevelGeometryLogicController:BaseLogicController
    {
        public Parameters.InteractiveObjectParameters _baseParameters;

        public override Parameters.InteractiveObjectParameters GetParameters()
        {
            return _baseParameters;
        }

        public override void TakeHit(Parameters.BulletParameters __bulletParameters)
        {
            
        }

        public override void Update(GameTime __gameTime)
        {
            
        }
    }
}
