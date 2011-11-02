using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using PhysX_test2.Content;
using PhysX_test2.Engine.CameraControllers;
using PhysX_test2.Engine.Helpers;
using PhysX_test2.Engine.Logic;
using PhysX_test2.Engine.Render;

using StillDesign.PhysX;


namespace PhysX_test2.Engine {
    public class GameEngine {

        //static variables for enviroment
        public static GameEngine Instance;
        public static GraphicsDeviceManager DeviceManager;
        public static GraphicsDevice Device;


        //engine camera and engine render
        public Camera Camera;
        public RenderPipeline GraphicPipeleine;

        
        //fps
        public FpsCounter FPSCounter;


        //for text rendering
        public SpriteFont Font1;
        public SpriteBatch spriteBatch;


        //content
        private PackList packs;


        //physx
        public Core Core;
        public Scene Scene;


        //scene and its objects
        public EngineScene gameScene;
        public LevelObject LevelObjectBox;
        public LevelObject LevelObjectCharacterBox;
        public LevelObject LevelObjectCursorSphere;
        public LevelObject LevelObjectTestSide;
        public LevelObject LevelObjectSCGunLO;
        public Actor groundplane;


        //controllers for camera and character (objects between user and game)
        public CameraControllers.CameraControllerPerson _cameraController;
        public InputControllers.InputControllerPerson _charcterController;


        //animation
        public Engine.AnimationManager.AnimationManager animationManager;


        //some additional variables
        public int visibleobjectscount;
        public Vector3 BoxScreenPosition;
        public Vector3 lightDir = new Vector3(-1, -1, -1);
       

        public GameEngine(MyGame game) {
            packs = new PackList();
            Instance = this;
            lightDir.Normalize();

            DeviceManager = new GraphicsDeviceManager(game);

            

            //разме рэкрана
            DeviceManager.PreferredBackBufferWidth = (int) (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.8);
            DeviceManager.PreferredBackBufferHeight = (int) (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.8);
        }


        public void Initalize() {
            FPSCounter = new FpsCounter();
            Camera = new Camera(this);
            spriteBatch = new SpriteBatch(DeviceManager.GraphicsDevice);

            //инит ФизиХ-а
            var coreDesc = new CoreDescription();
            var output = new UserOutput();

            Core = new Core(coreDesc, output);
            Core.SetParameter(PhysicsParameter.ContinuousCollisionDetection, false);
            Core.SetParameter(PhysicsParameter.ContinuousCollisionDetectionEpsilon, 0.01f);

            var sceneDesc = new SceneDescription {SimulationType = SimulationType.Software, //Hardware,
                                                  MaximumBounds = new Bounds3(-1000, -1000, -1000, 1000, 1000, 1000), 
                                                  UpAxis = 2,
                                                  Gravity = new StillDesign.PhysX.MathPrimitives.Vector3(0.0f, -9.81f * 1.7f, 0.0f), 
                                                  GroundPlaneEnabled = false};
            Scene = Core.CreateScene(sceneDesc);
            //для обработки столкновений
            Scene.UserContactReport = new ContactReport(MyGame.Instance);
            //аниматор
            animationManager = AnimationManager.AnimationManager.Manager;
            //шойдер
            /*using (var stream = new FileStream(@"Content\Shaders\ObjectRender.fx", FileMode.Open))
            {
                PFromStream(stream, Device);
            }*/
            PhysX_test2.Engine.Render.Materials.Material.ObjectRenderEffect = Shader.Load(MyGame.Instance.Content);
            //рендерщик
            GraphicPipeleine = new RenderPipeline(DeviceManager.GraphicsDevice, Camera);
            //загрузка всего
            Loaddata();
        }

