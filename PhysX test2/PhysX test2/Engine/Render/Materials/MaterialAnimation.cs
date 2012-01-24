using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Render.Materials
{
    public class MaterialAnimation : AnimationManager.AnimationUser
    {
        private int _currentFrame;
        private int _maxFrames;

        public float _animSpeed;
        public float _currentAnimSpeed;

        private bool _isInManager;

        public double _animStartTime;

        public MaterialAnimation(AnimatedMaterial __material, float __animSpeed)
        {
            _currentFrame = 0;
            _maxFrames = __material._frameMaterials.Length;
            _animSpeed = __animSpeed;
            _currentAnimSpeed = 0;

            _isInManager = false;

            _animStartTime = MyGame.UpdateTime.TotalGameTime.TotalMilliseconds;
        }

        public double LoopTime
        {
            get
            {
                return (double)_maxFrames / (double)_animSpeed;
            }
        }

        public double CurrentState
        {
            get
            {
                return (_animStartTime - MyGame.UpdateTime.TotalGameTime.TotalMilliseconds) / LoopTime;
            }
        }

        protected void AddToManager()
        {
            if (_isInManager)
                return;
            AnimationManager.AnimationManager.Manager.AddAnimationUser(this);
            _animStartTime = MyGame.UpdateTime.TotalGameTime.TotalMilliseconds;
            _isInManager = true;
        }

        protected void RemoveFromManager()
        {
            if (!_isInManager)
                return;
            _isInManager = false;
            AnimationManager.AnimationManager.Manager.RemoveUser(this);
        }

        public void StartAnimation(bool _dropFrame)
        {
            AddToManager();
            if(!_dropFrame)
                AffectToLoopTime();
            _currentAnimSpeed = _animSpeed;
        }

        public void StopAnimation()
        {
            _currentAnimSpeed = 0;
            RemoveFromManager();
        }

        protected void AffectToLoopTime()
        {
            double currentProgress = (double)_currentFrame /(double)_animSpeed;
            _animStartTime = MyGame.UpdateTime.TotalGameTime.TotalMilliseconds - currentProgress;
        }

        public bool Update(GameTime __gametime)
        {
            _currentFrame = (int)CurrentState;
            return false;
        }
    }
}
