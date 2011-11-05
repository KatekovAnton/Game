using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.Engine.Logic;

using Microsoft.Xna.Framework;

namespace PhysX_test2.TheGame.Level
{
    public class GameLevel
    {
        public EngineScene _scene;

        public GameLevel(EngineScene __scene)
        {
            _scene = __scene;
        }
        public float times = 0;
        public Vector3 GetSpawnPlace()
        {
            times += 1;
            return new Vector3(0 + times*2.0f, 16.0f, 0);
        }
      
        public void AddObject(Engine.Logic.PivotObject __object, Engine.Logic.PivotObject __parentObject = null)
        {
            LevelObject loNew = __object as LevelObject;
            if (loNew == null)
                return;

            if (loNew.renderaspect.isanimated)
            {
                Engine.Render.AnimRenderObject ro = loNew.renderaspect as Engine.Render.AnimRenderObject;
                Engine.AnimationManager.AnimationManager.Manager.AddAnimationUserEnd(ro.Update, ro.character);
            }

            _scene.AddObject(__object);

            if (__parentObject != null)
            {
                __object.behaviourmodel.SetParentObject(__parentObject);
                _scene._objects.AddRule(__parentObject, __object);
            }
        }

        public void RemoveObject(Engine.Logic.PivotObject theObjecr)
        {
            _scene.RemoveObject(theObjecr);
        }
    }
}
