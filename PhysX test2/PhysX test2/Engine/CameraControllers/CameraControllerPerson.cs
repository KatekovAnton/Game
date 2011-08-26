using Microsoft.Xna.Framework;
using PhysX_test2.Engine.Logic;
using System;


namespace PhysX_test2.Engine.CameraControllers {
    public class CameraControllerPerson: CameraControllerSuperClass {
        public LevelObject _character;
        public Vector3 _offset;
        public float _xAngle;


        public CameraControllerPerson(Camera cam, LevelObject character, Vector3 offset): 
            base(cam, character.transform.Translation + offset, character.transform.Translation) {
            _offset = offset;
            _character = character;
        }


        public void RotateCameraAroundChar(float angle) {
            if(angle == 0) {
                return;
            }
            _xAngle -= angle;

            _currentTarget = _character.transform.Translation;
            Matrix matrMove = Matrix.CreateTranslation(_currentTarget.X, 0, 0);
            Matrix rotateX = Matrix.CreateRotationY(angle);
            Matrix matrBack = Matrix.CreateTranslation(-_currentTarget.X, 0, 0);
            Matrix result = matrMove * rotateX * matrBack;
            Vector3 res = Vector3.Transform(_offset, result);
            _offset = res;
            _currendPosition = _currentTarget + _offset;
            base.UpdateCamera();
        }


        public void ZoomCameraFromCha (float value) {
            if(value == 1.0) {
                return;
            }
            _offset.X *= value;
            _offset.Y *= value;
            _offset.Z *= value;
            _currendPosition = _currentTarget + _offset;
            base.UpdateCamera();
        }


        public override void UpdateCamera() {
            _currentTarget = _character.transform.Translation;
            _currendPosition = _currentTarget + _offset;
            base.UpdateCamera();
        } 
    }


 
}
