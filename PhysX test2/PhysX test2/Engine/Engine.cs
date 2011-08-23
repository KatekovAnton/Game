using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;


using StillDesign.PhysX;


using PhysX_test2.Content;
using PhysX_test2.Engine.Render;
using PhysX_test2.Engine.ContentLoader;
using PhysX_test2.Engine.Logic;


namespace PhysX_test2.Engine
{
    public class GameEngine
	{

        public Actor groundplane;
        public ControllerManager manager;
        public RenderPipeline GraphicPipeleine;
        public static GameEngine Instance;
        
        public PhysX_test2.Engine.Logic.LevelObject LevelObjectBox;
        public PhysX_test2.Engine.Logic.LevelObject LevelObjectTestSide;
        public PhysX_test2.Engine.Logic.LevelObject LevelObjectCharacterBox;
        public PhysX_test2.Engine.Logic.LevelObject LevelObjectCursorSphere;
        
        public PhysX_test2.Engine.Helpers.FpsCounter FPSCounter;

        public SpriteBatch spriteBatch;
        public SpriteFont Font1;

        public PackList packs;

        public Vector3 lightDir = new Vector3(-1, -1, -1);

        public static GraphicsDeviceManager DeviceManager;
        public static GraphicsDevice Device;
        public GameScene gameScene;

        public int visibleobjectscount = 0;
        
        public GameEngine(MyGame game, PackList p)
		{
            Instance = this;
            lightDir.Normalize();
            packs = p;
			this.Game = game;
			DeviceManager = new GraphicsDeviceManager( game );
            //разме рэкрана
			DeviceManager.PreferredBackBufferWidth = (int)( GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.8 );
			DeviceManager.PreferredBackBufferHeight = (int)( GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.8 );
		}
 
        public void Initalize()
        {
            FPSCounter = new Helpers.FpsCounter();
            this.Camera = new Camera(this, new Vector3(20, 20, 10), new Vector3(0, 15, 0));
            spriteBatch = new SpriteBatch(DeviceManager.GraphicsDevice);
          
            CoreDescription coreDesc = new CoreDescription();
            UserOutput output = new UserOutput();

            Core = new Core(coreDesc, output);
            Core.SetParameter(PhysicsParameter.ContinuousCollisionDetection, false);
            Core.SetParameter(PhysicsParameter.ContinuousCollisionDetectionEpsilon, 0.01f);

            SceneDescription sceneDesc = new SceneDescription()
            {
                SimulationType = SimulationType.Software,//Hardware,
                MaximumBounds = new Bounds3(-1000,-1000,-1000,1000,1000,1000),
                UpAxis = 2,
                Gravity = new Vector3(0.0f, -9.81f*1.3f, 0.0f),
                GroundPlaneEnabled = false
            };
            this.Scene = Core.CreateScene(sceneDesc);
            manager = Scene.CreateControllerManager();
            loaddata();
            
            ebuchest e = new ebuchest();
        }


        void loaddata()
        {
            groundplane = CreateGroundPlane();

            using (var stream = new System.IO.FileStream(@"Content\Shaders\ObjectRender.fx", System.IO.FileMode.Open))
            {
                PhysX_test2.Engine.Render.Materials.Material.ObjectRenderEffect = PhysX_test2.Engine.Render.Shader.FromStream(stream, Device);
            }
            Scene.UserContactReport = new ContactReport(this.Game);


            gameScene = new GameScene();
            GraphicPipeleine = new RenderPipeline(DeviceManager.GraphicsDevice, Camera);


            ///box 
            LevelObjectDescription boxrdescription = new LevelObjectDescription();
            boxrdescription = packs.GetObject("WoodenCrate10WorldObject\0", boxrdescription) as LevelObjectDescription;

            LevelObject lo = ContentLoader.ContentLoader.LevelObjectFromDescription(boxrdescription, packs, Scene);
            lo.SetGlobalPose(Matrix.CreateRotationX(1.0f) * Matrix.CreateTranslation(0, 30, 0));
            GraphicPipeleine.ProceedObject(lo.renderaspect);
            gameScene.AddObject(lo);
            LevelObjectBox = lo;




            ////our character
            LevelObjectDescription boxcharacterdescription = new LevelObjectDescription();
            boxcharacterdescription = packs.GetObject("MyNewCharacter\0", boxcharacterdescription) as LevelObjectDescription;

            lo = ContentLoader.ContentLoader.LevelObjectFromDescription(boxcharacterdescription, packs, Scene);
            lo.SetGlobalPose(Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateTranslation(0, 20, 0));
            GraphicPipeleine.ProceedObject(lo.renderaspect);
            gameScene.AddObject(lo);
            LevelObjectCharacterBox = lo;




            ////test side
            LevelObjectDescription testsiderdescription = new LevelObjectDescription();
            testsiderdescription = packs.GetObject("TestSideWorldObject\0", testsiderdescription) as LevelObjectDescription;

            lo = ContentLoader.ContentLoader.LevelObjectFromDescription(testsiderdescription, packs, Scene);
            lo.SetGlobalPose(Matrix.CreateFromAxisAngle(new Vector3(1, 0, 0), -MathHelper.PiOver2) * Matrix.CreateTranslation(0, 15, 0));
            GraphicPipeleine.ProceedObject(lo.renderaspect);
            gameScene.AddObject(lo);
            LevelObjectTestSide = lo;




            /////sphere
            LevelObjectDescription spheredesc = new LevelObjectDescription();
            spheredesc = packs.GetObject("Cursor\0", spheredesc) as LevelObjectDescription;

            lo = ContentLoader.ContentLoader.LevelObjectFromDescription(spheredesc, packs, Scene);
            lo.SetGlobalPose(Matrix.CreateTranslation(0, 30, 0));
            GraphicPipeleine.ProceedObject(lo.renderaspect);
            gameScene.AddObject(lo);
            LevelObjectCursorSphere = lo;

            


            packs.Unload();

        }