        private void Loaddata()
        {
            //если ты с горы упал - ты ешё не экстримал
            //чтоб далеко не падать
            groundplane = CreateGroundPlane();
            

            //уровень
            gameScene = new EngineScene();

            ///box 
            {
                var boxrdescription = new LevelObjectDescription();
                boxrdescription = packs.GetObject("WoodenCrate10WorldObject\0", boxrdescription) as LevelObjectDescription;

                LevelObject lo = ContentLoader.ContentLoader.LevelObjectFromDescription(boxrdescription, packs, Scene);
                lo.SetGlobalPose(Matrix.CreateRotationX(1.0f) * Matrix.CreateTranslation(0, 30, 0));
                GraphicPipeleine.ProceedObject(lo.renderaspect);
                gameScene.AddObject(lo);
                LevelObjectBox = lo;
            }

            ////our character
            {
                var boxcharacterdescription = new LevelObjectDescription();
                boxcharacterdescription = packs.GetObject("MyNewCharacter\0", boxcharacterdescription) as LevelObjectDescription;

                LevelObject lo = ContentLoader.ContentLoader.LevelObjectFromDescription(boxcharacterdescription, packs, Scene);
                lo.SetPosition(new Vector3( 0, 16.0f, 0));
                GraphicPipeleine.ProceedObject(lo.renderaspect);
                gameScene.AddObject(lo);
                LevelObjectCharacterBox = lo;
                lo.useDeltaMatrix = true;
                lo.deltaMatrix = Matrix.CreateTranslation(new Vector3(0, 0, 0.1f));
                
                //хз как сделоать поудобнее TODO
                //if (lo.renderaspect.isanimated)
                {
                    Render.AnimRenderObject ro = lo.renderaspect as Render.AnimRenderObject;
                    AnimationManager.AnimationManager.Manager.AddAnimationUserEnd(ro.Update, ro.character);
                    ContentLoader.ContentLoader.boneToAdd = ro.character._baseCharacter.skeleton.WeaponIndex;
                    ContentLoader.ContentLoader.currentCharacter = ro.character;
                }
            }

            ////test side
            {
                var testsiderdescription = new LevelObjectDescription();
                testsiderdescription = packs.GetObject("TestSideWorldObject\0", testsiderdescription) as LevelObjectDescription;

                LevelObject lo = ContentLoader.ContentLoader.LevelObjectFromDescription(testsiderdescription, packs, Scene);
                lo.SetGlobalPose(Matrix.CreateFromAxisAngle(new Vector3(1, 0, 0), -MathHelper.PiOver2) * Matrix.CreateTranslation(0, 15, 0));
                GraphicPipeleine.ProceedObject(lo.renderaspect);
                gameScene.AddObject(lo);
                LevelObjectTestSide = lo;
            }

            /////sphere
            {
                var spheredesc = new LevelObjectDescription();
                spheredesc = packs.GetObject("Cursor\0", spheredesc) as LevelObjectDescription;

                LevelObject lo = ContentLoader.ContentLoader.LevelObjectFromDescription(spheredesc, packs, Scene);
                lo.SetGlobalPose(Matrix.CreateTranslation(0, 15, 0));
                GraphicPipeleine.ProceedObject(lo.renderaspect);
                gameScene.AddObject(lo);
                LevelObjectCursorSphere = lo;
            }

            //gun
            {
                var gundesc = new LevelObjectDescription();
                gundesc = packs.GetObject("SCGunLO\0", gundesc) as LevelObjectDescription;

                LevelObject lo = ContentLoader.ContentLoader.LevelObjectFromDescription(gundesc, packs, Scene);
                Logic.BehaviourModel.ObjectBoneRelatedBehaviourModel bm = lo.behaviourmodel as Logic.BehaviourModel.ObjectBoneRelatedBehaviourModel;
                //чем меньше тем выше задран нос пушки
                //хуз = вправо/влево, -верх/+низ
                lo.SetGlobalPose(Matrix.CreateRotationX(MathHelper.PiOver2 - 0.14f) * Matrix.CreateTranslation(new Vector3(-0.46f, -0.20f, -0.43f)));
                GraphicPipeleine.ProceedObject(lo.renderaspect);
                gameScene.AddObject(lo);
                //должен быть после чара
                gameScene._objects.AddRule(LevelObjectCharacterBox, lo);

                //memoriz
                LevelObjectSCGunLO = lo;
            }

            //gun addon
            {
                var gundesc = new LevelObjectDescription();
                gundesc = packs.GetObject("SСGunAddon\0", gundesc) as LevelObjectDescription;

                LevelObject lo = ContentLoader.ContentLoader.LevelObjectFromDescription(gundesc, packs, Scene);
                Logic.BehaviourModel.ObjectBoneRelatedBehaviourModel bm = lo.behaviourmodel as Logic.BehaviourModel.ObjectBoneRelatedBehaviourModel;
                //чем меньше тем выше задран нос пушки
                //хуз = вправо/влево, -верх/+низ
                lo.SetGlobalPose(Matrix.CreateRotationX(MathHelper.PiOver2 - 0.14f) * Matrix.CreateTranslation(new Vector3(-0.46f, -0.20f, -0.43f)));
                GraphicPipeleine.ProceedObject(lo.renderaspect);
                gameScene.AddObject(lo);
                //должен быть после чара
                gameScene._objects.AddRule(LevelObjectSCGunLO, lo);
            }

            //head
            {

                if (LevelObjectCharacterBox.renderaspect.isanimated)
                {
                    Render.AnimRenderObject ro = LevelObjectCharacterBox.renderaspect as Render.AnimRenderObject;
                    ContentLoader.ContentLoader.boneToAdd = ro.character._baseCharacter.skeleton.HeadIndex;
                }

                var headdesc = new LevelObjectDescription();
                headdesc = packs.GetObject("Head01\0", headdesc) as LevelObjectDescription;

                LevelObject lo = ContentLoader.ContentLoader.LevelObjectFromDescription(headdesc, packs, Scene);
                Logic.BehaviourModel.ObjectBoneRelatedBehaviourModel bm = lo.behaviourmodel as Logic.BehaviourModel.ObjectBoneRelatedBehaviourModel;
                //хуз = вправо/влево, ,-верх/+низ
                lo.SetGlobalPose(Matrix.CreateTranslation(new Vector3(-0.00f, -0.01f, 0.84f)));
                GraphicPipeleine.ProceedObject(lo.renderaspect);
                gameScene.AddObject(lo);
                //должен быть после чара
                gameScene._objects.AddRule(LevelObjectCharacterBox, lo);
            }

            //чистим временные какахи
            //это стоит делать елси объекты больше не будут подгружаться
            //тоесть если игра по уровням скажем
            packs.Unload();

            //подключаем камеру и управление от клавы/мыши к челобарику
            CreateCharCameraController();

        }


