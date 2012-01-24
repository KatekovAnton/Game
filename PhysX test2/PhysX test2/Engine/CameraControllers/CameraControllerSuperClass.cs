using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.CameraControllers
{
    abstract public class CameraControllerSuperClass
    {
        protected Camera _camera;
        public Vector3 _currendPosition;
        public Vector3 _currentTarget;
        public float _yAngle;

        public float _speedLimit = 0.0f;

        protected CameraControllerSuperClass(Camera cam, Vector3 currentPos, Vector3 targetPosition)
        {
            _speedLimit = 50.0f;
            _camera = cam;
            _currendPosition = currentPos;
            _currentTarget = targetPosition;
            _camera.Update(_currendPosition , _currentTarget );
        }

        protected CameraControllerSuperClass(Camera cam)
        {
            _speedLimit = 50.0f;
            _camera = cam;
            _currendPosition = _camera._position;
            _currentTarget = _camera._position + _camera._direction;
        }

        public virtual void UpdateCamera()
        {
            _camera.Update(_currendPosition , _currentTarget );
        }

        public virtual void UpdateCamerafromUser(Vector3 _targetPoint) { }

    }
}
