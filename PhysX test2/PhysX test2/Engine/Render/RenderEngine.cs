using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2;
using PhysX_test2.Content;
using PhysX_test2.Engine;
using PhysX_test2.Engine.CameraControllers;
using PhysX_test2.Engine.Logic;
using PhysX_test2.Engine.ContentLoader;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysX_test2.Engine.Render
{
    public class RenderPipeline
    {
        public static bool EnableShadows;
        public static bool SmoothShadows;
        public static bool EnableGrass;
        public static bool EnableDebugRender;


        private Dictionary<string, RenderArray> ArraysPerTehnique = new Dictionary<string, RenderArray>();


        private List<string> arrays;
        private RenderTarget2D shadowRenderTarget;
        private int shadowMapWidthHeight = 2048;
        private GraphicsDevice Device;
        private BasicEffect _visualizationEffect;

        /// <summary>
        /// added
        /// </summary>
        private SpriteBatch sprite;

        public Matrix lightViewProjection;
        public BoundingFrustum frustumForShadow;

        private DebugRender.DebugRenderer debugRenderer;
        private MyContainer<PivotObject> debugRenderArray;
        private Camera Camera;

        public RenderPipeline(GraphicsDevice dev, Camera c)
        {
            Device = dev;
            Camera = c;
            _visualizationEffect = new BasicEffect(this.Device)
            {
                VertexColorEnabled = true
            };
            /// <summary>
            /// added
            /// </summary>
            sprite = new SpriteBatch(this.Device);



            frustumForShadow = new BoundingFrustum(Matrix.Identity);
            debugRenderer = new DebugRender.DebugRenderer(Device, _visualizationEffect);
            debugRenderArray = new MyContainer<PivotObject>(10, 3);
            EnableShadows= EnableGrass = true;
           EnableDebugRender = true;
            //EnableShadows = false;
            //SmoothShadows = false;
            if (EnableShadows)
            {
                if (dev.GraphicsProfile == GraphicsProfile.HiDef)
                {
                    shadowRenderTarget = new RenderTarget2D(Device,
                                                           shadowMapWidthHeight,
                                                           shadowMapWidthHeight,
                                                           false,
                                                           SurfaceFormat.Single,
                                                           DepthFormat.Depth16);
                }
                else
                {
                    shadowRenderTarget = new RenderTarget2D(Device,
                                                           shadowMapWidthHeight,
                                                           shadowMapWidthHeight,
                                                           false,
                                                           SurfaceFormat.Color,
                                                           DepthFormat.None);
                    SmoothShadows = false;
                }
            }
            arrays = new List<string>();
            arrays.Add(Shader.AnimRenderNoSM);
            arrays.Add(Shader.NotAnimRenderNoSM);
            arrays.Add(Shader.AnimRenderSM);
            arrays.Add(Shader.NotAnimRenderSM);
  
            arrays.Add(Shader.CreateStaticShadowMap);
            arrays.Add(Shader.CreateAnimShadowMap);


            
            ArraysPerTehnique.Add(Shader.AnimRenderNoSM, new RenderArray(Shader.AnimRenderNoSM));
            ArraysPerTehnique.Add(Shader.NotAnimRenderNoSM, new RenderArray(Shader.NotAnimRenderNoSM));

            ArraysPerTehnique.Add(Shader.AnimRenderSM, new RenderArray(Shader.AnimRenderSM));
            ArraysPerTehnique.Add(Shader.NotAnimRenderSM, new RenderArray(Shader.NotAnimRenderSM));

            if (dev.GraphicsProfile == GraphicsProfile.HiDef)
            {
                ArraysPerTehnique.Add(Shader.AnimRenderSMSmooth, new RenderArray(Shader.AnimRenderSMSmooth));
                ArraysPerTehnique.Add(Shader.NotAnimRenderSMSmooth, new RenderArray(Shader.NotAnimRenderSMSmooth));
                arrays.Add(Shader.AnimRenderSMSmooth);
                arrays.Add(Shader.NotAnimRenderSMSmooth);
            }
            
            ArraysPerTehnique.Add(Shader.CreateStaticShadowMap, new RenderArray(Shader.CreateStaticShadowMap));
            ArraysPerTehnique.Add(Shader.CreateAnimShadowMap, new RenderArray(Shader.CreateAnimShadowMap));
        }
        int minim=100;
        public void NewParameters(bool _EnableShadows, bool _SmoothShadows, bool _EnableGrass)
        {

        }

        public void ProceedObject(RenderObject AddedObject)
        {
            //тут решаем что за рыба и как её соотв рендерить.

            if (AddedObject.isanimated)
            {
                if (AddedObject.isshadowcaster && EnableShadows)
                    AddedObject.ShadowTehnique = Shader.CreateAnimShadowMap;


                if (AddedObject.isshadowreceiver)
                    if (EnableShadows)
                        if (SmoothShadows)
                            AddedObject.PictureTehnique = Shader.AnimRenderSMSmooth;
                        else
                            AddedObject.PictureTehnique = Shader.AnimRenderSM;
                    else
                        AddedObject.PictureTehnique = Shader.AnimRenderNoSM;
            }
            else
            {
                if (AddedObject.isshadowcaster && EnableShadows)
                    AddedObject.ShadowTehnique = Shader.CreateStaticShadowMap;


                if (AddedObject.isshadowreceiver)
                    if (EnableShadows)
                        if (SmoothShadows)
                            AddedObject.PictureTehnique = Shader.NotAnimRenderSMSmooth;
                        else
                            AddedObject.PictureTehnique = Shader.NotAnimRenderSM;
                    else
                        AddedObject.PictureTehnique = Shader.NotAnimRenderNoSM;
                else
                    AddedObject.PictureTehnique = Shader.NotAnimRenderNoSM;

            }

        }

        public void NewFrame(Vector3 lightDir)
        {
            for (int i = 0; i < arrays.Count; i++)
                ArraysPerTehnique[arrays[i]].Objects.Clear();
            debugRenderArray.Clear();
            lightViewProjection = CreateLightViewProjectionMatrix(lightDir);
        }
        public void AddObjectToPipeline(MyContainer<PivotObject> AddedObjects)
        {
            foreach (PivotObject AddedObject in AddedObjects)
                ArraysPerTehnique[AddedObject.HaveRenderAspect().PictureTehnique].Objects.Add(AddedObject);
            foreach (PivotObject AddedObject in AddedObjects)
                debugRenderArray.Add(AddedObject);
        }
        public void AddObjectToShadow(MyContainer<PivotObject> AddedObjects)
        {
            if (AddedObjects.Count < minim)
                minim = AddedObjects.Count;
            foreach (PivotObject AddedObject in AddedObjects)
            {
                RenderObject ro = AddedObject.HaveRenderAspect();
                if (ro != null && ro.ShadowTehnique != null)
                    ArraysPerTehnique[ro.ShadowTehnique].Objects.Add(AddedObject);
            }

        }
        private Matrix CreateLightViewProjectionMatrix(Vector3 lightDir)
        {
            // Matrix with that will rotate in points the direction of the light
            Matrix lightRotation = Matrix.CreateLookAt(Vector3.Zero,
                                                       -lightDir,
                                                       Vector3.Up);

            // Get the corners of the frustum
            Vector3[] frustumCorners = Camera.cameraFrustum.GetCorners();

            // Transform the positions of the corners into the direction of the light
            for (int i = 0; i < frustumCorners.Length; i++)
            {
                frustumCorners[i] = Vector3.Transform(frustumCorners[i], lightRotation);
            }

            // Find the smallest box around the points
            BoundingBox lightBox = BoundingBox.CreateFromPoints(frustumCorners);

            Vector3 boxSize = lightBox.Max - lightBox.Min;
            Vector3 halfBoxSize = boxSize * 0.5f;

            // The position of the light should be in the center of the back
            // pannel of the box. 
            Vector3 lightPosition = lightBox.Min + halfBoxSize;
            lightPosition.Z = lightBox.Min.Z;

            // We need the position back in world coordinates so we transform 
            // the light position by the inverse of the lights rotation
            lightPosition = Vector3.Transform(lightPosition,
                                              Matrix.Invert(lightRotation));

            // Create the view matrix for the light
            Matrix lightView = Matrix.CreateLookAt(lightPosition,
                                                   lightPosition + lightDir,
                                                   Vector3.Up);

            // Create the projection matrix for the light
            // The projection is orthographic since we are using a directional light
            Matrix lightProjection = Matrix.CreateOrthographic(boxSize.X, boxSize.Y,
                                                               -boxSize.Z, boxSize.Z);
            Matrix final = lightView * lightProjection;
            frustumForShadow = new BoundingFrustum(final);
            return final;
        }

        private void RenderToShadowMap(Matrix lightViewProjection, Vector3 lightDir)
        {
            Device.SetRenderTarget(shadowRenderTarget);
            Device.Clear(Color.White);


            Render.Materials.Material.ObjectRenderEffect.Parameters["LightDirection"].SetValue(lightDir);
            Render.Materials.Material.ObjectRenderEffect.Parameters["LightViewProj"].SetValue(lightViewProjection);


            MyContainer<PivotObject> Objects = ArraysPerTehnique[Shader.CreateStaticShadowMap].Objects;
            if (!Objects.IsEmpty)
            {
                Materials.Material.ObjectRenderEffect.CurrentTechnique = Materials.Material.ObjectRenderEffect.Techniques[Shader.CreateStaticShadowMap];
                Render.Materials.Material.ObjectRenderEffect.Parameters["World"].SetValue(Matrix.Identity);
                Materials.Material.ObjectRenderEffect.CurrentTechnique.Passes[0].Apply();


                foreach (PivotObject wo in Objects)
                {
                    Render.Materials.Material.ObjectRenderEffect.Parameters["World"].SetValue(wo.renderMatrix);
                    wo.HaveRenderAspect().SelfRender(2, wo.HaveMaterial());
                }
            }
            MyContainer<PivotObject> ObjectsA = ArraysPerTehnique[Shader.CreateAnimShadowMap].Objects;
            if (!Objects.IsEmpty)
            {
                Materials.Material.ObjectRenderEffect.CurrentTechnique = Materials.Material.ObjectRenderEffect.Techniques[Shader.CreateAnimShadowMap];
                foreach (PivotObject wo in ObjectsA)
                {
                    Render.Materials.Material.ObjectRenderEffect.Parameters["World"].SetValue(wo.renderMatrix);
                    wo.HaveRenderAspect().SelfRender(2, wo.HaveMaterial());
                }
            }

            Device.SetRenderTarget(null);
        }
        
        public void RenderToPicture(Camera Camera, Vector3 lightDir)
        {
            
            Device.RasterizerState = RasterizerState.CullClockwise;
            Device.DepthStencilState = DepthStencilState.Default;
            Device.BlendState = BlendState.Opaque;

            Device.SamplerStates[0] = SamplerState.LinearWrap;
            Device.SamplerStates[1] = SamplerState.PointWrap;

            if (EnableShadows)
            {
                RenderToShadowMap(lightViewProjection, lightDir);
                Render.Materials.Material.ObjectRenderEffect.Parameters["ShadowMap"].SetValue(shadowRenderTarget);
            }

            this.Device.Clear(Color.CornflowerBlue);


            PhysX_test2.Engine.Render.Materials.Material.ObjectRenderEffect.Parameters["Projection"].SetValue(Camera.Projection);
            PhysX_test2.Engine.Render.Materials.Material.ObjectRenderEffect.Parameters["View"].SetValue(Camera.View);
            string AnimTeh = "", NotAnimTeh = "";
            if (EnableShadows)
            {
                if (SmoothShadows)
                {
                    AnimTeh = Shader.AnimRenderSMSmooth;
                    NotAnimTeh = Shader.NotAnimRenderSMSmooth;
                }
                else
                {
                    AnimTeh = Shader.AnimRenderSM;
                    NotAnimTeh = Shader.NotAnimRenderSM;
                }
            }
            else
            {
                AnimTeh = Shader.AnimRenderNoSM;
                NotAnimTeh = Shader.NotAnimRenderNoSM;
            }


            MyContainer<PivotObject> Objects = ArraysPerTehnique[AnimTeh].Objects;
            if (!Objects.IsEmpty)
            {
                Materials.Material.ObjectRenderEffect.CurrentTechnique = Materials.Material.ObjectRenderEffect.Techniques[AnimTeh];
                foreach (PivotObject wo in Objects)
                {
                    Render.Materials.Material.ObjectRenderEffect.Parameters["World"].SetValue(wo.renderMatrix);
                    wo.HaveRenderAspect().SelfRender(0, wo.HaveMaterial());
                }
            }


            Objects = ArraysPerTehnique[NotAnimTeh].Objects;
            if (!Objects.IsEmpty)
            {
                Materials.Material.ObjectRenderEffect.CurrentTechnique = Materials.Material.ObjectRenderEffect.Techniques[NotAnimTeh];
                foreach (PivotObject wo in Objects)
                {
                    Render.Materials.Material.ObjectRenderEffect.Parameters["World"].SetValue(wo.renderMatrix);
                    wo.HaveRenderAspect().SelfRender(0, wo.HaveMaterial());
                }
            }

            //////////
            if (EnableShadows)
            {
                Objects = ArraysPerTehnique[Shader.AnimRenderNoSM].Objects;
                if (!Objects.IsEmpty)
                {
                    Materials.Material.ObjectRenderEffect.CurrentTechnique = Materials.Material.ObjectRenderEffect.Techniques[Shader.AnimRenderNoSM];
                    foreach (PivotObject wo in Objects)
                    {
                        Render.Materials.Material.ObjectRenderEffect.Parameters["World"].SetValue(wo.renderMatrix);
                        wo.HaveRenderAspect().SelfRender(0, wo.HaveMaterial());
                    }
                }


                Objects = ArraysPerTehnique[Shader.NotAnimRenderNoSM].Objects;
                if (!Objects.IsEmpty)
                {
                    Materials.Material.ObjectRenderEffect.CurrentTechnique = Materials.Material.ObjectRenderEffect.Techniques[Shader.NotAnimRenderNoSM];
                    foreach (PivotObject wo in Objects)
                    {
                        Render.Materials.Material.ObjectRenderEffect.Parameters["World"].SetValue(wo.renderMatrix);
                        wo.HaveRenderAspect().SelfRender(0, wo.HaveMaterial());
                    }
                }
            }


            if (EnableDebugRender)
            {
                //ДЕБАГ РЕНДЕР

                _visualizationEffect.World = Matrix.Identity;
                _visualizationEffect.View = Camera.View;
                _visualizationEffect.Projection = Camera.Projection;
                _visualizationEffect.CurrentTechnique.Passes[0].Apply();
                foreach (PivotObject wo in debugRenderArray)
                {
                   // debugRenderer.RenderTransformedBB(wo.boundingShape);
                   // debugRenderer.RenderAABR(wo.boundingShape);
                    debugRenderer.RenderAABB(wo.raycastaspect.boundingShape);
                }

                /*sprite.Begin( SpriteSortMode.FrontToBack, new BlendState());
                sprite.Draw(shadowRenderTarget, new Rectangle(0, 0, 128, 128), Color.Wheat);
                sprite.End();*/
            }
        }
    }
}