        private void CreateCharCameraController() {
            _cameraController = new CameraControllerPerson(Camera, LevelObjectCharacterBox, new Vector3(-10, 10, 0));
            _charcterController = new InputControllers.InputControllerPerson(LevelObjectCharacterBox);
        }





        public void Update(GameTime gameTime)
        {
            //updatelogic
            animationManager.UpdateStart(gameTime);

            //Begin update of level objects
            foreach (PivotObject lo in gameScene._objects)
                lo.BeginDoFrame();
           
            
            foreach (PivotObject lo in gameScene._objects)
                lo.DoFrame(gameTime);


            // Update Physics
            Scene.Simulate((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f);


            //update world objects
            Scene.FlushStream();
            Scene.FetchResults(SimulationStatus.RigidBodyFinished, true);


            //calculatin info from controllers
            _charcterController.Update(LevelObjectCursorSphere.transform.Translation, _cameraController._yAngle);


            //End updating world objects
            foreach (PivotObject lo in gameScene._objects)
                lo.EndDoFrame();


            //Update world(calc ray trace, deleting bullets, applying forces and other)
            //------
            

            //Udating data for scenegraph
            gameScene.UpdateScene();

            {
                // обработка камеры
                _cameraController.UpdateCamerafromUser(MyGame.Instance._mousepoint);

                if (LevelObjectCharacterBox.behaviourmodel.moved)
                    _cameraController.UpdateCamera();
            }

            //очищаем конвейер
            GraphicPipeleine.NewFrame(lightDir);

            gameScene.CalculateVisibleObjects();
            Vector3 v1 = DeviceManager.GraphicsDevice.Viewport.Project(gameScene._objects[0].transform.Translation, Camera.Projection, Camera.View, Matrix.Identity);
            BoxScreenPosition = new Vector3(Convert.ToSingle(Convert.ToInt32(v1.X)), Convert.ToSingle(Convert.ToInt32(v1.Y)), v1.Z);

            //добавляем все нобходимые объекты на отрисовку
            GraphicPipeleine.AddObjectToPipeline(gameScene._visibleObjects);
            GraphicPipeleine.AddObjectToShadow(gameScene._shadowObjects);

            visibleobjectscount = gameScene._visibleObjects.Count;
            animationManager.UpdateEnd(gameTime);
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
    }
}