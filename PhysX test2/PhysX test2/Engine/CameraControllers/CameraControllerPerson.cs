using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhysX_test2.Engine.Logic;
using System;


namespace PhysX_test2.Engine.CameraControllers
{
    public class CameraControllerPerson : CameraControllerSuperClass
    {
        protected Vector3 _delta = Vector3.Zero;

        public float _yAngle;
        private float _zAngle;

        private Vector3 _offset;
        private LevelObject _character;
        public float _cameraDelta = 2.00f;

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
           // _delta = Vector3.Transform(Vector3.Forward, resMatr)*_cameraDelta;
            _offset = res;
            UpdateCamera();
        }

        public void UpDownCamera(float angle)
        {
            if (angle == 0)
                return;
            if (_zAngle + angle > 0.7f)     return;
            if (_zAngle + angle < -0.7f)    return;
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
                return;
            if (_cameraDelta * value < 0.46f)
                return;
            if (_cameraDelta * value > 3.8f)
                return;

          //  MyMath.perehod(ref _offset, _offset * value, 0.95f);
            _offset.X *= value;
            _offset.Y *= value;
            _offset.Z *= value;
            
            _cameraDelta *= value;

            UpdateCamera();
        }


        public Vector3 _lastTargetPoint;
        public void UpdateCamerafromUser(Vector3 _targetPoint)
        {

            Vector3 delta = _lastTargetPoint - _targetPoint;
            float maxl = _speedLimit * (float)(MyGame.UpdateTime.ElapsedGameTime.TotalMilliseconds / 1000.0);
            Vector3 newtarget = new Vector3();
            if (delta.LengthSquared() > maxl * maxl)
            {
                delta.Normalize();
                newtarget = _lastTargetPoint - delta * maxl;
                MyGame.Instance._engine.cameraneedsupdate = true;
            }
            else
            {
                newtarget = _targetPoint;
            }
            _lastTargetPoint = newtarget;

            float cursorPositionX = MouseManager.Manager.state.X;
            float deltaX = cursorPositionX - _lastMousePosX;
            float cursorPositionY = MouseManager.Manager.state.Y;
            float deltaY = cursorPositionY - _lastMousePosY;
            MouseState mouseState = MouseManager.Manager.state;
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

            delta = newtarget - _currentTarget;
            delta.Y = 0;
            delta.Normalize();
            delta *= _cameraDelta;
            MyMath.perehod(ref _delta, delta, 0.9f);
        }

        public override void UpdateCamera()
        {
            _currentTarget = _character.transform.Translation + _delta;
          //  MyMath.perehod(ref _currentTarget, _character.transform.Translation + _delta , 0.1f);
            MyMath.perehod(ref _currendPosition, _currentTarget + _offset + _delta, 0.9f);
           // _currendPosition = _currentTarget + _offset;
            base.UpdateCamera();
        }
    }
}
