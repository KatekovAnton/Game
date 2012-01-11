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

        private GameSimpleObject[] _objects;
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
                    GameSimpleObject gso = _objects[i];
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
                GameSimpleObject gso = _objects[i];
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
            foreach (GameSimpleObject o in _objects)
            {
                o.LocateToLevel(null);
            }
            _locateTime = __now;
            _onLevel = true;
        }

        public static EffectLogicController CreateEffect(string __id, GameLevel __level, Vector3 __position, Vector3 __normal)
        {
            EffectParameters ep = StaticObjects.EffectParameters[__id];
            EffectLogicController res = new EffectLogicController(__level, ep);
            res._objects = new GameSimpleObject[ep._effectItems.Length];
            for (int i = 0; i < ep._effectItems.Length; i++)
            {
                EffectParameters.EffectItem item = ep._effectItems[i];
                switch (item._type)
                {
                    case EffectItemType.Object:
                        {
                            GameSimpleObject effectObject = new GameSimpleObject(ep._effectItems[i]._parameters["objectName"].ToString(), __level);
                            effectObject._object.SetGlobalPose(Matrix.CreateBillboard(__position + __normal*0.02f, __position + __normal*20, Vector3.Up, -__normal),true);
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
