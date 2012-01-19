using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.TheGame.Level;
using PhysX_test2.TheGame.LogicControllers.Parameters;

using PhysX_test2.TheGame.Objects;

using Microsoft.Xna.Framework;

namespace PhysX_test2.TheGame.LogicControllers
{
    public class EffectLogicController:BaseLogicController
    {
        private EffectParameters _baseParameters;

        private GameObject[] _objects;
        private TimeSpan _locateTime;

        private bool _onLevel = false;

        public EffectLogicController(GameLevel __level, EffectParameters __parameters)
            : base(__level)
        {
            _baseParameters = __parameters;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime __gameTime)
        {
            if (!_onLevel)
                return;

            if ((__gameTime.TotalGameTime.TotalMilliseconds - _locateTime.TotalMilliseconds) > _baseParameters._lifeTime)
            {
                //stop the update
                RemoveFromLevel();
            }
            else
            {
                for(int i = 0; i< _objects.Length;i++)
                {
                    GameObject gso = _objects[i];
                    if ((__gameTime.TotalGameTime.TotalMilliseconds - _locateTime.TotalMilliseconds) > _baseParameters._effectItems[i]._lifeTime)
                        gso.RemoveFromLevel();
                    
                }
            }
        }

        public override void RemoveFromLevel()
        {
            _onLevel = false;
            _itsLevel.RemoveController(this);
            for (int i = 0; i < _objects.Length; i++)
            {
                GameObject gso = _objects[i];
                gso.RemoveFromLevel();
                gso.Unload();
            }
        }

        public override void TakeHit(Parameters.BulletDynamicParameters __bulletParameters)
        {
            //nothing
        }

        public void LocateOnLevel(TimeSpan __now)
        {
            
            for(int i =0;i< _objects.Length;i++)
            {
                GameObject o = _objects[i];
                EffectParameters.EffectItem item = _baseParameters._effectItems[i];
                switch (item._type)
                {
                    case EffectItemType.Object:
                        {
                            o.LocateToLevel(null);
                        } break;
                    case EffectItemType.Particles:
                        {
                            o.LocateToLevel(null);
                        } break;
                    default: break;
                }
            }
            _locateTime = __now;
            _onLevel = true;
        }

        public static EffectLogicController CreateEffect(string __id, GameLevel __level, Vector3 __position, Vector3 __normal)
        {
            EffectParameters ep = StaticObjects.EffectParameters[__id];
            EffectLogicController res = new EffectLogicController(__level, ep);
            res._objects = new GameObject[ep._effectItems.Length];
            for (int i = 0; i < ep._effectItems.Length; i++)
            {
                EffectParameters.EffectItem item = ep._effectItems[i];
                switch (item._type)
                {
                    case EffectItemType.Object:
                        {
                            GameSimpleObject effectObject = new GameSimpleObject(ep._effectItems[i]._parameters["objectName"].ToString(), __level);
                            effectObject._object.SetGlobalPose(Matrix.CreateBillboard(__position + __normal * 0.02f, __position + __normal * 20, Vector3.Up, -__normal), true);
                            res._objects[i] = effectObject;
                        } break;
                    //TODO: other cases
                    case EffectItemType.Billboard:
                        { 

                        }break;
                    case EffectItemType.ConstrainedBillboard:
                        {

                        } break;
                    case EffectItemType.Particles:
                        {
                            float size = Convert.ToSingle((ep._effectItems[i]._parameters["size"] as string).Replace('.', ','));
                            int count = Convert.ToInt32((ep._effectItems[i]._parameters["count"] as string).Replace('.',','));
                            float dispersion = Convert.ToSingle((ep._effectItems[i]._parameters["dispersion"] as string).Replace('.',','));
                            float gravity = Convert.ToSingle((ep._effectItems[i]._parameters["gravity"] as string).Replace('.',','));
                            float angledisp = Convert.ToSingle((ep._effectItems[i]._parameters["angledisp"] as string).Replace('.', ','));
                            float speed = Convert.ToSingle((ep._effectItems[i]._parameters["speed"] as string).Replace('.', ','));
                            float speeddisp = Convert.ToSingle((ep._effectItems[i]._parameters["speeddisp"] as string).Replace('.', ','));
                            GameParticleObject effectObject = new GameParticleObject(
                                ep._effectItems[i]._parameters["objectName"] as string,
                                __level, 
                                new Vector3(size), 
                                count,
                                __normal,
                                dispersion,
                                gravity, 
                                angledisp,
                                speed,
                                speeddisp,
                                item._lifeTime);

                            effectObject._object.SetGlobalPose(Matrix.CreateTranslation(__position), true);
                            res._objects[i] = effectObject;
                        } break;
                }
            }
            return res;
        }

       /* public override Parameters.DynamicParameters GetParameters()
        {
            return null;
        }*/
        ~EffectLogicController()
        {
 
        }
    }
}
