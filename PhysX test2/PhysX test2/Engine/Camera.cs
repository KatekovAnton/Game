using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace PhysX_test2.Engine {
    public class Camera {

        #region Variables
        private float _cameraPitch;
        private float _cameraYaw;
        private GameEngine _engine;
        public BoundingFrustum cameraFrustum = new BoundingFrustum(Matrix.Identity);
        #endregion



        public Camera(GameEngine engine, Vector3? cameraposition = null, Vector3? cameratarget = null) {
            if(cameratarget == null)
                cameratarget = new Vector3(30, 30, 0);
            if(cameraposition == null)
                cameraposition = new Vector3(30, 30, 10);
            _engine = engine;
            float katet = -(cameratarget.Value.Y - cameraposition.Value.Y);
            float gipotenuza = (cameraposition.Value - cameratarget.Value).Length();
            _cameraPitch = -MathHelper.PiOver2 + Convert.ToSingle(Math.Acos(Convert.ToDouble(katet / gipotenuza)));
            float determ = (cameratarget.Value.Z - cameraposition.Value.Z);
            if(determ == 0)
                determ = 0.001f;
            _cameraYaw = Convert.ToSingle(Math.Atan(Convert.ToSingle((cameratarget.Value.X - cameraposition.Value.X) / determ)));
            View = Matrix.CreateLookAt(cameraposition.Value, cameratarget.Value, new Vector3(0, 1, 0));
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GameEngine.Device.Viewport.AspectRatio, 1, 150);
            cameraFrustum.Matrix = View * Projection;
        }


        public Camera(GameEngine engine) {
            Vector3? cameratarget = new Vector3(1, 1, 1);
            Vector3? cameraposition = new Vector3(1, 1, 1);
            _engine = engine;
            View = Matrix.CreateLookAt(cameraposition.Value, cameratarget.Value, new Vector3(0, 1, 0));
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GameEngine.Device.Viewport.AspectRatio, 1, 150);
            cameraFrustum.Matrix = View * Projection;
        }


        public void Update(Vector3 cameraposition, Vector3 cameratarget)
        {
            float katet = -(cameratarget.Y - cameraposition.Y);
            float gipotenuza = (cameraposition - cameratarget).Length();
            this._cameraPitch = -MathHelper.PiOver2 + Convert.ToSingle(Math.Acos(Convert.ToDouble(katet / gipotenuza)));
            float determ = (cameratarget.Z - cameraposition.Z);
            if (determ == 0)
                determ = 0.001f;
            this._cameraYaw = Convert.ToSingle(Math.Atan(Convert.ToSingle((cameratarget.X - cameraposition.X) / determ)));
            this.View = Matrix.CreateLookAt(cameraposition, cameratarget, new Vector3(0, 1, 0));
            this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GameEngine.Device.Viewport.AspectRatio, 1, 150);
            cameraFrustum.Matrix = View * Projection;
        }

        public void Update(GameTime elapsedTime) {
            var elapsed = (float) (elapsedTime.ElapsedGameTime.TotalMilliseconds / 1000.0);
            const float speed = 14.0f;
            float distance = speed * elapsed;
            GameWindow window = MyGame.Instance.Window;

            

            // обработка мыши
            
            //Vector3 forward = Matrix.Invert(View).Forward;
            Vector3 position = Matrix.Invert(View).Translation;
            Matrix cameraRotation = Matrix.CreateFromYawPitchRoll(_cameraYaw, _cameraPitch, 0.0f);
            Vector3 newForward = Vector3.TransformNormal(Vector3.Forward, cameraRotation);


            // обработка клавиатуры
/*            KeyboardState states = Keyboard.GetState();
            Vector3 translateDirection = Vector3.Zero;
            if(states.IsKeyDown(Keys.W)) // Forwards
                translateDirection += Vector3.TransformNormal(Vector3.Forward, cameraRotation);

            if(states.IsKeyDown(Keys.S)) // Backwards
                translateDirection += Vector3.TransformNormal(Vector3.Backward, cameraRotation);

            if(states.IsKeyDown(Keys.A)) // Left
                translateDirection += Vector3.TransformNormal(Vector3.Left, cameraRotation);

            if(states.IsKeyDown(Keys.D)) // Right
                translateDirection += Vector3.TransformNormal(Vector3.Right, cameraRotation);

            translateDirection.Y = 0;

            Vector3 newPosition = position;
            if(translateDirection.LengthSquared() > 0)
                newPosition += Vector3.Normalize(translateDirection) * distance;
 */

            //View = Matrix.CreateLookAt(newPosition, newPosition + newForward, Vector3.Up);

            cameraFrustum.Matrix = View * Projection;
        }


        public float CameraYaw () {
            return _cameraYaw;
        }

        #region Properties
        public Matrix Projection;
        public Matrix View;
        #endregion
    }
}