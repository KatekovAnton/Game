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
            animatedObjectsAtStart = new MyContainer<AnimationUser>(100, 3);
            scheduledObjects = new MyContainer<ScheduledObject>(100, 3);

            buffer = new MyContainer<AnimationUser>(100, 3);
        }

        private MyContainer<AnimationUser> animatedObjectsAtStart;
        
        private MyContainer<ScheduledObject> scheduledObjects;

        private MyContainer<AnimationUser> buffer;

        public void Update(GameTime gameTime)
        {
            buffer.Clear();
            for (int i = 0; i < animatedObjectsAtStart.Count; i++)
            {
                animatedObjectsAtStart[i].Update(gameTime);
                if (animatedObjectsAtStart[i].animationFinished)
                    buffer.Add(animatedObjectsAtStart[i]);
            }
            for (int i = 0; i < scheduledObjects.Count; i++)
                scheduledObjects[i].Update(gameTime);

            if (!buffer.IsEmpty)
            {
                foreach (AnimationUser user in buffer)
                    animatedObjectsAtStart.Remove(user);
            }
        }

        public void AddAnimationUser(AnimationStep _action, object newUser)
        {
            animatedObjectsAtStart.Add(new AnimationUser(_action, newUser));
        }

        public void RemoveUser(object user)
        {
            bool removed = true;
            while (removed)
            {
                removed = false;
                foreach (AnimationUser _obj in animatedObjectsAtStart)
                {
                    if (_obj.selfObject == user)
                    {
                        animatedObjectsAtStart.Remove(_obj);
                        removed = true;
                        break;
                    }
                }
            }
        }

        public void AddScheduledUser(AnimationStepAction _action, double updateFreq, object newUser)
        {
            scheduledObjects.Add(new ScheduledObject(_action, updateFreq, newUser));
        }

        public void RemoveScheduledUser(ScheduledObject user)
        {
            bool removed = true;
            while (removed)
            {
                removed = false;
                foreach (ScheduledObject _obj in scheduledObjects)
                {
                    if (_obj.selfObject == user)
                    {
                        scheduledObjects.Remove(_obj);
                        removed = true;
                        break;
                    }
                }
            }
        }
    }
}
