using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.AnimationManager
{
    public class AnimationManager
    {
        private sealed class AnimationManagerCreator
        {
            private static readonly AnimationManager instance = new AnimationManager();

            public static AnimationManager _AnimationManager
            {
                get { return instance; }
            }
        }

        public static AnimationManager Manager
        {
            get { return AnimationManagerCreator._AnimationManager; }
        }

        protected AnimationManager()
        {
            animatedObjects = new MyContainer<IAnimationUser>(100, 3);
        }

        private MyContainer<IAnimationUser> animatedObjects;

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0);
            for (int i = 0; i < animatedObjects.Count; i++)
                animatedObjects[i].Update(elapsed);
        }

        public void AddAnimationUser(IAnimationUser newUser)
        {
            animatedObjects.Add(newUser);
        }

        public void RemoveUser(IAnimationUser user)
        {
            animatedObjects.Remove(user);
        }
    }

}
