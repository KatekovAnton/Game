using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhysX_test2.Engine.Logic;
using System;

namespace PhysX_test2.Engine.CameraControllers
{
    public class FreeCamera : CameraControllerSuperClass
    {
        protected Vector3 _delta = Vector3.Zero;

        public FreeCamera(Camera cam, Vector3 pos,  Vector3 dir) :
            base(cam, pos, dir)
        {
            
            
        }

        public override void UpdateCamera()
        {
            base.UpdateCamera();
        }

        public override void UpdateCamerafromUser(Vector3 _targetPoint)
        {
            base.UpdateCamerafromUser(_targetPoint);
        }

    }
}