﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;


namespace PhysX_test2.Engine.Helpers
{
    public class FpsCounter
    {
        private float _frameTime = 0;

        private int _framesPerSecond = 0;

        private int _frameCounter = 0;

        private TimeSpan _elapsedTime = TimeSpan.Zero;

        public int FramesPerSecond
        {
            get { return _framesPerSecond; }
        }

        public float FramesPerSecond_Smooth;

        public float FrameTime
        {
            get { return _frameTime; }
        }

        public void Update(GameTime gameTime)
        {
            //накапливаем время прошедшее с момента отрисовки последнего кадра
            _elapsedTime += gameTime.ElapsedGameTime;

            //если накопленное время больше секунды, то считаем кадры
            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _framesPerSecond = _frameCounter;
                StatisticContainer.Instance().UpdateParameter("fps", _frameCounter);
                _frameCounter = 0;
            }

            //увеличиваем счетчик кадров
            _frameCounter++;

            //получаем время затраченное на отрисовку одного кадра
            if (_framesPerSecond > 0)
            {
                _frameTime = 1000f / _framesPerSecond;
            }

            FramesPerSecond_Smooth = MyMath.perehod(FramesPerSecond_Smooth, _framesPerSecond, 0.9f);
        }
    }
}
