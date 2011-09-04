using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysX_test2.Content;
using PhysX_test2.Engine.CameraControllers;
using PhysX_test2.Engine.Helpers;
using PhysX_test2.Engine.Logic;
using PhysX_test2.Engine.Render;
using StillDesign.PhysX;
using Material = PhysX_test2.Engine.Render.Materials.Material;
using Microsoft.Xna.Framework.Input;


namespace PhysX_test2.Engine {
    public class GameEngine {
        public static GameEngine Instance;
        public static GraphicsDeviceManager DeviceManager;
        public static GraphicsDevice Device;
        public Vector3 BoxScreenPosition;
        public FpsCounter FPSCounter;
        public SpriteFont Font1;
        public RenderPipeline GraphicPipeleine;

        public LevelObject LevelObjectBox;
        public LevelObject LevelObjectCharacterBox;
        public LevelObject LevelObjectCursorSphere;
        public LevelObject LevelObjectTestSide;


        public CameraControllers.CameraControllerPerson _cameraController;
        public CharacterControllers.CharacterControllerSuperClass _charcterController;


        public GameScene gameScene;
        public Actor groundplane;
        public Vector3 lightDir = new Vector3(-1, -1, -1);
        public ControllerManager manager;
        public PackList packs;
        public SpriteBatch spriteBatch;

        public int visibleobjectscount;
        

        private float _lastMousePosX;
        private float _lastMousePosY;
        private int _mouseWheelOld;

        public GameEngine(MyGame game, PackList p) {
            Instance = this;
            lightDir.Normalize();
            packs = p;
            Game = game;
            DeviceManager = new GraphicsDeviceManager(game);
            //разме рэкрана
            DeviceManager.PreferredBackBufferWidth = (int) (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.8);
            DeviceManager.PreferredBackBufferHeight = (int) (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.8);
        }


        public void Initalize() {
            FPSCounter = new FpsCounter();
            Camera = new Camera(this);
            spriteBatch = new SpriteBatch(DeviceManager.GraphicsDevice);

            var coreDesc = new CoreDescription();
            var output = new UserOutput();

            Core = new Core(coreDesc, output);
            Core.SetParameter(PhysicsParameter.ContinuousCollisionDetection, false);
            Core.SetParameter(PhysicsParameter.ContinuousCollisionDetectionEpsilon, 0.01f);

            var sceneDesc = new SceneDescription {SimulationType = SimulationType.Software, //Hardware,
                                                  MaximumBounds = new Bounds3(-1000, -1000, -1000, 1000, 1000, 1000), 
                                                  UpAxis = 2, 
                                                  Gravity = new Vector3(0.0f, -9.81f * 1.5f, 0.0f), 
                                                  GroundPlaneEnabled = false};
            Scene = Core.CreateScene(sceneDesc);
            manager = Scene.CreateControllerManager();
            Loaddata();

            var e = new ebuchest();
        }


