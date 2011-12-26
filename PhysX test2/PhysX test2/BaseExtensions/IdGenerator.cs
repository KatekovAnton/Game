using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2
{
    public class IdGenerator
    {
        public static IdGenerator StaticGenerator = new IdGenerator(0);
        public uint _lastValue;
        public IdGenerator(uint lastEnd)
        {
            _lastValue = lastEnd;
        }
        public uint NewId()
        {
            return _lastValue++;
        }

        public void ClearIdsCounter()
        {
            _lastValue = 0;
        }
    }
}
