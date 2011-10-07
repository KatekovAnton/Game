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
            animatedObjects = new MyContainer<AnimationUser>(100, 3);
            scheduledObjects = new MyContainer<ScheduledObject>(100, 3);

            buffer = new MyContainer<AnimationUser>(100, 3);
        }

        private MyContainer<AnimationUser> animatedObjects;
        private MyContainer<ScheduledObject> scheduledObjects;

        private MyContainer<AnimationUser> buffer;

        public void Update(GameTime gameTime)
        {
            buffer.Clear();
            for (int i = 0; i < animatedObjects.Count; i++)
            {
                animatedObjects[i].Update(gameTime);
                if (animatedObjects[i].animationFinished)
                    buffer.Add(animatedObjects[i]);
            }
            for (int i = 0; i < scheduledObjects.Count; i++)
                scheduledObjects[i].Update(gameTime);

            if (!buffer.IsEmpty)
            {
                foreach (AnimationUser user in buffer)
                    animatedObjects.Remove(user);
            }
        }

        public void AddAnimationUser(AnimationUser newUser)
        {
            animatedObjects.Add(newUser);
        }

        public void RemoveUser(AnimationUser user)
        {
            animatedObjects.Remove(user);
        }

        public void AddScheduledUser(ScheduledObject newUser)
        {
            scheduledObjects.Add(newUser);
        }

        public void RemoveScheduledUser(ScheduledObject user)
        {
            scheduledObjects.Remove(user);
        }
    }

}