        private void Loaddata() {
            groundplane = CreateGroundPlane();

            using (var stream = new FileStream(@"Content\Shaders\ObjectRender.fx", FileMode.Open)) {
                Material.ObjectRenderEffect = Shader.FromStream(stream, Device);
            }
            Scene.UserContactReport = new ContactReport(Game);

            gameScene = new GameScene();
            GraphicPipeleine = new RenderPipeline(DeviceManager.GraphicsDevice, Camera);

            ///box 
            var boxrdescription = new LevelObjectDescription();
            boxrdescription = packs.GetObject("WoodenCrate10WorldObject\0", boxrdescription) as LevelObjectDescription;

            LevelObject lo = ContentLoader.ContentLoader.LevelObjectFromDescription(boxrdescription, packs, Scene);
            lo.SetGlobalPose(Matrix.CreateRotationX(1.0f) * Matrix.CreateTranslation(0, 30, 0));
            GraphicPipeleine.ProceedObject(lo.renderaspect);
            gameScene.AddObject(lo);
            LevelObjectBox = lo;

            ////our character
            var boxcharacterdescription = new LevelObjectDescription();
            boxcharacterdescription = packs.GetObject("MyNewCharacter\0", boxcharacterdescription) as LevelObjectDescription;

            lo = ContentLoader.ContentLoader.LevelObjectFromDescription(boxcharacterdescription, packs, Scene);
            lo.SetGlobalPose(Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateTranslation(0, 20, 0));
            GraphicPipeleine.ProceedObject(lo.renderaspect);
            gameScene.AddObject(lo);
            LevelObjectCharacterBox = lo;

            ////test side
            var testsiderdescription = new LevelObjectDescription();
            testsiderdescription = packs.GetObject("TestSideWorldObject\0", testsiderdescription) as LevelObjectDescription;

            lo = ContentLoader.ContentLoader.LevelObjectFromDescription(testsiderdescription, packs, Scene);
            lo.SetGlobalPose(Matrix.CreateFromAxisAngle(new Vector3(1, 0, 0), -MathHelper.PiOver2) * Matrix.CreateTranslation(0, 15, 0));
            GraphicPipeleine.ProceedObject(lo.renderaspect);
            gameScene.AddObject(lo);
            LevelObjectTestSide = lo;

            /////sphere
            var spheredesc = new LevelObjectDescription();
            spheredesc = packs.GetObject("Cursor\0", spheredesc) as LevelObjectDescription;

            lo = ContentLoader.ContentLoader.LevelObjectFromDescription(spheredesc, packs, Scene);
            lo.SetGlobalPose(Matrix.CreateTranslation(0, 30, 0));
            GraphicPipeleine.ProceedObject(lo.renderaspect);
            gameScene.AddObject(lo);
            LevelObjectCursorSphere = lo;

            packs.Unload();
            CreateCharCameraController();

        }


        private void CreateCharCameraController() {
            _cameraController = new CameraControllerPerson(Camera, LevelObjectCharacterBox, new Vector3(-10, 6, 0));
            _charcterController = new CharacterControllers.CharacterControllerPerson(LevelObjectCharacterBox);
        }





        public void Update(GameTime gameTime)
        {
            //Begin update world objects
            foreach (PivotObject lo in gameScene.objects)
                lo.BeginDoFrame();



            // обработка нажатий клавы
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.W))
                LevelObjectCharacterBox.behaviourmodel.Move(Extensions.VectorForCharacterMoving(Extensions.Route.Forward, _cameraController._yAngle));
            if (keyboardState.IsKeyDown(Keys.S))
                LevelObjectCharacterBox.behaviourmodel.Move(Extensions.VectorForCharacterMoving(Extensions.Route.Back, _cameraController._yAngle));
            if (keyboardState.IsKeyDown(Keys.A))
                LevelObjectCharacterBox.behaviourmodel.Move(Extensions.VectorForCharacterMoving(Extensions.Route.Left, _cameraController._yAngle));
            if (keyboardState.IsKeyDown(Keys.D))
                LevelObjectCharacterBox.behaviourmodel.Move(Extensions.VectorForCharacterMoving(Extensions.Route.Right, _cameraController._yAngle));

            // LevelObjectCharacterBox.behaviourmodel.Rotate(CreateAngleForCharacter());


            //Update world(calc ray trace, deleting bullets, applying forces and other)
            //------

