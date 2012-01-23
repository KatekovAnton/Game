using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhysX_test2.Engine.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhysX_test2;
namespace PhysX_test2.Engine.CameraControllers
{
    public enum CameraMode { Character, Free }
    public static class CameraManager
    {
        
        public static Camera Camera;
        //controllers for camera and character (objects between user and game)
        public static  CameraControllers.CameraControllerPerson _cameraController;
        public static  CameraControllers.FreeCamera _cameraControllerFree;


        static public void Update()
        {
            switch (cam_mode)
            {
                case CameraControllers.CameraMode.Character:
                    _cameraController.UpdateCamerafromUser(MyGame.Instance._mousepoint);
                    _cameraController.UpdateCamera();
                    break;
                case CameraControllers.CameraMode.Free: 
                    _cameraControllerFree.UpdateCamerafromUser(MyGame.Instance._mousepoint);
                    _cameraControllerFree.UpdateCamera();
                    break;
                default: break;
            }
            
            
        }

        public static void Init()
        {
            Camera = new Camera(MyGame.Instance._engine);
        }

        public static void CreateCharCameraController(LevelObject __targetCharacter)
        {
            _cameraController = new CameraControllerPerson(Camera, __targetCharacter, new Vector3(-10, 10, 0));
            CameraMode = CameraControllers.CameraMode.Character;
        }

        static private CameraMode cam_mode = CameraControllers.CameraMode.Character;
        static public CameraMode CameraMode 
        {
            set
            {
                switch (value)
                {
                    case CameraControllers.CameraMode.Character: break;
                    case CameraControllers.CameraMode.Free:
                        if (_cameraControllerFree == null) _cameraControllerFree = new FreeCamera(Camera, _cameraController._currendPosition, Camera._direction);
                        break;
                    default: break;
                }
                cam_mode = value; 
            }

            get { return cam_mode; }
        }

    }
}
