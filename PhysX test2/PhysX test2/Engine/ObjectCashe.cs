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

        public ObjectCashe()
        {
            _cashedObjects = new Dictionary<string, PivotObject>();
        }

        public void ClearCashe()
        {
            foreach (string key in _cashedObjects.Keys)
                ContentLoader.ContentLoader.UnloadPivotObject(_cashedObjects[key]);

            _cashedObjects.Clear();
        }

        public void RemoveObject(string __name)
        {
            if(!_cashedObjects.Keys.Contains(__name))
                return;

            PivotObject obj = _cashedObjects[__name];
            
            ContentLoader.ContentLoader.UnloadPivotObject(obj);
        }

        public void CasheObject(string __name,
            Matrix? __deltaMatrix,
            bool __needMouseCast,
            PivotObject __parentObject = null,
            PivotObjectDependType __dependType = PivotObjectDependType.Body)
        {
            PivotObject newObject = GameEngine.LoadObject(__name, __deltaMatrix, __needMouseCast, __parentObject, __dependType);
            _cashedObjects.Add(__name, newObject);
        }
    }
}
