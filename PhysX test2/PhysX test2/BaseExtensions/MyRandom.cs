using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2
{
    public class MyRandom
    {
        private static Random _generator;

        public static Random Instance
        {
            get
            {
                if (_generator == null)
                    _generator = new Random((int)DateTime.Now.Ticks);
                return _generator;
            }
        }
    }
}
