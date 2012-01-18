using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.Engine.Render;
using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Logic
{
    /// <summary>
    /// Является системой частиц, тут material - один для рисования одной частицы, 
    /// renderaspect - специальный, для рисования всех частиц одним батчем.
    /// Объект умеет сам удалять партиклы, но после смерти всех партиклов его  
    /// вручную нужно удалить из общей кучи объектов чтоб он не апдейтился!!!
    /// </summary>
    public class ParticleObject:PivotObject
    {
        /// <summary>
        /// for group для графики
        /// </summary>
        public ParticleRenderObject renderaspect;

        /// <summary>
        /// for group для графики - материал отделен от RenderObject-а чтоб их можно было по-разному группировать
        /// </summary>
        public Render.Materials.Material material;

 
        public MyContainer<ParticleData> _particles;
        private static MyContainer<ParticleData> _particlesForRemove = new MyContainer<ParticleData>();

        
        public bool _isBillboards = true;

        protected Vector3 _direction;
        protected Vector3 _position;
        protected Vector3 _maxSize;
        public float _gravityRelationMultiplier;
        public float _dispRadius;


        public ParticleObject(Vector3 __maxSize, int __particleCount, Vector3 __position, Vector3 __direction, float __dispRadius, float __gravityRelationMultiplier)
        {
            //TODO
            //it can be moving also
            behaviourmodel = new BehaviourModel.ObjectStaticBehaviourModel();
            raycastaspect = new RaycastBoundObject(new SceneGraph.OTBoundingShape(__maxSize), null);
            SetGlobalPose(Matrix.CreateTranslation(__position), true);

            _dispRadius = __dispRadius;
            _direction = __direction;
            _position = __position;
            _maxSize = __maxSize;
            _particles = new MyContainer<ParticleData>(__particleCount, 1);
            for (int i = 0; i < __particleCount; i++)
                _particles.Add(CreateParticle());
        }

        protected ParticleData CreateParticle()
        {
            float yaw = MyRandom.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
            float pitch = MyRandom.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
            float roll = MyRandom.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);

            Matrix result = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            Vector3 resultDirection = Vector3.TransformNormal(_direction, result);

            Vector3 delta = new Vector3(MyRandom.NextFloat(_dispRadius), MyRandom.NextFloat(_dispRadius), MyRandom.NextFloat(_dispRadius));

            float mult = MyRandom.NextFloat(_gravityRelationMultiplier * (1.0f - _dispRadius), _gravityRelationMultiplier * (1.0f + _dispRadius));
            return new ParticleData(MyGame.UpdateTime.TotalGameTime.TotalMilliseconds, 1000.0 - MyRandom.Instance.Next(1000 / 3), _position + delta, resultDirection, mult);
        }

        public override Render.Materials.Material HaveMaterial()
        {
            return material;
        }

        public override RenderObject HaveRenderAspect()
        {
            return renderaspect;
        }

        public override void DoFrame(Microsoft.Xna.Framework.GameTime gt)
        {
            behaviourmodel.DoFrame(gt);

            renderaspect.SetPosition(behaviourmodel.CurrentPosition);

            UpdateParticles();
        }

        public override void AfterUpdate()
        {
            //set particle data
            if (_isOnScreen)
            {
                SortParticles();
                renderaspect.SetParticleData(GetParticleData());
            }
            base.AfterUpdate();
        }

        protected void UpdateParticles()
        {
            _particlesForRemove.Clear();


            //TODO: calculate data
            double time = MyGame.UpdateTime.ElapsedGameTime.TotalSeconds;
            foreach (ParticleData pd in _particles)
            {
                if ((MyGame.UpdateTime.TotalGameTime.TotalMilliseconds - pd._createTime) > pd._liveTime)
                    _particlesForRemove.Add(pd);
                else
                    pd.Update(time);            
            }


            //delete partcles
            foreach (ParticleData pd in _particlesForRemove)
                _particles.Remove(pd);
            _particlesForRemove.Clear();
        }

        protected Matrix[] GetParticleData()
        {
            //aggregating results
            Matrix[] data = new Matrix[_particles.Count];
            if (_isBillboards)
                for (int i = 0; i < _particles.Count; i++)
                    data[i] = Matrix.CreateBillboard(_particles[i]._position, MyGame.Instance._engine.Camera._position, Vector3.Up, MyGame.Instance._engine.Camera._direction);
            else
                for (int i = 0; i < _particles.Count; i++)
                    data[i] = Matrix.CreateTranslation(_particles[i]._position);
            return data;
        }

        protected void SortParticles()
        { 
            //TODO
            //sort particals by camera distance
        }

       
    }

    public class ParticleData
    {
        public float _gravityRelationMultiplier;
        /// <summary>
        /// Position of particle
        /// </summary>
        public Vector3 _position;
        /// <summary>
        /// Direction of current paticle
        /// </summary>
        public Vector3 _curentDiection;

        //TODO: add random live time
        public double _liveTime;
        public double _createTime;

        public ParticleData(double __createTime, double __liveTime, Vector3 __position, Vector3 __direction, float __gravityRelationMultiplier)
        {
            _createTime = __createTime;
            _liveTime = __liveTime;
            _gravityRelationMultiplier = __gravityRelationMultiplier;
        }

        public void Update(double __elapsedTime)
        {
            _position = _position + _curentDiection * (float)__elapsedTime;
           
            //TODO 
            //simulate gravity like
            _curentDiection.Y -= _gravityRelationMultiplier * (float)__elapsedTime;
        }

        
    }
}
