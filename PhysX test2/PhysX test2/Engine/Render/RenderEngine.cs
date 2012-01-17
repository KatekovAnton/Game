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

    public enum InstancingTechnique
    {
        HardwareInstancing,
        NoInstancing,
        NoInstancingOrStateBatching
    }


    public class RenderPipeline
    {
        
        public static InstancingTechnique instancingTechnique = InstancingTechnique.HardwareInstancing;

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
            EnableShadows = EnableGrass = SmoothShadows = true;
            EnableDebugRender = false;
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
                    if (GraphicsAdapter.DefaultAdapter.IsProfileSupported(GraphicsProfile.HiDef))
                    {
                        shadowRenderTarget = new RenderTarget2D(Device,
                                                               shadowMapWidthHeight,
                                                               shadowMapWidthHeight,
                                                               false,
                                                               SurfaceFormat.Color,
                                                               DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
                    }
                    else
                    {
                        //TODO to cofig
                        shadowRenderTarget = new RenderTarget2D(Device,
                                                                                     dev.PresentationParameters.BackBufferWidth,
                                                                                      dev.PresentationParameters.BackBufferHeight,
                                                                                     false,
                                                                                     SurfaceFormat.Color,
                                                                                     DepthFormat.None);
                    }
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



            ArraysPerTehnique.Add(Shader.AnimRenderNoSM, new RenderArray(   Shader.AnimRenderNoSM));
            ArraysPerTehnique.Add(Shader.NotAnimRenderNoSM, new RenderArray(Shader.NotAnimRenderNoSM));

            ArraysPerTehnique.Add(Shader.AnimRenderSM, new RenderArray(     Shader.AnimRenderSM));
            ArraysPerTehnique.Add(Shader.NotAnimRenderSM, new RenderArray(  Shader.NotAnimRenderSM));

            if (dev.GraphicsProfile == GraphicsProfile.HiDef)
            {
                ArraysPerTehnique.Add(Shader.AnimRenderSMSmooth, new RenderArray(   Shader.AnimRenderSMSmooth));
                ArraysPerTehnique.Add(Shader.NotAnimRenderSMSmooth, new RenderArray(Shader.NotAnimRenderSMSmooth));
                arrays.Add(Shader.AnimRenderSMSmooth);
                arrays.Add(Shader.NotAnimRenderSMSmooth);

                ArraysPerTehnique.Add(Shader.TransparentSMSmooth, new RenderArray(Shader.TransparentSMSmooth));
                arrays.Add(Shader.TransparentSMSmooth);
            }

            arrays.Add(Shader.TransparentSM);
            arrays.Add(Shader.TransparentSelfIlmnNoSM);
            arrays.Add(Shader.TransparentNoSM);

            ArraysPerTehnique.Add(Shader.TransparentSM, new RenderArray(Shader.TransparentSM));
            ArraysPerTehnique.Add(Shader.TransparentSelfIlmnNoSM, new RenderArray(Shader.TransparentSelfIlmnNoSM));
            ArraysPerTehnique.Add(Shader.TransparentNoSM, new RenderArray(Shader.TransparentNoSM));

            ArraysPerTehnique.Add(Shader.CreateStaticShadowMap, new RenderArray(Shader.CreateStaticShadowMap));
            ArraysPerTehnique.Add(Shader.CreateAnimShadowMap, new RenderArray(Shader.CreateAnimShadowMap));
        }

        public void NewParameters(bool _EnableShadows, bool _SmoothShadows, bool _EnableGrass)
        {

        }

        public void ProceedObject(RenderObject AddedObject)
        {
            //тут решаем что за рыба и как её соотв рендерить.
            if (AddedObject.isTransparent)
            {
                //не мб шадовкастером и не мб анимированным
                if (AddedObject.isshadowreceiver)
                    if (EnableShadows)
                        if (SmoothShadows)
                            AddedObject.PictureTehnique = AddedObject.isSelfIllumination ? Shader.TransparentSelfIlmnNoSM : Shader.TransparentSMSmooth;
                        else
                            AddedObject.PictureTehnique = AddedObject.isSelfIllumination ? Shader.TransparentSelfIlmnNoSM : Shader.TransparentSM;
                    else
                        AddedObject.PictureTehnique = AddedObject.isSelfIllumination ? Shader.TransparentSelfIlmnNoSM : Shader.TransparentNoSM;
                else
                    AddedObject.PictureTehnique = AddedObject.isSelfIllumination ? Shader.TransparentSelfIlmnNoSM : Shader.TransparentNoSM;
            }
            else
            {
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

        Matrix CreateLookAt(Vector3 cameraPos, Vector3 target, Vector3 up)
        {
            Vector3 zaxis = (cameraPos - target);
            zaxis.Normalize();
            Vector3 xaxis = Vector3.Cross(up, zaxis);
            xaxis.Normalize();
            Vector3 yaxis = Vector3.Cross(zaxis, xaxis);

            Matrix view = new Matrix(xaxis.X, yaxis.X, zaxis.X, 0,
                                    xaxis.Y, yaxis.Y, zaxis.Y, 0,
                                    xaxis.Z, yaxis.Z, zaxis.Z, 0,
                                    -Vector3.Dot(xaxis, cameraPos), -Vector3.Dot(yaxis, cameraPos),
                                    -Vector3.Dot(zaxis, cameraPos), 1);

            return view;
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
            if (Device.GraphicsProfile == GraphicsProfile.Reach)
                Device.SamplerStates[1] = SamplerState.PointClamp;
            else
                Device.SamplerStates[1] = SamplerState.PointWrap;
            if (EnableShadows)
            {
                RenderToShadowMap(lightViewProjection, lightDir);
                
               
                Render.Materials.Material.ObjectRenderEffect.Parameters["ShadowMap"].SetValue(shadowRenderTarget);
            }

            this.Device.Clear(Color.CornflowerBlue);


            PhysX_test2.Engine.Render.Materials.Material.ObjectRenderEffect.Parameters["Projection"].SetValue(Camera.Projection);
            PhysX_test2.Engine.Render.Materials.Material.ObjectRenderEffect.Parameters["View"].SetValue(Camera.View);
            string AnimTeh = "", NotAnimTeh = "", BlendSITeh = Shader.TransparentSelfIlmnNoSM, BlendTeh = "";
            if (EnableShadows)
            {
                if (SmoothShadows)
                {
                    AnimTeh = Shader.AnimRenderSMSmooth;
                    NotAnimTeh = Shader.NotAnimRenderSMSmooth;
                    BlendTeh = Shader.TransparentSMSmooth;
                }
                else
                {
                    AnimTeh = Shader.AnimRenderSM;
                    NotAnimTeh = Shader.NotAnimRenderSM;
                    BlendTeh = Shader.TransparentSM;
                }
            }
            else
            {
                AnimTeh = Shader.AnimRenderNoSM;
                NotAnimTeh = Shader.NotAnimRenderNoSM;
                BlendTeh = Shader.TransparentNoSM;
            }


            RenderArrayWithTehnique(AnimTeh);
            RenderArrayWithTehnique(NotAnimTeh);

            RenderBlendArrayWithTehnique(BlendSITeh);
            RenderBlendArrayWithTehnique(BlendTeh);

            //////////
            if (EnableShadows)
            {
                RenderArrayWithTehnique(Shader.AnimRenderNoSM);
                RenderArrayWithTehnique(Shader.NotAnimRenderNoSM);

                RenderArrayWithTehnique(Shader.TransparentNoSM);
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
                    debugRenderer.RenderTransformedBB(wo.raycastaspect.boundingShape);
                   // debugRenderer.RenderAABR(wo.boundingShape);
                    debugRenderer.RenderAABB(wo.raycastaspect.boundingShape);
                }
                
               /* sprite.Begin(SpriteSortMode.FrontToBack, new BlendState());

                SamplerState ss = createNewState(SamplerState.PointClamp);
                ss.Filter = TextureFilter.Point;
                Device.SamplerStates[1] = ss;
                sprite.Draw(shadowRenderTarget, new Rectangle(0, 0, 128, 128), Color.Wheat);
                sprite.End();*/
                
            }
        }

        public void RenderArrayWithTehnique(string name)
        {
            MyContainer<PivotObject> Objects = ArraysPerTehnique[name].Objects;
            if (!Objects.IsEmpty)
            {
                Materials.Material.ObjectRenderEffect.CurrentTechnique = Materials.Material.ObjectRenderEffect.Techniques[name];
                foreach (PivotObject wo in Objects)
                {
                    Render.Materials.Material.ObjectRenderEffect.Parameters["World"].SetValue(wo.renderMatrix);
                    wo.HaveRenderAspect().SelfRender(0, wo.HaveMaterial());
                }
            }
        }
        public void RenderBlendArrayWithTehnique(string name)
        {
            MyContainer<PivotObject> Objects = ArraysPerTehnique[name].Objects;
            if (Objects.IsEmpty)
                return;

            //  (source × Blend.One) + (destination × Blend.One)
            BlendState oldstate = Device.BlendState;
            BlendState newstate = new BlendState();
            newstate.AlphaBlendFunction = BlendFunction.Add;
            newstate.AlphaDestinationBlend = Blend.One;
            newstate.AlphaSourceBlend = Blend.SourceAlpha;
            newstate.ColorBlendFunction = BlendFunction.Add;
            newstate.ColorDestinationBlend = Blend.One;
            newstate.ColorSourceBlend = Blend.SourceColor;
            Device.BlendState = newstate;
            RenderArrayWithTehnique(name);
            Device.BlendState = oldstate;
        }

        public RasterizerState createNewState(RasterizerState aotherstate)
        {
            RasterizerState res = new RasterizerState();
            res.CullMode = aotherstate.CullMode;
            res.DepthBias = aotherstate.DepthBias;
            res.FillMode = aotherstate.FillMode;
            res.MultiSampleAntiAlias = aotherstate.MultiSampleAntiAlias;
            res.ScissorTestEnable = aotherstate.ScissorTestEnable;
            res.SlopeScaleDepthBias = aotherstate.SlopeScaleDepthBias;

            return res;
        }
        public SamplerState createNewState(SamplerState aotherstate)
        {
            SamplerState res = new SamplerState();
            res.AddressU = aotherstate.AddressU;
            res.AddressV = aotherstate.AddressV;
            res.AddressW = aotherstate.AddressW;
            res.Filter = aotherstate.Filter;

            res.MaxAnisotropy = aotherstate.MaxAnisotropy;
            res.MaxMipLevel = aotherstate.MaxMipLevel;
            res.MipMapLevelOfDetailBias = aotherstate.MipMapLevelOfDetailBias;
            

            return res;
        }

        ~RenderPipeline()
        {
            
        }
    }
}
