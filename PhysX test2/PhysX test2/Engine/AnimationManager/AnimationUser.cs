using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.AnimationManager
{
    /// <summary>
    /// returns is animation finished
    /// </summary>
    /// <param name="gametime">elapsed time</param>
    /// <returns>is animation finished</returns>
    public delegate bool AnimationStep(GameTime gametime);
    public delegate void AnimationStepAction();
    public class AnimationUser
    {
        public AnimationStep action;//animTime between 0.0 ....... animLength
        public object selfObject;
        public bool animationFinished;
        public AnimationUser(AnimationStep _actionOnStep, object _key)
        {
            this.action = _actionOnStep;
            this.selfObject = _key;
        }

        public void Update(GameTime gametime)
        {
            animationFinished = action(gametime);
        }
    }

    public class ScheduledObject
    {
        public AnimationStepAction action;
        public double lastUpdateTime;
        public double updateFreq;
        public object selfObject;
        public ScheduledObject(AnimationStepAction _action, double _updateFreq, object _key)
        {
            this.action = _action;
            updateFreq = _updateFreq;
            selfObject = _key;
        }
        public void Update(GameTime gtime)
        {
            if (gtime.TotalGameTime.TotalMilliseconds - lastUpdateTime >= updateFreq)
            {
                lastUpdateTime = gtime.TotalGameTime.TotalMilliseconds;
                action();
            }

        }
    }
}

