using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhysX_test2.Engine.Logic;
using System;

namespace PhysX_test2.Engine.CameraControllers
{
    public class FreeCamera : CameraControllerSuperClass
    {
        protected Vector3 _delta = Vector3.Zero;

        public FreeCamera(Vector3 pos, Vector3 dir) : base(Program.game._engine.Camera, pos, Vector3.Zero)
        {
            
        }
        
    }
}