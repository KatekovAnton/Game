﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace PhysX_test2.Engine
{
    public class Camera
    {
        public Matrix Projection;
        public Matrix View;

        public float CameraPitch
        {
            get;
            private set;
        }
        public float CameraYaw
        {
            get;
            private set;
        }
        private GameEngine _engine;

        public BoundingFrustum cameraFrustum = new BoundingFrustum(Matrix.Identity);

        public Vector3 _position;
        public Vector3 _target;
        public Vector3 _direction;

        public Camera(GameEngine engine, Vector3? cameraposition = null, Vector3? cameratarget = null)
        {
            if (cameratarget == null)
                cameratarget = new Vector3(30, 30, 0);
            if (cameraposition == null)
                cameraposition = new Vector3(30, 30, 10);
            _engine = engine;
            _position = cameraposition.Value;
            _target = cameratarget.Value;
            _direction = (_target - _position);
            _direction.Normalize();
            
            float katet = -(cameratarget.Value.Y - cameraposition.Value.Y);
            float gipotenuza = (cameraposition.Value - cameratarget.Value).Length();
            CameraPitch = -MathHelper.PiOver2 + Convert.ToSingle(Math.Acos(Convert.ToDouble(katet / gipotenuza)));
            float determ = (cameratarget.Value.Z - cameraposition.Value.Z);
            if (determ == 0)
                determ = 0.001f;
            CameraYaw = Convert.ToSingle(Math.Atan(Convert.ToSingle((cameratarget.Value.X - cameraposition.Value.X) / determ)));
            View = Matrix.CreateLookAt(cameraposition.Value, cameratarget.Value, new Vector3(0, 1, 0));
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, MyGame.Device.Viewport.AspectRatio, 1, 100);
            cameraFrustum.Matrix = View * Projection;
        }


        public Camera(GameEngine engine)
        {
            Vector3? cameratarget = new Vector3(1, 1, 1);
            Vector3? cameraposition = new Vector3(1, 1, 1);
            _engine = engine;
            View = Matrix.CreateLookAt(cameraposition.Value, cameratarget.Value, new Vector3(0, 1, 0));
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, MyGame.Device.Viewport.AspectRatio, 1, 150);
            cameraFrustum.Matrix = View * Projection;
        }


        public void Update(Vector3 cameraposition, Vector3 cameratarget)
        {
            
            _position = cameraposition;
            _target = cameratarget;
          
            _direction = (_target - _position);
            _direction.Normalize();
            
            float katet = -(cameratarget.Y - cameraposition.Y);
            float gipotenuza = (cameraposition - cameratarget).Length();
            this.CameraPitch = -MathHelper.PiOver2 + Convert.ToSingle(Math.Acos(Convert.ToDouble(katet / gipotenuza)));
            float determ = (cameratarget.Z - cameraposition.Z);
            if (determ == 0)
                determ = 0.001f;
            this.CameraYaw = Convert.ToSingle(Math.Atan(Convert.ToSingle((cameratarget.X - cameraposition.X) / determ)));
            this.View = Matrix.CreateLookAt(cameraposition, cameratarget, new Vector3(0, 1, 0));
            this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, MyGame.Device.Viewport.AspectRatio, 1, 150);
            cameraFrustum.Matrix = View * Projection;
        }

        public void Update(GameTime elapsedTime)
        {
            var elapsed = (float)(elapsedTime.ElapsedGameTime.TotalMilliseconds / 1000.0);
            const float speed = 14.0f;
            float distance = speed * elapsed;
            GameWindow window = MyGame.Instance.Window;
            cameraFrustum.Matrix = View * Projection;
        }
    }
}