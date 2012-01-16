using System;
using System.Collections.Generic;
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
    public class MyGame : Game, IKeyboardUser
    {
        private List<HotKey> _hotkeys;
        public bool AllKeys { get { return false; } }
        public static MyGameState _currentState;

        public static GraphicsDeviceManager DeviceManager;
        public static GraphicsDevice Device;
        public static MyGame Instance;

        /// <summary>
        /// last received gameTime of updating
        /// </summary>
        public static GameTime UpdateTime = new GameTime();

        public Vector3 _mousepoint = Vector3.Zero;
        public Vector3 _mousepointNormal = Vector3.UnitY;
        public Ray ray;

        //some additional variables
        public Vector3 _boxScreenPosition;

        private Vector2 _fontPos;
     
        public GameEngine _engine;
        private BasicEffect _lighting;

        public TheGame.TheGame _MAINGAME;

        public UserInterface.GameInterface UI;

        private bool showConsole = true;
        private string _outputstring = string.Empty;
        public SpriteBatch _spriteBatch;

        private string _helpHint = "\'Escape\' to Show MainMenu\n\'O\' to swich debug render\n\'P\' to toggle physic model of box\n\'I\' to force marine to drop gun\n\'~\' to toggle event console\n\'Left Ctrl + C\' to console writing";

        public static ScreenLog ScreenLog;

        public bool GlobalUser { set{} get { return true; } }
        
        public MyGame()
        {
            _hotkeys = new List<HotKey>();
            _hotkeys.Add(new HotKey(new Keys[] { Keys.OemTilde }, SwitchConsoleView));
            _hotkeys.Add(new HotKey(new Keys[] { Keys.Escape }, ShowMainMenu));
            _hotkeys.Add(new HotKey(new Keys[] { Keys.LeftControl, Keys.C }, SwitchConsoleEditMode));

            KeyboardManager.Manager.AddKeyboardUser(this);

            ScreenLog = new Engine.Helpers.ScreenLog();

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

            Fonts.Load("base_skin");

            _fontPos = new Vector2(10.0f, 10.0f);
            _lighting = new BasicEffect(MyGame.Device);
            this.Exiting += new EventHandler<EventArgs>(MyGame_Exiting);
        }

        void MyGame_Exiting(object sender, EventArgs e)
        {
            _engine.UnLoad();
            LogProvider.Close();
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            _engine.UnLoad();
            LogProvider.Close();
        }

        private void HandleKeyboard(KeyboardState keyboardState)
        {
            ray = Extensions.FromScreenPoint(GraphicsDevice.Viewport, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), _engine.Camera.View, _engine.Camera.Projection);
        }
        bool ft = true;
        

        protected override void Update(GameTime gameTime)
        {
            UpdateTime = gameTime;
            StatisticContainer.Instance().EndFrame();
            StatisticContainer.Instance().BeginFrame();
            KeyboardManager.Manager.Update();

            


            MouseManager.Manager.Update();
            HandleKeyboard(Keyboard.GetState());

            _engine.Update(gameTime);

            _MAINGAME.Update(gameTime);
            _engine.UpdateEnd(gameTime);
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
                _spriteBatch.DrawString(PhysX_test2.Content.Fonts._font1, "Box position = " + _engine.LevelObjectBox.behaviourmodel.GetGlobalPose().Translation.ToString(), new Vector2(_engine.BoxScreenPosition.X, _engine.BoxScreenPosition.Y), Color.Black, 0, new Vector2(), 1.0f, SpriteEffects.None, 0.5f);


            _spriteBatch.DrawString(Fonts._font1, string.Format("FPS: {0} Frame time: {1}", _engine.FPSCounter.FramesPerSecond, _engine.FPSCounter.FrameTime), Vector2.Zero, Color.Black);
            _spriteBatch.DrawString(Fonts._font1, "Visible objects count: " + _engine.visibleobjectscount.ToString(), new Vector2(0, 15), Color.Black);
            _spriteBatch.DrawString(Fonts._font1, "Recalulcalated objects count: " + _engine.gameScene._sceneGraph.recalulcalated().ToString(), new Vector2(0, 30), Color.Black);
            _spriteBatch.DrawString(Fonts._font1, "Character angle: " + _engine.playerState.ToString(), new Vector2(0, 45), Color.Black);
            _spriteBatch.DrawString(Fonts._font1, _helpHint, new Vector2(0, 60), Color.Black);


            if (showConsole)
            {
                int i = 0;
                foreach (ScreenLogMessage ScreenLogMessage in ScreenLog)
                {
                    _spriteBatch.DrawString(Fonts._font1, ScreenLogMessage.text, new Vector2(0, GameConfiguration.ScreenResolution.Y - (i++) * 10 - 15), ScreenLogMessage.Color);
                }
            }
            _spriteBatch.End();
        }

        public static void ScreenLogMessage(string __message, Color? color = null)
        {
            //VOVA - так выводятся сообщения на экран
            Color c =  color == null? Color.Black:color.Value;

            ScreenLog.TraceMessage(__message, c);
        }

        public void SwitchConsoleView()
        {
            showConsole = !showConsole;
        }

        public void ShowMainMenu()
        {
        
        }

        public void SwitchConsoleEditMode()
        {
            showConsole = true;
            
        }

        public bool IsKeyboardCaptured()
        {
            return false;
        }

        public List<HotKey> hotkeys
        {
            get
            {
                return _hotkeys;
            }
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