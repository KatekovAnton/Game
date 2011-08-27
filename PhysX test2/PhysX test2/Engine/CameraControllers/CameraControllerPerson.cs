using Microsoft.Xna.Framework;
using PhysX_test2.Engine.Logic;
using System;


namespace PhysX_test2.Engine.CameraControllers {
    public class CameraControllerPerson: CameraControllerSuperClass {
        public LevelObject _character;
        public Vector3 _offset;
        public float _yAngle;
        public float _zAngle;


        public CameraControllerPerson(Camera cam, LevelObject character, Vector3 offset): 
            base(cam, character.transform.Translation + offset, character.transform.Translation) {
            _offset = offset;
            _character = character;
        }


        public void RotateCameraAroundChar(float angle) {
            if(angle == 0) {
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



        public void UpDownCamera(float angle) {
            if (angle == 0) {
                return;
            }

            _zAngle += angle;

            Matrix resMatr;
            Vector3 vectX =  Extensions.VectorForCharacterMoving(Extensions.Route.Left, _yAngle);
            vectX.Normalize();
            Matrix.CreateFromAxisAngle(ref vectX, angle, out resMatr);

            Vector3 res = Vector3.Transform(_offset, resMatr);
            _offset = res;

            UpdateCamera();
        }



        public void ZoomCameraFromCha (float value) {
            if(value == 1.0) {
                return;
            }
            _offset.X *= value;
            _offset.Y *= value;
            _offset.Z *= value;
            UpdateCamera();
        }


        public override void UpdateCamera() {
            _currentTarget = _character.transform.Translation;
            _currendPosition = _currentTarget + _offset;
            base.UpdateCamera();
        } 
    }


 
}
