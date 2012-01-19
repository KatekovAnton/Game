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
        protected float _gravityRelationMultiplier;
        protected float _dispRadius;
        protected int _particleCount;

        protected float _angledisp;
        protected float _speed;
        protected float _speeddisp;

        protected bool _firstEmitted;
        protected bool _parametersSetted;

        protected double _livetime;

        public ParticleObject(
            BehaviourModel.ObjectBehaviourModel _behaviourmodel,
            ParticleRenderObject _renderaspect, 
            Render.Materials.Material _material, 
            RaycastBoundObject _raycastaspect) 
        {
            behaviourmodel = _behaviourmodel;
            renderaspect = _renderaspect;
            raycastaspect = _raycastaspect;
            material = _material;

            _parametersSetted = _firstEmitted = false;
        }

        public void SetParticlesParameters(int __particleCount, Vector3 __direction, float __dispRadius, float __gravityRelationMultiplier, float __angledisp, float __speed, float __speeddisp, double __livetime)
        {
            _dispRadius = __dispRadius;
            _direction = __direction;
            _gravityRelationMultiplier = __gravityRelationMultiplier;
            _particleCount = __particleCount;

            _angledisp = __angledisp;
            _speed = __speed;
            _speeddisp = __speeddisp;

            _livetime = __livetime;

            _parametersSetted = true;
        }

        public void FirstEmition()
        {
            if (_firstEmitted)
                return;

            if (!_parametersSetted)
                return;

            _firstEmitted = true;
            _position = this.behaviourmodel.CurrentPosition.Translation;
            _particles = new MyContainer<ParticleData>(_particleCount, 1);
            for (int i = 0; i < _particleCount; i++)
                _particles.Add(CreateParticle());
        }

        protected ParticleData CreateParticle()
        {
            float yaw = MyRandom.NextFloat(-_angledisp, _angledisp);
            float pitch = MyRandom.NextFloat(-_angledisp, _angledisp);
            float roll = MyRandom.NextFloat(-_angledisp, _angledisp);

            Matrix result = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            Vector3 resultDirection = Vector3.TransformNormal(_direction, result);
            resultDirection = resultDirection * MyRandom.NextFloat(_speed - _speeddisp, _speed + _speeddisp);
            resultDirection.Y -= _gravityRelationMultiplier * 0.1f;
            Vector3 delta = new Vector3(MyRandom.NextFloat(_dispRadius), MyRandom.NextFloat(_dispRadius), MyRandom.NextFloat(_dispRadius));

            float mult = MyRandom.NextFloat(_gravityRelationMultiplier * (1.0f - _dispRadius), _gravityRelationMultiplier * (1.0f + _dispRadius));
            return new ParticleData(MyGame.UpdateTime.TotalGameTime.TotalMilliseconds, _livetime - MyRandom.Instance.Next((int)(_livetime / 2.0)), _position + delta, resultDirection, mult);
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
            _position = behaviourmodel.CurrentPosition.Translation;
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
            _position = __position;
            _curentDiection = __direction;
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
