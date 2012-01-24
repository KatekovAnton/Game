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
        public static  CameraControllers.CameraControllerSuperClass _cameraController;
        //public static  CameraControllers.FreeCamera _cameraControllerFree;


        static public void Update()
        {
            _cameraController.UpdateCamerafromUser(MyGame.Instance._mousepoint);
            _cameraController.UpdateCamera();
        }

        public static void Init()
        {
            Camera = new Camera(MyGame.Instance._engine);
        }

   
        private static LevelObject _targetCharacter;
        public static  LevelObject __targetCharacter
        {
            set
            {
                _targetCharacter = value;
                if (value == null)
                    CameraMode = CameraControllers.CameraMode.Free;
                else 
                CameraMode = CameraControllers.CameraMode.Character;
            }

            get { return _targetCharacter; }
        }

        static private CameraMode cam_mode = CameraControllers.CameraMode.Character;
        static public CameraMode CameraMode 
        {
            set
            {
                switch (value)
                {
                    case CameraControllers.CameraMode.Character:
                            _cameraController = new CameraControllerPerson(Camera, _targetCharacter, new Vector3(-10,10,0));
                        break;
                    case CameraControllers.CameraMode.Free:
                        _targetCharacter = null;
                        MyGame.Instance._MAINGAME._characters["player"]._hisInput.Enabled = false;
                        _cameraController = new FreeCamera(Camera);
                        break;
                    default: break;
                }
                cam_mode = value; 
            }

            get { return cam_mode; }
        }

    }
}
