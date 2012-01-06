using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.CameraControllers
{
    abstract public class CameraControllerSuperClass
    {
        private Camera _camera;
        public Vector3 _currendPosition;
        public Vector3 _currentTarget;

        public float _speedLimit = 0.0f;
        protected Vector3 _delta = Vector3.Zero;

        public bool _justcreated = true;

        protected CameraControllerSuperClass(Camera cam, Vector3 currentPos, Vector3 targetPosition)
        {
            _speedLimit = 50.0f;
            _camera = cam;
            _currendPosition = currentPos;
            _currentTarget = targetPosition;
            _camera.Update(_currendPosition + _delta, _currentTarget + _delta);
        }


        public Vector3 CurrentPosition()
        {
            return _currendPosition;
        }


        public void SetCurrentPosition(Vector3 vect)
        {
            _currendPosition = vect;
            _camera.Update(_currendPosition + _delta, _currentTarget + _delta);
            _justcreated = true;
        }


        public Vector3 CurrentTarget()
        {
            return _currentTarget;
        }


        public void SetCurrentTarget(Vector3 vect)
        {
            _currentTarget = vect;
            _camera.Update(_currendPosition + _delta, _currentTarget + _delta);
            _justcreated = true;
        }


        public virtual void UpdateCamera()
        {
            _camera.Update(_currendPosition + _delta, _currentTarget + _delta);
            _justcreated = false;
        }

    }
}
