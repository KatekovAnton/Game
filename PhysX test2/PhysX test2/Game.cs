using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PhysX_test2.Content;
using PhysX_test2.Engine;
using StillDesign.PhysX;
using Ray = Microsoft.Xna.Framework.Ray;


namespace PhysX_test2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MyGame : Game
    {

        public static MyGame Instance;


        //some additional variables
        public Vector3 _boxScreenPosition;
        private SpriteFont _font1;
        private Vector2 _fontPos;
        private Core _core;
        public GameEngine _engine;
        private BasicEffect _lighting;
        private Scene _scene;
        private GraphicsDeviceManager _graphics;
        private Log _log;
        private string _outputstring = string.Empty;
        private SpriteBatch _spriteBatch;


        public MyGame()
        {
            Instance = this;
            _log = new Log();

            _engine = new GameEngine(this);

            _graphics = GameEngine.DeviceManager;
            if (!GraphicsAdapter.DefaultAdapter.IsProfileSupported(GraphicsProfile.HiDef))
                _graphics.GraphicsProfile = GraphicsProfile.Reach;
            else
                _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";

            _graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
        }


        protected override void Initialize()
        {
            base.Initialize();

            _engine.Initalize();

            // For convenience
            _core = _engine.Core;
            _scene = _engine.Scene;

            IsMouseVisible = true;
            LoadPhysics();
        }


        private void LoadPhysics()
        {
            _core.SetParameter(PhysicsParameter.ContinuousCollisionDetection, true);
            _core.SetParameter(PhysicsParameter.ContinuousCollisionDetectionEpsilon, 0.01f);
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            GameEngine.Device = GraphicsDevice;
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _font1 = Content.Load<SpriteFont>("Courier New");
            _fontPos = new Vector2(10.0f, 10.0f);
            _lighting = new BasicEffect(GameEngine.Device);

            _engine.Font1 = _font1;
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            _engine.UnLoad();
        }


        private void HandleKeyboard(KeyboardState keyboardState)
        {
            Ray ray = Extensions.FromScreenPoint(GraphicsDevice.Viewport, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), _engine.Camera.View, _engine.Camera.Projection);

            Vector3? point = _engine.LevelObjectTestSide.raycastaspect.IntersectionClosest(ray, _engine.LevelObjectTestSide.transform);
            if (point != null)
                _engine.LevelObjectCursorSphere.behaviourmodel.SetGlobalPose(Matrix.CreateTranslation(point.Value), null);
        }


        protected override void Update(GameTime gameTime)
        {
            HandleKeyboard(Keyboard.GetState());
            _engine.Update(gameTime);

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            _engine.Draw();

            drawstring();
        }


        public void drawstring()
        {
            _outputstring = _engine.Camera.View.Translation.ToString() + '\n' + _engine.Camera.View.Up.ToString();
            _spriteBatch.Begin();
            if (_engine.BoxScreenPosition.Z < 1.0)
                _spriteBatch.DrawString(_font1, "Box position = " + _engine.LevelObjectBox.behaviourmodel.GetGlobalPose().Translation.ToString(), new Vector2(_engine.BoxScreenPosition.X, _engine.BoxScreenPosition.Y), Color.Black, 0, new Vector2(), 1.0f, SpriteEffects.None, 0.5f);
            _spriteBatch.DrawString(_font1, DateTime.Now.ToString(), new Vector2(10, 170), Color.Black, 0, new Vector2(), 1.0f, SpriteEffects.None, 0.5f);

            _spriteBatch.DrawString(_font1, string.Format("FPS: {0} Frame time: {1}", _engine.FPSCounter.FramesPerSecond, _engine.FPSCounter.FrameTime), Vector2.Zero, Color.White);
            _spriteBatch.DrawString(_font1, "Visible objects count: " + _engine.visibleobjectscount.ToString(), new Vector2(0, 15), Color.White);
            _spriteBatch.DrawString(_font1, "Recalulcalated objects count: " + _engine.gameScene.sceneGraph.recalulcalated().ToString(), new Vector2(0, 30), Color.White);

            _spriteBatch.DrawString(_font1, "Character angle: " + _engine._charcterController.movestate.ToString() , new Vector2(0, 45), Color.White);

            _spriteBatch.End();
        }
    }


    public class UserOutput : UserOutputStream
    {
        public override void Print(string message)
        {
            if (LogProvider.NeedTracePhysXMessages)
                LogProvider.TraceMessage("PhysX: " + message);
        }


        public override AssertResponse ReportAssertionViolation(string message, string file, int lineNumber)
        {
            if (LogProvider.NeedTracePhysXMessages)
                LogProvider.TraceMessage("PhysX: " + message);

            return AssertResponse.Continue;
        }


        public override void ReportError(ErrorCode errorCode, string message, string file, int lineNumber)
        {
            if (LogProvider.NeedTracePhysXMessages)
                LogProvider.TraceMessage("PhysX: " + message);
        }
    }
}