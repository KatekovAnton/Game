using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PhysX_test2.Content;
using PhysX_test2.Engine;
using StillDesign.PhysX;
using Ray = Microsoft.Xna.Framework.Ray;

using PhysX_test2.TheGame;
using PhysX_test2.Engine.Helpers;

namespace PhysX_test2
{
    public enum MyGameState
    {
        Game,
        Menu,
        Pause,
        Loading
    }


    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MyGame : Game
    {
        public static MyGameState _currentState;

        public static GraphicsDeviceManager DeviceManager;
        public static GraphicsDevice Device;
        public static MyGame Instance;

        public Vector3 _mousepoint = Vector3.Zero;
        public Ray ray;

        //some additional variables
        public Vector3 _boxScreenPosition;
        private SpriteFont _font1;
        private Vector2 _fontPos;
     
        public GameEngine _engine;
        private BasicEffect _lighting;

        public TheGame.TheGame _MAINGAME;

      
        private string _outputstring = string.Empty;
        private SpriteBatch _spriteBatch;

        private string _helpHint = "Press \'O\' to swich debug render\nPress \'P\' to toggle physic model of box\nPress \'I\' to force marine to drop gun";

        public MyGame()
        {
            PhysX_test2.MouseManager.Manager = new MouseManager();
            Instance = this;
            DeviceManager = new GraphicsDeviceManager(this);
            IsMouseVisible = true;

            if (!GraphicsAdapter.DefaultAdapter.IsProfileSupported(GraphicsProfile.HiDef))
                DeviceManager.GraphicsProfile = GraphicsProfile.Reach;
            else
                DeviceManager.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";


            DeviceManager.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;

            _MAINGAME = new TheGame.TheGame(this);
            int n, sum = 0, mult = 1;
            n = 105;
            while (n > 0)
            {
                sum = (sum + n) % 10;
                mult = mult * (n % 10);
                n = n / 10;
            }
            int t = sum + mult;
            t++;
        }


        protected override void Initialize()
        {
            base.Initialize();

            _MAINGAME.Initialize();

            _currentState = MyGameState.Game;
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            MyGame.Device = GraphicsDevice;
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _font1 = Content.Load<SpriteFont>("Courier New");
            _fontPos = new Vector2(10.0f, 10.0f);
            _lighting = new BasicEffect(MyGame.Device);

            _engine.Font1 = _font1;
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            _engine.UnLoad();
            LogProvider.logMessage("Closing application");
            LogProvider.SaveLog();
            LogProvider.Close();
           
        }

        private void HandleKeyboard(KeyboardState keyboardState)
        {
            ray = Extensions.FromScreenPoint(GraphicsDevice.Viewport, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), _engine.Camera.View, _engine.Camera.Projection);
        }

        public void SearchClickedObject()
        {
            //ищем объект на кот тыкнули
            Engine.Logic.PivotObject clickedlo = null;
            Vector3 newpoint = _mousepoint;
            float distance = 10000;

            Vector3 camerapos = ray.Position;
            foreach (Engine.Logic.PivotObject lo in this._engine.gameScene._visibleObjects)
            {
                if (!lo._needMouseCast)
                    continue;
                Vector3? point = lo.raycastaspect.IntersectionClosest(ray, lo.transform);
                if (point != null)
                {
                    float range = (point.Value - camerapos).Length();
                    if (range < distance)
                    {
                        clickedlo = lo;
                        distance = range;
                        newpoint = point.Value;
                        newpoint.Y -= 0.1f;
                    }
                }
            }
            _mousepoint = newpoint;
            
            _engine.LevelObjectCursorSphere.behaviourmodel.SetGlobalPose(Matrix.CreateTranslation(newpoint), null);
        }

        protected override void Update(GameTime gameTime)
        {
            StatisticContainer.Instance().EndFrame();
            StatisticContainer.Instance().BeginFrame();
            KeyboardManager.Manager.Update();
            MouseManager.Manager.Update();
            HandleKeyboard(Keyboard.GetState());
            _engine.Update(gameTime);
            SearchClickedObject();

            _MAINGAME.Update(gameTime);

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
          //  _spriteBatch.DrawString(_font1, DateTime.Now.ToString(), new Vector2(10, 170), Color.Black, 0, new Vector2(), 1.0f, SpriteEffects.None, 0.5f);

            _spriteBatch.DrawString(_font1, string.Format("FPS: {0} Frame time: {1}", _engine.FPSCounter.FramesPerSecond, _engine.FPSCounter.FrameTime), Vector2.Zero, Color.Black);
            _spriteBatch.DrawString(_font1, "Visible objects count: " + _engine.visibleobjectscount.ToString(), new Vector2(0, 15), Color.Black);
            _spriteBatch.DrawString(_font1, "Recalulcalated objects count: " + _engine.gameScene._sceneGraph.recalulcalated().ToString(), new Vector2(0, 30), Color.Black);

            _spriteBatch.DrawString(_font1, "Character angle: " + _engine.playerState.ToString(), new Vector2(0, 45), Color.Black);

            _spriteBatch.DrawString(_font1, _helpHint, new Vector2(0, 60), Color.Black);



            for (int i = 0; i < ScreenLog.Messages.Count; i++)
            {
                _spriteBatch.DrawString(_font1, ScreenLog.Messages[i], new Vector2(0, GameConfiguration.ScreenResolution.Y - i * 10 - 30),  ScreenLog.MessageColors[i]);
            }
        

            _spriteBatch.End();
        }

        public static void ScreenLogMessage(string __message, Color? color = null)
        {
            //VOVA - так выводятся сообщения на экран
            Color c =  color == null? Color.Black:color.Value;

            ScreenLog.TraceMessage(__message, c);
        }
    }
    

    public class UserOutput : UserOutputStream
    {
        public override void Print(string message)
        {
            if (LogProvider.NeedTracePhysXMessages)
                LogProvider.logMessage("PhysX: " + message);
        }


        public override AssertResponse ReportAssertionViolation(string message, string file, int lineNumber)
        {
            if (LogProvider.NeedTracePhysXMessages)
                LogProvider.logMessage("PhysX: " + message);

            return AssertResponse.Continue;
        }


        public override void ReportError(ErrorCode errorCode, string message, string file, int lineNumber)
        {
            if (LogProvider.NeedTracePhysXMessages)
                LogProvider.logMessage("PhysX: " + message);
        }
    }
}