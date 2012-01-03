using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.Engine.Render;
using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Logic
{
    /// <summary>
    /// Является системой частиц, тут renderaspect и material - для рисования одной частицы. 
    /// Объект умеет сам удалять партиклы, но после смерти всех партиклов его вручную нужно 
    /// удалить из общей кучи объектов чтоб он не апдейтился!!!
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

        public int _particlesCount;
        public MyContainer<ParticleData> _particles;
        private static MyContainer<ParticleData> _particlesForRemove = new MyContainer<ParticleData>();


        public bool _isBillboards = true;

        public ParticleObject(TimeSpan __currentTime, Vector3 __maxSize, int __particleCount)
        {
            behaviourmodel = new BehaviourModel.ObjectStaticBehaviourModel();
            raycastaspect = new RaycastBoundObject(new SceneGraph.OTBoundingShape(__maxSize), null);


            _particlesCount = __particleCount;
            _particles = new MyContainer<ParticleData>(_particlesCount, 1);
            for (int i = 0; i < _particlesCount; i++)
                _particles.Add(new ParticleData(__currentTime.TotalMilliseconds, 1000.0));
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
            //dont need - static only
            //behaviourmodel.DoFrame(gt);

            //set position - lookslike dont need too
            //renderaspect.SetPosition(behaviourmodel.CurrentPosition);

            //calculate particle data
            UpdateParticles(gt);

            //set particle data
            renderaspect.SetParticleData(GetParticleData());
        }


        private void UpdateParticles(Microsoft.Xna.Framework.GameTime __gt)
        {
            _particlesForRemove.Clear();


            //TODO: calculate data
            double time = __gt.ElapsedGameTime.TotalMilliseconds/1000.0;
            foreach (ParticleData pd in _particles)
            {
                if ((__gt.TotalGameTime.TotalMilliseconds - pd._createTime) > pd._liveTime)
                    _particlesForRemove.Add(pd);
                else
                    pd.Update(time);            
            }


            //delete partcles
            foreach (ParticleData pd in _particlesForRemove)
                _particles.Remove(pd);
            _particlesForRemove.Clear();
        }

        private Matrix[] GetParticleData()
        {
            //aggregating results
            Matrix[] data = new Matrix[_particlesCount];
            if (_isBillboards)
                for (int i = 0; i < _particlesCount; i++)
                    data[i] = Matrix.CreateBillboard(_particles[i]._position, MyGame.Instance._engine.Camera._position, Vector3.Up, MyGame.Instance._engine.Camera._direction);
            else
                for (int i = 0; i < _particlesCount; i++)
                    data[i] = Matrix.CreateTranslation(_particles[i]._position);
            return data;
        }
    }

    public class ParticleData
    {
        public float _size;
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

        public ParticleData(double __createTime, double __liveTime)
        {
            _createTime = __createTime;
            _liveTime = __liveTime;
        }

        public void Update(double __elapsedTime)
        {
            
        }
    }
}
