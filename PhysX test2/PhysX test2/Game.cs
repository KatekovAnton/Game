using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using StillDesign.PhysX;
using PhysX_test2.Engine;
using PhysX_test2.Engine.DebugRender;
using PhysX_test2.Content;

namespace PhysX_test2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MyGame : Microsoft.Xna.Framework.Game
    {
        

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont Font1;
        private BasicEffect _lighting;
        public GameEngine _engine;
        private Vector2 FontPos;
        private Core _core;
        private Scene _scene;
        private Log log;
        string outputstring = string.Empty;
        PackList packs;

        public MyGame()
        {
           log = new Log();
            packs = new PackList();
            _engine = new GameEngine(this, packs);

            graphics = GameEngine.DeviceManager;
            if (!Microsoft.Xna.Framework.Graphics.GraphicsAdapter.DefaultAdapter.IsProfileSupported(GraphicsProfile.HiDef))
                graphics.GraphicsProfile = GraphicsProfile.Reach;
            else
                graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";

            graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;


        }

   
        protected override void Initialize()
        {
           base.Initialize();

            _engine.Initalize();

            // For convenience
            _core = _engine.Core;
            _scene = _engine.Scene;
            // TODO: Add your initialization logic here
            
            
            this.IsMouseVisible = true;

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
            spriteBatch = new SpriteBatch(GraphicsDevice);
           
            Font1 = Content.Load<SpriteFont>("Courier New");
            FontPos = new Vector2(10.0f, 10.0f);
            _lighting = new BasicEffect(GameEngine.Device);

            _engine.Font1 = Font1;
           
            outputstring = packs.packs.Length.ToString();
        }
       
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            _engine.UnLoad();
        }
        bool lfirst = false;


        private void HandleKeyboard(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.L) && !lfirst)
            {
                lfirst = true;

              //  _engine.WorldObjectCharacterBox.behaviourmodel.SetGlobalPose(Matrix.CreateRotationX(-MathHelper.PiOver2)* Matrix.CreateTranslation(0, 20, 0), null);
            }
            if (keyboardState.IsKeyUp(Keys.L) && lfirst)
                lfirst = false;


           

            if (keyboardState.IsKeyDown(Keys.K))
                _engine.LevelObjectCharacterBox.behaviourmodel.Move(new Vector3(5.0f, 0, 0));

            if (keyboardState.IsKeyDown(Keys.H))
                _engine.LevelObjectCharacterBox.behaviourmodel.Move(new Vector3(-5.0f, 0, 0));

            if (keyboardState.IsKeyDown(Keys.J))
                _engine.LevelObjectCharacterBox.behaviourmodel.Move(new Vector3(0, 0, 5.0f));

            if (keyboardState.IsKeyDown(Keys.U))
                _engine.LevelObjectCharacterBox.behaviourmodel.Move(new Vector3(0, 0, -5.0f));

            

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            var ray = Extensions.FromScreenPoint(
                GraphicsDevice.Viewport,
                new Vector2(Mouse.GetState().X, Mouse.GetState().Y),
                _engine.Camera.View,
                _engine.Camera.Projection);


            Vector3? point =  _engine.LevelObjectTestSide.raycastaspect.IntersectionClosest(ray, _engine.LevelObjectTestSide.transform);
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

            outputstring = _engine.Camera.View.Translation.ToString() + '\n' + _engine.Camera.View.Up.ToString(); ;
            spriteBatch.Begin();
            if(_engine.BoxScreenPosition.Z<1.0)
                spriteBatch.DrawString(Font1, "Box position = " + _engine.LevelObjectBox.behaviourmodel.GetGlobalPose().Translation.ToString(), new Vector2(_engine.BoxScreenPosition.X,_engine.BoxScreenPosition.Y), Color.Black, 0, new Vector2(), 1.0f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(Font1, DateTime.Now.ToString(), new Vector2(10,170), Color.Black, 0, new Vector2(), 1.0f, SpriteEffects.None, 0.5f);

            spriteBatch.DrawString(Font1, string.Format("FPS: {0} Frame time: {1}", _engine.FPSCounter.FramesPerSecond, _engine.FPSCounter.FrameTime), Vector2.Zero, Color.White);
            spriteBatch.DrawString(Font1, "Visible objects count: " + _engine.visibleobjectscount.ToString(), new Vector2(0,15), Color.White);
            spriteBatch.DrawString(Font1, "Recalulcalated objects count: " +_engine._sceneGraph.recalulcalated().ToString(), new Vector2(0, 30), Color.White);
            
            spriteBatch.End();
        }
    }
    
    public class UserOutput : UserOutputStream
    {
        public override void Print(string message)
        {
            if(LogProvider.NeedTracePhysXMessages)
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
