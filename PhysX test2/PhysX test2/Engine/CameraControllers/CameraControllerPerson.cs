﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhysX_test2.Engine.Logic;
using System;


namespace PhysX_test2.Engine.CameraControllers
{
    public class CameraControllerPerson : CameraControllerSuperClass
    {
        protected Vector3 _delta = Vector3.Zero;

        public float _camera_sence;
        private float _zAngle;

        private Vector3 _offset;
        private LevelObject _character;
        public float _cameraDelta = 1.50f;

        private int _mouseWheelOld;
        private float _lastMousePosX;
        private float _lastMousePosY;

        public CameraControllerPerson(Camera cam, LevelObject character, Vector3 offset) :
            base(cam, character.transform.Translation + offset, character.transform.Translation)
        {
            _offset = offset;
            _character = character;
            _camera_sence = (float)Config.Instance["_player_camera_sence"];
        }

        public void RotateCameraAroundChar(float angle)
        {
            if (angle == 0)
                return;

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

            _offset.X *= value;
            _offset.Y *= value;
            _offset.Z *= value;
            
            _cameraDelta *= value;

            UpdateCamera();
        }

        public override void UpdateCamerafromUser(Vector3 _targetPoint)
        {
            float cursorPositionX = MouseManager.Manager.state.X;
            float deltaX = cursorPositionX - _lastMousePosX;
            float cursorPositionY = MouseManager.Manager.state.Y;
            float deltaY = cursorPositionY - _lastMousePosY;
            MouseState mouseState = MouseManager.Manager.state;

            if (MouseManager.Manager.rmbState == ButtonState.Pressed)
            {
                RotateCameraAroundChar(-deltaX * Settings.rotateSpeed);
              //  UpDownCamera(deltaY * Settings.rotateSpeed);
            }

            _lastMousePosX = cursorPositionX;
            _lastMousePosY = cursorPositionY;


            // обработка зума
            /*
            if (mouseState.ScrollWheelValue > _mouseWheelOld)
                ZoomCameraFromCha(1 / Settings.zoomSpeed);
            else if (mouseState.ScrollWheelValue < _mouseWheelOld)
                ZoomCameraFromCha(Settings.zoomSpeed);
            */
            _mouseWheelOld = mouseState.ScrollWheelValue;

            _delta = _targetPoint - _currentTarget;
            _delta.Y = 0;
            if(_delta.LengthSquared()!=0)
                _delta.Normalize();
            _delta *= _cameraDelta;
        }

        public override void UpdateCamera()
        {
            _currentTarget = MyMath.perehod_fps(_currentTarget, _character.transform.Translation + _delta, _camera_sence);
            _currendPosition = MyMath.perehod_fps(_currendPosition, _currentTarget + _offset, _camera_sence);


            base.UpdateCamera();
        }
    }
}
