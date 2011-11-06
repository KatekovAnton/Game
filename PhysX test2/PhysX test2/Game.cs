using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PhysX_test2.Content;
using PhysX_test2.Engine;
using StillDesign.PhysX;
using Ray = Microsoft.Xna.Framework.Ray;

using PhysX_test2.TheGame;

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

        private Log _log;
        private string _outputstring = string.Empty;
        private SpriteBatch _spriteBatch;

        private string _helpHint = "Press \'O\' to swich debug render\nPress \'P\' to toggle physic model of box\nPress \'I\' to force marine to drop gun";

        public MyGame()
        {
            PhysX_test2.MouseManager.Manager = new MouseManager();
            Instance = this;
            _log = new Log();
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
        }

        private void HandleKeyboard(KeyboardState keyboardState)
        {
            ray = Extensions.FromScreenPoint(GraphicsDevice.Viewport, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), _engine.Camera.View, _engine.Camera.Projection);
          /*  _engine.LevelObjectCursorSphere.behaviourmodel.SetGlobalPose(Matrix.Identity,null);
            System.Collections.Generic.List<Vector3?> VectorList=new System.Collections.Generic.List<Vector3?>();
            Vector3? point=new Vector3?();
            int CrossCount = 0;
           // VectorList.Add(_engine.LevelObjectTestSide.raycastaspect.IntersectionClosest(ray,_engine.LevelObjectTestSide.transform));
            foreach (Engine.Logic.PivotObject Object in _engine.gameScene._visibleObjects)
            {
                
                if (Object._needMouseCast)
                {
                    if(Object.raycastaspect.IntersectionClosest(ray, Object.transform)!=null)
                    {
                    VectorList.Add( Object.raycastaspect.IntersectionClosest(ray, Object.transform));
                    }
                }

            }
            foreach (Vector3? Vec in VectorList)
            {
                if (Vec != null)
                {
                    CrossCount++;
                }
            }
            bool CrossedWithLevel = false;
            if (CrossCount == 1)
            {

                if (_engine.LevelObjectTestSide.raycastaspect.IntersectionClosest(ray, _engine.LevelObjectTestSide.transform) != null)
                {
                    CrossedWithLevel = true;
                    point = _engine.LevelObjectTestSide.raycastaspect.IntersectionClosest(ray, _engine.LevelObjectTestSide.transform);
                }
            }
            if (!CrossedWithLevel)
            {
                {
                    bool IsPointed = true;
                    foreach (Vector3? point1 in VectorList)
                    {


                        Vector3? Vec1 = point1 + _engine.Camera.View.Translation;


                        foreach (Vector3? point2 in VectorList)
                        {
                            if (point1 == null | point2 == null) break;
                            
                            Vector3? Vec2 = point2 + _engine.Camera.View.Translation;
                            if (Vec1.Value.LengthSquared() > Vec2.Value.LengthSquared())
                            {

                                IsPointed = false;
                                break;
                            }
                        }
                        if (IsPointed)
                        {
                            if (point1 != null)
                            {
                                point = point1;
                                break;
                            }
                        }
                        IsPointed = true;
                    }
                }
            }

            if (point != null)
            {
                _engine.LevelObjectCursorSphere.behaviourmodel.SetGlobalPose(Matrix.CreateTranslation(point.Value), null);
                _mousepoint = point.Value;
            }*/
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
                    }
                }
            }
            _mousepoint = newpoint;
            _engine.LevelObjectCursorSphere.behaviourmodel.SetGlobalPose(Matrix.CreateTranslation(newpoint), null);
        }

        protected override void Update(GameTime gameTime)
        {
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
            _spriteBatch.DrawString(_font1, DateTime.Now.ToString(), new Vector2(10, 170), Color.Black, 0, new Vector2(), 1.0f, SpriteEffects.None, 0.5f);

            _spriteBatch.DrawString(_font1, string.Format("FPS: {0} Frame time: {1}", _engine.FPSCounter.FramesPerSecond, _engine.FPSCounter.FrameTime), Vector2.Zero, Color.White);
            _spriteBatch.DrawString(_font1, "Visible objects count: " + _engine.visibleobjectscount.ToString(), new Vector2(0, 15), Color.White);
            _spriteBatch.DrawString(_font1, "Recalulcalated objects count: " + _engine.gameScene._sceneGraph.recalulcalated().ToString(), new Vector2(0, 30), Color.White);

            _spriteBatch.DrawString(_font1, "Character angle: " + _engine._charcterController.movestate.ToString() , new Vector2(0, 45), Color.White);

            _spriteBatch.DrawString(_font1, _helpHint, new Vector2(0, 60), Color.Black);

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