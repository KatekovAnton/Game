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


namespace PhysX_test2.Engine 
{
    public class GameEngine
    {

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
  //      public Core Core;
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
       

        public GameEngine(MyGame game)
        {
            packs = new PackList();
            Instance = this;
            lightDir.Normalize();

            DeviceManager = new GraphicsDeviceManager(game);

            

            //разме рэкрана
            DeviceManager.PreferredBackBufferWidth = (int) (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.8);
            DeviceManager.PreferredBackBufferHeight = (int) (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.8);
        }


        public void Initalize() 
        {
            FPSCounter = new FpsCounter();
            Camera = new Camera(this);
            spriteBatch = new SpriteBatch(DeviceManager.GraphicsDevice);

            
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

        public static PivotObject loadObject(string __name,
            Matrix? __deltaMatrix,
            bool __needMouseCast, 
            PivotObject __parentObject = null,
            PivotObjectDependType __dependType = PivotObjectDependType.Body)
        {
            var boxcharacterdescription = new LevelObjectDescription();
            boxcharacterdescription = PackList.Instance .GetObject(__name, boxcharacterdescription) as LevelObjectDescription;
            Matrix position = Matrix.Identity;
            bool needSetPosition = false;
            if (__parentObject != null)
            {
                //we need to create dependeces
                switch (boxcharacterdescription.BehaviourType)
                {
                    case LevelObjectDescription.objectBonerelatedbehaviourmodel:
                        {
                            LevelObject lo = __parentObject as LevelObject;
                            if (lo == null)
                                throw new Exception();
                            Render.AnimRenderObject ro = lo.renderaspect as Render.AnimRenderObject;
                            if (ro == null)
                                throw new Exception();

                            //TODO
                            //все сломается когда заменится объект при смерте
                            //будет депедндится на живой объект
                            //надо катко тут все поменять
                            ContentLoader.ContentLoader.currentParentObject = __parentObject;

                            
                            switch (__dependType)
                            {
                                case PivotObjectDependType.Head:
                                    {
                                        needSetPosition = true;
                                        position = ro.character._baseCharacter.skeleton.HeadMatrix;
                                        ContentLoader.ContentLoader.boneToAdd = ro.character._baseCharacter.skeleton.HeadIndex;
                                    } break;
                                case PivotObjectDependType.Weapon:
                                    {
                                        needSetPosition = true;
                                        position = ro.character._baseCharacter.skeleton.WeaponMatrix;
                                        ContentLoader.ContentLoader.boneToAdd = ro.character._baseCharacter.skeleton.WeaponIndex;
                                    } break;
                                default: break;
                            }
                        } break;
                    case LevelObjectDescription.objectrelatedbehaviourmodel:
                        { 
                            if(__dependType != PivotObjectDependType.Body)
                                throw new Exception();
                        }break;
                    default: break;
                }
            }
            LevelObject loNew = ContentLoader.ContentLoader.LevelObjectFromDescription(boxcharacterdescription, PackList.Instance, GameEngine.Instance.Scene);
            GameEngine.Instance.GraphicPipeleine.ProceedObject(loNew.renderaspect);

            loNew.useDeltaMatrix = __deltaMatrix != null && __deltaMatrix.HasValue;
            if (loNew.useDeltaMatrix)
                loNew.deltaMatrix = __deltaMatrix.Value;
            loNew._needMouseCast = __needMouseCast;

            //хз как сделоать поудобнее TODO
            if (loNew.renderaspect.isanimated)
            {
                Render.AnimRenderObject ro = loNew.renderaspect as Render.AnimRenderObject;
                AnimationManager.AnimationManager.Manager.AddAnimationUserEnd(ro.Update, ro.character);
            }
            if (needSetPosition)
                loNew.SetGlobalPose(position);
            return loNew;
        }

        private void Loaddata()
        {
            //уровень
            gameScene = new EngineScene();
            Scene = gameScene.Scene;
            //если ты с горы упал - ты ешё не экстримал
            //чтоб далеко не падать
            groundplane = CreateGroundPlane();
            

            ///box 
            {
                LevelObjectBox = loadObject("WoodenCrate10WorldObject\0", null, true) as LevelObject;
                LevelObjectBox.SetGlobalPose(Matrix.CreateRotationX(1.0f) * Matrix.CreateTranslation(0, 25, 0));
                gameScene.AddObject(LevelObjectBox);
            }

            float delta = 4;
            int x = 5;
            int y = 5;

            for (int i = 0; i < x;i++ )
                for (int j = 0; j < y; j++)
                {
                    LevelObject lo = loadObject("WoodenCrate10WorldObject\0", null, true) as LevelObject;
                    lo.SetGlobalPose(Matrix.CreateRotationX(1.0f) * Matrix.CreateTranslation(i * delta, 25, j * delta));
                    gameScene.AddObject(lo);
                }


            ////our character
            {
                LevelObjectCharacterBox = loadObject("MyNewCharacter\0", Matrix.CreateTranslation(new Vector3(0, 0, 0.1f)), true) as LevelObject;
                LevelObjectCharacterBox.SetPosition(new Vector3(0, 16.0f, 0));
                gameScene.AddObject(LevelObjectCharacterBox);

                //хз как сделоать поудобнее TODO
                //if (lo.renderaspect.isanimated)
                {
                    Render.AnimRenderObject ro = LevelObjectCharacterBox.renderaspect as Render.AnimRenderObject;
                    ContentLoader.ContentLoader.boneToAdd = ro.character._baseCharacter.skeleton.WeaponIndex;
                    ContentLoader.ContentLoader.currentParentObject = LevelObjectCharacterBox;
                }
            }

            ////test side
            {
                LevelObjectTestSide = loadObject("TestSideWorldObject\0", null, true) as LevelObject;
                LevelObjectTestSide.SetGlobalPose(Matrix.CreateFromAxisAngle(new Vector3(1, 0, 0), -MathHelper.PiOver2) * Matrix.CreateTranslation(0, 15, 0));
                gameScene.AddObject(LevelObjectTestSide);
            }

            /////sphere
            {
                LevelObjectCursorSphere = loadObject("Cursor\0", null, true) as LevelObject;
                gameScene.AddObject(LevelObjectCursorSphere);
            }

            //gun
            {
                LevelObjectSCGunLO = loadObject("SCGunLO\0", null, false, LevelObjectCharacterBox, PivotObjectDependType.Weapon) as LevelObject;
                gameScene.AddObject(LevelObjectSCGunLO);
                //должен быть после чара
                gameScene._objects.AddRule(LevelObjectCharacterBox, LevelObjectSCGunLO);
            }

            //gun addon
            {
                LevelObject SCGunLO = loadObject("SСGunAddon\0", null, false, LevelObjectCharacterBox, PivotObjectDependType.Weapon) as LevelObject;
                gameScene.AddObject(SCGunLO);
                //должен быть после чара
                gameScene._objects.AddRule(LevelObjectSCGunLO, SCGunLO);
            }

            //head
            {
                LevelObject head = loadObject("Head01\0", null, false, LevelObjectCharacterBox, PivotObjectDependType.Head) as LevelObject;
                gameScene.AddObject(head);
                //должен быть после чара
                gameScene._objects.AddRule(LevelObjectSCGunLO, head);
            }

            //чистим временные какахи
            //это стоит делать елси объекты больше не будут подгружаться
            //тоесть если игра по уровням скажем
            packs.Unload();

            //подключаем камеру и управление от клавы/мыши к челобарику
            CreateCharCameraController();

        }


        private void CreateCharCameraController()
        {
            _cameraController = new CameraControllerPerson(Camera, LevelObjectCharacterBox, new Vector3(-10, 10, 0));
            _charcterController = new InputControllers.InputControllerPerson(LevelObjectCharacterBox);
        }




        bool frst = false;
        bool enabeld = true;
        public void Update(GameTime gameTime)
        {
            MouseState stet = Mouse.GetState();
            if (stet.LeftButton == ButtonState.Pressed)
            {
                if (!frst)
                {
                    frst = true;
                    if (enabeld)
                        LevelObjectBox.behaviourmodel.Disable();
                    else
                        LevelObjectBox.behaviourmodel.Enable();

                    enabeld = !enabeld;
                }
            }
            else if (stet.LeftButton == ButtonState.Released)
                frst = false;

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
            gameScene.Core.Dispose();
            gameScene.Scene.Dispose();
            //  BoxActor.Dispose();
            groundplane.Dispose();
        }
    }
}