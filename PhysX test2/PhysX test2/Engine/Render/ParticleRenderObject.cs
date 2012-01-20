using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysX_test2.Engine.Render
{
    public class ParticleRenderObject : RenderObject
    {
        /// <summary>
        /// Enum describes the various possible techniques
        /// that can be chosen to implement instancing.
        /// </summary>
        static VertexDeclaration instanceVertexDeclaration = new VertexDeclaration
         (
             new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
             new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
             new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
             new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
         );

        private Matrix[] _matrices;
        public MyModel _model;
        private DynamicVertexBuffer _instanceVertexBuffer;

      
        public ParticleRenderObject(MyModel __model, bool __ShadowCaster,
                                    bool __ShadowReceiver,
                                    bool __Transparent,
                                    bool __SelfIlmn)
        {
            _model = __model;

            isshadowcaster = __ShadowCaster;
            isshadowreceiver = __ShadowReceiver;
            isTransparent = __Transparent;
            isSelfIllumination = __SelfIlmn;
        }

        public override void SelfRender(int lod, Materials.Material mat = null)
        {
            mat.Apply(0,0);
            switch (RenderPipeline.instancingTechnique)
            {
                case InstancingTechnique.HardwareInstancing:
                    DrawModelHardwareInstancing();
                    break;

                case InstancingTechnique.NoInstancing:
                    DrawModelNoInstancing();
                    break;
            }

        }

        /// <summary>
        /// Efficiently draws several copies of a piece of geometry using hardware instancing.
        /// </summary>
        void DrawModelHardwareInstancing()
        {

            if (_matrices.Length == 0)
                return;

            // If we have more instances than room in our vertex buffer, grow it to the neccessary size.
            if ((_instanceVertexBuffer == null) ||
                (_matrices.Length > _instanceVertexBuffer.VertexCount))
            {
                if (_instanceVertexBuffer != null)
                    _instanceVertexBuffer.Dispose();

                _instanceVertexBuffer = new DynamicVertexBuffer(MyGame.Device, instanceVertexDeclaration,
                                                               _matrices.Length, BufferUsage.WriteOnly);
            }

            // Transfer the latest instance transform matrices into the instanceVertexBuffer.
            _instanceVertexBuffer.SetData(_matrices, 0, _matrices.Length, SetDataOptions.Discard);
            Materials.Material.ObjectRenderEffect.Parameters["World"].SetValue(Matrix.Identity);

            // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
            MyGame.Device.SetVertexBuffers(
                new VertexBufferBinding(_model.subsets[0].mesh.VertexBuffer, 0, 0),
                new VertexBufferBinding(_instanceVertexBuffer, 0, 1)
            );

            MyGame.Device.Indices = _model.subsets[0].mesh.IndexBuffer;

            // Draw all the instance copies in a single call.
            foreach (var pass in PhysX_test2.Engine.Render.Materials.Material.ObjectRenderEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                MyGame.Device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                       _model.subsets[0].mesh.VertexBuffer.VertexCount, 0,
                                                       _model.subsets[0].mesh.IndexBuffer.IndexCount / 3, _matrices.Length);
            }

        }


        /// <summary>
        /// Draws several copies of a piece of geometry without using any
        /// special GPU instancing techniques at all. This just does a
        /// regular loop and issues several draw calls one after another.
        /// </summary>
        void DrawModelNoInstancing()
        {
            MyGame.Device.SetVertexBuffer(_model.subsets[0].mesh.VertexBuffer);
            MyGame.Device.Indices = _model.subsets[0].mesh.IndexBuffer;

            // Set up the rendering effect.
            // effect.CurrentTechnique = effect.Techniques["NoInstancing"];

            EffectParameter transformParameter = Materials.Material.ObjectRenderEffect.Parameters["World"];

            // Draw a single instance copy each time around this loop.
            for (int i = 0; i < _matrices.Length; i++)
            {
                transformParameter.SetValue(_matrices[i]);

                foreach (var pass in PhysX_test2.Engine.Render.Materials.Material.ObjectRenderEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    _model.subsets[0].mesh.Render();
                }
            }

        }

        public override void Dispose()
        {
            if (!Disposed)
            {
                if ((_instanceVertexBuffer != null) && !(_instanceVertexBuffer.IsDisposed))
                    _instanceVertexBuffer.Dispose();
                _model.Dispose();
                _matrices = null;
                base.Dispose();
            }
        }

        /// <summary>
        /// This technique is NOT a good idea! It is only included in the sample
        /// for comparison purposes, so you can compare its performance with the
        /// other more sensible approaches. This uses the exact same shader code
        /// as the preceding NoInstancing technique, but with a key difference.
        /// Where the NoInstancing technique worked like this:
        /// 
        ///     SetRenderStates()
        ///     foreach instance
        ///     {
        ///         Update effect with per-instance transform matrix
        ///         DrawIndexedPrimitives()
        ///     }
        /// 
        /// NoInstancingOrStateBatching works like so:
        /// 
        ///     foreach instance
        ///     {
        ///         Set per-instance transform matrix into the effect
        ///         SetRenderStates()
        ///         DrawIndexedPrimitives()
        ///     }
        ///      
        /// As you can see, this is repeatedly setting the same renderstates.
        /// Not so efficient.
        /// 
        /// In other words, the built-in Model.Draw method is pretty inefficient when
        /// it comes to drawing more than one instance! Even without using any fancy
        /// shader techniques, you can get a significant speed boost just by rearranging
        /// your drawing code to work more like the earlier NoInstancing technique.
        /// </summary>
        /*   void DrawModelNoInstancingOrStateBatching(MyModel model, Matrix[] modelBones,
                                                     Matrix[] instances, Matrix view, Matrix projection)
           {
               for (int i = 0; i < instances.Length; i++)
               {
                   foreach (ModelMesh mesh in model.Meshes)
                   {
                       foreach (Effect effect in mesh.Effects)
                       {
                           effect.CurrentTechnique = effect.Techniques["NoInstancing"];

                           effect.Parameters["World"].SetValue(modelBones[mesh.ParentBone.Index] * instances[i]);
                           effect.Parameters["View"].SetValue(view);
                           effect.Parameters["Projection"].SetValue(projection);
                       }

                       mesh.Draw();
                   }
               }
           }*/


        /// <summary>
        /// Set data in update for using it in draw()
        /// </summary>
        /// <param name="matrices">Matrices per particle</param>
        public void SetParticleData(Matrix[] matrices)
        {


            _matrices = matrices;
        }
    }
}
