using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhysX_test2.Engine.Logic;
using System;

namespace PhysX_test2.Engine.CameraControllers
{
    public class FreeCamera : CameraControllerSuperClass
    {
        private Vector2 lastmousepos;
        private float _cameraYaw, _cameraPitch;


        public FreeCamera(Camera cam) :
            base(cam)
        {
            _cameraYaw = cam.CameraYaw;
            _cameraPitch = cam.CameraPitch;
        }

        public override void UpdateCamera()
        {
            base.UpdateCamera();
        }

        public void Update(bool w, bool a, bool s, bool d, bool shift)
        {
            const float speed = 40.0f;
            float distance = speed * (float)MyGame.UpdateTime.ElapsedGameTime.TotalSeconds;


            var cursorPosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Vector2 delta = cursorPosition - lastmousepos;
            Vector3 forward = Matrix.Invert(_camera.View).Forward;
            Vector3 position = Matrix.Invert(_camera.View).Translation;

            Matrix cameraRotation = Matrix.CreateFromYawPitchRoll(_cameraYaw, _cameraPitch, 0.0f);

            Vector3 newForward = Vector3.TransformNormal(Vector3.Forward, cameraRotation);

            if (shift)
            {
                Vector2 deltaDampened = delta * 0.0015f;

                _cameraYaw -= deltaDampened.X;
                _cameraPitch -= deltaDampened.Y;

                cameraRotation = Matrix.CreateFromYawPitchRoll(_cameraYaw, _cameraPitch, 0.0f);
                newForward = Vector3.TransformNormal(Vector3.Forward, cameraRotation);
            }

            Vector3 translateDirection = Vector3.Zero;

            if (w) // Forwards
                translateDirection += Vector3.TransformNormal(Vector3.Forward, cameraRotation);

            if (s) // Backwards
                translateDirection += Vector3.TransformNormal(Vector3.Backward, cameraRotation);

            if (a) // Left
                translateDirection += Vector3.TransformNormal(Vector3.Left, cameraRotation);

            if (d) // Right
                translateDirection += Vector3.TransformNormal(Vector3.Right, cameraRotation);


            Vector3 newPosition = position;
            if (translateDirection.LengthSquared() > 0)
                newPosition += Vector3.Normalize(translateDirection) * distance;

            lastmousepos = cursorPosition;


            _currentTarget = newPosition + newForward;
            _currendPosition = newPosition;
        }

        public override void UpdateCamerafromUser(Vector3 _targetPoint)
        {
            
            Update( KeyboardManager.currentState.IsKeyDown(Keys.W),
                    KeyboardManager.currentState.IsKeyDown(Keys.A), 
                    KeyboardManager.currentState.IsKeyDown(Keys.S),
                    KeyboardManager.currentState.IsKeyDown(Keys.D),
                    KeyboardManager.Shift);

            base.UpdateCamerafromUser(_targetPoint);
        }

    }
}