        public Vector3 BoxScreenPosition;


		public void Update( GameTime gameTime )
		{
            
            //Begin update world objects
            foreach (PivotObject lo in gameScene.objects)
                lo.BeginDoFrame();
            //Update world(calc ray trace, deleting bullets, applying forces and other)
            //------

            foreach (PivotObject lo in gameScene.objects)
                lo.DoFrame(gameTime);
            // Update Physics
            Scene.Simulate((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f);

            //update world objects
			Scene.FlushStream();
			Scene.FetchResults( SimulationStatus.RigidBodyFinished, true );

            //End updating world objects
            foreach (PivotObject lo in gameScene.objects)
                lo.EndDoFrame();
            
            
            
            //Udating data for scenegraph
            gameScene.UpdateScene();
           
            //Garbage collection(nulling deleted objects)
            //------


            //очищаем конвейер
            GraphicPipeleine.NewFrame(lightDir);
            //Updating camera
			Camera.Update( gameTime );


            Vector3 v1 = DeviceManager.GraphicsDevice.Viewport.Project(gameScene.objects[0].transform.Translation, Camera.Projection, Camera.View, Matrix.Identity);
            BoxScreenPosition = new Vector3(Convert.ToSingle(Convert.ToInt32(v1.X)), Convert.ToSingle(Convert.ToInt32(v1.Y)), v1.Z);
            

            
           
         
            //добавляем все нобходимые объекты на отрисовку
            GraphicPipeleine.AddObjectToPipeline(gameScene.VisibleObjects);
            GraphicPipeleine.AddObjectToShadow(gameScene.ShadowObjects);
            visibleobjectscount = gameScene.VisibleObjects.Count;


            FPSCounter.Update(gameTime);
		}




        public void Draw()
        {
            //основной рендер. будет потом в колор рендертаргет. Внутри- дефферед шэйдинг и вся хрень
            GraphicPipeleine.RenderToPicture(Camera, lightDir);

            //потом пост-процессинг

            //потом ещё чонить
        }
       
		public static Color Int32ToColor( int color )
		{
			byte a = (byte)( ( color & 0xFF000000 ) >> 32 );
			byte r = (byte)( ( color & 0x00FF0000 ) >> 16 );
			byte g = (byte)( ( color & 0x0000FF00 ) >> 8 );
			byte b = (byte)( ( color & 0x000000FF ) >> 0 );

			return new Color( r, g, b, a );
		}
		public static int ColorToArgb( Color color )
		{
			int a = (int)( color.A );
			int r = (int)( color.R );
			int g = (int)( color.G );
			int b = (int)( color.B );

			return ( a << 24 ) | ( r << 16 ) | ( g << 8 ) | ( b << 0 );
		}
        Actor CreateGroundPlane()
        {
            // Create a plane with default descriptor
            PlaneShapeDescription planeDesc = new PlaneShapeDescription();
            ActorDescription actorDesc = new ActorDescription();
            StillDesign.PhysX.Material defaultMaterial = Scene.Materials[0];
            planeDesc.Material = defaultMaterial;
            actorDesc.Shapes.Add(planeDesc);
           // actorDesc.ContactPairFlags = ContactPairFlag.All;
            //Scene.SetGroupCollisionFlag(1, 2, true);
            actorDesc.Group = 1;
            return Scene.CreateActor(actorDesc);
           
            
        }
        public void UnLoad()
        {
            this.Core.Dispose();
            Scene.Dispose();
          //  BoxActor.Dispose();
            groundplane.Dispose();
            
           
        }
		#region Properties
		public MyGame Game
		{
			get;
			private set;
		}
        public Camera Camera;

		public Core Core
		{
			get;
			private set;
		}
		public Scene Scene
		{
			get;
			private set;
		}

		#endregion

    }
}
