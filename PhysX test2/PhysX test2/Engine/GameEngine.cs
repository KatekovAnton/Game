using System;
using System.IO;
using System.Collections.Generic;

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
    public class GameEngine : IKeyboardUser
    {

        //static variables for enviroment
        public static GameEngine Instance;
        public bool AllKeys { get { return false; } }


        //engine camera and engine render
        public Camera Camera;
        public RenderPipeline GraphicPipeleine;

        
        //fps
        public FpsCounter FPSCounter;


        //screen log
        public ScreenLog screenLog;

        //for text rendering
        public SpriteBatch spriteBatch;


        //content
        private PackList packs;
        private ContentNew.PackList newPacks;


        //physx
        public Scene Scene;


        //cashe for small objects
        public ObjectCashe _cashe;


        //scene and its objects
        public EngineScene gameScene;
        public LevelObject LevelObjectBox;
        public LevelObject LevelObjectCursorSphere;
        public LevelObject LevelObjectTestSide;
        public Actor groundplane;


        //controllers for camera and character (objects between user and game)
        public CameraControllers.CameraControllerPerson _cameraController;


        //animation
        public Engine.AnimationManager.AnimationManager animationManager;


        //some additional variables
        public int visibleobjectscount;
        public Vector3 BoxScreenPosition;
        public Vector3 lightDir = new Vector3(-1, -1, -1);


        private List<HotKey> _hotkeys;
        public bool cameraneedsupdate = true;
        public bool GlobalUser { set { } get { return false; } }

        

        public GameEngine(MyGame game)
        {
            screenLog = new ScreenLog();
            _hotkeys = new List<HotKey>();
            _hotkeys.Add(new HotKey(new Keys[] { Keys.O }, SwichDebugRender));
            _hotkeys.Add(new HotKey(new Keys[] { Keys.P }, SwichBehaviourModel));

            KeyboardManager.Manager.AddKeyboardUser(this);

            game._engine = this;
            packs = new PackList();
         //   newPacks = new ContentNew.PackList();
            Instance = this;
            lightDir.Normalize();

            gameScene = new EngineScene();
            Scene = gameScene.Scene;
            

            //разме рэкрана
            MyGame.DeviceManager.PreferredBackBufferWidth = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width *0.8 );
            MyGame.DeviceManager.PreferredBackBufferHeight = (int) (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height*0.8);
            GameConfiguration.ScreenResolution = new Vector2(MyGame.DeviceManager.PreferredBackBufferWidth, MyGame.DeviceManager.PreferredBackBufferHeight);

            MyGame.DeviceManager.IsFullScreen = false;
            
            _cashe = new ObjectCashe();
        }

        public PhysX_test2.TheGame.InputManagers.CharacterMoveState playerState = TheGame.InputManagers.CharacterMoveState.Stay;

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

        public void Initalize() 
        {
            FPSCounter = new FpsCounter();
            Camera = new Camera(this);
            spriteBatch = new SpriteBatch(MyGame.DeviceManager.GraphicsDevice);
            
            //аниматор
            animationManager = AnimationManager.AnimationManager.Manager;

            //шойдер
           /* using (var stream = new FileStream(@"Content\Shaders\ObjectRender.fx", FileMode.Open))
            {
                PhysX_test2.Engine.Render.Materials.Material.ObjectRenderEffect = Shader.FromStream(stream, MyGame.Device);
            }
            */
            PhysX_test2.Engine.Render.Materials.Material.ObjectRenderEffect = Shader.Load(MyGame.Instance.Content);

            //рендерщик
            GraphicPipeleine = new RenderPipeline(MyGame.DeviceManager.GraphicsDevice, Camera);


        }

        public void AfterLoading()
        {
            // обработка камеры
            _cameraController.UpdateCamerafromUser(MyGame.Instance._mousepoint);
            _cameraController.UpdateCamera();
          
        }

        public void CasheObject(string __name,
            Matrix? __deltaMatrix,
            bool __needMouseCast,
            bool __needBulletCast, 
            PivotObject __parentObject = null,
            PivotObjectDependType __dependType = PivotObjectDependType.Body)
        {
            _cashe.CasheObject(__name, __deltaMatrix, __needMouseCast, __needBulletCast, __parentObject, __dependType);
        }

        public static PivotObject LoadObject(string __name,
            Matrix? __deltaMatrix,
            bool __needMouseCast,
            bool __needBulletCast,
            PivotObjectDependType __dependType = PivotObjectDependType.Body)
        {
            var description = new LevelObjectDescription();
            description = PackList.Instance.GetObject(__name, description) as LevelObjectDescription;

            LevelObject loNew = ContentLoader.ContentLoader.LevelObjectFromDescription(description, PackList.Instance, GameEngine.Instance.Scene, __dependType);
            GameEngine.Instance.GraphicPipeleine.ProceedObject(loNew.renderaspect);

            loNew.useDeltaMatrix = __deltaMatrix != null && __deltaMatrix.HasValue;
            if (loNew.useDeltaMatrix)
                loNew.deltaMatrix = __deltaMatrix.Value;
            loNew._needMouseCast = __needMouseCast;
            loNew._needBulletCast = __needBulletCast;
         
            loNew.Update();
            return loNew;
        }

        public void Loaddata()
        {
            Scene = gameScene.Scene;
            //если ты с горы упал - ты ешё не экстримал
            //чтоб далеко не падать
            groundplane = CreateGroundPlane();
            

            ///box 
            {
                LevelObjectBox = LoadObject("WoodenCrate10WorldObject\0", null, true, true) as LevelObject;
                LevelObjectBox.matrialType = PivotObjectMaterialType.DynamicWood;
                LevelObjectBox.SetGlobalPose(Matrix.CreateRotationX(1.0f) * Matrix.CreateTranslation(0, 25, 0));
                AddObjectToScene(LevelObjectBox);
            }

            float delta = 4;
            int x = 5;
            int y = 5;

            for (int i = 0; i < x;i++ )
                for (int j = 0; j < y; j++)
                {
                    LevelObject lo = LoadObject("WoodenCrate10WorldObject\0", null, true, true) as LevelObject;
                    lo.matrialType = PivotObjectMaterialType.DynamicWood;
                    lo.SetGlobalPose(Matrix.CreateRotationX(1.0f) * Matrix.CreateTranslation(i * delta, 25, j * delta));
                    AddObjectToScene(lo);
                }

           

            ////test side
            {
                LevelObjectTestSide = LoadObject("TestSideWorldObject\0", null, true, true) as LevelObject;
                LevelObjectTestSide.SetGlobalPose(Matrix.CreateFromAxisAngle(new Vector3(1, 0, 0), -MathHelper.PiOver2) * Matrix.CreateTranslation(0, 15, 0));
                AddObjectToScene(LevelObjectTestSide);
            }

            //чистим временные какахи
            //это стоит делать елси объекты больше не будут подгружаться
            //тоесть если игра по уровням скажем
           // packs.Unload();
        }

        private void AddObjectToScene(PivotObject __object, PivotObject __parentObject = null)
        {
            LevelObject loNew = __object as LevelObject;
            if (loNew == null)
                return;

            if (loNew.renderaspect.isanimated)
            {
                Render.AnimRenderObject ro = loNew.renderaspect as Render.AnimRenderObject;
                AnimationManager.AnimationManager.Manager.AddAnimationUserEnd(ro.Update, ro.character);
            }

            gameScene.AddObject(__object);

            if (__parentObject != null)
            {
                __object.behaviourmodel.SetParentObject(__parentObject);
                gameScene._objects.AddRule(__parentObject, __object);
            }
        }

        public void RemoveObjectFromScene(PivotObject __object)
        {
            LevelObject loNew = __object as LevelObject;
            if (loNew == null)
                return;

            if (loNew.renderaspect.isanimated)
            {
                Render.AnimRenderObject ro = loNew.renderaspect as Render.AnimRenderObject;
                AnimationManager.AnimationManager.Manager.RemoveUser(ro.character);
            }

            gameScene.RemoveObject(__object);
        }

        public void CreateCharCameraController(LevelObject __targetCharacter)
        {
            _cameraController = new CameraControllerPerson(Camera, __targetCharacter, new Vector3(-10, 10, 0));
            cameraneedsupdate = true;
        }

        public void SwichBehaviourModel()
        {
            if (enabeld)
                LevelObjectBox.behaviourmodel.Disable();
            else
                LevelObjectBox.behaviourmodel.Enable();
            enabeld = !enabeld;
        }

        public void SwichDebugRender()
        {
            RenderPipeline.EnableDebugRender = !RenderPipeline.EnableDebugRender;
        }
        
        bool enabeld = true;
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
            Scene.FlushStream();
            Scene.FetchResults(SimulationStatus.RigidBodyFinished, true);


            //End updating world objects
            foreach (PivotObject lo in gameScene._objects)
                lo.EndDoFrame();


            //Udating data for scenegraph
            gameScene.UpdateScene();
            SearchClickedObject();
          //  if (MouseManager.Manager.moved || cameraneedsupdate || MouseManager.Manager.scrollWheelDelta != 0)
           // {
                cameraneedsupdate = false;
                // обработка камеры
                _cameraController.UpdateCamerafromUser(MyGame.Instance._mousepoint);
                _cameraController.UpdateCamera();
           // }

            
        }

        public void SearchClickedObject()
        {
            //ищем объект на кот тыкнули
            Engine.Logic.PivotObject clickedlo = null;
            Vector3 newpoint = MyGame.Instance._mousepoint;
            float distance = 10000;

            Vector3 camerapos = MyGame.Instance. ray.Position;
            foreach (Engine.Logic.PivotObject lo in this.gameScene._visibleObjects)
            {
                if (!lo._needMouseCast)
                    continue;
                Vector3? point = lo.raycastaspect.IntersectionClosest(MyGame.Instance.ray, lo.transform, ref MyGame.Instance._mousepointNormal);
                if (point != null)
                {
                    float range = (point.Value - camerapos).Length();
                    if (range < distance)
                    {
                        Vector3 direction = MyGame.Instance.ray.Direction;
                        direction.Normalize();
                        clickedlo = lo;
                        distance = range;
                        newpoint = point.Value;
                        newpoint += (direction * 0.3f);
                    }
                }
            }
            MyGame.Instance._mousepoint = newpoint;
        }

        public void UpdateEnd(GameTime gameTime)
        {
            //очищаем конвейер
            GraphicPipeleine.NewFrame(lightDir);

            gameScene.CalculateVisibleObjects();
            gameScene.AfterUpdate();

            Vector3 v1 = MyGame.DeviceManager.GraphicsDevice.Viewport.Project(LevelObjectBox.transform.Translation, Camera.Projection, Camera.View, Matrix.Identity);
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


        public void UnLoad() 
        {
            _cashe.ClearCashe();
            gameScene.Core.Dispose();
            gameScene.Scene.Dispose();
            //  BoxActor.Dispose();
            groundplane.Dispose();
        }
    }
}