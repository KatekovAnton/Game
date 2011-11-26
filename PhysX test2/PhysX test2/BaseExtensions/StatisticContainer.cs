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
        }

        Dictionary<string, StatisticParameter> _statistic;

        public float GetMaxValue(string __name)
        {
            if (!_statistic.Keys.Contains(__name))
                return 0;

            StatisticParameter parameter = _statistic[__name];
            return parameter._maxValue;
        }

        protected StatisticContainer()
        {
            _statistic = new Dictionary<string, StatisticParameter>();
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
            if (!_statistic.Keys.Contains(__name))
            {
                StatisticParameter parameter = new StatisticParameter();
                parameter._lastValue = parameter._maxValue = parameter._minValue = parameter._currentValue = __value ;
                _statistic.Add(__name, parameter);
            }
        }

        public void UpdateParameter(string __name, int __value)
        {
          //  if (_statistic.Keys.Contains(__name))
          //  {
                StatisticParameter parameter = _statistic[__name];
                parameter.AddFrameValue(__value);
           // }
           // else
            //    AddParameter(__name, __value);
        }

        public void BeginFrame(string __name)
        {
            if (_statistic.Keys.Contains(__name))
            {
                StatisticParameter parameter = _statistic[__name];
                parameter.StartFrame();
            }
            else
                AddParameter(__name, 0);
        }

        public void EndFrame(string __name)
        {
            if (_statistic.Keys.Contains(__name))
            {
                StatisticParameter parameter = _statistic[__name];
                parameter.EndFrame();
            }
            else
                AddParameter(__name, 0);
        }
    }
}
