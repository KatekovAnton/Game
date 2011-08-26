using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.CameraControllers {
   abstract public class CameraControllerSuperClass {
        public Camera _camera;
        public Vector3 _currendPosition;
        public Vector3 _currentTarget;


       protected CameraControllerSuperClass(Camera cam, Vector3 currentPos, Vector3 targetPosition) {
            _camera = cam;
            _currendPosition = currentPos;
            _currentTarget = targetPosition;
            _camera.Update(_currendPosition, _currentTarget);
        }


       public Vector3 CurrentPosition() {
            return _currendPosition;
        }


        public void SetCurrentPosition(Vector3 vect) {
            _currendPosition = vect;
            _camera.Update(_currendPosition, _currentTarget);
        }


        public Vector3 CurrentTarget() {
            return _currentTarget;
        }


        public void SetCurrentTarget(Vector3 vect) {
            _currentTarget = vect;
            _camera.Update(_currendPosition, _currentTarget);
        }

        
       public virtual void UpdateCamera() {
           _camera.Update(_currendPosition, _currentTarget);
       }

   }
}
