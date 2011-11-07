using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.TheGame.LogicControllers
{
    public class BulletLogicController : BaseLogicController
    {
        public Parameters.BulletParameters _baseParameters;
        public Parameters.BulletParameters _instanceParameters;
        public Vector3 _startPosition;
        public Vector3 _moveVector;
        public float _moveSpeed;

        public override void Update(GameTime __gameTime)
        {
            //throw new NotImplementedException();
        }

        public override Parameters.InteractiveObjectParameters GetParameters()
        {
            return _instanceParameters;
        }

        public override void TakeHit(Parameters.BulletParameters __bulletParameters)
        {
            return;
        }
    }
}
