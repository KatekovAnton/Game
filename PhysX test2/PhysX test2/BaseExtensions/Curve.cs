using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2
{
    public class Curve
    {
        public class CurveStep
        {
            public float _start;
            public float _end;
            public CurveStep(float __start, float __end)
            {
                _start = __start;
                _end = __end;
            }
        }

        public float _CureveStepLength;
        public CurveStep[] _steps;

        public Curve( float __start, float __end)
        {
            _steps = new CurveStep[1];
            _steps[0] = new CurveStep(__start, __end);
        }

        public void AddStep(float __delta)
        {
            List<CurveStep> steps = new List<CurveStep>(_steps);
            steps.Add(new CurveStep(steps[steps.Count - 1]._end, steps[steps.Count - 1]._end + __delta));
            _steps = steps.ToArray();
        }

        public float GetValue(double _x)
        {
            int fullpart = (int)_x;
            float ostatok = (float)(_x - Math.Floor(_x));

            fullpart = fullpart % _steps.Length;

            CurveStep cs = _steps[fullpart];

            return MathHelper.Lerp(cs._start, cs._end, ostatok);
        }
    }
}