            foreach (PivotObject lo in gameScene.objects)
                lo.DoFrame(gameTime);
            // Update Physics
            Scene.Simulate((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f);


            //update world objects
            Scene.FlushStream();
            Scene.FetchResults(SimulationStatus.RigidBodyFinished, true);


            //calculatin info from controllers
            _charcterController.Update(LevelObjectCursorSphere.transform.Translation);


            //End updating world objects
            foreach (PivotObject lo in gameScene.objects)
                lo.EndDoFrame();

            //Udating data for scenegraph
            gameScene.UpdateScene();

            {
                // обработка вращения камеры
                float cursorPositionX = Mouse.GetState().X;
                float deltaX = cursorPositionX - _lastMousePosX;
                float cursorPositionY = Mouse.GetState().Y;
                float deltaY = cursorPositionY - _lastMousePosY;
                MouseState mouseState = Mouse.GetState();
                if (mouseState.RightButton == ButtonState.Pressed)
                {
                    _cameraController.RotateCameraAroundChar(-deltaX * Settings.rotateSpeed);
                    _cameraController.UpDownCamera(deltaY * Settings.rotateSpeed);
                }
                _lastMousePosX = cursorPositionX;
                _lastMousePosY = cursorPositionY;


                // обработка зума
                if (mouseState.ScrollWheelValue > _mouseWheelOld)
                    _cameraController.ZoomCameraFromCha(1 / Settings.zoomSpeed);
                
                else if (mouseState.ScrollWheelValue < _mouseWheelOld)
                    _cameraController.ZoomCameraFromCha(Settings.zoomSpeed);
                
                _mouseWheelOld = mouseState.ScrollWheelValue;


                if (LevelObjectCharacterBox.behaviourmodel.moved)
                    _cameraController.UpdateCamera();
                
            }

            //очищаем конвейер
            GraphicPipeleine.NewFrame(lightDir);

            gameScene.CalculateVisibleObjects();
            Vector3 v1 = DeviceManager.GraphicsDevice.Viewport.Project(gameScene.objects[0].transform.Translation, Camera.Projection, Camera.View, Matrix.Identity);
            BoxScreenPosition = new Vector3(Convert.ToSingle(Convert.ToInt32(v1.X)), Convert.ToSingle(Convert.ToInt32(v1.Y)), v1.Z);

            //добавляем все нобходимые объекты на отрисовку
            GraphicPipeleine.AddObjectToPipeline(gameScene.VisibleObjects);
            GraphicPipeleine.AddObjectToShadow(gameScene.ShadowObjects);
            visibleobjectscount = gameScene.VisibleObjects.Count;

            FPSCounter.Update(gameTime);
        }


        public void Draw() {
            //основной рендер. будет потом в колор рендертаргет. Внутри- дефферед шэйдинг и вся хрень
            GraphicPipeleine.RenderToPicture(Camera, lightDir);

            //потом пост-процессинг

            //потом ещё чонить
        }


        public static Color Int32ToColor(int color) {
            var a = (byte) ((color & 0xFF000000) >> 32);
            var r = (byte) ((color & 0x00FF0000) >> 16);
            var g = (byte) ((color & 0x0000FF00) >> 8);
            var b = (byte) ((color & 0x000000FF) >> 0);

            return new Color(r, g, b, a);
        }


        public static int ColorToArgb(Color color) {
            int a = (color.A);
            int r = (color.R);
            int g = (color.G);
            int b = (color.B);

            return (a << 24) | (r << 16) | (g << 8) | (b << 0);
        }


        private Actor CreateGroundPlane() {
            // Create a plane with default descriptor
            var planeDesc = new PlaneShapeDescription();
            var actorDesc = new ActorDescription();
            StillDesign.PhysX.Material defaultMaterial = Scene.Materials[0];
            planeDesc.Material = defaultMaterial;
            actorDesc.Shapes.Add(planeDesc);
            // actorDesc.ContactPairFlags = ContactPairFlag.All;
            //Scene.SetGroupCollisionFlag(1, 2, true);
            actorDesc.Group = 1;
            return Scene.CreateActor(actorDesc);
        }


        public void UnLoad() {
            Core.Dispose();
            Scene.Dispose();
            //  BoxActor.Dispose();
            groundplane.Dispose();
        }

        #region Properties
        public Camera Camera;
        public MyGame Game {
            get;
            private set;
        }

        public Core Core {
            get;
            private set;
        }
        public Scene Scene {
            get;
            private set;
        }
        #endregion
    }
}