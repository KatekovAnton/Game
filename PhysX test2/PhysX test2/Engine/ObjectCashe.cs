using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;


using PhysX_test2.Engine.Logic;

namespace PhysX_test2.Engine
{
    public class ObjectCashe
    {
        public Dictionary<string, PivotObject> _cashedObjects;
        public Dictionary<string, ParticleObject> _cashedPObjects;

        public ObjectCashe()
        {
            _cashedObjects = new Dictionary<string, PivotObject>();
            _cashedPObjects = new Dictionary<string, ParticleObject>();
        }

        public void ClearCashe()
        {
            foreach (string key in _cashedObjects.Keys)
                ContentLoader.ContentLoader.UnloadPivotObject(_cashedObjects[key]);

            _cashedObjects.Clear();


            foreach (string key in _cashedPObjects.Keys)
                ContentLoader.ContentLoader.UnloadParticleObject(_cashedPObjects[key]);

            _cashedPObjects.Clear();
        }

        public void RemoveObject(string __name)
        {
            if(!_cashedObjects.Keys.Contains(__name))
                return;

            PivotObject obj = _cashedObjects[__name];
            
            ContentLoader.ContentLoader.UnloadPivotObject(obj);
        }

        public void RemoveParticleObject(string __name)
        {
            if (!_cashedPObjects.Keys.Contains(__name))
                return;

            ParticleObject obj = _cashedPObjects[__name];

            ContentLoader.ContentLoader.UnloadParticleObject(obj);
        }

        public void CasheObject(string __name,
            Matrix? __deltaMatrix,
            bool __needMouseCast,
            bool __needBulletCast,
            PivotObject __parentObject = null,
            PivotObjectDependType __dependType = PivotObjectDependType.Body)
        {
            PivotObject newObject = GameEngine.LoadObject(__name, __deltaMatrix, __needMouseCast, __needBulletCast, __dependType);
            _cashedObjects.Add(__name, newObject);
        }

        public void CasheParticleObject(string __name)
        {
            ParticleObject newObject = GameEngine.LoadParticleObject(__name, new Vector3(1, 1, 1));
            _cashedPObjects.Add(__name, newObject);
        }
    }
}
