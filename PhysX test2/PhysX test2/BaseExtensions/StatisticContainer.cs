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
            public float _lastValue;
            //public float _averageValue;
            public float _minValue;
            public float _maxValue;

            public StatisticParameter()
            {
 
            }

            public void UpdateParameter(float _newValue)
            {
                _lastValue = _newValue;
                _minValue = _minValue > _newValue ? _newValue : _newValue;
                _maxValue = _maxValue > _newValue ? _maxValue : _newValue;
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


        public void AddParameter(string __name, float __value)
        {
            if (!_statistic.Keys.Contains(__name))
            {
                StatisticParameter parameter = new StatisticParameter();
                parameter._lastValue = parameter._maxValue = parameter._minValue = __value;
                _statistic.Add(__name, parameter);
            }
        }

        public void UpdateParameter(string __name, float __value)
        {
            if (_statistic.Keys.Contains(__name))
            {
                StatisticParameter parameter = _statistic[__name];
                parameter.UpdateParameter(__value);
            }
            else
                AddParameter(__name, __value);
            
        }
    }
}
