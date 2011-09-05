using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhysX_test2.Engine.Logic;
using System;


namespace PhysX_test2.Engine.CameraControllers
{
    public class CameraControllerPerson : CameraControllerSuperClass
    {
        
        public float _yAngle;
        private float _zAngle;

        private Vector3 _offset;
        private LevelObject _character;


        private int _mouseWheelOld;
        private float _lastMousePosX;
        private float _lastMousePosY;
        

        public CameraControllerPerson(Camera cam, LevelObject character, Vector3 offset) :
            base(cam, character.transform.Translation + offset, character.transform.Translation)
        {
            _offset = offset;
            _character = character;
        }


        public void RotateCameraAroundChar(float angle)
        {
            if (angle == 0)
            {
                return;
            }
            _yAngle += angle;

            Matrix resMatr;
            Vector3 vect = new Vector3(0, 1, 0);
            Matrix.CreateFromAxisAngle(ref vect, angle, out resMatr);
            Vector3 res = Vector3.Transform(_offset, resMatr);
            _offset = res;
            UpdateCamera();
        }



        public void UpDownCamera(float angle)
        {
            if (angle == 0)
            {
                return;
            }

            _zAngle += angle;

            Matrix resMatr;
            Vector3 vectX = Extensions.VectorForCharacterMoving(Extensions.Route.Left, _yAngle);
            vectX.Normalize();
            Matrix.CreateFromAxisAngle(ref vectX, angle, out resMatr);

            Vector3 res = Vector3.Transform(_offset, resMatr);
            _offset = res;

            UpdateCamera();
        }



        public void ZoomCameraFromCha(float value)
        {
            if (value == 1.0)
            {
                return;
            }
            _offset.X *= value;
            _offset.Y *= value;
            _offset.Z *= value;
            UpdateCamera();
        }


        public void UpdateCamerafromUser()
        {
            float cursorPositionX = Mouse.GetState().X;
            float deltaX = cursorPositionX - _lastMousePosX;
            float cursorPositionY = Mouse.GetState().Y;
            float deltaY = cursorPositionY - _lastMousePosY;
            MouseState mouseState = Mouse.GetState();
            if (mouseState.RightButton == ButtonState.Pressed)
            {
                RotateCameraAroundChar(-deltaX * Settings.rotateSpeed);
                UpDownCamera(deltaY * Settings.rotateSpeed);
            }
            _lastMousePosX = cursorPositionX;
            _lastMousePosY = cursorPositionY;


            // обработка зума
            if (mouseState.ScrollWheelValue > _mouseWheelOld)
                ZoomCameraFromCha(1 / Settings.zoomSpeed);

            else if (mouseState.ScrollWheelValue < _mouseWheelOld)
                ZoomCameraFromCha(Settings.zoomSpeed);

            _mouseWheelOld = mouseState.ScrollWheelValue;
        }

        public override void UpdateCamera()
        {
            _currentTarget = _character.transform.Translation;
            _currendPosition = _currentTarget + _offset;
            base.UpdateCamera();
        }
    }
}
