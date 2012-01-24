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
            animatedObjectsAtEnd = new MyContainer<AnimationUser>(100, 3);
            scheduledObjects = new MyContainer<ScheduledObject>(100, 3);

            buffer = new MyContainer<AnimationUser>(100, 3);
        }

        private MyContainer<AnimationUser> animatedObjectsAtStart;
        private MyContainer<AnimationUser> animatedObjectsAtEnd;
        private MyContainer<ScheduledObject> scheduledObjects;

        private MyContainer<AnimationUser> buffer;

        public void UpdateStart(GameTime gameTime)
        {
            buffer.Clear();
            for (int i = 0; i < animatedObjectsAtStart.Count; i++)
            {
                if (animatedObjectsAtStart[i].Update(gameTime))
                    buffer.Add(animatedObjectsAtStart[i]);
            }

            if (!buffer.IsEmpty)
            {
                foreach (AnimationUser user in buffer)
                    animatedObjectsAtStart.Remove(user);
            }
        }
        public void UpdateEnd(GameTime gameTime)
        {
            buffer.Clear();
            for (int i = 0; i < animatedObjectsAtEnd.Count; i++)
            {
                if (animatedObjectsAtEnd[i].Update(gameTime))
                    buffer.Add(animatedObjectsAtEnd[i]);
            }
            for (int i = 0; i < scheduledObjects.Count; i++)
                scheduledObjects[i].Update(gameTime);

            if (!buffer.IsEmpty)
            {
                foreach (AnimationUser user in buffer)
                    animatedObjectsAtEnd.Remove(user);
            }
        }

        public void AddAnimationUser(AnimationUser __user)
        {
            animatedObjectsAtStart.Add(__user);
        }

        public void AddAnimationUserEnd(AnimationUser __user)
        {
            animatedObjectsAtEnd.Add(__user);
        }

        public void RemoveUser(AnimationUser user)
        {
            foreach (AnimationUser _obj in animatedObjectsAtStart)
            {
                if (_obj == user)
                {
                    animatedObjectsAtStart.Remove(_obj);
                    break;
                }
            }
            
            foreach (AnimationUser _obj in animatedObjectsAtEnd)
            {
                if (_obj == user)
                {
                    animatedObjectsAtEnd.Remove(_obj);
                    break;
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
