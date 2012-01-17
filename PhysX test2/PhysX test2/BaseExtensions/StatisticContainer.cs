using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2
{
    public class StatisticContainer
    {
        class StatisticParameter
        {
            public int _lastValue;
            //public float _averageValue;
            public int _minValue;
            public int _maxValue;

            public int _currentValue;

            public StatisticParameter()
            {
 
            }

            public void StartFrame()
            {
                _currentValue = 0;
            }

            public void EndFrame()
            {
                _lastValue = _currentValue;
                _minValue = _minValue > _currentValue ? _currentValue : _minValue;
                _maxValue = _maxValue > _currentValue ? _maxValue : _currentValue;
            }

            public void AddFrameValue(int __value)
            {
                _currentValue += __value;
            }

            public void ToStream(System.IO.StreamWriter __stream)
            {
 
            }
        }

        Dictionary<string, StatisticParameter> _globalStatistic;
        Dictionary<string, StatisticParameter> _perFrameStatistic;

        public float GetMaxValue(string __name)
        {
            if (_perFrameStatistic.Keys.Contains(__name))
            {
                StatisticParameter parameter = _perFrameStatistic[__name];
                return parameter._maxValue;
            }
            else if (_globalStatistic.Keys.Contains(__name))
            {
                StatisticParameter parameter = _globalStatistic[__name];
                return parameter._maxValue;
            }
            return 0;
        }

        protected StatisticContainer()
        {
            _perFrameStatistic = new Dictionary<string, StatisticParameter>();
            _globalStatistic = new Dictionary<string, StatisticParameter>();
        }

        protected static StatisticContainer instance;
        public static StatisticContainer Instance()
        {
            if (instance == null)
                instance = new StatisticContainer();
            return instance;
        }


        void AddParameter(string __name, int __value)
        {
            if (!_perFrameStatistic.Keys.Contains(__name))
            {
                StatisticParameter parameter = new StatisticParameter();
                parameter._lastValue = parameter._maxValue = parameter._minValue = parameter._currentValue = __value ;
                _perFrameStatistic.Add(__name, parameter);
            }
        }

        void AddGlobalParameter(string __name, int __value)
        {
            if (!_globalStatistic.Keys.Contains(__name))
            {
                StatisticParameter parameter = new StatisticParameter();
                parameter._lastValue = parameter._maxValue = parameter._minValue = parameter._currentValue = __value;
                _globalStatistic.Add(__name, parameter);
            }
        }

        public void UpdateParameter(string __name, int __value)
        {
            if (_perFrameStatistic.Keys.Contains(__name))
            {
                StatisticParameter parameter = _perFrameStatistic[__name];
                parameter.AddFrameValue(__value);
            }
            else
                AddParameter(__name, __value);
        }

        public void UpdateGlobalParameter(string __name, int __value)
        {
            if (_globalStatistic.Keys.Contains(__name))
            {
                StatisticParameter parameter = _globalStatistic[__name];
                parameter.AddFrameValue(__value);
            }
            else
                AddGlobalParameter(__name, __value);
        }

        public void BeginFrame()
        {
            foreach (string p in _perFrameStatistic.Keys)
            {
                StatisticParameter param = _perFrameStatistic[p];
                param.StartFrame();
            }
        }

        public void EndFrame()
        {
            foreach (string p in _perFrameStatistic.Keys)
            {
                StatisticParameter param = _perFrameStatistic[p];
                param.EndFrame();
            }
        }

        public void ToStream(System.IO.StreamWriter _stream)
        { }
    }
}
